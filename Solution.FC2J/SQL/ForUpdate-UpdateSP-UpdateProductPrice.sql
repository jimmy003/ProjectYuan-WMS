USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[UpdateProductPrice]    Script Date: 03/03/2020 6:57:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[UpdateProductPrice]
@Id bigint,
@PriceListId bigint,
@SalePrice varchar(20),
@UnitDiscount varchar(20)

AS BEGIN

	DECLARE @SQL AS NVARCHAR(MAX)= 
			'
			update [pricelist.template].['+Convert(varchar,@PricelistId)+']
			Set [SalePrice] = '''+@SalePrice+'''
						,[UnitDiscount] = '''+@UnitDiscount+'''
			Where Id = Convert(bigint,'''+Convert(Varchar,@Id)+''');
			'
	EXEC(@SQL)

END


