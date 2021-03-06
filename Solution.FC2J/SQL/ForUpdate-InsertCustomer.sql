USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[InsertCustomer]    Script Date: 02/02/2020 7:30:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[InsertCustomer]
@ReferenceNo varchar(50)='',
@Name varchar(50)='',
@Address1 varchar(200)='',
@Address2 varchar(200)='',
@MobileNo varchar(120)='',
@TelNo varchar(20)='',
@TIN varchar(20)='',
@PaymentTypeId bigint,
@PriceListId bigint,
@PersonnelId int,
@FarmId bigint


AS BEGIN
	set nocount on;
	Declare @Id as bigint;

	INSERT INTO [dbo].[Customer]
           ([ReferenceNo]
		   ,[FarmId]
           ,[Name]
           ,[Address1]
           ,[Address2]
           ,[MobileNo]
           ,[TelNo]
           ,[TIN]
		   ,[PersonnelId]
           ,[Deleted])
     VALUES
           (@ReferenceNo
           ,@FarmId
		   ,@Name
           ,@Address1
           ,@Address2
           ,@MobileNo
           ,@TelNo
           ,@TIN
		   ,@PersonnelId
           ,0)

	Set @Id = @@IDENTITY; 
	
	Delete [dbo].[CustomerPayment] where CustomerId = @Id;
	Insert Into CustomerPayment (CustomerId, PaymentId) Values(@Id, @PaymentTypeId);	

	Delete [dbo].[CustomerPriceList] where CustomerId = @Id;
	Insert Into [CustomerPriceList] (CustomerId, PriceListId) Values(@Id, @PriceListId);	

	Exec CreateCustomerSaleTables @Id;	

	Select Id = @Id;

END
