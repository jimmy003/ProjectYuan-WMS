USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[CreateSaleInventory]    Script Date: 03/12/2020 7:22:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[CreateSaleInventory] 
@ProductId bigint=0,
@CustomerId varchar(12)='',
@Quantity money=0,
@PONo varchar(50)='',
@SupplierId int,
@CancelledOrReturned int = 0

AS BEGIN
	
-- @CancelledOrReturned
--0 = Do nothing
--1 = Cancelled
--2 = Returned

--SupplierId	Supplier
--0	Coron
--1	Lubang
--2	San Ildefonso

	set nocount on;
	declare @inventoryDate datetime = getdate();
	declare @InternalCategory varchar(100)=''
	declare @CustomerName varchar(50)=''
	declare @ProductName varchar(50)=''

	select @InternalCategory=Isnull(InternalCategory,''),@ProductName=isnull(name,'') from product where Id=@ProductId;
	
	if (@CustomerId = '0' OR @CustomerId = 'Z')
		BEGIN
			SET @CustomerName = ''
		END 
	else
		BEGIN
			select @CustomerName=Name from customer where Id=Convert(bigint,@CustomerId);

			if @CancelledOrReturned = 1 --Cancelled 
				BEGIN
					SET @CustomerName = @CustomerName + '_CANCELLED'
					SET @Quantity = @Quantity * -1
				END
			if @CancelledOrReturned = 2 --Returned
				BEGIN
					SET @CustomerName = @CustomerName + '_RETURNED'
					SET @Quantity = @Quantity * -1
				END
		END 

	insert into [report].[DailyInventory] 
	(
		[InventoryDate]
		,[ProductId]
		,[ProductName]
		,[InternalCategory]
		,[Sinulid]
		,[CustomerId]
		,[CustomerName]
		,[Quantity]
		,[PONo]
	)
	VALUES
		( @inventoryDate
		, @ProductId
		, @ProductName
		, @InternalCategory
		, ''
		, @CustomerId
		, @CustomerName
		, @Quantity
		, @PONo	)	

	if @SupplierId = 0 -- Coron
	BEGIN
		insert into [report].[DailyInventoryCoron] 
		(
			[InventoryDate]
			,[ProductId]
			,[ProductName]
			,[InternalCategory]
			,[Sinulid]
			,[CustomerId]
			,[CustomerName]
			,[Quantity]
			,[PONo]
		)
		VALUES
			( @inventoryDate
			, @ProductId
			, @ProductName
			, @InternalCategory
			, ''
			, @CustomerId
			, @CustomerName
			, @Quantity
			, @PONo	)	
	END 
	
	if @SupplierId = 1 -- Lubang 
	BEGIN
		insert into [report].[DailyInventoryLubang] 
		(
			[InventoryDate]
			,[ProductId]
			,[ProductName]
			,[InternalCategory]
			,[Sinulid]
			,[CustomerId]
			,[CustomerName]
			,[Quantity]
			,[PONo]
		)
		VALUES
			( @inventoryDate
			, @ProductId
			, @ProductName
			, @InternalCategory
			, ''
			, @CustomerId
			, @CustomerName
			, @Quantity
			, @PONo	)	
	END 

	if @SupplierId = 2 -- San Ildefonso 
	BEGIN
		insert into [report].[DailyInventorySanIldefonso] 
		(
			[InventoryDate]
			,[ProductId]
			,[ProductName]
			,[InternalCategory]
			,[Sinulid]
			,[CustomerId]
			,[CustomerName]
			,[Quantity]
			,[PONo]
		)
		VALUES
			( @inventoryDate
			, @ProductId
			, @ProductName
			, @InternalCategory
			, ''
			, @CustomerId
			, @CustomerName
			, @Quantity
			, @PONo	)	
	END 

END