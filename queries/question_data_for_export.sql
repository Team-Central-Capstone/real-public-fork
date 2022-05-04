
select
	 u.Id as 'UserId'
	,u.Birthdate
    ,timestampdiff(year, u.Birthdate, current_timestamp()) as 'Age'
    ,u.PreferredName
    ,u.LastName
    ,ug.Name as 'UserGender'
    ,group_concat(distinct uag_ug.Name order by uag_ug.Id separator '|') as GendersAttractedTo
    ,ubt.Name as 'UserBodyType'
    ,u.HeightInches
    ,sq.Id as 'QuestionId'
    ,sq.QuestionText
	,usr.UserSurveyResponseWeight
    ,group_concat(distinct coalesce(usr.SurveyAnswerResponse, sa.AnswerText) separator '|') as SurveyAnswerResponse
from
	Users u
    left join UserGenders ug on u.UserGenderId = ug.Id
    
	left join UserAttractedGenders uag on u.Id = uag.UserAttractedToId
    left join UserGenders uag_ug on uag.GendersAttractedToId = uag_ug.Id
    
    left join UserBodyTypes ubt on u.UserBodyTypeId = ubt.Id
    inner join UserSurveyResponses usr on u.Id = usr.UserId
    inner join SurveyQuestions sq on usr.SurveyQuestionId = sq.Id
    left join SurveyAnswerUserSurveyResponse sausr on usr.Id = sausr.UserSurveyResponsesId
    left join SurveyAnswers sa on sausr.SurveyAnswersId = sa.Id
group by
	 u.Id
	,u.Birthdate
    ,u.PreferredName
    ,u.LastName
    ,ug.Name
    ,ubt.Name
    ,u.HeightInches
    ,sq.Id
    ,sq.QuestionText
	,usr.UserSurveyResponseWeight
order by sq.Id, u.Id;