USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[GetCustomerPriceList]    Script Date: 03/14/2020 8:58:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[GetCustomerPriceList]

@CustomerId bigint,
@SupplierId int

AS BEGIN
	set nocount on;

	Declare @Id as bigint;
	DECLARE @SQL AS NVARCHAR(MAX)

	SELECT @Id=PricelistId FROM [CustomerPriceList] 
	WHERE CustomerId = @CustomerId;

	IF @@ROWCOUNT > 0 
	BEGIN 

		if @SupplierId = 0
			BEGIN
				SET @SQL = '
						SELECT 
							pt.[Id]
							  ,pt.[SFAReferenceNo]
							  ,pt.[Name]
							  ,pt.[Description]
							  ,pt.[Category]
							  ,pt.[UnitOfMeasure]
							  ,pt.[CostPrice]
							  ,pt.[UnitDiscount]
							  ,pt.[SFAUnitOfMeasure]
							  ,pt.[IsTaxable]
							  ,pt.[Deleted]
							  ,pt.[ProductType]
							  ,pt.[CanBeSold]
							  ,pt.[CanBePurchased]
							  ,pt.[Barcode]
							  ,pt.[InternalCategory]
							  ,pt.[PurchaseUnitOfMeasure]
							  ,pt.[ControlPurchaseBills]
							  ,pt.[DeductionFixPrice]
							  ,pt.[DeductionOutright]
							  ,pt.[Discount]
							  ,pt.[DeductionCashDiscount]
							  ,pt.[DeductionPromoDiscount]
							  ,pt.[Image]
							  ,pt.[UpdatedDate]
							  ,SalePrice = pt.[SalePrice_CORON]
							, StockQuantity = p.[Stock_CORON] + p.[Stock_LUBANG] + p.[Stock_SANILDEFONSO]
							, p.[Stock_CORON], p.[Stock_LUBANG], p.[Stock_SANILDEFONSO]
							, p.[KiloPerUnit]
							FROM 
							[pricelist.template].['+Convert(varchar,@Id)+'] pt
							Inner Join Product p On p.Id = pt.Id
							Where pt.Deleted = 0
									Order By pt.[Category], pt.Name;'
			END
		else if @SupplierId = 1
			BEGIN
				SET @SQL = '
						SELECT 
								pt.[Id]
							  ,pt.[SFAReferenceNo]
							  ,pt.[Name]
							  ,pt.[Description]
							  ,pt.[Category]
							  ,pt.[UnitOfMeasure]
							  ,pt.[CostPrice]
							  ,pt.[UnitDiscount]
							  ,pt.[SFAUnitOfMeasure]
							  ,pt.[IsTaxable]
							  ,pt.[Deleted]
							  ,pt.[ProductType]
							  ,pt.[CanBeSold]
							  ,pt.[CanBePurchased]
							  ,pt.[Barcode]
							  ,pt.[InternalCategory]
							  ,pt.[PurchaseUnitOfMeasure]
							  ,pt.[ControlPurchaseBills]
							  ,pt.[DeductionFixPrice]
							  ,pt.[DeductionOutright]
							  ,pt.[Discount]
							  ,pt.[DeductionCashDiscount]
							  ,pt.[DeductionPromoDiscount]
							  ,pt.[Image]
							  ,pt.[UpdatedDate]
							  ,SalePrice = pt.[SalePrice_LUBANG]
							, StockQuantity = p.[Stock_CORON] + p.[Stock_LUBANG] + p.[Stock_SANILDEFONSO]
							, p.[Stock_CORON], p.[Stock_LUBANG], p.[Stock_SANILDEFONSO]
							, p.[KiloPerUnit]
							FROM 
							[pricelist.template].['+Convert(varchar,@Id)+'] pt
							Inner Join Product p On p.Id = pt.Id
							Where pt.Deleted = 0
									Order By pt.[Category], pt.Name;'
			END
		else if @SupplierId = 2
			BEGIN
				SET @SQL = '
						SELECT 
							pt.[Id]
							  ,pt.[SFAReferenceNo]
							  ,pt.[Name]
							  ,pt.[Description]
							  ,pt.[Category]
							  ,pt.[UnitOfMeasure]
							  ,pt.[CostPrice]
							  ,pt.[UnitDiscount]
							  ,pt.[SFAUnitOfMeasure]
							  ,pt.[IsTaxable]
							  ,pt.[Deleted]
							  ,pt.[ProductType]
							  ,pt.[CanBeSold]
							  ,pt.[CanBePurchased]
							  ,pt.[Barcode]
							  ,pt.[InternalCategory]
							  ,pt.[PurchaseUnitOfMeasure]
							  ,pt.[ControlPurchaseBills]
							  ,pt.[DeductionFixPrice]
							  ,pt.[DeductionOutright]
							  ,pt.[Discount]
							  ,pt.[DeductionCashDiscount]
							  ,pt.[DeductionPromoDiscount]
							  ,pt.[Image]
							  ,pt.[UpdatedDate]
							  ,SalePrice = pt.[SalePrice_SANILDEFONSO]							
							, StockQuantity = p.[Stock_CORON] + p.[Stock_LUBANG] + p.[Stock_SANILDEFONSO]
							, p.[Stock_CORON], p.[Stock_LUBANG], p.[Stock_SANILDEFONSO]
							, p.[KiloPerUnit]
							FROM 
							[pricelist.template].['+Convert(varchar,@Id)+'] pt
							Inner Join Product p On p.Id = pt.Id
							Where pt.Deleted = 0
									Order By pt.[Category], pt.Name;'
			END

		EXEC(@SQL);

	END
	ELSE
		BEGIN 
			if @SupplierId = 0
				BEGIN
					SELECT 
								pt.[Id]
							  ,pt.[SFAReferenceNo]
							  ,pt.[Name]
							  ,pt.[Description]
							  ,pt.[Category]
							  ,pt.[UnitOfMeasure]
							  ,pt.[CostPrice]
							  ,pt.[SFAUnitOfMeasure]
							  ,pt.[IsTaxable]
							  ,pt.[Deleted]
							  ,pt.[ProductType]
							  ,pt.[CanBeSold]
							  ,pt.[CanBePurchased]
							  ,pt.[Barcode]
							  ,pt.[InternalCategory]
							  ,pt.[PurchaseUnitOfMeasure]
							  ,pt.[ControlPurchaseBills]
							  ,pt.[DeductionFixPrice]
							  ,pt.[DeductionOutright]
							  ,pt.[Discount]
							  ,pt.[DeductionCashDiscount]
							  ,pt.[DeductionPromoDiscount]
							  ,pt.[Image]
							  ,pt.[UpdatedDate]
							  ,SalePrice = pt.[SalePrice_CORON]
							,StockQuantity = p.[Stock_CORON]
							, p.[KiloPerUnit] FROM [pricelist.template].[0] pt
						Inner Join Product p On p.Id = pt.Id		
								Where pt.Deleted = 0
								Order By pt.[Category], pt.Name;
				END 
			else if @SupplierId = 1
				BEGIN
					SELECT 
							pt.[Id]
							  ,pt.[SFAReferenceNo]
							  ,pt.[Name]
							  ,pt.[Description]
							  ,pt.[Category]
							  ,pt.[UnitOfMeasure]
							  ,pt.[CostPrice]
							  ,pt.[SFAUnitOfMeasure]
							  ,pt.[IsTaxable]
							  ,pt.[Deleted]
							  ,pt.[ProductType]
							  ,pt.[CanBeSold]
							  ,pt.[CanBePurchased]
							  ,pt.[Barcode]
							  ,pt.[InternalCategory]
							  ,pt.[PurchaseUnitOfMeasure]
							  ,pt.[ControlPurchaseBills]
							  ,pt.[DeductionFixPrice]
							  ,pt.[DeductionOutright]
							  ,pt.[Discount]
							  ,pt.[DeductionCashDiscount]
							  ,pt.[DeductionPromoDiscount]
							  ,pt.[Image]
							  ,pt.[UpdatedDate]
							  ,SalePrice = pt.[SalePrice_LUBANG]
							,StockQuantity = p.[Stock_LUBANG] 
							, p.[KiloPerUnit] FROM [pricelist.template].[0] pt
						Inner Join Product p On p.Id = pt.Id		
								Where pt.Deleted = 0
								Order By pt.[Category], pt.Name;
				END 
			else if @SupplierId = 2
				BEGIN
					SELECT 
								pt.[Id]
							  ,pt.[SFAReferenceNo]
							  ,pt.[Name]
							  ,pt.[Description]
							  ,pt.[Category]
							  ,pt.[UnitOfMeasure]
							  ,pt.[CostPrice]
							  ,pt.[SFAUnitOfMeasure]
							  ,pt.[IsTaxable]
							  ,pt.[Deleted]
							  ,pt.[ProductType]
							  ,pt.[CanBeSold]
							  ,pt.[CanBePurchased]
							  ,pt.[Barcode]
							  ,pt.[InternalCategory]
							  ,pt.[PurchaseUnitOfMeasure]
							  ,pt.[ControlPurchaseBills]
							  ,pt.[DeductionFixPrice]
							  ,pt.[DeductionOutright]
							  ,pt.[Discount]
							  ,pt.[DeductionCashDiscount]
							  ,pt.[DeductionPromoDiscount]
							  ,pt.[Image]
							  ,pt.[UpdatedDate]
							  ,SalePrice = pt.[SalePrice_SANILDEFONSO]
							  ,StockQuantity = p.[Stock_SANILDEFONSO]
							  , p.[KiloPerUnit] FROM [pricelist.template].[0] pt
						Inner Join Product p On p.Id = pt.Id		
								Where pt.Deleted = 0
								Order By pt.[Category], pt.Name;
				END 

		END



END
