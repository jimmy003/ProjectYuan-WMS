USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[GetCustomers]    Script Date: 02/02/2020 7:19:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetCustomers]
AS BEGIN
	set nocount on;

	SELECT 
		 c.[Id]
		,c.[ReferenceNo]
		,c.[FarmId]
		,c.[Name]
		,c.[Address1]
		,c.[Address2]
		,c.[MobileNo]
		,c.[TelNo]
		,c.[TIN]
		,PaymentTypeId=cPayment.PaymentId
		,PaymentType=[Payment].PaymentType
		,PriceListId=cPriceList.PriceListId
		,PriceList=[PriceList].Name	  
		,c.PersonnelId
		,c.[Deleted]	
	FROM ((([Customer] c
		
		Left Join [CustomerPayment] cPayment On c.Id = cPayment.CustomerId)
		Left Join [Payment] On [Payment].Id = cPayment.PaymentId)

		Left Join [CustomerPriceList] cPriceList On c.Id = cPriceList.CustomerId)
		Left Join [PriceList] On [PriceList].Id = cPriceList.PriceListId
		
	Where c.Deleted = 0 
	Order By Id, c.[Name]
END


