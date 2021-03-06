USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[UpdateCustomer]    Script Date: 02/02/2020 7:34:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[UpdateCustomer]

@Id bigint,
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
	UPDATE [dbo].[Customer]
	   SET [ReferenceNo] = @ReferenceNo
		  ,[FarmId] = @FarmId
		  ,[Name] = @Name
		  ,[Address1] = @Address1
		  ,[Address2] = @Address2
		  ,[MobileNo] = @MobileNo
		  ,[TelNo] = @TelNo
		  ,[TIN] = @TIN
		  ,[PersonnelId] = @PersonnelId
	 WHERE Id = @Id

	Delete [dbo].[CustomerPayment] where CustomerId = @Id;
	Insert Into CustomerPayment (CustomerId, PaymentId) Values(@Id, @PaymentTypeId);	

	Delete [dbo].[CustomerPriceList] where CustomerId = @Id;
	Insert Into [CustomerPriceList] (CustomerId, PriceListId) Values(@Id, @PriceListId);	
 
 END
