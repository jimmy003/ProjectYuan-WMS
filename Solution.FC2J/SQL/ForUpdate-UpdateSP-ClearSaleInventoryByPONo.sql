USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[ClearSaleInventoryByPONo]    Script Date: 03/03/2020 7:39:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[ClearSaleInventoryByPONo] 
@PONo varchar(50)='',
@CustomerId varchar(12)='',
@SupplierId int

AS BEGIN
	
	set nocount on;
	declare @inventoryDate datetime = getdate();
	DELETE FROM [report].[DailyInventory] WHERE convert(varchar, [InventoryDate], 101) = convert(varchar, @inventoryDate, 101)
		AND [PONo] = @PONo AND [CustomerId] = @CustomerId

	if @SupplierId = 0 -- Coron
	BEGIN
		DELETE FROM [report].[DailyInventoryCoron] WHERE convert(varchar, [InventoryDate], 101) = convert(varchar, @inventoryDate, 101)
			AND [PONo] = @PONo AND [CustomerId] = @CustomerId
	END 
	
	if @SupplierId = 1 -- Lubang 
	BEGIN
		DELETE FROM [report].[DailyInventoryLubang] WHERE convert(varchar, [InventoryDate], 101) = convert(varchar, @inventoryDate, 101)
			AND [PONo] = @PONo AND [CustomerId] = @CustomerId
	END 

	if @SupplierId = 2 -- San Ildefonso 
	BEGIN
		DELETE FROM [report].[DailyInventorySanIldefonso] WHERE convert(varchar, [InventoryDate], 101) = convert(varchar, @inventoryDate, 101)
			AND [PONo] = @PONo AND [CustomerId] = @CustomerId
	END 
END