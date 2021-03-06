USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[DeletePayment_FC2J]    Script Date: 03/11/2020 3:16:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[DeletePayment_FC2J]  
@Id bigint=0,
@DeletedBy varchar(50)='',
@InvoiceNo varchar(25) = ''

AS BEGIN
		
		set nocount on;

		UPDATE [sale.order.header].[Payment_FC2J] SET DELETED = 1, DeletedBy = @DeletedBy, [DateDeleted]=GetDate() where [Id]=@Id 

		Declare @OrderHeaderId bigint
		Select @OrderHeaderId= [OrderHeaderId] from [sale.order.header].[Payment_FC2J] where [Id] = @Id 

		DELETE [sale.order.detail].[Payment_FC2J] Where InvoiceNo = @InvoiceNo and [OrderHeaderId] = @OrderHeaderId

END



