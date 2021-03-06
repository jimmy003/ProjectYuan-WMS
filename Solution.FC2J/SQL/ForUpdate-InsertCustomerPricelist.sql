USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[InsertCustomerPricelist]    Script Date: 31/01/2020 8:00:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[InsertCustomerPricelist]
@CustomerId bigint,
@PriceListId bigint

AS BEGIN
-- 
	set nocount on;
	declare @ExistingPriceListId bigint = 0
	
	select @ExistingPriceListId = [PriceListId] from CustomerPriceList where CustomerId=@CustomerId and [PriceListId] <> @PriceListId

	if @ExistingPriceListId > 0
		BEGIN
			UPDATE PRICELIST SET Subscribed = Subscribed - 1 WHERE Id = @ExistingPriceListId
		END


	DELETE [CustomerPriceList] WHERE CUSTOMERID = @CustomerId;

	INSERT INTO [dbo].[CustomerPriceList]
           ([CustomerId]
           ,[PriceListId])
     VALUES
           (@CustomerId
           ,@PriceListId)

	UPDATE PRICELIST SET Subscribed = Subscribed + 1 WHERE Id = @PriceListId 
	Select Id = @ExistingPriceListId

 END


 
