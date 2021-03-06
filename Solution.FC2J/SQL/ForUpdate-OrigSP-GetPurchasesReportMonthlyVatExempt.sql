USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[GetPurchasesReportMonthlyVatExempt]    Script Date: 03/11/2020 1:59:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetPurchasesReportMonthlyVatExempt]  
@DateFrom datetime,
@DateTo datetime

AS BEGIN
	Declare @IsVatable bit = 0

	set nocount on;

	declare @date_To as datetime = getdate()
	declare @date_From as datetime 
	set @date_From = dateadd(month,-1, @date_To);

	set @date_From = @DateFrom;
	set @date_To = @DateTo;


	
	
	WITH PurchaseOrder_CTE (PurchaseDate, InvoiceNo, Amount, [TotalPurchases], [Total Amount], [WTAX], [BagsNotConverted], [BagsConverted], [IsTaxable])  
	AS  
	-- Define the CTE query.  
	(  
		SELECT 
				PurchaseDate = Convert(varchar,h.PurchaseDate, 103) 
				, p.InvoiceNo
				, p.Amount
				,[TotalPurchases] = ''  
				,[Total Amount] = ''
				,[WTAX] = cast(p.Amount * 0.01 as decimal(9,2))
				,[BagsNotConverted] = (select sum(d.quantity) from [sale.order.detail].[FC2J] d inner join product prd on prd.Id = d.ProductId where d.POHeaderId = h.Id and prd.IsConvertibleToBag=0)
				,[BagsConverted] = (select cast(sum(d.quantity * isnull(prd.KiloPerUnit,0)) / 50 as decimal(9,2)) from [sale.order.detail].[FC2J] d inner join product prd on prd.Id = d.ProductId where d.POHeaderId = h.Id and prd.IsConvertibleToBag=1)
				,[IsTaxable] = (select top 1 d.IsTaxable from [sale.order.detail].[FC2J] d where d.POHeaderId = h.Id and d.InvoiceNo = p.InvoiceNo)

		  FROM	[sale.order.header].[Payment_FC2J] p inner join
				[sale.order.header].[FC2J] h on h.Id = p.OrderHeaderId  
		  where (p.[Deleted] = 0 or p.[Deleted] is null)
		  
		  AND h.PurchaseDate
			Between Convert(datetime,convert(varchar, @date_From, 101) + ' 00:00:00.000') And Convert(datetime,convert(varchar, @date_To, 101) + ' 23:59:59.999')					
		
	)  
	-- Define the outer query referencing the CTE name.  

	SELECT PurchaseDate, InvoiceNo, Amount, [Total Amount], [WTAX], [BagsNotConverted], [BagsConverted]  
	FROM PurchaseOrder_CTE WHERE [IsTaxable] = @IsVatable 
	ORDER BY PurchaseDate ASC			

END