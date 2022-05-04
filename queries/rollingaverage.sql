

select locations.FirebaseUserId, locations.records as 'LocationRecords', precalc.records as 'RollupRecords'
from
	(
		select FirebaseUserId, count(*) as records
		from Locations
		group by FirebaseUserId
	) locations
    inner join (
		select FirebaseUserId, count(*) as records
		from Precalc_Locations
		group by FirebaseUserId
	) precalc on locations.FirebaseUserId = precalc.FirebaseUserId




drop procedure if exists sp_ProcessLocationRecord;

DELIMITER $$
CREATE DEFINER=`admin`@`%` PROCEDURE `sp_ProcessLocationRecord`(
	newFirebaseUserId varchar(36),
	newLatitude double,
	newLongitude double,
	newTimestamp datetime
)
begin    
	set @debug := 0;
    
	-- insert into location_trace (data) values (
-- 		concat(
-- 			'sp_ProcessLocationRecord('
--             ,newFirebaseUserId, ', '
--             ,newLatitude, ', '
--             ,newLongitude, ', '
--             ,newTimestamp
--             ,')'
--         )
-- 	);
    
    
	select Value into @round from AppSettings where Setting = 'GPSRoundDecimalPlaces';
    select 0.5 into @tolerance;

	-- last rollup record
	select l.Id, l.Latitude, l.Longitude, l.EndTime
	into @id, @latitude, @longitude, @timestamp
    from Precalc_Locations l
    where FirebaseUserId = newFirebaseUserId
    order by Id desc
    limit 1;
    
    -- last raw location record
    select l.Id, l.Latitude, l.Longitude, l.Timestamp
	into @prevId, @prevLatitude, @prevLongitude, @prevTimestamp
    from Locations l
    where FirebaseUserId = newFirebaseUserId
    order by Id desc
    limit 1;
    
    select abs((DistanceBetweenPoints(@latitude, @longitude, @prevLatitude, @prevLongitude) / 1000.0 * 0.62137) / (timestampdiff(microsecond, @prevTimestamp, @timestamp) / 1000.0 / 1000.0 / 3600.0))
    into @speedfromlast;
    
    select avg(MpH) over (order by Id rows 10 preceding)
    into @rollingaverage
	from (
		select *, abs((DistanceBetweenPoints(Latitude, Longitude, PrevLatitude, PrevLongitude) / 1000.0 * 0.62137) / (timestampdiff(microsecond, prevTimestamp, timestamp) / 1000.0 / 1000.0 / 3600.0)) as MpH
		from (
			select
				 *
				,lag(Id) over(order by Id) as prevId
				,lag(Timestamp) over(order by Id) as prevTimestamp
				,lag(Latitude) over(order by Id) as prevLatitude
				,lag(Longitude) over(order by Id) as prevLongitude
			from (
				select *
				from Locations
				where FirebaseUserId=newFirebaseUserId
				order by id desc
				limit 10
			) x1
			order by id
		) x2
	) x3
	order by timestamp desc
	limit 1;
    
    select case
		when @id is null then 1
		when @speedfromlast between (@tolerance * @rollingaverage) and ((1.0 + @tolerance) * @rollingaverage) then 1
        else 0
    end
    into @insertingrecord;
    
