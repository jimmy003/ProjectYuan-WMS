CREATE PROCEDURE [dbo].[spEvent_GetList_FromSqlProject]
AS
BEGIN	
	
		SELECT TOP 1000 [Id]
			  ,[EventName]
			  ,[EventDate]
			  ,[UserName]
		  FROM [PROJECT.FC2J].[dbo].[Event]


END
