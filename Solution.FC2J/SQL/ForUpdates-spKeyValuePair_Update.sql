CREATE PROCEDURE [dbo].[spKeyValuePair_Update]
	@Key NVARCHAR(50), 
	@Value NVARCHAR(150) 
	
AS
BEGIN
	
	SET NOCOUNT ON;

	UPDATE [KeyValuePair] SET [Value] = @Value WHERE [Key] = @Key

END