-- 	insert into location_trace (data) values (concat('@round = ', coalesce(@round, ''), ' '));
-- 	insert into location_trace (data) values (concat('@tolerance = ', coalesce(@tolerance, ''), ' '));
-- 	insert into location_trace (data) values (concat('@id = ', coalesce(@id, ''), ' '));
-- 	insert into location_trace (data) values (concat('@latitude = ', coalesce(@latitude, ''), ' '));
-- 	insert into location_trace (data) values (concat('@longitude = ', coalesce(@longitude, ''), ' '));
-- 	insert into location_trace (data) values (concat('@timestamp = ', coalesce(@timestamp, ''), ' '));
-- 	insert into location_trace (data) values (concat('@prevId = ', coalesce(@prevId, ''), ' '));
-- 	insert into location_trace (data) values (concat('@prevLatitude = ', coalesce(@prevLatitude, ''), ' '));
-- 	insert into location_trace (data) values (concat('@prevLongitude = ', coalesce(@prevLongitude, ''), ' '));
-- 	insert into location_trace (data) values (concat('@prevTimestamp = ', coalesce(@prevTimestamp, ''), ' '));
-- 	insert into location_trace (data) values (concat('@speedfromlast = ', coalesce(@speedfromlast, ''), ' '));
-- 	insert into location_trace (data) values (concat('@rollingaverage = ', coalesce(@rollingaverage, ''), ' '));
-- 	insert into location_trace (data) values (concat('@speedmin = ', coalesce((@tolerance * @rollingaverage), ''), ' '));
-- 	insert into location_trace (data) values (concat('@speedmax = ', coalesce(((1.0 + @tolerance) * @rollingaverage), ''), ' '));
-- 	insert into location_trace (data) values (concat('@insertingrecord = ', coalesce(@insertingrecord, '')));

    if @insertingrecord = 1 then
		if @id is null then
			-- first record for user
			insert into Precalc_Locations (FirebaseUserId, Latitude, Longitude, StartTime, EndTime) values (newFirebaseUserId, round(newLatitude, @round), round(newLongitude, @round), newTimestamp, newTimestamp);
            
--             if @debug = 1 then select 'insert first record for user'; end if;
		elseif @latitude = round(newLatitude, @round) and @longitude = round(newLongitude, @round) then
			update Precalc_Locations set EndTime = newTimestamp where Id = @Id;
-- 			if @debug = 1 then select 'update existing record'; end if;
		else
			insert into Precalc_Locations (FirebaseUserId, Latitude, Longitude, StartTime, EndTime) values (newFirebaseUserId, round(newLatitude, @round), round(newLongitude, @round), newTimestamp, newTimestamp);
-- 			if @debug = 1 then select 'insert new location record'; end if;
		end if;
	end if;

end$$
DELIMITER ;

-- CREATE DEFINER=`admin`@`%` TRIGGER `Locations_after_insert` AFTER INSERT ON `Locations` FOR EACH ROW begin
--     call sp_ProcessLocationRecord(new.FirebaseUserId, new.Latitude, new.Longitude, new.Timestamp);
-- end


-- -- set @newFirebaseUserId := 109409728062644297175;
-- -- -- set @newTimestamp := '2022-03-12 03:30:05.351177';
-- -- set @newTimestamp := '2022-03-18 12:55';
-- -- set @newLatitude := 41.78653930720767;
-- -- set @newLongitude := -72.57906391601573;

-- -- call sp_ProcessLocationRecord(@newFirebaseUserId, @newLatitude, @newLongitude, @newTimestamp);

-- set @newFirebaseUserId := '7dc2a570ad2611ecab9402d193d5fff8';
-- -- -- set @newTimestamp := '2022-03-27 01:15:24';
-- -- -- set @newLatitude := 41.435426275606815;
-- -- -- set @newLongitude := -72.87757127915724;

-- delete from Locations where FirebaseUserId = '7dc2a570ad2611ecab9402d193d5fff8';
-- delete from Precalc_Locations where FirebaseUserId = '7dc2a570ad2611ecab9402d193d5fff8';
-- -- call sp_ProcessLocationRecord(@newFirebaseUserId, @newLatitude, @newLongitude, @newTimestamp);

-- -- select u.FirebaseUserId, u.FirstName, u.LastName, count(l.Id)
-- -- from
-- -- 	Users u
-- --     left join Locations l on u.FirebaseUserId = l.FirebaseUserId
-- -- where
-- -- 	u.RegisteredTimestamp = '2022-02-24 02:36:08'
-- --     and u.FirebaseUserId = @newFirebaseUserId
-- -- group by u.FirebaseUserId, u.PreferredName, u.LastName;

-- select *
-- from Locations
-- where FirebaseUserId = @newFirebaseUserId
-- order by Id;

-- select *
-- from Precalc_Locations
-- where FirebaseUserId = @newFirebaseUserId
-- order by Id;


-- -- copy mine and matt's to temp table, truncate table, reset auto_increment, copy back
-- -- delete from Locations where FirebaseUserId not in ('101183912071887397401', '109409728062644297175');

-- select * from Users;
