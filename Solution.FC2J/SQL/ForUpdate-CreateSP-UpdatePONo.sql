USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[AcknowledgedPO]    Script Date: 02/29/2020 1:15:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[UpdatePONo]

	@CustomerId varchar(10) = '',
	@OldPONo varchar(15)='',
	@NewPONo varchar(15)='',
	@Id bigint

AS BEGIN


	DECLARE @SQL AS NVARCHAR(MAX) = ''		

	SET @SQL = '
				UPDATE [sale.order.header].['+@CustomerId+'] SET PONO = '''+@NewPONo+''' WHERE Id = Convert(bigint,'''+ Convert(varchar,@Id) +''')
				'
	EXEC(@SQL)
				

	SET @SQL = '
				UPDATE [sale.order.header].[Payment_'+@CustomerId+'] SET PONO = '''+@NewPONo+''' WHERE OrderHeaderId = Convert(bigint,'''+ Convert(varchar,@Id) +''')	
				'
	EXEC(@SQL)

	SET @SQL = '
				UPDATE [sale.order.deduction].['+@CustomerId+'] SET PONo = '''+@NewPONo+''' WHERE PONo = '''+@OldPONo+'''	
				'
	EXEC(@SQL)
			   
END 


-- select * from [sale.order.header].[177]

-- exec UpdatePONo @CustomerId=N'177',@OldPONo=N'19991',@NewPONo=N'19992',@Id=1
