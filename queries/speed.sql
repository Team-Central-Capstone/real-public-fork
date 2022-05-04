
-- select *
-- from Locations
-- where Id in (25000084, 25000085);

-- timestampdiff(microsecond, '2022-01-25 02:46:30.970550', '2022-01-25 02:46:32.125792') / 1000.0 / 1000.0

select *
from (

	select
		 DistanceBetweenPoints(Latitude, Longitude, PrevLatitude, PrevLongitude) / 1000.0 * 0.62137 as DistanceMi
		,timediff(Timestamp, prevTimestamp) as T
        ,TIMESTAMPDIFF(microsecond, prevTimestamp, Timestamp) / 1000.0 / 1000.0 / 3600.0 as hours
		,(DistanceBetweenPoints(Latitude, Longitude, PrevLatitude, PrevLongitude) / 1000.0 * 0.62137) 
			/ (time_to_sec(timediff(Timestamp, prevTimestamp)) / 3600.0) as MpH
        ,(DistanceBetweenPoints(Latitude, Longitude, PrevLatitude, PrevLongitude) / 1000.0 * 0.62137) 
			/ (timestampdiff(microsecond, prevTimestamp, timestamp) / 1000.0 / 1000.0 / 3600.0) as MpH2

        ,Id
        ,prevId
	from (
		select
			 Id
			,lag(Id) over (partition by FirebaseUserId order by FirebaseUserId, Id) as PrevId
			,timestamp
			,lag(Timestamp) over (partition by FirebaseUserId order by FirebaseUserId, Id) as PrevTimestamp
            ,latitude
			,lag(Latitude) over (partition by FirebaseUserId order by FirebaseUserId, Id) as PrevLatitude
            ,longitude
			,lag(Longitude) over (partition by FirebaseUserId order by FirebaseUserId, Id) as PrevLongitude
		from Locations
		where FirebaseUserId = (select FirebaseUserId from Users where Id=15)
		order by Id
	) X
) X
order by MpH2 desc;
