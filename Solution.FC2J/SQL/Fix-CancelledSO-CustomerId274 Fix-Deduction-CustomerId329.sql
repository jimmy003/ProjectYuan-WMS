-- FIX on Cancel SO, remove the created entry
-- the system should not create entry, if the status is Submitted and Cancelled
SELECT * FROM [report].[DailyInventory] where CUSTOMERID ='274' AND InventoryDate between '2020-05-11 00:00:00.000' and '2020-05-11 23:59:59.999'
-- DELETE [report].[DailyInventory] where id in (46754, 46755)
SELECT * FROM [report].[DailyInventorySanIldefonso] where CUSTOMERID ='274' AND InventoryDate between '2020-05-11 00:00:00.000' and '2020-05-11 23:59:59.999'
-- DELETE [report].[DailyInventorySanIldefonso] where id in (45721, 45722)




-- FIX on deduction, the system did not update the deduction entry
select * from [sale.order.header].[329] where pono = '1530'
SELECT * FROM [sale.order.deduction].[329]
-- UPDATE [sale.order.deduction].[329] SET pono = '1530' WHERE Id in (3,4)
