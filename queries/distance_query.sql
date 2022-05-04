

-- insert into dac_locationtest values (curdate(), point(41.78467527951608, -72.51998677340622)) -- home
-- insert into dac_locationtest values (curdate(), point(41.691082910246564, -72.76822942301118)) -- school


-- ;with 
-- data as (
-- 	select 
-- 		 id
-- 		,time
-- 		,st_x(location) as latitude
-- 		,st_y(location) as longitude
-- 		,lag(st_x(location), 1, null) over (order by time) as prev_latitude
-- 		,lag(st_y(location), 1, null) over (order by time) as prev_longitude
-- 	from dac_locationtest
-- )
-- ,distance_km as (
-- 	select 
-- 		 *
--          -- https://stackoverflow.com/questions/24370975/find-distance-between-two-points-using-latitude-and-longitude-in-mysql
-- 		,111.111 * DEGREES(ACOS(LEAST(1.0, COS(RADIANS(latitude))
-- 				 * COS(RADIANS(prev_latitude))
-- 				 * COS(RADIANS(longitude - prev_longitude))
-- 				 + SIN(RADIANS(latitude))
-- 				 * SIN(RADIANS(prev_latitude))))) AS distance_in_km
-- 	from data
-- )
-- ,distance_mi as (
-- 	select
-- 		 *
--         ,distance_in_km * 0.621371 as distance_in_mi
--     from distance_km
-- )
-- select *
-- from distance_mi
-- where distance_in_km > 0.001
-- order by id, time
;


-- SELECT * FROM restaurants WHERE ST_Distance_Sphere(location, ST_GeomFromText(?)) <= 10 * 1000 ORDER BY name;


-- most recent distance query:
-- select distinct
-- 	 -- row_number() over (partition by L.id, R.id, cast(L.time as date) order by L.id, R.id) as row_num
--      cast(L.time as date) as date
-- 	,L.id
-- -- 	,L.time
-- -- 	,st_x(L.location) as L_latitude
-- --     ,st_y(L.location) as L_longitude
-- 	,R.id
-- 	-- ,R.time
-- -- 	,st_x(R.location) as R_latitude
-- --     ,st_y(R.location) as R_longitude
-- --     ,round(111.111 * DEGREES(ACOS(LEAST(1.0, COS(RADIANS(st_x(L.location)))
-- --          * COS(RADIANS(st_x(R.location)))
-- --          * COS(RADIANS(st_y(L.location) - st_y(R.location)))
-- --          + SIN(RADIANS(st_x(L.location)))
-- --          * SIN(RADIANS(st_x(R.location)))))), 3) as distance_in_km
-- -- 	,abs(TIMESTAMPDIFF(minute, L.time, R.time)) as time_diff_minutes
-- from
-- 	 dac_locationtest L
-- 	,dac_locationtest R
-- where
-- 		L.id <> R.id
-- 	and (111.111 * DEGREES(ACOS(LEAST(1.0, COS(RADIANS(st_x(L.location)))
--          * COS(RADIANS(st_x(R.location)))
--          * COS(RADIANS(st_y(L.location) - st_y(R.location)))
--          + SIN(RADIANS(st_x(L.location)))
--          * SIN(RADIANS(st_x(R.location))))))) < 1
-- 	and abs(TIMESTAMPDIFF(minute, L.time, R.time)) between 0 and 5
-- order by
-- 	 L.id
-- 	,R.id
--     


SET @rownum = 1;
SET @partition = 0;
;with
data as (
	SELECT
		 Id
		,FirebaseUserId
		,round(Latitude, 4) as Latitude -- round to 10m accuracy (https://en.wikipedia.org/wiki/Decimal_degrees)
		,round(Longitude, 4) as Longitude -- round to 10m accuracy
		,from_unixtime(round(unix_timestamp(date_add(Timestamp, interval -5 hour))/(60*5))*(60*5)) as Timestamp -- convert to EST, rounds to nearest 5 mintues
	FROM capstone.Locations
	where FirebaseUserId = '109409728062644297175'
		-- and Id between 2222 and 2334
    order by FirebaseUserId, Id
)
,lagged_data as (
	select
		 *
		,lag(FirebaseUserId, 1) over (order by FirebaseUserId, Id) as Prev_FirebaseUserId
        ,lag(Latitude, 1) over (order by FirebaseUserId, Id) as Prev_Latitude
        ,lag(Longitude, 1) over (order by FirebaseUserId, Id) as Prev_Longitude
	from data
)
select
	 *
    ,case
		when FirebaseUserId = Prev_FirebaseUserId and Latitude = Prev_Latitude and Longitude = Prev_Longitude then @rownum := @rownum + 1
		else @rownum := 1
	 end rownum
	,case
		when FirebaseUserId = Prev_FirebaseUserId and Latitude = Prev_Latitude and Longitude = Prev_Longitude then @partition := @partition
		else @partition := @partition + 1
	 end 'partition'

from lagged_data
order by FirebaseUserId, Id

