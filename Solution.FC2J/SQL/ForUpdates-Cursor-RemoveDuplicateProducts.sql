
	
	DECLARE @Id varchar(10), @SQL NVARCHAR(300) 
	DECLARE db_cursor CURSOR FOR 

	SELECT Id FROM PRICELIST WHERE DELETED = 0

	OPEN db_cursor  
	FETCH NEXT FROM db_cursor INTO @Id 

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		
		SET @SQL = 'DELETE FROM [pricelist.template].['+CONVERT(VARCHAR, @ID)+'] WHERE ID IN (20124, 20125, 20126, 20127, 20128)' 
		EXEC(@SQL)
		FETCH NEXT FROM db_cursor INTO @Id

	END 

	CLOSE db_cursor  
	DEALLOCATE db_cursor 

	DELETE Product WHERE ID IN (20124, 20125, 20126, 20127, 20128)