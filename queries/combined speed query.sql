

select Value into @round from AppSettings where Setting = 'GPSRoundDecimalPlaces';

select 0.50 into @tolerance;

set @newFirebaseUserId := 109409728062644297175;
set @newTimestamp := cast('2022-03-12 03:30:05.351177' as datetime);
set @newLatitude := 41.78653930720767;
set @newLongitude := -72.57906391601573;

select @newFirebaseUserId, @newTimestamp, @newLatitude, @newLongitude, @round;

select l.Id, l.Latitude, l.Longitude
into @id, @latitude, @longitude
from Precalc_Locations l
where FirebaseUserId = @newFirebaseUserId
order by Id desc
limit 1;

select @id, @latitude, @longitude;



select Id, Timestamp, Latitude, Longitude, RollingAverage
into @lastId, @lastTimestamp, @lastLatitude, @lastLongitude, @lastRollingAverage
from (
	select *, avg(MpH) over (order by Id rows 10 preceding) as RollingAverage
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
				where FirebaseUserId=@newFirebaseUserId
				order by id desc
				limit 10
			) x
			order by id
		) x
	) x
) x
order by Id desc
limit 1;

select abs((DistanceBetweenPoints(@newLatitude, @newLongitude, @lastLatitude, @lastLongitude) / 1000.0 * 0.62137) / (timestampdiff(microsecond, @lastTimestamp, @newTimestamp) / 1000.0 / 1000.0 / 3600.0))
into @newSpeed;

select
	 @lastId, @lastTimestamp, @lastLatitude, @lastLongitude, @lastRollingAverage	
	,@newSpeed
    
    ,@tolerance
    ,@lastRollingAverage - (@lastRollingAverage * @tolerance) as 'minSpeed'
    ,@lastRollingAverage + (@lastRollingAverage * @tolerance) as 'maxSpeed'
    
    ,case
		when @id is null then 'first record for user'
		when @latitude = round(@newLatitude, @round) and @longitude = round(@newLongitude, @round) then 'Updating existing record'
        else 'Inserting new record'
	 end as 'branch'
	,case
		when @id is null then 
			concat('insert into Precalc_Locations (FirebaseUserId, Latitude, Longitude, StartTime, EndTime)\nvalues (',
				@newFirebaseUserId,', ',
                format(round(@newLatitude, @round), @round),', ',
                format(round(@newLongitude, @round), @round),', ',
                @newTimestamp,', ',
                @newTimestamp,');')
		when @latitude = round(@newLatitude, @round) and @longitude = round(@newLongitude, @round) then
			concat('update Precalc_Locations set EndTime = ',@newTimestamp,', where Id = ',@Id,';')
        else
			concat('insert into Precalc_Locations (FirebaseUserId, Latitude, Longitude, StartTime, EndTime)\nvalues (',
				@newFirebaseUserId,', ',
                format(round(@newLatitude, @round), @round),', ',
                format(round(@newLongitude, @round), @round),', ',
                @newTimestamp,', ',
                @newTimestamp,');')
	 end as 'query';



