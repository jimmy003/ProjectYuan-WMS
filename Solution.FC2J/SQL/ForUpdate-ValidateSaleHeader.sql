USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[ValidateSaleHeader]    Script Date: 02/01/2020 1:24:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[ValidateSaleHeader]
	@Id bigint,
	@Revalidate bit,
	@OverrideUser varchar(50)='',
	@PONo varchar(15)='',

	@PaymentTypeId bigint,
	@DeliveryDate datetime,
	@DueDate datetime,

	@TotalOrderQuantity float,
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
	@OrderStatusId bigint=0,

	@CustomerId bigint	,
	@IsVatable bit


AS BEGIN
	
	set nocount on;
	DECLARE @SQL AS NVARCHAR(MAX) = ''

	exec [RemoveCustomerPayment] @CustomerId;	
	exec [InsertCustomerPayment] @CustomerId, @PaymentTypeId;	

	if @Revalidate = 1
	BEGIN
		SET @SQL = 
			'
				DECLARE @OrderDetailId bigint
				DECLARE @ProductId bigint 
				DECLARE @OrderQuantity float
				DECLARE @SupplierId int

				DECLARE db_cursor CURSOR FOR 
				SELECT Id, ProductId, [OrderQuantity], SupplierId  
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
		EXEC(@SQL)

	END
	
	
	Declare @PaymentType varchar(50);
	SELECT 
      @PaymentType = [PaymentType]
	FROM [dbo].[Payment] Where Id = @PaymentTypeId

	Declare @OrderStatus varchar(50) = '';
	Select 
		@OrderStatus = Replace(OrderStatus, '''', '''''')
	From [dbo].[OrderStatus] Where Id=@OrderStatusId;


	SET @SQL = '		 
		Update [sale.order.header].['+Convert(varchar,@CustomerId)+'] 
			SET 
			OrderStatusId = Convert(bigint,'''+Convert(varchar,@OrderStatusId)+'''),
			OrderStatus = '''+Isnull(@OrderStatus,'')+''','

		if @OrderStatusId=2
			BEGIN
				SET @SQL = @SQL + 'ValidatedDate = GetDate(),'
			END
		else
			BEGIN
				SET @SQL = @SQL + 'ValidatedDate = GetDate(), ReceivedDate = GetDate(),ReceivedUser = '''+@OverrideUser+''','
			END
			
	 SET @SQL = @SQL + 'OverrideUser = '''+@OverrideUser+''',
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
			DueDate = Convert(DATETIME,'''+Convert(varchar, @DueDate)+'''),
			IsVatable = Convert(bit,'''+Convert(varchar,@IsVatable)+''')			 			 
		WHERE Id = Convert(bigint,'''+Convert(varchar, @Id)+''')
	';
	EXEC(@SQL)

	DELETE [Invoice] WHERE Id=@Id And [PONo]=@PONo And [CustomerId]=@CustomerId;

	if @OrderStatusId=2
		BEGIN
			INSERT INTO [dbo].[Invoice]
				   ([Id]
				   ,[PONo]
				   ,[CustomerId])
			 VALUES
				   (@Id
				   ,@PONo
				   ,@CustomerId);
		END
	else
		BEGIN
			INSERT INTO [dbo].[Invoice]
				   ([Id]
				   ,[PONo]
				   ,[CustomerId]
				   ,[IsReceived])
			 VALUES
				   (@Id
				   ,@PONo
				   ,@CustomerId
				   ,1);
		END


END

		
