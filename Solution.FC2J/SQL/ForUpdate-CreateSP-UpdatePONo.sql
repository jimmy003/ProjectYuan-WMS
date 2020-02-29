USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[AcknowledgedPO]    Script Date: 02/29/2020 1:15:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdatePONo]

	@CustomerId varchar(10) = '',
	@OldPONo varchar(15)='',
	@NewPONo varchar(15)='',
	@Id bigint

AS BEGIN


	DECLARE @SQL AS NVARCHAR(MAX) = ''		

	SET @SQL = '
				UDPATE SET PONO = '''+@NewPONo+''' [sale.order.header].['+@CustomerId+'] WHERE Id = Convert(bigint,'''+@Id+''')	
				'
	exec(@SQL)
				

	SET @SQL = '
				UDPATE SET PONO = '''+@NewPONo+''' [sale.order.header].[Payment_'+@CustomerId+'] WHERE OrderHeaderId = Convert(bigint,'''+@Id+''')	
				'
	exec(@SQL)

	SET @SQL = '
				UDPATE SET PONo = '''+@NewPONo+''' [sale.order.deduction].['+@CustomerId+'] WHERE PONo = '''+@OldPONo+'''	
				'
	exec(@SQL)
			   
END 
