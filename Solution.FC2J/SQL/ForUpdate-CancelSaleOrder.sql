USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[CancelSaleOrder]    Script Date: 02/01/2020 1:45:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[CancelSaleOrder]
@Id bigint,
@OverrideUser varchar(50)='', 
@CustomerId bigint,
@PONo varchar(15)=''

AS BEGIN
	
	DECLARE @SQL AS NVARCHAR(MAX) = ''

	SET @SQL = 
			'
				set nocount on;
				DECLARE @OrderDetailId bigint
				DECLARE @ProductId bigint 
				DECLARE @OrderQuantity float
				DECLARE @SupplierId int 

				DECLARE @OrderStatusId bigint
				SELECT @OrderStatusId = OrderStatusId FROM [sale.order.header].['+Convert(varchar,@CustomerId)+'] WHERE [Id] = '''+Convert(varchar,@Id)+''' 


				DECLARE db_cursor CURSOR FOR 
				SELECT Id, ProductId, [OrderQuantity] , SupplierId  
				FROM [sale.order.detail].['+Convert(varchar,@CustomerId)+']
				WHERE [OrderHeaderId] = '''+Convert(varchar,@Id)+''' AND [Deleted]=0

				OPEN db_cursor  
				FETCH NEXT FROM db_cursor INTO @OrderDetailId, @ProductId, @OrderQuantity, @SupplierId    

				WHILE @@FETCH_STATUS = 0  
				BEGIN  

					if @OrderStatusId = 2
						BEGIN
							if @SupplierId = 0 
								BEGIN						
									Update [dbo].[Product] Set [Stock_CORON]=[Stock_CORON] + @OrderQuantity Where Id=@ProductId;
								END 

							else if @SupplierId = 1 
								BEGIN						
									Update [dbo].[Product] Set [Stock_LUBANG]=[Stock_LUBANG] + @OrderQuantity Where Id=@ProductId;
								END 

							else if @SupplierId = 2 
								BEGIN						
									Update [dbo].[Product] Set [Stock_SANILDEFONSO]=[Stock_SANILDEFONSO] + @OrderQuantity Where Id=@ProductId;
								END 
						END 

					FETCH NEXT FROM db_cursor INTO @OrderDetailId, @ProductId, @OrderQuantity, @SupplierId    

				END 

				CLOSE db_cursor  
				DEALLOCATE db_cursor 

				UPDATE
				[sale.order.header].['+Convert(varchar,@CustomerId)+']   
					SET
					OrderStatusId = 3,
					OrderStatus = ''Cancelled'',
					CancelledDate = GetDate(),
					OverrideUser =  '''+Convert(varchar, @OverrideUser)+'''
				Where Id = '''+Convert(varchar,@Id)+'''
			';

	EXEC(@SQL);
	
	set nocount on;
	DELETE [Invoice] WHERE Id=@Id And [PONo]=@PONo And [CustomerId]=@CustomerId;	

	EXEC [ManageSaleDeduction]
		@t=9,
		@CustomerId=@CustomerId,
		@PONo=@PONo


END

