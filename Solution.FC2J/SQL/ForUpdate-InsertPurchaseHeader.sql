USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[InsertPurchaseHeader]    Script Date: 02/01/2020 3:09:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[InsertPurchaseHeader]

	@PONo varchar(15)='',
	@PurchaseDate datetime,
	@DeliveryDate datetime,

	@PickUpDiscount money=0,
	@Outright money=0,
	@CashDiscount money=0,
	@PromoDiscount money=0,
	@OtherDeduction money=0,

	@TotalQuantity float,
	@TotalQuantityUOMComputed float=0,
	@SubTotal money,
	@TaxPrice money,
	@Total money,

	@UserName varchar(50)='',
	@PriceListId bigint,
	@SupplierName varchar(50)='',
	@SupplierEmail varchar(200)='',
	@IsVatable bit = 0,
	@IsResubmit varchar(50)=''
	

AS BEGIN

	Declare @POStatusId int = 1;
	Declare @POStatus varchar(50) = 'Submitted'
	Declare @Id bigint = 0

	set nocount on;

	if @IsResubmit <> '' 
		BEGIN
			
			if @IsResubmit <> @PONo
			BEGIN
				IF (EXISTS (SELECT Id FROM [sale.order.header].[FC2J] WHERE PONo = @PONo)) 
				BEGIN
					Set @Id = -1; 
				END
			END 

			if @Id <> -1
				BEGIN
					update [sale.order.header].[FC2J] set Deleted = 1 where PONo = @IsResubmit -- oldPONo
				END
		END 
	else
		BEGIN
			IF (EXISTS (SELECT Id FROM [sale.order.header].[FC2J] WHERE PONo = @PONo)) 
			BEGIN
				Set @Id = -1; 
			END
		END 
	
	
	if @Id = -1
		BEGIN 
			Select Id = -1; 	
		END 

	else
		BEGIN
			INSERT INTO [sale.order.header].[FC2J]
					   ([PONo]
					   ,[PurchaseDate]
					   ,[DeliveryDate]

					   ,[PickUpDiscount]
					   ,[Outright]
					   ,[CashDiscount]
					   ,[PromoDiscount]
					   ,[OtherDeduction]

					   ,[TotalQuantity]
					   ,[TotalQuantityUOMComputed]
					   ,[SubTotal]
					   ,[TaxPrice]
					   ,[Total]

					   ,[SubmittedDate]
					   ,[UserName]

					   ,[PriceListId]
					   ,[SupplierName]
					   ,[POStatusId] 
					   ,[POStatus] 
					   ,[SupplierEmail]
					   ,[Deleted]
					   ,[IsVatable])
				 VALUES
					   (@PONo
					   ,@PurchaseDate
					   ,@DeliveryDate
					   ,@PickUpDiscount
					   ,@Outright
					   ,@CashDiscount
					   ,@PromoDiscount
					   ,@OtherDeduction
					   ,@TotalQuantity
					   ,@TotalQuantityUOMComputed
					   ,@SubTotal
					   ,@TaxPrice
					   ,@Total
					   ,GETDATE()
					   ,@UserName
					   ,@PriceListId
					   ,@SupplierName
					   ,@POStatusId
					   ,@POStatus
					   ,@SupplierEmail
					   ,0
					   ,@IsVatable)
	
			Select Id = @@IDENTITY; 	
		END


END 


