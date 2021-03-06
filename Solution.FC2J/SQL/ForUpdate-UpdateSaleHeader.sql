USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[UpdateSaleHeader]    Script Date: 02/01/2020 1:19:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[UpdateSaleHeader]

@Id bigint,
@UserName varchar(50)='',
@PaymentTypeId bigint,
@DeliveryDate datetime,
@DueDate datetime,

@TotalOrderQuantity float=0,
@TotalOrderQuantityUOMComputed float=0,
@TotalProductSalePrice money,
@TotalProductTaxPrice money,
@TotalDeductionPrice money,

@PickupDiscount money=0,
@Outright money=0,
@CashDiscount money=0,
@PromoDiscount money=0,

@LessPrice money,
@TotalPrice money,
@Total money=0,
@CustomerId bigint

AS BEGIN
	
	Declare @OrderStatus varchar(50) = '';
	Declare @PaymentType varchar(50);
	
	DECLARE @SQL AS NVARCHAR(MAX) = ''
	set nocount on;

	SELECT 
      @PaymentType = [PaymentType]
	FROM [dbo].[Payment] Where Id = @PaymentTypeId

	exec [RemoveCustomerPayment] @CustomerId;	
	exec [InsertCustomerPayment] @CustomerId, @PaymentTypeId;	

	SET @SQL = 
		'
			DECLARE @OrderDetailId bigint
			DECLARE @ProductId bigint 
			DECLARE @SupplierId int 
			DECLARE @OrderQuantity float

			DECLARE db_cursor CURSOR FOR 
			SELECT Id, ProductId, [OrderQuantity], [SupplierId]  
					FROM [sale.order.detail].['+Convert(varchar,@CustomerId)+'] 
					WHERE [OrderHeaderId] = '''+Convert(varchar,@Id)+''' AND [Deleted]=0

			OPEN db_cursor  
			FETCH NEXT FROM db_cursor INTO @OrderDetailId, @ProductId, @OrderQuantity, @SupplierId  

			WHILE @@FETCH_STATUS = 0  
			BEGIN  

				DELETE [sale.order.detail].['+Convert(varchar,@CustomerId)+'] Where Id=@OrderDetailId;

				FETCH NEXT FROM db_cursor INTO @OrderDetailId, @ProductId, @OrderQuantity, @SupplierId  

			END 

			CLOSE db_cursor  
			DEALLOCATE db_cursor 			
		';
	EXEC (@SQL);

	SET @SQL = '		 
		Update [sale.order.header].['+Convert(varchar,@CustomerId)+'] 
			SET 
			SubmittedDate = GetDate(),

			TotalOrderQuantity = Convert(float,'''+Convert(varchar, @TotalOrderQuantity)+'''),
			TotalOrderQuantityUOMComputed = Convert(float,'''+Convert(varchar, @TotalOrderQuantityUOMComputed)+'''),

			TotalProductSalePrice = Convert(money,'''+Convert(varchar, @TotalProductSalePrice)+'''),
			TotalProductTaxPrice = Convert(money,'''+Convert(varchar, @TotalProductTaxPrice)+'''),
			TotalDeductionPrice = Convert(money,'''+Convert(varchar, @TotalDeductionPrice)+'''),

			PickUpDiscount = Convert(money,'''+Convert(varchar, @PickUpDiscount)+'''),
			Outright = Convert(money,'''+Convert(varchar, @Outright)+'''),
			CashDiscount = Convert(money,'''+Convert(varchar, @CashDiscount)+'''),
			PromoDiscount = Convert(money,'''+Convert(varchar, @PromoDiscount)+'''),

			LessPrice = Convert(money,'''+Convert(varchar, @LessPrice)+'''),
			TotalPrice = Convert(money,'''+Convert(varchar, @TotalPrice)+'''),
			Total = Convert(money,'''+Convert(varchar, @Total)+'''),

			SelectedPaymentTypeId = Convert(bigint,'''+Convert(varchar, @PaymentTypeId)+'''),
			SelectedPaymentType = '''+Convert(varchar, @PaymentType)+''',
			DeliveryDate = Convert(DATETIME,'''+Convert(varchar, @DeliveryDate)+'''),
			DueDate = Convert(DATETIME,'''+Convert(varchar, @DueDate)+''')			
			 			 
		WHERE Id = Convert(bigint,'''+Convert(varchar, @Id)+''')
	';
	EXEC (@SQL)


	


END


