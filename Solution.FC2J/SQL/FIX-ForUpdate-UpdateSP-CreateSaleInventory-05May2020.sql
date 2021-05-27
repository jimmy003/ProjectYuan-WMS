/** 
Ticket - No created entries in Inventory for the cancelled sales order

select [Stock_SANILDEFONSO], * from Product where id = 46
select [Stock_SANILDEFONSO], * from Product where id = 48
select [Stock_SANILDEFONSO], * from Product where id = 45
select [Stock_SANILDEFONSO], * from Product where id = 43
select [Stock_SANILDEFONSO], * from Product where id = 68
select [Stock_SANILDEFONSO], * from Product where id = 76

exec CancelSaleOrder @Id=3,@CustomerId=30394,@PONo=N'1288',@OverrideUser=N'admin'

-- fix: to run this to insert the cancelled records
exec CreateSaleInventory @ProductId=46,@CustomerId=30394,@Quantity=20,@PONo=N'1288',@SupplierId=2,@CancelledOrReturned=1, @enteredInventoryDate = '2020-05-04 13:56:08.837'
exec CreateSaleInventory @ProductId=48,@CustomerId=30394,@Quantity=5,@PONo=N'1288',@SupplierId=2,@CancelledOrReturned=1, @enteredInventoryDate = '2020-05-04 13:56:08.837'
exec CreateSaleInventory @ProductId=45,@CustomerId=30394,@Quantity=3,@PONo=N'1288',@SupplierId=2,@CancelledOrReturned=1, @enteredInventoryDate = '2020-05-04 13:56:08.837'
exec CreateSaleInventory @ProductId=43,@CustomerId=30394,@Quantity=2,@PONo=N'1288',@SupplierId=2,@CancelledOrReturned=1, @enteredInventoryDate = '2020-05-04 13:56:08.837'
exec CreateSaleInventory @ProductId=68,@CustomerId=30394,@Quantity=3,@PONo=N'1288',@SupplierId=2,@CancelledOrReturned=1, @enteredInventoryDate = '2020-05-04 13:56:08.837'
exec CreateSaleInventory @ProductId=76,@CustomerId=30394,@Quantity=2,@PONo=N'1288',@SupplierId=2,@CancelledOrReturned=1, @enteredInventoryDate = '2020-05-04 13:56:08.837'

select Convert(varchar, InventoryDate, 101), * from [report].[DailyInventory] 
 where customerid = '30394' order by InventoryDate Desc




exec ManageSaleDeduction @t=9,@PONo=N'1288',@CustomerId=30394
	UPDATE [sale.order.deduction].['+Convert(varchar,@CustomerId)+']
						   SET [PONo] = ''N'' 
							  ,[UsedAmount] = 0 
							  ,[UpdatedDate] = GetDate()
						 WHERE [PONo] = '''+@PONo+'''

select * from [sale.order.deduction].[30394]


-- altered createSaleInventory 
to allow to enter target inventory date
[CreateSaleInventory] 
@ProductId bigint=0,
@CustomerId varchar(12)='',
@Quantity money=0,
@PONo varchar(50)='',
@SupplierId int,
@CancelledOrReturned int = 0,
@enteredInventoryDate datetime = null

-- 
@enteredInventoryDate = '2020-05-04 13:56:08.837'

**/

-- exec CreateSaleInventory @ProductId=46,@CustomerId=30394,@Quantity=20,@PONo=N'1288',@SupplierId=2,@CancelledOrReturned=1

select Convert(varchar, InventoryDate, 101), * from [report].[DailyInventory] 
 where customerid = '30394' order by InventoryDate Desc

delete [report].[DailyInventory] where id >= 45561
select Convert(varchar, InventoryDate, 101), * from [report].[DailyInventory] where id >= 45555

select getdate() 

 --and 
 --Convert(varchar, InventoryDate, 101) = '05/04/2020' 

