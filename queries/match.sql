
drop procedure if exists sp_CalculateMatches;

DELIMITER $$
CREATE DEFINER=`admin`@`%` PROCEDURE `sp_CalculateMatches`(
	firebaseUserId varchar(36),
	startTime datetime,
	endTime datetime,
	rangeMiles double,
    timespan time
)
begin    

	select Value
    into @showResults
    from AppSettings where Setting = 'sp_CalculateMatches_ReturnResults';

	drop temporary table if exists distance_results;
    drop temporary table if exists final_results;

	create temporary table distance_results
	select *
	from (
		select
			 case
				when l.StartTime <= r.StartTime and l.EndTime <= r.EndTime then r.StartTime  -- 'left partial'
				when r.StartTime <= l.StartTime and r.EndTime <= l.EndTime then l.StartTime -- 'right partial'
				when r.StartTime < l.StartTime and l.EndTime < r.EndTime then l.StartTime -- 'right contains left'
				when l.StartTime < r.StartTime and r.EndTime < l.EndTime then r.StartTime -- 'left contains right'
				when l.StartTime = r.StartTime and l.EndTime = r.EndTime then l.StartTime -- 'equal'
			 end as 'start_time'
			,case
				when l.StartTime <= r.StartTime and l.EndTime <= r.EndTime then l.EndTime -- 'left partial'
				when r.StartTime <= l.StartTime and r.EndTime <= l.EndTime then r.EndTime -- 'right partial'
				when r.StartTime < l.StartTime and l.EndTime < r.EndTime then l.EndTime -- 'right contains left'
				when l.StartTime < r.StartTime and r.EndTime < l.EndTime then r.EndTime -- 'left contains right'
				when l.StartTime = r.StartTime and l.EndTime = r.EndTime then l.EndTime -- 'equal'
			 end as 'end_time'
			,cast(null as time) as 'overlaptime'
			,ST_Distance_Sphere(point(l.latitude, l.longitude), point(r.latitude, r.longitude)) * .000621371192 as 'miles'
			,l.latitude as 'lLatitude'
			,l.longitude as 'lLongitude'
			,r.latitude as 'rLatitude'
			,r.longitude as 'rLongitude'
			,l.FirebaseUserId as 'lFirebaseUserId'
			,r.FirebaseUserId as 'rFirebaseUserId'
			,l.starttime as 'lStartTime'
			,l.endtime as 'lEndTime'
			,r.starttime as 'rStartTime'
			,r.endtime as 'rEndTime'
		from
			(
				select *
                from Precalc_Locations p1
				where p1.FirebaseUserId = firebaseUserId and p1.starttime between startTime and endTime
            ) l
			inner join (
				select *
                from Precalc_Locations p2
				where p2.FirebaseUserId != firebaseUserId and p2.starttime between startTime and endTime
            ) r on (
				   l.starttime between r.starttime and r.endtime
				or r.starttime between l.starttime and l.endtime
				or l.endtime between r.starttime and r.endtime
				or r.endtime between l.starttime and l.endtime
				or l.starttime = r.starttime
				or l.endtime = r.endtime
			)
	) x;

	update distance_results
    set overlaptime = timediff(start_time, end_time);

	create temporary table final_results
	select
		 pm.RawMatchPercentage
		,pm.WeightedMatchPercentage
        ,pm.Timestamp as 'MatchCalculcatedTimestamp'
		,lu.Id as 'lUserId'
        ,ru.Id as 'rUserId'
		,r.*
    from
		distance_results r
        
        inner join Users lu on r.lFirebaseUserId=lu.FirebaseUserId
        inner join UserAttractedGenders luag on lu.Id = luag.UserAttractedToId
        inner join UserGenders lug on luag.GendersAttractedToId = lug.Id
        
        inner join Users ru on r.rFirebaseUserId=ru.FirebaseUserId
		inner join UserAttractedGenders ruag on ru.Id = ruag.UserAttractedToId
        inner join UserGenders rug on ruag.GendersAttractedToId = rug.Id
        
        left join Precalc_ProfileMatches pm on lu.Id = pm.lUserId and ru.Id = pm.rUserId
    where
		r.miles <= rangeMiles
        and r.overlaptime >= timespan
        and luag.GendersAttractedToId = ru.UserGenderId
        and ruag.GendersAttractedToId = lu.UserGenderId
	order by r.miles, r.overlaptime;

	insert into UserMatches (UserId1, UserId2, MatchedOnDate, MatchedLatitude, MatchedLongitude, RawMatchPercentage, WeightedMatchPercentage)
	select distinct lUserId, rUserId, current_timestamp(), lLatitude, lLongitude, RawMatchPercentage, WeightedMatchPercentage
    from
		final_results r
        left join UserMatches m on r.lUserId = m.UserId1 and r.rUserId = m.UserId2
	where m.Id is null
    group by lUserId, rUserId;


	if (@showResults = 1) then
		select * from final_results;
	end if;

end$$
DELIMITER ;


call sp_CalculateMatches('7dc2a570ad2611ecab9402d193d5fff8', '2022-04-04', '2022-04-05', 1000, '00:05:00');

