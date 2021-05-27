CREATE PROCEDURE [dbo].[spKeyValuePair_Insert]
	@Key NVARCHAR(50), 
	@Value NVARCHAR(150) ,
	@Id INT = 0 output

AS
BEGIN

	SET NOCOUNT ON;

	INSERT INTO [KeyValuePair]
           (
			[Key] ,
			[Value] 
		   ) 	
    VALUES
           (
			@Key, 
			@Value 
			)	
			
	select @Id = SCOPE_IDENTITY();


END 

-- to execute 
-- to generate initial dataset 
/*
INSERT INTO [KeyValuePair] ( [Key] , [Value] ) 	
	VALUES ('PeriodNumber', '1'),
		   ('Period', 'Week')

select * from [KeyValuePair]
*/

