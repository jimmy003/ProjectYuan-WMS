USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[GetSales]    Script Date: 05/16/2020 1:15:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetSales]
@UserName varchar(50) = ''

AS BEGIN
	
	set nocount on;

	declare @date_To as datetime = getdate()
	declare @date_From as datetime 
	set @date_From = dateadd(month,-1, @date_To)

	DECLARE @SQL AS NVARCHAR(MAX) = ''			
	DECLARE @CustomerId bigint
	DECLARE @Id varchar(50)
	DECLARE db_cursor CURSOR FOR 

	SELECT Id
		FROM Customer
		WHERE [Deleted]=0;

	OPEN db_cursor  
	FETCH NEXT FROM db_cursor INTO @CustomerId 

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		
		Set @Id = Convert(varchar,@CustomerId);

		IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'sale.order.header' AND  TABLE_NAME = @Id))
			BEGIN 

				if @SQL <> ''
					BEGIN 
						SET @SQL = @SQL +char(13)+char(10)+ ' UNION '
					END 

				if @UserName = 'admin'
					BEGIN

							SET @SQL = @SQL +char(13)+char(10)+ 'Select h'+@Id+'.* From [sale.order.header].['+@Id+'] h'+@Id+' 
																	Where (h'+@Id+'.[Deleted] = 0 Or h'+@Id+'.[Deleted] is null) 
																		AND OrderStatusId <=6
																		AND OrderDate Between Convert(datetime,'''+convert(varchar, @date_From, 101)+''' + '' 00:00:00.000'') And Convert(datetime,'''+convert(varchar, @date_To, 101)+''' + '' 23:59:59.999'')'
					END
				else
					BEGIN
							SET @SQL = @SQL +char(13)+char(10)+ 'Select h'+@Id+'.* From [sale.order.header].['+@Id+'] h'+@Id+' 
																Where h'+@Id+'.[UserName] = '''+@UserName+''' 
																	AND (h'+@Id+'.[Deleted] = 0 Or h'+@Id+'.[Deleted] is null)  
																	AND OrderStatusId <=6
																	AND OrderDate Between Convert(datetime,'''+convert(varchar, @date_From, 101)+''' + '' 00:00:00.000'') And Convert(datetime,'''+convert(varchar, @date_To, 101)+''' + '' 23:59:59.999'')'
					END

			END

		FETCH NEXT FROM db_cursor INTO @CustomerId

	END 

	CLOSE db_cursor  
	DEALLOCATE db_cursor 

	Set @SQL = @SQL +char(13)+char(10)+ 'ORDER BY SubmittedDate DESC '
	exec(@SQL)


END


