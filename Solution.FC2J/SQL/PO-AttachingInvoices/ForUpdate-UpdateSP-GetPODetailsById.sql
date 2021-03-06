USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[GetPODetailsById]    Script Date: 03/11/2020 3:06:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[GetPODetailsById] 
@POHeaderId bigint

AS BEGIN
	set nocount on;

	SELECT 
	  d.Id 
	  ,d.[POHeaderId]
      ,d.[LineNo]
      ,d.[ProductId]
      ,Quantity = d.[Quantity] - isnull((select sum(x.Quantity) from [sale.order.detail].[Payment_FC2J] x where x.OrderHeaderId=26 And x.ProductId = d.ProductId),0)
      ,d.[Name]
      ,d.[Category]
      ,d.[UnitOfMeasure]
      ,d.[SalePrice]
      ,d.[UnitDiscount]
      ,d.[SFAUnitOfMeasure]
      ,d.[SFAReferenceNo]
      ,d.[SubTotal]
      ,d.[IsTaxable]
      ,d.[TaxRate]
      ,d.[TaxPrice]
      ,d.[IsDelivered]
	  ,d.InvoiceNo	
	  FROM [sale.order.detail].[FC2J] d
	  Where d.POHeaderId = @POHeaderId
	  Order by d.[LineNo]
	
END
