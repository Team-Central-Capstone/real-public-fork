
-- set @newfid := '6QUsr0aeWih0Z3Zb7ibbBPLjEk73';
-- set @newfid := 'nCBkw9r9ItcpEc9KAvd2llgfmeJ3';

-- select * from UserMatches order by Id desc limit 2;
-- select (select Id from Users where FirebaseUserId = '6QUsr0aeWih0Z3Zb7ibbBPLjEk73'), (select Id from Users where FirebaseUserId = 'nCBkw9r9ItcpEc9KAvd2llgfmeJ3');

select *
from UserMatches
where
	(
		UserId1 = (select Id from Users where FirebaseUserId = '6QUsr0aeWih0Z3Zb7ibbBPLjEk73')
        and
        UserId2 = (select Id from Users where FirebaseUserId = 'nCBkw9r9ItcpEc9KAvd2llgfmeJ3')
    )
    or
    (
		UserId2 = (select Id from Users where FirebaseUserId = '6QUsr0aeWih0Z3Zb7ibbBPLjEk73')
        and
        UserId1 = (select Id from Users where FirebaseUserId = 'nCBkw9r9ItcpEc9KAvd2llgfmeJ3')
    )
;




-- set @fid := '7dc4da1bad2611ecab9402d193d5fff8';
-- set @newfid := '6QUsr0aeWih0Z3Zb7ibbBPLjEk73';
-- set @fid := '7dc7193fad2611ecab9402d193d5fff8';
-- set @newfid := 'nCBkw9r9ItcpEc9KAvd2llgfmeJ3';


set @id := (select Id from Users where FirebaseUserId = @fid);
select @fid, @id;


start transaction;

update Users set FirebaseUserId=null where FirebaseUserId=@newfid;

insert into Users (
    `RegisteredTimestamp`,
    `FirebaseUserId`,
    `Active`,
    `Birthdate`,
    `FirstName`,
    `LastLoginTimestamp`,
    `LastName`,
    `PreferredName`,
    `ProfileLastUpdatedTimestamp`,
    `UserGenderId`,
    `UserBodyTypeId`,
    `HeightInches`,
    `ProfileIntro`,
    `Email`
)
SELECT
    `Users`.`RegisteredTimestamp`,
    @newfid,
    `Users`.`Active`,
    `Users`.`Birthdate`,
    `Users`.`FirstName`,
    `Users`.`LastLoginTimestamp`,
    `Users`.`LastName`,
    `Users`.`PreferredName`,
    `Users`.`ProfileLastUpdatedTimestamp`,
    `Users`.`UserGenderId`,
    `Users`.`UserBodyTypeId`,
    `Users`.`HeightInches`,
    `Users`.`ProfileIntro`,
    `Users`.`Email`
FROM `capstone`.`Users`
where FirebaseUserId = @fid;

set @newid := (select Id from Users where Id=last_insert_id());
select @newfid, @newid;


insert into Locations (
	`Locations`.`Timestamp`,
    `Locations`.`FirebaseUserId`,
    `Locations`.`Latitude`,
    `Locations`.`Longitude`,
    `Locations`.`Source`,
    `Locations`.`DeviceID`,
    `Locations`.`RollingAverageSpeed`,
    `Locations`.`SpeedFromLast`
)
SELECT
    `Locations`.`Timestamp`,
    @newfid,
    `Locations`.`Latitude`,
    `Locations`.`Longitude`,
    `Locations`.`Source`,
    `Locations`.`DeviceID`,
    `Locations`.`RollingAverageSpeed`,
    `Locations`.`SpeedFromLast`
FROM `capstone`.`Locations`
where FirebaseUserId = @fid;

select *
FROM `capstone`.`Locations`
where FirebaseUserId = @newfid;

insert into Precalc_Locations(
	`Precalc_Locations`.`FirebaseUserId`,
    `Precalc_Locations`.`Latitude`,
    `Precalc_Locations`.`Longitude`,
    `Precalc_Locations`.`StartTime`,
    `Precalc_Locations`.`EndTime`,
    `Precalc_Locations`.`trace`
)
SELECT
    @newfid,
    `Precalc_Locations`.`Latitude`,
    `Precalc_Locations`.`Longitude`,
    `Precalc_Locations`.`StartTime`,
    `Precalc_Locations`.`EndTime`,
    `Precalc_Locations`.`trace`
FROM `capstone`.`Precalc_Locations`
where FirebaseUserId = @fid;

select *
FROM `capstone`.`Precalc_Locations`
where FirebaseUserId = @newfid;


insert into Precalc_ProfileMatches (
	`Precalc_ProfileMatches`.`lUserId`,
    `Precalc_ProfileMatches`.`rUserId`,
    `Precalc_ProfileMatches`.`TotalPossibleQuestions`,
    `Precalc_ProfileMatches`.`MatchedQuestions`,
    `Precalc_ProfileMatches`.`RawMatchPercentage`,
    `Precalc_ProfileMatches`.`WeightedMatchPercentage`,
    `Precalc_ProfileMatches`.`Timestamp`
)
SELECT
    @newid,
    `Precalc_ProfileMatches`.`rUserId`,
    `Precalc_ProfileMatches`.`TotalPossibleQuestions`,
    `Precalc_ProfileMatches`.`MatchedQuestions`,
    `Precalc_ProfileMatches`.`RawMatchPercentage`,
    `Precalc_ProfileMatches`.`WeightedMatchPercentage`,
    `Precalc_ProfileMatches`.`Timestamp`
