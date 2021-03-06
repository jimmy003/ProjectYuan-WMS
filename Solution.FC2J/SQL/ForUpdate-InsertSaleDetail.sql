USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[InsertSaleDetail]    Script Date: 02/01/2020 1:12:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[InsertSaleDetail]

@CustomerId bigint,
@OrderHeaderId bigint,
@LineNo int,
@ProductId bigint,
@OrderQuantity int,
@TaxRate money,
@SubTotalProductSalePrice money,
@SubTotalProductTaxPrice money,
@PriceListId bigint,
@SupplierId int,
@Supplier varchar(50) = '',
@OrderStatusId bigint


AS BEGIN

	DECLARE @SQL AS NVARCHAR(MAX)= '
	
	Declare @Id bigint;
	Declare @ProductName varchar(50) 
	Declare @ProductDescription varchar(200)
	Declare @ProductCategory varchar(200) 
	Declare @ProductUnitOfMeasure varchar(50)
	Declare @ProductSalePrice money = 0;
	Declare @ProductCostPrice money = 0;
	Declare @ProductSFAUnitOfMeasure varchar(50);
	Declare @ProductSFAReferenceNo varchar(50);
	Declare @IsTaxable bit;

	Declare @DeductionFixPrice money;
	Declare @DeductionOutright money;
	Declare @Discount float;
	Declare @DeductionCashDiscount money;
	Declare @DeductionPromoDiscount money;

	set nocount on;
	Select 
		@ProductName = [Name],
		@ProductDescription = [Description],
		@ProductCategory = [Category],
		@ProductUnitOfMeasure = [UnitOfMeasure],'
		--
	if @SupplierId = 0 
		BEGIN
			SET @SQL = @SQL + '@ProductSalePrice = [SalePrice_CORON],'
		END
	else if @SupplierId = 1 
		BEGIN
			SET @SQL = @SQL + '@ProductSalePrice = [SalePrice_LUBANG],'
		END
	else if @SupplierId = 2 
		BEGIN
			SET @SQL = @SQL + '@ProductSalePrice = [SalePrice_SANILDEFONSO],'
		END
		
	SET @SQL = @SQL + '@ProductCostPrice = [CostPrice],
		@ProductSFAUnitOfMeasure = [SFAUnitOfMeasure],
		@ProductSFAReferenceNo = [SFAReferenceNo],
		@IsTaxable = [IsTaxable],		
		@DeductionFixPrice = [DeductionFixPrice],
		@DeductionOutright = [DeductionOutright],
		@Discount = [Discount],
		@DeductionCashDiscount = [DeductionCashDiscount],
		@DeductionPromoDiscount = [DeductionPromoDiscount]
	From [pricelist.template].['+Convert(varchar,@PriceListId)+'] Where Id='''+Convert(varchar,@ProductId)+''';

	Insert Into [sale.order.detail].['+Convert(varchar,@CustomerId)+']  
		(OrderHeaderId,
		[LineNo],
		ProductId,
		OrderQuantity,
		ProductName, 
		ProductDescription,
		ProductCategory,
		ProductUnitOfMeasure, 
		ProductSalePrice,
		ProductCostPrice, 
		ProductSFAUnitOfMeasure,
		ProductSFAReferenceNo,
		SubTotalProductSalePrice,
		IsTaxable,
		TaxRate,
		SubTotalProductTaxPrice,
		DeductionFixPrice,
		DeductionOutright,
		Discount,
		DeductionCashDiscount,
		DeductionPromoDiscount,		
		Deleted,
		SupplierId,
		Supplier
		)
	Values(Convert(bigint,'''+Convert(varchar,@OrderHeaderId)+'''),
		'''+Convert(varchar,@LineNo)+''',
		Convert(bigint,'''+Convert(varchar,@ProductId)+'''),
		Convert(int,'''+Convert(varchar,@OrderQuantity)+'''),
		@ProductName,
		@ProductDescription,
		@ProductCategory,
		@ProductUnitOfMeasure,
		@ProductSalePrice,
		@ProductCostPrice,
		@ProductSFAUnitOfMeasure,
		@ProductSFAReferenceNo,
		Convert(money,'''+Convert(varchar,@SubTotalProductSalePrice)+'''),
		@IsTaxable,
		Convert(money,'''+Convert(varchar,@TaxRate)+'''),  
		Convert(money,'''+Convert(varchar,@SubTotalProductTaxPrice)+'''),  
		@DeductionFixPrice,
		@DeductionOutright,
		@Discount,
		@DeductionCashDiscount,
		@DeductionPromoDiscount,
		0,
		Convert(int,'''+Convert(varchar,@SupplierId)+'''),  
		Convert(varchar,'''+Convert(varchar,@Supplier)+''')  
		); '
	

	if @OrderStatusId <> 1
	BEGIN
		if @SupplierId = 0 
			BEGIN
				SET @SQL = @SQL + 'Update [dbo].[Product] Set IsEditable = 0, [Stock_CORON]=[Stock_CORON] - Convert(int,'''+Convert(varchar,@OrderQuantity)+''') Where Id= Convert(bigint,'''+Convert(varchar,@ProductId)+''')'
			END
		else if @SupplierId = 1 
			BEGIN
				SET @SQL = @SQL + 'Update [dbo].[Product] Set IsEditable = 0, [Stock_LUBANG]=[Stock_LUBANG] - Convert(int,'''+Convert(varchar,@OrderQuantity)+''') Where Id= Convert(bigint,'''+Convert(varchar,@ProductId)+''')'
			END
		else if @SupplierId = 2 
			BEGIN
				SET @SQL = @SQL + 'Update [dbo].[Product] Set IsEditable = 0, [Stock_SANILDEFONSO]=[Stock_SANILDEFONSO] - Convert(int,'''+Convert(varchar,@OrderQuantity)+''') Where Id= Convert(bigint,'''+Convert(varchar,@ProductId)+''')'
			END
	END

	
	SET @SQL = @SQL + ' Select @Id = @@IDENTITY; 
	';
	exec(@SQL)


END


