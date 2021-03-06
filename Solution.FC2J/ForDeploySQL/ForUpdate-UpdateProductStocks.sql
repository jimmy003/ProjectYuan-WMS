USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePriceListTemplateInternalCategoryFromProduct]    Script Date: 02/01/2020 7:43:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[UpdateProductStocks] 
@CustomerId bigint,
@OrderHeaderId bigint

AS BEGIN

	DECLARE @SQL AS NVARCHAR(MAX) =  
	
		'
			set nocount on;	
			select ProductName, ProductId, OrderQuantity, SupplierId from [sale.order.detail].['+Convert(varchar,@CustomerId)+'] where OrderHeaderId = Convert(bigint,'+Convert(varchar,@OrderHeaderId)+')

			DECLARE @ProductId bigint
			DECLARE @OrderQuantity float
			DECLARE @SupplierId int

			DECLARE db_cursor CURSOR FOR 

			select ProductId, OrderQuantity, SupplierId from [sale.order.detail].['+Convert(varchar,@CustomerId)+'] where OrderHeaderId = Convert(bigint,'+Convert(varchar,@OrderHeaderId)+')
			OPEN db_cursor  
			FETCH NEXT FROM db_cursor INTO @ProductId, @OrderQuantity, @SupplierId 

			WHILE @@FETCH_STATUS = 0  
			BEGIN  
		
				if @SupplierId = 0
					BEGIN
						UPDATE PRODUCT set Stock_CORON = Stock_CORON + @OrderQuantity Where Id = @ProductId				
					END
				if @SupplierId = 1
					BEGIN
						UPDATE PRODUCT set Stock_LUBANG = Stock_LUBANG + @OrderQuantity Where Id = @ProductId
					END
				if @SupplierId = 2
					BEGIN
						UPDATE PRODUCT set Stock_SANILDEFONSO = Stock_SANILDEFONSO + @OrderQuantity Where Id = @ProductId
					END

				FETCH NEXT FROM db_cursor INTO @ProductId, @OrderQuantity, @SupplierId

			END 

			CLOSE db_cursor  
			DEALLOCATE db_cursor 

			
		'
		EXEC(@SQL)
END