FROM `capstone`.`Precalc_ProfileMatches`
where lUserId = @id;
insert into Precalc_ProfileMatches (
	`Precalc_ProfileMatches`.`lUserId`,
    `Precalc_ProfileMatches`.`rUserId`,
    `Precalc_ProfileMatches`.`TotalPossibleQuestions`,
    `Precalc_ProfileMatches`.`MatchedQuestions`,
    `Precalc_ProfileMatches`.`RawMatchPercentage`,
    `Precalc_ProfileMatches`.`WeightedMatchPercentage`,
    `Precalc_ProfileMatches`.`Timestamp`
)
SELECT
    `Precalc_ProfileMatches`.`lUserId`,
    @newid,
    `Precalc_ProfileMatches`.`TotalPossibleQuestions`,
    `Precalc_ProfileMatches`.`MatchedQuestions`,
    `Precalc_ProfileMatches`.`RawMatchPercentage`,
    `Precalc_ProfileMatches`.`WeightedMatchPercentage`,
    `Precalc_ProfileMatches`.`Timestamp`
FROM `capstone`.`Precalc_ProfileMatches`
where rUserId = @id;

select *
FROM `capstone`.`Precalc_ProfileMatches`
where lUserId = @newid;

select *
FROM `capstone`.`Precalc_ProfileMatches`
where rUserId = @newid;

insert into UserSurveyResponses (
    `UserSurveyResponses`.`UserId`,
    `UserSurveyResponses`.`SurveyQuestionId`,
    `UserSurveyResponses`.`SurveyAnswerResponse`,
    `UserSurveyResponses`.`UserSurveyResponseWeight`
)
SELECT
    @newid,
    `UserSurveyResponses`.`SurveyQuestionId`,
    `UserSurveyResponses`.`SurveyAnswerResponse`,
    `UserSurveyResponses`.`UserSurveyResponseWeight`
FROM `capstone`.`UserSurveyResponses`
where UserId = @id;

select *
FROM `capstone`.`UserSurveyResponses`
where UserId = @newid;

insert into SurveyAnswerUserSurveyResponse(
	`SurveyAnswerUserSurveyResponse`.`SurveyAnswersId`,
    `SurveyAnswerUserSurveyResponse`.`UserSurveyResponsesId`
)
SELECT 
	`SurveyAnswerUserSurveyResponse`.`SurveyAnswersId`,
    @newid
FROM `capstone`.`SurveyAnswerUserSurveyResponse`
where UserSurveyResponsesId in (select Id from UserSurveyResponses where UserId = @newid);

select *
FROM `capstone`.`SurveyAnswerUserSurveyResponse`
where UserSurveyResponsesId in (select Id from UserSurveyResponses where UserId = @newid);

insert into UserAttractedGenders (
	`UserAttractedGenders`.`GendersAttractedToId`,
    `UserAttractedGenders`.`UserAttractedToId`
)
SELECT 
	`UserAttractedGenders`.`GendersAttractedToId`,
    @newid
FROM `capstone`.`UserAttractedGenders`
where UserAttractedToId=@id;

select *
FROM `capstone`.`UserAttractedGenders`
where UserAttractedToId=@newid;

insert into UserMatches (
    `UserMatches`.`UserId1`,
    `UserMatches`.`UserId2`,
    `UserMatches`.`MatchedOnDate`,
    `UserMatches`.`User1AcceptedDate`,
    `UserMatches`.`User2AcceptedDate`,
    `UserMatches`.`MatchedLatitude`,
    `UserMatches`.`MatchedLongitude`,
    `UserMatches`.`RawMatchPercentage`,
    `UserMatches`.`WeightedMatchPercentage`
)
SELECT
    @newid,
    `UserMatches`.`UserId2`,
    `UserMatches`.`MatchedOnDate`,
    `UserMatches`.`User1AcceptedDate`,
    `UserMatches`.`User2AcceptedDate`,
    `UserMatches`.`MatchedLatitude`,
    `UserMatches`.`MatchedLongitude`,
    `UserMatches`.`RawMatchPercentage`,
    `UserMatches`.`WeightedMatchPercentage`
FROM `capstone`.`UserMatches`
where UserId1 = @id;
insert into UserMatches (
    `UserMatches`.`UserId1`,
    `UserMatches`.`UserId2`,
    `UserMatches`.`MatchedOnDate`,
    `UserMatches`.`User1AcceptedDate`,
    `UserMatches`.`User2AcceptedDate`,
    `UserMatches`.`MatchedLatitude`,
    `UserMatches`.`MatchedLongitude`,
    `UserMatches`.`RawMatchPercentage`,
    `UserMatches`.`WeightedMatchPercentage`
)
SELECT
    `UserMatches`.`UserId1`,
    @newid,
    `UserMatches`.`MatchedOnDate`,
    `UserMatches`.`User1AcceptedDate`,
    `UserMatches`.`User2AcceptedDate`,
    `UserMatches`.`MatchedLatitude`,
    `UserMatches`.`MatchedLongitude`,
    `UserMatches`.`RawMatchPercentage`,
    `UserMatches`.`WeightedMatchPercentage`
FROM `capstone`.`UserMatches`
where UserId2 = @id;

select *
FROM `capstone`.`UserMatches`
where UserId1 = @id;

select *
FROM `capstone`.`UserMatches`
where UserId2 = @id;

commit;
