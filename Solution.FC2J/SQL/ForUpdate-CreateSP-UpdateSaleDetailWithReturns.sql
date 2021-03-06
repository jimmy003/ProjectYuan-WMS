USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[UpdateSaleDetailWithReturns]    Script Date: 04/25/2020 8:29:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[UpdateSaleDetailWithReturns]

@CustomerId bigint,
@OrderDetailId bigint,
@ProductId bigint,
@SupplierId int,
@ReturnQuantity float

AS BEGIN


/**
if @SupplierId = 0 -- Coron
if @SupplierId = 1 -- Lubang 
if @SupplierId = 2 -- San Ildefonso 
**/	
	DECLARE @SQL AS NVARCHAR(MAX)= '
	
	set nocount on;

	Declare @ReturnDate datetime = GetDate();
	Declare @Price money=0;
	Declare @SellingPrice money=0;
	Declare @DeductionFixPrice money=0;	
	Declare @TaxRate money=0;	
	Declare @SubTotalProductSalePrice money=0;	
	Declare @SubTotalProductTaxPrice money=0;	
	
	--update details
	Select 
		@DeductionFixPrice = DeductionFixPrice,
		@SellingPrice = ProductSalePrice,
		@TaxRate = TaxRate
	from [sale.order.detail].['+Convert(varchar,@CustomerId)+'] Where Id = Convert(bigint,'''+Convert(varchar,@OrderDetailId)+''') 

	Set @Price = @SellingPrice;
	if (@DeductionFixPrice > 0)
		Set @Price = @DeductionFixPrice;

	Set @SubTotalProductSalePrice = @Price * Convert(money,'''+Convert(varchar,@ReturnQuantity)+''');	
	Set @SubTotalProductTaxPrice = @Price * Convert(money,'''+Convert(varchar,@ReturnQuantity)+''') * @TaxRate;	

	UPDATE [sale.order.detail].['+Convert(varchar,@CustomerId)+']  
		SET IsReturn = 1,
			ReturnQuantity = Convert(money,'''+Convert(varchar,@ReturnQuantity)+'''),
			ReturnDate = @ReturnDate,
			OrderQuantity = OrderQuantity - Convert(money,'''+Convert(varchar,@ReturnQuantity)+'''),
			SubTotalProductSalePrice = SubTotalProductSalePrice - @SubTotalProductSalePrice,
			SubTotalProductTaxPrice = SubTotalProductTaxPrice - @SubTotalProductTaxPrice
	Where Id = Convert(bigint,'''+Convert(varchar,@OrderDetailId)+''')
	'
	
	if @SupplierId = 0
		Set @SQL = @SQL + '
	Update [dbo].[Product] Set [Stock_CORON]=[Stock_CORON] + Convert(money,'''+Convert(varchar,@ReturnQuantity)+''') 
	Where Id= Convert(bigint,'''+Convert(varchar,@ProductId)+''') 
						'
	if @SupplierId = 1
		Set @SQL = @SQL + '
	Update [dbo].[Product] Set [Stock_LUBANG]=[Stock_LUBANG] + Convert(money,'''+Convert(varchar,@ReturnQuantity)+''') 
	Where Id= Convert(bigint,'''+Convert(varchar,@ProductId)+''')
						'

	if @SupplierId = 2
		Set @SQL = @SQL + '
	Update [dbo].[Product] Set [Stock_SANILDEFONSO]=[Stock_SANILDEFONSO] + Convert(money,'''+Convert(varchar,@ReturnQuantity)+''') 
	Where Id= Convert(bigint,'''+Convert(varchar,@ProductId)+''')
						'
	EXEC(@SQL)


END


