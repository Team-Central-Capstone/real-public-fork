

select *
from (
	select
		 l.FirebaseUserId as 'l.Id'
		,l.StartTime as 'l.StartTime'
		,l.EndTime as 'l.EndTime'
		,r.StartTime as 'r.StartTime'
		,r.EndTime as 'r.EndTime'
		,r.FirebaseUserId as 'r.Id'
		,@start := case
			when l.StartTime <= r.StartTime and l.EndTime <= r.EndTime then r.StartTime  -- 'left partial'
			when r.StartTime <= l.StartTime and r.EndTime <= l.EndTime then l.StartTime -- 'right partial'
			when r.StartTime < l.StartTime and l.EndTime < r.EndTime then l.StartTime -- 'right contains left'
			when l.StartTime < r.StartTime and r.EndTime < l.EndTime then r.StartTime -- 'left contains right'
            when l.StartTime = r.StartTime and l.EndTime = r.EndTime then l.StartTime -- 'equal'
		 end as 'start_time'
		,@end := case
			when l.StartTime <= r.StartTime and l.EndTime <= r.EndTime then l.EndTime -- 'left partial'
			when r.StartTime <= l.StartTime and r.EndTime <= l.EndTime then r.EndTime -- 'right partial'
			when r.StartTime < l.StartTime and l.EndTime < r.EndTime then l.EndTime -- 'right contains left'
			when l.StartTime < r.StartTime and r.EndTime < l.EndTime then r.EndTime -- 'left contains right'
            when l.StartTime = r.StartTime and l.EndTime = r.EndTime then l.EndTime -- 'equal'
		 end as 'end_time'
		,cast(time_format(timediff(@end,@start),'%H') as signed) as 'H'
		,cast(time_format(timediff(@end,@start),'%i') as signed) as 'M'
		,cast(time_format(timediff(@end,@start),'%s') as signed) as 'S'
	from
		Precalc_Locations l
		inner join Precalc_Locations r
									on l.FirebaseUserId != r.FirebaseUserId
									and (
										   date_add(l.StartTime, interval (timestampdiff(second, l.StartTime, l.EndTime) / 2) second) between r.StartTime and r.EndTime
										or date_add(r.StartTime, interval (timestampdiff(second, r.StartTime, r.EndTime) / 2) second) between l.StartTime and l.EndTime
									)
	where l.FirebaseUserId = (select FirebaseUserId from Users where id=15)
	order by l.FirebaseUserId, r.FirebaseUserId
) x
