GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetBmegReport]
	@FROM varchar(12)='',
	@TO varchar(12)=''

AS BEGIN
	
	set nocount on;

	-- this is the temp table to use to store the recordsets
	create table #bmeg (
			[Delivery Date] datetime,
			[Customer Name] nvarchar(120),
			[LINENO] int,
			[SFA ReferenceNo] nvarchar(25),
			[PO No] nvarchar(20),
			[Material Code] nvarchar(25),
			[Product Name] nvarchar(120),
			[Order Quantity] float,
			[Product Sale Price] money
			)

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

			SET @SQL = '
				Insert into #bmeg
			
					SELECT
							h'+@Id+'.DeliveryDate, 
							h'+@Id+'.CustomerName,
							d'+@Id+'.[LineNo],
							h'+@Id+'.SFAReferenceNo,
							h'+@Id+'.PONo,
							d'+@Id+'.ProductSFAReferenceNo,
							d'+@Id+'.ProductName,
							d'+@Id+'.OrderQuantity,
							d'+@Id+'.ProductSalePrice
						From [sale.order.header].['+@Id+'] h'+@Id+' 
							Inner Join [sale.order.detail].['+@Id+'] d'+@Id+'
							On h'+@Id+'.Id = d'+@Id+'.OrderHeaderId 
						Where h'+@Id+'.DeliveryDate Between '''+@FROM+''' And '''+@TO+'''
						AND h'+@Id+'.OrderStatusId = ''2''
				'
			-- PRINT (@SQL)
			EXEC (@SQL)


		END

		FETCH NEXT FROM db_cursor INTO @CustomerId

	END 

	CLOSE db_cursor  
	DEALLOCATE db_cursor 
	
	SELECT * FROM #bmeg
	
	DROP TABLE #bmeg

END


