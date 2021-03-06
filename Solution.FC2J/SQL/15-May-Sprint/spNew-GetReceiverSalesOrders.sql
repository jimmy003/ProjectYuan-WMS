USE [PROJECT.FC2J]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetReceiverSalesOrders]

AS BEGIN
	
	set nocount on;

	-- this is the temp table to use to store the recordsets
	create table #table 
	(
			[Date] nvarchar(12),
			[Partner] nvarchar(120),			
			[SoNo] nvarchar(20),
			[Amount] money,
			[DueDate] datetime
	)

	DECLARE @SQL AS NVARCHAR(MAX) = ''			
	DECLARE @CustomerId bigint
	DECLARE @PoHeaderId bigint
	DECLARE db_cursor CURSOR FOR 	

	SELECT CustomerId, Id FROM Invoice WHERE [IsReceived]=0 or IsReceived is Null

	OPEN db_cursor  
	FETCH NEXT FROM db_cursor INTO @CustomerId, @PoHeaderId 

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		
		IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'sale.order.header' AND  TABLE_NAME = @CustomerId))
		BEGIN 

			SET @SQL = '
			
				Insert into #table
			
					SELECT 
						[Date]=Convert(varchar, DeliveryDate, 101), 
						[Partner]=CustomerName+''-'+convert(varchar,@CustomerId)+''', 
						[SONo]=PONo, 
						[Amount]=TotalPrice,
						[DueDate]
					FROM [sale.order.header].['+convert(varchar,@CustomerId)+'] WHERE Id = '+convert(varchar, @PoHeaderId)+'
				'
			-- PRINT (@SQL)
			EXEC (@SQL)
			
			
		END

		FETCH NEXT FROM db_cursor INTO @CustomerId, @PoHeaderId

	END 

	CLOSE db_cursor  
	DEALLOCATE db_cursor 
	
	SELECT * FROM #table 
	
	DROP TABLE #table

END


