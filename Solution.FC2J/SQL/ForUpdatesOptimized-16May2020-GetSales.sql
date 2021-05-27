USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[GetSales]    Script Date: 05/17/2020 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetSales]
	@UserName varchar(50) = ''

AS BEGIN

	set nocount on;

	-- this is the temp table to use to store the recordsets
	create table #saleOrderHeader
	(
		[Id] [bigint],
		[SONo] [varchar](20),
		[UserName] [varchar](50),
		[InvoiceNo] [varchar](15),
		[PONo] [varchar](15),
		[OrderDate] [datetime],
		[DeliveryDate] [datetime],
		[DueDate] [datetime],
		[TotalOrderQuantity] [float],
		[TotalOrderQuantityUOMComputed] [float],
		[TotalProductSalePrice] [money],
		[TotalProductTaxPrice] [money],
		[TotalDeductionPrice] [money],
		[PickUpDiscount] [money],
		[Outright] [money],
		[CashDiscount] [money],
		[PromoDiscount] [money],
		[LessPrice] [money],
		[TotalPrice] [money],
		[Total] [money],
		[CustomerId] [bigint],
		[CustomerName] [varchar](50),
		[CustomerAddress1] [varchar](200),
		[CustomerAddress2] [varchar](200),
		[MobileNo] [varchar](20),
		[TelNo] [varchar](20),
		[TIN] [varchar](20),
		[SFAReferenceNo] [varchar](50),
		[OrderStatusId] [bigint],
		[OrderStatus] [varchar](50),
		[SelectedPaymentTypeId] [int],
		[SelectedPaymentType] [varchar](50),
		[OverrideUser] [varchar](50),
		[SubmittedDate] [datetime],
		[ValidatedDate] [datetime],
		[CancelledDate] [datetime],
		[ReceivedDate] [datetime],
		[CollectedDate] [datetime],
		[ReceivedUser] [varchar](50),
		[CollectedUser] [varchar](50),
		[Deleted] [bit],
		[IsVatable] [bit],
		[PaidAmount] [money]
	)


	declare @PeriodNumber nvarchar(50), @Period nvarchar(150)
	declare @date_To as datetime = getdate(), 
			@date_From as datetime
	set @date_From = dateadd(week,-1, @date_To)

	select @PeriodNumber=[value]
	from KeyValuePair
	where [key] = 'PeriodNumber'
	select @Period=[value]
	from KeyValuePair
	where [key] = 'Period'

	if @Period = 'Month'
		BEGIN
		set @date_From = dateadd(month,-1 * Convert(int, @PeriodNumber), @date_To)
	END
	if @Period = 'Week'
		BEGIN
		set @date_From = dateadd(week,-1 * Convert(int, @PeriodNumber), @date_To)
	END
	if @Period = 'Day'
		BEGIN
		set @date_From = dateadd(day,-1 * Convert(int, @PeriodNumber), @date_To)
	END


	DECLARE @SQL AS NVARCHAR(MAX) = ''
	DECLARE @CustomerId bigint
	DECLARE @Id varchar(50)
	DECLARE db_cursor CURSOR FOR 

	SELECT Id
	FROM Customer
	WHERE [Deleted]=0;

	OPEN db_cursor
	FETCH NEXT FROM db_cursor INTO @CustomerId

	WHILE @@FETCH_STATUS = 0  
	BEGIN

		Set @Id = Convert(varchar,@CustomerId);

		IF (EXISTS (SELECT *
		FROM INFORMATION_SCHEMA.TABLES
		WHERE TABLE_SCHEMA = 'sale.order.header' AND TABLE_NAME = @Id))
			BEGIN

			if @UserName = 'admin'
					BEGIN

				SET @SQL = '
									Insert into #saleOrderHeader

									Select h'+@Id+'.* From [sale.order.header].['+@Id+'] h'+@Id+' 
									Where (h'+@Id+'.[Deleted] = 0 Or h'+@Id+'.[Deleted] is null) 
										AND OrderStatusId <=6
										AND OrderDate Between Convert(datetime,'''+convert(varchar, @date_From, 101)+''' + '' 00:00:00.000'') 
										And Convert(datetime,'''+convert(varchar, @date_To, 101)+''' + '' 23:59:59.999'')
									'
			END

			else
					BEGIN
				SET @SQL = '
									Insert into #saleOrderHeader
									
									Select h'+@Id+'.* From [sale.order.header].['+@Id+'] h'+@Id+' 
									Where h'+@Id+'.[UserName] = '''+@UserName+''' 
										AND (h'+@Id+'.[Deleted] = 0 Or h'+@Id+'.[Deleted] is null)  
										AND OrderStatusId <=6
										AND OrderDate Between Convert(datetime,'''+convert(varchar, @date_From, 101)+''' + '' 00:00:00.000'') 
										And Convert(datetime,'''+convert(varchar, @date_To, 101)+''' + '' 23:59:59.999'')
									'
			END

			EXEC (@SQL)

		END

		FETCH NEXT FROM db_cursor INTO @CustomerId

	END

	CLOSE db_cursor
	DEALLOCATE db_cursor

	SELECT *
	FROM #saleOrderHeader

	DROP TABLE #saleOrderHeader

END


