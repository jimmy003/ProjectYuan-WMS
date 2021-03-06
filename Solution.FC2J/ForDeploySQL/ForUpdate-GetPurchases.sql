USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[GetPurchases]    Script Date: 02/01/2020 3:12:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetPurchases]
@UserName varchar(50) = ''

AS BEGIN
	
	set nocount on;

	declare @date_To as datetime = getdate()
	declare @date_From as datetime 
	set @date_From = dateadd(month,-1, @date_To)

	if @UserName = 'admin'
	BEGIN
		select 
			[Id]
		  ,[PONo]
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
		  ,[AcknowledgedDate]
		  ,[AcknowledgedUser]
		  ,[DeliveredDate]
		  ,[DeliveredUser]
		  ,[PriceListId]
		  ,[SupplierName]
		  ,[POStatusId]
		  ,[POStatus]
		  ,[SupplierEmail]
		  ,[IsVatable]		
		from [sale.order.header].[FC2J]
		WHERE PurchaseDate Between Convert(datetime,convert(varchar, @date_From, 101) + ' 00:00:00.000') And Convert(datetime,convert(varchar, @date_To, 101) + ' 23:59:59.999')
		AND Deleted = 0
		ORDER BY PurchaseDate, PONO
	END

	else
	BEGIN
		select 
			[Id]
		  ,[PONo]
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
		  ,[AcknowledgedDate]
		  ,[AcknowledgedUser]
		  ,[DeliveredDate]
		  ,[DeliveredUser]
		  ,[PriceListId]
		  ,[SupplierName]
		  ,[POStatusId]
		  ,[POStatus]
		  ,[SupplierEmail]
		  ,[IsVatable]				
		from [sale.order.header].[FC2J]
		WHERE username = @UserName
		AND PurchaseDate Between Convert(datetime,convert(varchar, @date_From, 101) + ' 00:00:00.000') And Convert(datetime,convert(varchar, @date_To, 101) + ' 23:59:59.999')
		AND Deleted = 0
		ORDER BY PurchaseDate, PONO
	END



END


