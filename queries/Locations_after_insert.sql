
select
	 pl.Id
	,pl.FirebaseUserId
    ,pl.Latitude
    ,pl.Longitude
    ,pl.StartTime
    ,pl.EndTime
    ,pl.trace
	,count(l.Id) as LocationRecords
from
	Precalc_Locations pl
    inner join Locations l on   pl.firebaseuserid=l.firebaseuserid
							and l.timestamp between pl.starttime and pl.endtime
where pl.FirebaseUserId = (select FirebaseUserId from Users where Id=15)
group by
	 pl.Id
	,pl.FirebaseUserId
    ,pl.Latitude
    ,pl.Longitude
    ,pl.StartTime
    ,pl.EndTime
    ,pl.trace
order by pl.Id;




-- delimiter $$

-- drop trigger if exists Locations_after_insert $$

-- create trigger Locations_after_insert
-- after insert
-- on Locations for each row
-- begin
-- 	select Value into @round from AppSettings where Setting = 'GPSRoundDecimalPlaces';
--     
-- 	select l.Id, l.Latitude, l.Longitude
-- 	into @id, @latitude, @longitude
--     from Precalc_Locations l
--     where FirebaseUserId = new.FirebaseUserId
--     order by Id desc
--     limit 1;
--     
--     set @trace := concat('(@id, @latitude, @longitude) = (', coalesce(@id, 'NULL'), ',', coalesce(@latitude, 'NULL'), ',', coalesce(@longitude, 'NULL'), ')');
--     
--     if @id is null then
--     -- first record for user
-- 		set @trace := concat(@trace, '\ninsert into Precalc_Locations (FirebaseUserId, Latitude, Longitude, StartTime, EndTime, trace)\nvalues (',new.FirebaseUserId,', ',round(new.Latitude, @round),', ',round(new.Longitude, @round),', ',new.Timestamp,', ',new.Timestamp,', @trace);');
--         
-- 		insert into Precalc_Locations (FirebaseUserId, Latitude, Longitude, StartTime, EndTime, trace)
-- 		values (new.FirebaseUserId, round(new.Latitude, @round), round(new.Longitude, @round), new.Timestamp, new.Timestamp, @trace);
--     else
-- 		if @latitude = round(new.Latitude, @round) and @longitude = round(new.Longitude, @round) then
-- 			-- update existing time if still at same location
-- 			set @trace := concat(@trace, '\nupdate Precalc_Locations\nset EndTime = ',new.Timestamp,', trace = @trace\nwhere Id = ',@Id,';');
-- 			update Precalc_Locations
-- 			set EndTime = new.Timestamp, trace = @trace
--             where Id = @Id;
-- 		else
-- 			-- create new roll up location
-- 			set @trace := concat(@trace, '\ninsert into Precalc_Locations (FirebaseUserId, Latitude, Longitude, StartTime, EndTime, trace)\nvalues (',new.FirebaseUserId,', ',round(new.Latitude, @round),', ',round(new.Longitude, @round),', ',new.Timestamp,', ',new.Timestamp,', @trace);');
-- 			
--             insert into Precalc_Locations (FirebaseUserId, Latitude, Longitude, StartTime, EndTime, trace)
-- 			values (new.FirebaseUserId, round(new.Latitude, @round), round(new.Longitude, @round), new.Timestamp, new.Timestamp, @trace);
--         end if;
--     end if;
--     
-- end $$

-- delimiter ;


-- '109409728062644297175'



-- drop table if exists Precalc_Locations;
-- CREATE TABLE `Precalc_Locations` (
--   `Id` bigint NOT NULL AUTO_INCREMENT,
--   `FirebaseUserId` varchar(34) NOT NULL,
--   `Latitude` double NOT NULL,
--   `Longitude` double NOT NULL,
--   `StartTime` datetime(6) NOT NULL,
--   `EndTime` datetime(6) NOT NULL,
--   `trace` longtext,
--   PRIMARY KEY (`Id`),
--   KEY `IX_Locations_Rollup_Id` (`Id`),
--   KEY `IX_Locations` (`FirebaseUserId`,`Latitude`,`Longitude`,`StartTime`,`EndTime`)
-- );
