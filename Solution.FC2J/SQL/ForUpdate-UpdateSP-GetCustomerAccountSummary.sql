USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[GetCustomerAccountSummary]    Script Date: 03/01/2020 1:12:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetCustomerAccountSummary] 
@DateFrom datetime,
@DateTo datetime

AS BEGIN

	set nocount on;
	
	declare @date_To as datetime = getdate()
	declare @date_From as datetime 
	set @date_From = dateadd(month,-1, @date_To);

	set @date_From = @DateFrom
	set @date_To = @DateTo

	create table #sales (CustomerName varchar(50), 
				[Sales Receipt] varchar(50), [Total Amount] money, OrderStatus varchar(50), [BagsNotConverted] decimal(9,2), [BagsConverted] decimal(9,2), [Deductions] money)


	Declare @Id bigint
	Declare @Name varchar(50)
	DECLARE @SQL AS NVARCHAR(MAX) = ''			

	-- loop from existing and active customers
	DECLARE db_cursor CURSOR FOR 
	SELECT c.Id
		FROM CUSTOMER c
		WHERE (Deleted = 0 or Deleted is null) 
		
	-- and insert records to temp table, if there is 
	OPEN db_cursor  
	FETCH NEXT FROM db_cursor INTO @Id
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		
		IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'sale.order.header' AND  TABLE_NAME = convert(varchar,@Id)))
			BEGIN
				SET @SQL='

				Insert into #sales
				SELECT distinct h.CustomerName, [Sales Receipt] = h.PONo, [TotalAmount]=h.TotalPrice, 
					CASE
					WHEN OrderStatus = ''Delivered'' THEN ''Unpaid''
					WHEN OrderStatus = ''Delivered With Returns'' THEN ''Unpaid''
					WHEN OrderStatus = ''Returned All'' THEN ''Returned''
					WHEN OrderStatus = ''Partial'' THEN ''Partially Paid''
					WHEN OrderStatus = ''Bad Accounts'' THEN ''Bad Accounts''
					ELSE ''Paid''
					END AS [Status Of Payment]
					,[BagsNotConverted] = (select sum(d.OrderQuantity) from [sale.order.detail].['+convert(varchar,@Id)+'] d inner join product prd on prd.Id = d.ProductId where d.OrderHeaderId = h.Id and prd.IsConvertibleToBag=0)
					,[BagsConverted] = (select cast(sum(d.OrderQuantity * isnull(prd.KiloPerUnit,0)) / 50 as decimal(9,2)) from [sale.order.detail].['+convert(varchar,@Id)+'] d inner join product prd on prd.Id = d.ProductId where d.OrderHeaderId = h.Id and prd.IsConvertibleToBag=1)
					,[Deductions]=h.LessPrice
				FROM 
					[sale.order.detail].['+convert(varchar,@Id)+'] d inner join 
					[sale.order.header].['+convert(varchar,@Id)+'] h on h.id = d.OrderHeaderId
					where 
						h.OrderDate 
							Between Convert(datetime,'''+convert(varchar, @date_From, 101)+''' + '' 00:00:00.000'') And Convert(datetime,'''+convert(varchar, @date_To, 101)+''' + '' 23:59:59.999'')	
						and h.OrderStatusId >= 4
					 
					'
		
				exec (@SQL)		

			END
		
		FETCH NEXT FROM db_cursor INTO @Id

	END 
	CLOSE db_cursor  
	DEALLOCATE db_cursor 

	SELECT * FROM #sales

	DROP TABLE #sales
	
END




