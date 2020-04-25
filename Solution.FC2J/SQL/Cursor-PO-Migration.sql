
	DECLARE @InvoiceNo varchar(25)
	DECLARE db_cursor CURSOR FOR 

	SELECT InvoiceNo FROM [PROJECT.FC2J].[sale.order.header].[Payment_FC2J]		
	WHERE [Deleted] is null OR [Deleted] = 0

	OPEN db_cursor  
	FETCH NEXT FROM db_cursor INTO @InvoiceNo 

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		
		INSERT INTO [sale.order.detail].[Payment_FC2J]
				   ([OrderHeaderId]
				   ,[ProductId]
				   ,[Quantity]
				   ,[Name]
				   ,[Category]
				   ,[UnitOfMeasure]
				   ,[SalePrice]
				   ,[UnitDiscount]

				   ,[SFAUnitOfMeasure]
				   ,[SFAReferenceNo]
				   ,[SubTotal]
				   ,[IsTaxable]

				   ,[TaxRate]
				   ,[TaxPrice]
				   ,[InvoiceNo])
			 SELECT 
			   [POHeaderId]
			  ,[ProductId]
			  ,[Quantity]
			  ,[Name]
			  ,[Category]
			  ,[UnitOfMeasure]
			  ,[SalePrice]
			  ,[UnitDiscount]
			  ,[SFAUnitOfMeasure]
			  ,[SFAReferenceNo]
			  ,[SubTotal]
			  ,[IsTaxable]
			  ,[TaxRate]
			  ,[TaxPrice]
			  ,[InvoiceNo]
		  FROM [sale.order.detail].[FC2J] WHERE [InvoiceNo] = @InvoiceNo
		

		FETCH NEXT FROM db_cursor INTO @InvoiceNo

	END 

	CLOSE db_cursor  
	DEALLOCATE db_cursor 
