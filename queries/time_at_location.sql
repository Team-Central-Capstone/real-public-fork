
-- select * from Locations_Processed;

set @now := current_timestamp();
set @round := 4;
-- insert into Locations_Processed
with
basedata as (
	select
		 FirebaseUserId
		,Timestamp
		,cast(round(latitude, @round) as float) as Latitude
		,cast(round(longitude, @round) as float) as Longitude
	
		,lag(FirebaseUserId, 1) over () as PrevFirebaseUserId
        ,lag(cast(round(Latitude, @round) as float), 1) over () as PrevLatitude
        ,lag(cast(round(Longitude, @round) as float), 1) over () as PrevLongitude
        
	from Locations
	where firebaseuserid = 'testuser-0000001' -- = 'testuser-0000001' -- ='109409728062644297175' -- runs in 0.053 seconds
    -- where Timestamp = '2022-02-09 02:27:48.732288'
    order by FirebaseUserId, Timestamp
)
,ranks as (
	select *
		,case
			when PrevFirebaseUserId is null then (select @n := 1)
			when (FirebaseUserId = PrevFirebaseUserId) and (Latitude = PrevLatitude) and (Longitude = PrevLongitude) then @n := @n + 1
			else @n := 1
		end as n
		,case
			when PrevFirebaseUserId is null then (select @r := 1)
			when (FirebaseUserId = PrevFirebaseUserId) and (Latitude = PrevLatitude) and (Longitude = PrevLongitude) then @r := @r
			else @r := @r + 1
		end as r
		from basedata
		order by FirebaseUserId, Timestamp
)
,maxrowperrank as (
	select
		 FirebaseUserId
		,cast(r as signed) as r
        ,cast(max(n) as signed) as n
    from ranks
    group by FirebaseUserId, r
    having cast(max(n) as signed) > 1
)
,minandmaxperrank as (
	select
		 ranks.*
		,case when ranks.n != 1 then lag(Timestamp, 1) over (partition by ranks.FirebaseUserId order by ranks.FirebaseUserId, ranks.r, ranks.n) end as 'PrevTimestamp'
    from
		ranks
        inner join maxrowperrank on ranks.FirebaseUserId=maxrowperrank.FirebaseUserId
								and ranks.r=maxrowperrank.r
                                and (ranks.n=maxrowperrank.n or ranks.n=1)
	order by ranks.FirebaseUserId, cast(ranks.r as signed), cast(ranks.n as signed)
)
select
	 @now
	,FirebaseUserId
	,Latitude
    ,Longitude
    ,PrevTimestamp as 'StartTime'
    ,Timestamp as 'EndTime'
	,cast(time_format(timediff(Timestamp, PrevTimestamp),'%H') as signed) as 'H'
	,cast(time_format(timediff(Timestamp, PrevTimestamp),'%i') as signed) as 'M'
	,cast(time_format(timediff(Timestamp, PrevTimestamp),'%s') as signed) as 'S'
    ,n as 'RecordsAtLocation'
from minandmaxperrank
where PrevTimestamp is not null;



-- CREATE TABLE `Locations_Processed` (
--   `ProcessedTimestamp` datetime not null default current_timestamp(),
--   `FirebaseUserId` varchar(34) NOT NULL,
--   `Latitude` double NOT NULL,
--   `Longitude` double NOT NULL,
--   `StartTime` datetime not null,
--   `EndTime` datetime not null,
--   H int not null,
--   M int not null,
--   S int not null,
--   RecordsAtLocation int not null,
--   `Timestamp` datetime not null default current_timestamp()
-- ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
