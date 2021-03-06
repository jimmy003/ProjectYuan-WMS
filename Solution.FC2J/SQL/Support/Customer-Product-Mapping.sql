
SELECT C.[Name] AS [Customer Name]
      ,C.[ReferenceNo] AS [Customer Code]
      ,CONVERT(varchar, C.[PersonnelId]) + '-' + P.[Name] AS [Name of Salesperson]
  FROM [PROJECT.FC2J].[dbo].[Customer] C
  LEFT JOIN [Personnel] P 
	ON C.[PersonnelId] = P.[Id]
	WHERE C.[Deleted] = 0

/* 

Bro patulong po nito. Humihingi lang po si san miguel kasi ichecheck lang po nila kung tugma ung data ng system natin sa system nila.

For the dsp mapping ganto po dapat ung format nya ma'am: excel file: (dsp_map.xlsx)
a. Customer Name
b. Customer Code
c. Name of Salesperson 

For product mapping: excel file:  (product_map.xlsx)
a. Product Name
b. Product Code
c. UNSPC/Material code (taken from the salesforce)
d. SRP


*/


SELECT 
      [Name] AS [Product Name]
      ,[SFAReferenceNo] AS [UNSPC/Material code]
      ,[SalePrice_SANILDEFONSO] AS [SRP]
  FROM [PROJECT.FC2J].[dbo].[Product]
  WHERE DELETED =0