USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[InsertPricelist]    Script Date: 02/06/2020 9:02:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdatePricelistName]  
@Name varchar(100),
@Id bigint

AS BEGIN

		set nocount on;		
		UPDATE [PriceList] SET [Name] = @Name where Id=@Id
END

