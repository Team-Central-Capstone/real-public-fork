
-- call sp_RecalculateProfileMatch(15);

select lUserId, rUserId, count(*)
from v_UserMatches
where lUserId=15
group by lUserId, rUserId;

select * from Precalc_ProfileMatches where lUserId=15;
           
-- create or replace view v_UserMatches as
-- select
-- 	 l.SurveyQuestionId
-- 	,l.QuestionType
--     ,l.QuestionText
-- 	,l.UserId as 'lUserId'
-- 	,r.UserId as 'rUserId'
--     ,case
-- 		when
-- 			l.SurveyQuestionId=r.SurveyQuestionId
-- 			and (l.SurveyAnswerId=r.SurveyAnswerId or  l.SurveyAnswerResponse=r.SurveyAnswerResponse)
-- 		then 1.0
--      end as ResponsesMatch
-- 	,case
-- 		when l.UserSurveyResponseWeight = r.UserSurveyResponseWeight then 1.0
--         else (6.0 - abs(l.UserSurveyResponseWeight - r.UserSurveyResponseWeight)) / 6.0
--      end as Scale

-- 	,l.SurveyAnswerId as 'lSurveyAnswerId'
-- 	,r.SurveyAnswerId as 'rSurveyAnswerId'
-- 	,l.SurveyAnswerResponse as 'lSurveyAnswerResponse'
--     ,r.SurveyAnswerResponse as 'rSurveyAnswerResponse'
--     ,l.UserSurveyResponseWeight as 'lUserSurveyResponseWeight'
--     ,r.UserSurveyResponseWeight as 'rUserSurveyResponseWeight'
-- from
-- 	(
-- 		select
-- 			 usr.UserId
-- 			,usr.SurveyQuestionId
--             ,sq.QuestionType
-- 			,sq.QuestionText
-- 			,coalesce(usr.SurveyAnswerResponse, sa.AnswerText) as SurveyAnswerResponse
--             ,t.SurveyAnswersId as 'SurveyAnswerId'
-- 			,usr.UserSurveyResponseWeight
-- 		from
-- 			SurveyQuestions sq
-- 			left join UserSurveyResponses usr on usr.SurveyQuestionId=sq.Id
-- 			left join SurveyAnswerUserSurveyResponse t on usr.Id=t.UserSurveyResponsesId
--             left join SurveyAnswers sa on t.SurveyAnswersId=sa.Id
-- 		where
-- 			sq.QuestionType in (2,3,5)
-- 	) l
-- 	left join (
-- 		select
-- 			 usr.UserId
-- 			,usr.SurveyQuestionId
--             ,sq.QuestionType
-- 			,sq.QuestionText
-- 			,coalesce(usr.SurveyAnswerResponse, sa.AnswerText) as SurveyAnswerResponse
-- 			,t.SurveyAnswersId as 'SurveyAnswerId'
-- 			,usr.UserSurveyResponseWeight
-- 		from
-- 			SurveyQuestions sq
-- 			left join UserSurveyResponses usr on usr.SurveyQuestionId=sq.Id
-- 			left join SurveyAnswerUserSurveyResponse t on usr.Id=t.UserSurveyResponsesId
--             left join SurveyAnswers sa on t.SurveyAnswersId=sa.Id
-- 		where
-- 			sq.QuestionType in (2,3,5)
--      ) r on l.UserId != r.UserId
-- 		and l.SurveyQuestionId = r.SurveyQuestionId
-- order by
-- 	 l.UserId
--     ,l.SurveyQuestionId
--     ,l.SurveyAnswerId;

-- test data
-- insert into Precalc_ProfileMatches (lUserId, rUserId, TotalPossibleQuestions, MatchedQuestions, RawMatchPercentage, WeightedMatchPercentage)
-- select
-- 	 lUserId
-- 	,rUserId
--     ,count(*) as TotalPossibleQuestions
--     ,sum(ResponsesMatch) as MatchedQuestions
--     ,(count(ResponsesMatch)) / count(*) as RawMatchPercentage
--     ,sum(case when ResponsesMatch=1 then Scale else 0 end) / count(*) as WeightedMatchPercentage
-- from v_UserMatches
-- where lUserId=15 or rUserId=15
-- group by
-- 	 lUserId
-- 	,rUserId
-- order by RawMatchPercentage desc;

