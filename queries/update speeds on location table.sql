
-- select *
-- -- 	,lag(Timestamp) over (partition by FirebaseUserId order by Timestamp) as prevTimestamp
-- -- 	,lag(Latitude) over (partition by FirebaseUserId order by Timestamp) as prevLatitude
-- -- 	,lag(Longitude) over (partition by FirebaseUserId order by Timestamp) as prevLongitude
-- 	,abs((DistanceBetweenPoints(Latitude, Longitude, lag(Latitude) over (partition by FirebaseUserId order by Timestamp), lag(Longitude) over (partition by FirebaseUserId order by Timestamp)) / 1000.0 * 0.62137) / (timestampdiff(microsecond, lag(Timestamp) over (partition by FirebaseUserId order by Timestamp), timestamp) / 1000.0 / 1000.0 / 3600.0)) as MpH
-- from (
-- 	select *
-- 	from Locations
--     where FirebaseUserId='109409728062644297175'
-- 	order by FirebaseUserId, Timestamp
-- 	limit 1000
-- ) x


-- select distinct FirebaseUserId from Locations;





select FirebaseUserId, sum(case when SpeedFromLast > 0 then 1 else 0 end)
from Locations
group by FirebaseUserId;