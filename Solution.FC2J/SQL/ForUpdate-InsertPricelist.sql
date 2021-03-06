USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[InsertPricelist]    Script Date: 31/01/2020 6:45:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[InsertPricelist]  
@Name varchar(100),
@DiscountPolicy bit,
@Subscribed int=0,
@IsForSalesOrder int=1

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
						   ,@DiscountPolicy
						   ,@Subscribed
						   ,0
						   ,@IsForSalesOrder)	
	
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
							,[SalePrice_CORON] 
							,[SalePrice_LUBANG] 
							,[SalePrice_SANILDEFONSO] 
							,[CostPrice]
							,[SFAUnitOfMeasure]
							,[IsTaxable]
							,[Deleted]
							,''Stockable Product''
							,1
							,1
							,''''
							,[InternalCategory]
							,[UnitOfMeasure]
							,''''
							,''''
						FROM [dbo].[Product]
						WHERE [Deleted]=0 
				
						';

						EXEC(@SQL)
			END
		Select Id=@Id;
END