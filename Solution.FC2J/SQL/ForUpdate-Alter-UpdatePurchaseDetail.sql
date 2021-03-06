USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePurchaseDetail]    Script Date: 02/29/2020 11:18:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[UpdatePurchaseDetail]
-- to tag if delivered or not 
	@Id bigint=0,
	@ProductId bigint=0,
	@IsDelivered bit=0,
	@DeliveredUser varchar(50)='',
	@Quantity float=0,
	@SupplierId int

AS BEGIN

	UPDATE [sale.order.detail].[FC2J] SET [IsDelivered]=@IsDelivered, DeliveredUser=@DeliveredUser, DeliveredDate=GetDate() WHERE Id=@Id;	

	if @IsDelivered = 1
		BEGIN
			if @SupplierId = 0 -- Coron
			BEGIN
				UPDATE [Product] SET Stock_CORON = Stock_CORON + @Quantity  WHERE Id=@ProductId;	
			END 
			
			if @SupplierId = 1 -- Lubang
			BEGIN
				UPDATE [Product] SET Stock_LUBANG = Stock_LUBANG + @Quantity  WHERE Id=@ProductId;	
			END 

			if @SupplierId = 2 -- San Ildefonso
			BEGIN
				UPDATE [Product] SET Stock_SANILDEFONSO = Stock_SANILDEFONSO + @Quantity  WHERE Id=@ProductId;	
			END 
			
			
		END 



END 


