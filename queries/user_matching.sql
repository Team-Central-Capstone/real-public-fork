
;with
data as (
	select
		 u.FirstName
		,u.LastName
		,sq.QuestionText
		,coalesce(usr.surveyanswerresponse, sa.answertext) as 'Response'
		,usr.UserSurveyResponseWeight
		
		,u.id as UserId
		,sq.id as SurveyQuestionId
	from
		SurveyQuestions sq
		left join UserSurveyResponses usr on sq.Id = usr.SurveyQuestionId
		left join Users u on usr.UserId = u.id
		left join SurveyAnswerUserSurveyResponse m on usr.id = m.usersurveyresponsesid
		left join SurveyAnswers sa on m.surveyanswersid = sa.id
	order by
		 u.Id
        ,coalesce(usr.surveyanswerresponse, sa.answertext)
)
,matches as (
	select
		 l.UserId as 'L_UserId'
		,r.UserId as 'R_UserId'
		,sum(case when l.Response=r.Response then 1 else 0 end) as 'Matches'
        ,sum(1) as 'PossibleMatches'
        ,sum(case when l.Response=r.Response then l.UserSurveyResponseWeight else 0 end) as 'WeightedMatches'
        ,sum(l.UserSurveyResponseWeight) as 'Weights'
	from
		data l
		left join data r on l.UserId != r.UserId
						and l.SurveyQuestionId = r.SurveyQuestionId
	group by
		 l.UserId
		,r.UserId
)
,scores as (
	select 
		 *
		,case when PossibleMatches is not null then Matches / PossibleMatches else 0 end as 'Score'
		,case when Weights is not null then WeightedMatches / Weights else 0 end as 'WeightedScore'
	from matches
)
select * from scores