-- select * from Precalc_ProfileMatches where lUserId=15 or rUserId=15;


-- drop table if exists Precalc_ProfileMatches;
-- CREATE TABLE `Precalc_ProfileMatches` (
-- 	`Id` bigint NOT NULL AUTO_INCREMENT,
-- 	`lUserId` int NOT NULL,
-- 	`rUserId` int NOT NULL,
-- 	`TotalPossibleQuestions` bigint NOT NULL,
-- 	`MatchedQuestions` int NOT NULL,
-- 	`RawMatchPercentage` numeric(9,8) NOT NULL,
-- 	`WeightedMatchPercentage` numeric(9,8) not null,
-- 	`Timestamp` datetime not null default current_timestamp(),
-- 	PRIMARY KEY (`Id`),
-- 	INDEX `IX_Precalc_ProfileMatches_Id` (`Id`),
-- 	INDEX `IX_Precalc_ProfileMatches_UserKeys` (`lUserId`, `rUserId`),
-- 	CONSTRAINT `FK_Precalc_ProfileMatches_lUserId` FOREIGN KEY (`lUserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
-- 	CONSTRAINT `FK_Precalc_ProfileMatches_rUserId` FOREIGN KEY (`rUserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
-- );

-- drop procedure if exists sp_RecalculateProfileMatch;

-- DELIMITER $$
-- CREATE DEFINER=`admin`@`%` PROCEDURE `sp_RecalculateProfileMatch`(userId int)
-- begin
-- 	delete from Precalc_ProfileMatches where lUserId = userId or rUserId = userId;
--     
--     insert into Precalc_ProfileMatches (lUserId, rUserId, TotalPossibleQuestions, MatchedQuestions, RawMatchPercentage, WeightedMatchPercentage)
-- 	select
-- 		 lUserId
-- 		,rUserId
-- 		,count(*) as TotalPossibleQuestions
-- 		,sum(ResponsesMatch) as MatchedQuestions
-- 		,(count(ResponsesMatch)) / count(*) as RawMatchPercentage
-- 		,sum(case when ResponsesMatch=1 then Scale else 0 end) / count(*) as WeightedMatchPercentage
-- 	from v_UserMatches
-- 	where lUserId = userId
-- 	group by
-- 		 lUserId
-- 		,rUserId
-- 	union all
--     select
-- 		 rUserId
-- 		,lUserId
-- 		,count(*) as TotalPossibleQuestions
-- 		,sum(ResponsesMatch) as MatchedQuestions
-- 		,(count(ResponsesMatch)) / count(*) as RawMatchPercentage
-- 		,sum(case when ResponsesMatch=1 then Scale else 0 end) / count(*) as WeightedMatchPercentage
-- 	from v_UserMatches
-- 	where lUserId = userId
-- 	group by
-- 		 lUserId
-- 		,rUserId;
-- end$$
-- DELIMITER ;


-- DROP TRIGGER IF EXISTS `capstone`.`UserSurveyResponses_AFTER_INSERT`;

-- DELIMITER $$
-- USE `capstone`$$
-- CREATE DEFINER = CURRENT_USER TRIGGER `capstone`.`UserSurveyResponses_AFTER_INSERT` AFTER INSERT ON `UserSurveyResponses` FOR EACH ROW
-- BEGIN
-- 	call sp_RecalculateProfileMatch(new.UserId);
-- END$$
-- DELIMITER ;


-- DROP TRIGGER IF EXISTS `capstone`.`UserSurveyResponses_AFTER_UPDATE`;

-- DELIMITER $$
-- USE `capstone`$$
-- CREATE DEFINER = CURRENT_USER TRIGGER `capstone`.`UserSurveyResponses_AFTER_UPDATE` AFTER UPDATE ON `UserSurveyResponses` FOR EACH ROW
-- BEGIN
-- 	call sp_RecalculateProfileMatch(new.UserId);
-- END$$
-- DELIMITER ;
