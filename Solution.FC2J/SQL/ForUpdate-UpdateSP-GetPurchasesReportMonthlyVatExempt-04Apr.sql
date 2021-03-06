-- exec GetPurchasesReportMonthlyVatExempt @DateFrom='2020-03-01 00:00:00',@DateTo='2020-03-31 00:00:00'

USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[GetPurchasesReportMonthlyVatExempt]    Script Date: 03/11/2020 1:44:15 PM ******/
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

	declare @df as int;
	-- 101 select convert(varchar, getdate(), 101)	12/30/200
	-- 102 select convert(varchar, getdate(), 102)	2006.12.30
	-- 103 select convert(varchar, getdate(), 103)	30/12/2006
	-- 23 select convert(varchar, getdate(), 23)	2006-12-30 
	set @df = 23;

	declare @from as varchar(50) = convert(varchar, @date_From, @df) + ' 00:00:00.000';
	declare @to as varchar(50) = convert(varchar, @date_To, @df) + ' 23:59:59.999';
	
	
	WITH PurchaseOrder_CTE (PurchaseDate, InvoiceNo, Amount, [TotalPurchases], [Total Amount], [WTAX], [BagsNotConverted], [BagsConverted], [IsTaxable])  
	AS  
	-- Define the CTE query.  
	(  
		SELECT 
				PurchaseDate = Convert(varchar,h.PurchaseDate, @df) 
				, p.InvoiceNo
				, p.Amount
				,[TotalPurchases] = ''  
				,[Total Amount] = ''
				,[WTAX] = cast(p.Amount * 0.01 as decimal(9,2))
				,[BagsNotConverted] = (select sum(d.quantity) from [sale.order.detail].[Payment_FC2J] d where d.OrderHeaderId = h.Id and d.InvoiceNo = p.InvoiceNo)
				,[BagsConverted] = (select cast(sum(d.quantity * isnull(prd.KiloPerUnit,0)) / 50 as decimal(9,2)) from [sale.order.detail].[Payment_FC2J] d inner join product prd on prd.Id = d.ProductId where d.OrderHeaderId = h.Id and d.InvoiceNo = p.InvoiceNo and prd.IsConvertibleToBag=1)
				,[IsTaxable] = (select top 1 d.IsTaxable from [sale.order.detail].[Payment_FC2J] d where d.OrderHeaderId = h.Id and d.InvoiceNo = p.InvoiceNo)

		  FROM	[sale.order.header].[Payment_FC2J] p inner join
				[sale.order.header].[FC2J] h on h.Id = p.OrderHeaderId  
		  where (p.[Deleted] = 0 or p.[Deleted] is null)
		  
		  AND h.PurchaseDate
			Between convert(datetime,@from) And convert(datetime,@to)			
	)  
	-- Define the outer query referencing the CTE name.  

	SELECT 
		PurchaseDate, 
		InvoiceNo, 
		Amount, 
		[Total Amount], 
		[WTAX], 
		BagsNotConverted = isnull([BagsNotConverted],0), 
		[BagsConverted] = isnull([BagsConverted],0) 
	FROM PurchaseOrder_CTE WHERE [IsTaxable] = @IsVatable 
	ORDER BY PurchaseDate ASC		

END