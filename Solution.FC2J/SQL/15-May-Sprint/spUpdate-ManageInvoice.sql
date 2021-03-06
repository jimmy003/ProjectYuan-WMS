USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[ManageInvoice]    Script Date: 05/11/2020 2:21:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[ManageInvoice]
	@t int=0,
	@Id bigint=0,
	@PONo varchar(15)='N',
	@CustomerId bigint=0,
	@WithReturns bit=0

AS BEGIN

	-- select all
	if @t = 0
		BEGIN
			set nocount on;
			SELECT Id, PoNo, CustomerId, IsReceived, WithReturns
				FROM Invoice
				WHERE [IsReceived]=0 or IsReceived is Null;
		END 
	
	-- update to Received
	if @t = 1
		BEGIN
			UPDATE Invoice
			SET [IsReceived]=1, WithReturns=@WithReturns
				WHERE [Id]=@Id AND [PONo]=@PONo AND [CustomerId]=@CustomerId
		END 
	
END
