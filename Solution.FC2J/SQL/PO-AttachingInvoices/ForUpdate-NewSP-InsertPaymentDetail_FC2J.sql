USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[InsertInvoiceDetail]    Script Date: 03/11/2020 2:44:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[InsertPaymentDetail_FC2J]  
@POHeaderId bigint=0,
@ProductId bigint=0,
@Quantity float=0,
@InvoiceNo varchar(25)=''

AS BEGIN

		Declare @Name as varchar(50)
        Declare @Category as varchar(200)
        Declare @UnitOfMeasure as varchar(50)
        Declare @SalePrice as money
        Declare @UnitDiscount as money
        Declare @SFAUnitOfMeasure as varchar(50)
        Declare @SFAReferenceNo as varchar(50)
        Declare @SubTotal as money --computed by quantity and saleprice-unitdiscount
        Declare @IsTaxable as bit
        Declare @TaxRate as money
        Declare @TaxPrice as money

		set nocount on;	

		SELECT	
				@Name=[Name]
				,@Category=[Category]
				,@UnitOfMeasure=[UnitOfMeasure]
				,@SalePrice=[SalePrice]
				,@UnitDiscount=[UnitDiscount]
				,@SFAUnitOfMeasure=[SFAUnitOfMeasure]
				,@SFAReferenceNo=[SFAReferenceNo]
				,@IsTaxable=[IsTaxable]
				,@TaxRate=[TaxRate]
				,@TaxPrice=[TaxPrice]
			FROM [sale.order.detail].[FC2J] where [POHeaderId] = @POHeaderId And [ProductId] = @ProductId
		
		set @SubTotal = (@SalePrice-@UnitDiscount) * @Quantity

		Insert Into [sale.order.detail].[Payment_FC2J]
			([OrderHeaderId]
           ,[ProductId]
           ,[Quantity]

           ,[Name]
           ,[Category]
           ,[UnitOfMeasure]
           ,[SalePrice]
           ,[UnitDiscount]
           
		   ,[SFAUnitOfMeasure]
           ,[SFAReferenceNo]
           ,[SubTotal] --computed by quantity and saleprice-unitdiscount
           ,[IsTaxable]
           ,[TaxRate]
           ,[TaxPrice]
           
		   ,[InvoiceNo])
     VALUES
           (@POHeaderId
           ,@ProductId
           ,@Quantity
           
		   ,@Name
           ,@Category
           ,@UnitOfMeasure
           ,@SalePrice
           ,@UnitDiscount
           
		   ,@SFAUnitOfMeasure
           ,@SFAReferenceNo
           ,@SubTotal
           ,@IsTaxable
           ,@TaxRate
           ,@TaxPrice
           
		   ,@InvoiceNo)

END