USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[InsertPricelist]    Script Date: 31/01/2020 6:28:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CopyPricelist]  
@Name varchar(100),
@Source bigint

AS BEGIN

		Declare @Id as bigint;		
		DECLARE @SQL AS NVARCHAR(MAX)			
		set nocount on;
		
		IF (EXISTS (SELECT * FROM [PriceList] WHERE [NAME] = @Name)) 
			BEGIN
				Set @Id = -1; 
			END
		ELSE
			BEGIN
				INSERT INTO [dbo].[PriceList]
							([Name]
							,[DiscountPolicy]
							,[Subscribed]
							,[Deleted]
							,[IsForSalesOrder])
						VALUES
							(@Name
							,1
							,0
							,0
							,1)	
	
				Set @Id = @@IDENTITY; 
		
				Exec [CreateCustomerPricelistTemplateTable] @Id;

				set @SQL = '

				INSERT INTO [pricelist.template].['+Convert(varchar,@Id)+']
							([Id]
							,[SFAReferenceNo]
							,[Name]
							,[Description]
							,[Category]
							,[UnitOfMeasure]
							,[SalePrice_CORON] 
							,[SalePrice_LUBANG] 
							,[SalePrice_SANILDEFONSO] 
							,[CostPrice]
							,[UnitDiscount]
							
							,[DeductionFixPrice]
							,[DeductionOutright]
							,[Discount]
							,[DeductionCashDiscount]
							,[DeductionPromoDiscount]


							,[SFAUnitOfMeasure]
							,[IsTaxable]
							,[Deleted]
							,[ProductType]
							,[CanBeSold]
							,[CanBePurchased]
							,[Barcode]
							,[InternalCategory]
							,[PurchaseUnitOfMeasure]
							,[ControlPurchaseBills]
							,[Image])
						SELECT [Id]
							,[SFAReferenceNo]
							,[Name]
							,[Description]
							,[Category]
							,[UnitOfMeasure]
							,[SalePrice_CORON] [money] 
							,[SalePrice_LUBANG] [money]
							,[SalePrice_SANILDEFONSO] [money] 
							,[CostPrice]
							,[UnitDiscount]

							,[DeductionFixPrice]
							,[DeductionOutright]
							,[Discount]
							,[DeductionCashDiscount]
							,[DeductionPromoDiscount]

							,[SFAUnitOfMeasure]
							,[IsTaxable]
							,[Deleted]
							,[ProductType]
							,[CanBeSold]
							,[CanBePurchased]
							,[Barcode]
							,[InternalCategory]
							,[PurchaseUnitOfMeasure]
							,[ControlPurchaseBills]
							,[Image]
						FROM [pricelist.template].['+Convert(varchar,@Source)+']			
						';

						EXEC(@SQL)
			END


		Select Id=@Id;
END


