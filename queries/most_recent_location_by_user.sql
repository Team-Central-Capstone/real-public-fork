-- show grants for 'admin'@'%'
-- GRANT ALL PRIVILEGES ON capstone.* TO 'admin'@'%' with grant option;
-- GRANT FILE ON *.* TO 'admin'@'%';

select
	 L.*
    ,ST_Distance_Sphere(point(Lat1, Lon1), point(Latitude, Longitude)) as 'OldDistance'
	,DistanceBetweenPoints(Lat1, Lon1, Latitude, Longitude) as 'mDistance'
from
	Locations L
	inner join (
		select s1.FirebaseUserId, 41.7832245299161 as 'Lat1', -72.52338396778168 as 'Lon1', max(Timestamp) as Timestamp
		from Locations s1
		group by s1.FirebaseUserId
    ) L2 on L.FirebaseUserId=L2.FirebaseUserId and L.Timestamp=L2.Timestamp
order by DistanceBetweenPoints(Lat1, Lon1, Latitude, Longitude)
-- order by ST_Distance_Sphere(point(Lat1, Lon1), point(Latitude, Longitude))


