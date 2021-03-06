USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[RemovePriceListCustomer]    Script Date: 31/01/2020 8:03:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[RemovePriceListCustomer]
@CustomerId bigint

AS BEGIN
	
	declare @PriceListId bigint
	set nocount on;

	select @PriceListId = PricelistId from CustomerPriceList where CustomerId=@CustomerId

	UPDATE PRICELIST SET Subscribed = Subscribed - 1 WHERE Id = @PriceListId

	Delete from [CustomerPriceList] Where CustomerId=@CustomerId;


 END
 
 