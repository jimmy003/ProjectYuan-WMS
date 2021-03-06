USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[GetSalesReport]    Script Date: 02/02/2020 1:18:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetSalesReport] 
@address2 varchar(200)='',
@InternalCategory varchar(200)='',
@IsFeeds bit = 1,
@DateFrom datetime,
@DateTo datetime

AS BEGIN
	set nocount on;
	
	declare @date_To as datetime = getdate()
	declare @date_From as datetime 
	-- set @date_From = dateadd(month,-1, @date_To);

	set @date_From = @DateFrom
	set @date_To = @DateTo


	-- this is the temp table to use to store the recordsets
	create table #sales (CustomerId bigint, CustomerName varchar(50), PersonnelId bigint, ProductId bigint, ProductName varchar(50), OrderQuantity float)

	Declare @Id bigint
	Declare @Name varchar(50)
	Declare @PersonnelId bigint
	DECLARE @SQL AS NVARCHAR(MAX) = ''			

	-- this will get target customers based on the parameters
	DECLARE db_cursor CURSOR FOR 
	SELECT c.Id, c.Name, c.PersonnelId
		FROM CUSTOMER c
		WHERE (Deleted = 0 or Deleted is null) 
		AND ADDRESS2=@address2

	OPEN db_cursor  
	FETCH NEXT FROM db_cursor INTO @Id, @Name, @PersonnelId
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		Insert into #sales
		select @Id, @Name, @PersonnelId, p.Id, p.Name, 0 from product p where (p.Deleted = 0 or p.Deleted is null) AND p.IsFeeds=@IsFeeds and p.InternalCategory = @InternalCategory
		FETCH NEXT FROM db_cursor INTO @Id, @Name, @PersonnelId

	END 
	CLOSE db_cursor  
	DEALLOCATE db_cursor 



	DECLARE db_cursor_sales CURSOR FOR 
	SELECT c.Id
		FROM Customer c
		WHERE (Deleted = 0 or Deleted is null) 
		AND ADDRESS2=@address2

	OPEN db_cursor_sales  
	FETCH NEXT FROM db_cursor_sales INTO @Id 

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		
		
		IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'sale.order.detail' AND  TABLE_NAME = convert(varchar,@Id)))
			BEGIN
				SET @SQL='

					Declare @ProductId bigint
					Declare @OrderQuantity float

					DECLARE db_cursor_products CURSOR FOR 		
			
					SELECT d.ProductId, Sum(d.OrderQuantity) as [OrderQuantity]
					FROM 
						[sale.order.detail].['+convert(varchar,@Id)+'] d inner join 
						[sale.order.header].['+convert(varchar,@Id)+'] h on h.id = d.OrderHeaderId
						where 
							h.OrderDate 
								Between Convert(datetime,'''+convert(varchar, @date_From, 101)+''' + '' 00:00:00.000'') And Convert(datetime,'''+convert(varchar, @date_To, 101)+''' + '' 23:59:59.999'')	
							and h.OrderStatusId not in (1,3) 		
							and d.ProductName is not null		

						group by d.ProductId 


					OPEN db_cursor_products  
					FETCH NEXT FROM db_cursor_products INTO @ProductId, @OrderQuantity 

					WHILE @@FETCH_STATUS = 0  
					BEGIN 

						UPDATE #sales SET OrderQuantity = OrderQuantity + @OrderQuantity WHERE CustomerId=convert(bigint,'+convert(varchar,@Id)+') AND ProductId=@ProductId
						FETCH NEXT FROM db_cursor_products INTO @ProductId, @OrderQuantity 

					END 

					CLOSE db_cursor_products  
					DEALLOCATE db_cursor_products 	
					'
		
				exec (@SQL)		

			END

		FETCH NEXT FROM db_cursor_sales INTO @Id

	END 

	CLOSE db_cursor_sales  
	DEALLOCATE db_cursor_sales 

	-- SELECT * FROM #sales

	
	DECLARE @cols AS NVARCHAR(MAX), @query AS NVARCHAR(MAX);

	SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(p.ProductName) 
				FROM #sales p 
				FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)') 
			,1,1,'')

	print (@cols)

	set @query = 'SELECT CustomerId, CustomerName, PersonnelId, ' + @cols + ' from 
				(
					select CustomerId, CustomerName, PersonnelId
						, OrderQuantity
						, ProductName
					from #sales
			   ) x
				pivot 
				(
					max(OrderQuantity)
					for ProductName in (' + @cols + ')
				) p 
				
				ORDER BY CustomerName'


	EXEC(@query)

	DROP TABLE #sales

END




