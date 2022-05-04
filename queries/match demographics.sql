select um.*
	,u1.Birthdate as lBirthdate
	,ug1.Name as lGender
    ,ub1.Name as lBodyType
	,u2.Birthdate as rBirthdate
	,ug2.Name as rGender
    ,ub2.Name as rBodyType
from
	v_UserMatches um
    
    left join Users u1 on um.lUserId = u1.Id
    left join UserGenders ug1 on u1.UserGenderId=ug1.Id
    left join UserBodyTypes ub1 on u1.UserBodyTypeId=ub1.Id
    
    left join Users u2 on um.rUserId = u2.Id
	left join UserGenders ug2 on u2.UserGenderId=ug2.Id
    left join UserBodyTypes ub2 on u2.UserBodyTypeId=ub2.Id
where um.lUserId=12
order by um.lUserId, um.rUserId, um.SurveyQuestionId

