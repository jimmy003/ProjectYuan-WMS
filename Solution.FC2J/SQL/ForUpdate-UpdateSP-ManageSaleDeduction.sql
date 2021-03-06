USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[ManageSaleDeduction]    Script Date: 04/02/2020 6:43:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[ManageSaleDeduction]
	@t int=0,
	@CustomerId bigint=0,
	@Id bigint=0,
	@Particular varchar(200)='',
	@Amount money=0,
	@PONo varchar(15)='N',
	@UsedAmount money=0

AS BEGIN

	set nocount on;

	DECLARE @SQL AS NVARCHAR(MAX)=''
	 	
	-- select, with Id
	if @t = 0
		BEGIN
			SET @SQL = '
					SELECT [Id]
							,[Particular]
							,[Amount]
							,[PONo]
							,[UsedAmount]
							,[UpdatedDate]
					FROM [sale.order.deduction].['+Convert(varchar,@CustomerId)+']
  					WHERE Id=Convert(bigint,'''+Convert(varchar, @Id)+''')
				'
			EXEC(@SQL)
		END 
	
	-- insert, without Id
	if @t = 1
		BEGIN
			SET @SQL = '
				INSERT INTO [sale.order.deduction].['+Convert(varchar,@CustomerId)+']
				   ([Particular]
				   ,[Amount]
				   ,[PONo]
				   ,[UsedAmount]
				   ,[UpdatedDate])
				VALUES
				   (
					   '''+@Particular+''',
					   Convert(money,'''+Convert(varchar, @Amount)+'''),
					   '''+@PONo+''',
					   Convert(money,'''+Convert(varchar, @UsedAmount)+'''),
					   GetDate()
				   )		
				Select Id = @@IDENTITY;
				'
			EXEC(@SQL)
		END 
	
	-- edit, with Id
	if @t = 2
		BEGIN
			SET @SQL = '
					UPDATE [sale.order.deduction].['+Convert(varchar,@CustomerId)+']
					   SET [Particular] = '''+@Particular+'''
						  ,[Amount] = Convert(money,'''+Convert(varchar, @Amount)+''')
						  ,[PONo] = '''+@PONo+'''
						  ,[UsedAmount] = Convert(money,'''+Convert(varchar, @UsedAmount)+''')
						  ,[UpdatedDate] = GetDate()
					 WHERE Id=Convert(bigint,'''+Convert(varchar, @Id)+''')
				'
			EXEC(@SQL)
		END 
	
	-- query records allowed to edit, PONo = 'N'
	-- when PONo is N, this means record is new and not yet used 
	if @t = 3
		BEGIN
			SET @SQL = '
					SELECT [Id]
							,[Particular]
							,[Amount]
							,[PONo]
							,[UsedAmount]
							,[UpdatedDate]
					FROM [sale.order.deduction].['+Convert(varchar,@CustomerId)+']
  					WHERE PONo = ''N'' Order By [UpdatedDate] 
				'
			EXEC(@SQL)
		END 

	-- query records allowed to edit, PONo <> 'N'
	-- when PONo is not N, this means record has used amount  
	if @t = 4
		BEGIN
			SET @SQL = '
					SELECT [Id]
							,[Particular]
							,[Amount]
							,[PONo]
							,[UsedAmount]
							,[UpdatedDate]
					FROM [sale.order.deduction].['+Convert(varchar,@CustomerId)+']
  					WHERE PONo <> ''N'' and PONo <> ''C''  Order By [UpdatedDate] 
				'
			EXEC(@SQL)
		END 

	-- query all records
	if @t = 5
		BEGIN
			SET @SQL = '
					SELECT [Id]
							,[Particular]
							,[Amount]
							,PONo = CASE When [PONo]=''N'' Then '''' ELSE [PONo] END
							,[UsedAmount]
							,[UpdatedDate]
					FROM [sale.order.deduction].['+Convert(varchar,@CustomerId)+']
					WHERE PONo <> ''C''
  					Order By [UpdatedDate] 
				'
			EXEC(@SQL)
		END 

	-- cancel record
	if @t = 6
		BEGIN
			SET @SQL = '
					UPDATE [sale.order.deduction].['+Convert(varchar,@CustomerId)+']
					   SET [PONo] = ''C''
					 WHERE Id=Convert(bigint,'''+Convert(varchar, @Id)+''')
				'
			EXEC(@SQL)
		END 

	-- edit using Id only with PONo
	if @t = 7
		BEGIN
			SET @SQL = '
					UPDATE [sale.order.deduction].['+Convert(varchar,@CustomerId)+']
					   SET [PONo] = '''+@PONo+'''
						  ,[UsedAmount] = [Amount] 
						  ,[UpdatedDate] = GetDate()
					 WHERE Id=Convert(bigint,'''+Convert(varchar, @Id)+''')
				'
			EXEC(@SQL)
		END 


	-- query records using PONo
	if @t = 8
		BEGIN
			SET @SQL = '
					SELECT [Id]
							,[Particular]
							,[Amount]
							,[PONo]
							,[UsedAmount]
							,[UpdatedDate]							
					FROM [sale.order.deduction].['+Convert(varchar,@CustomerId)+']
  					WHERE PONo = '''+@PONo+''' Order By [UpdatedDate] 
				'
			EXEC(@SQL)
		END 

	-- reset the deductions using Id only with PONo
	if @t = 9
		BEGIN
			SET @SQL = '
					UPDATE [sale.order.deduction].['+Convert(varchar,@CustomerId)+']
					   SET [PONo] = ''N'' 
						  ,[UsedAmount] = 0 
						  ,[UpdatedDate] = GetDate()
					 WHERE [PONo] = '''+@PONo+'''
				'
			EXEC(@SQL)
		END 
END
