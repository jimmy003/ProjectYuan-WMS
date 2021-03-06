USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[GetSalesReportMonthlyBIR]    Script Date: 05/17/2020 4:17:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetSalesReportMonthlyBIR] 
@IsVatable bit = 0,
@DateFrom datetime,
@DateTo datetime

AS BEGIN
	set nocount on;	
	
	declare @date_To as datetime = getdate()
	declare @date_From as datetime 
	set @date_From = dateadd(month,-1, @date_To);

	set @date_From = @DateFrom
	set @date_To = @DateTo

	-- this is the temp table to use to store the recordsets
	-- NAME / STORE	S.I NO#	AMOUNT
	
	create table #sales (CustomerName varchar(50), PONo varchar(50), Amount money)

	Declare @Id bigint
	Declare @Name varchar(50)
	DECLARE @SQL AS NVARCHAR(MAX) = ''			

	-- this will get target customers based on the parameters
	DECLARE db_cursor CURSOR FOR 
	SELECT c.Id
		FROM CUSTOMER c
		WHERE (Deleted = 0 or Deleted is null) 

	OPEN db_cursor  
	FETCH NEXT FROM db_cursor INTO @Id
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		
		IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'sale.order.header' AND  TABLE_NAME = convert(varchar,@Id)))
		BEGIN
			SET @SQL='
			
				Insert into #sales
			
				select CustomerName, PONo, TotalPrice from [sale.order.header].['+convert(varchar,@Id)+']
						where OrderDate 
							Between Convert(datetime,'''+convert(varchar, @date_From, 101)+''' + '' 00:00:00.000'') And Convert(datetime,'''+convert(varchar, @date_To, 101)+''' + '' 23:59:59.999'')	
						and OrderStatusId not in (1,3) 		
						and IsVatable = Convert(bit,'''+Convert(varchar,@IsVatable)+''') 		
		
				'		
			EXEC (@SQL)
		END 		
		FETCH NEXT FROM db_cursor INTO @Id

	END 
	CLOSE db_cursor  
	DEALLOCATE db_cursor 


	select * from #sales order by PONo

	DROP TABLE #sales
END