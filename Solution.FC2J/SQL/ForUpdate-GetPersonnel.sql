USE [PROJECT.FC2J]
GO
/****** Object:  StoredProcedure [dbo].[GetCustomerAddress2]    Script Date: 02/02/2020 1:46:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetPersonnel]

AS BEGIN
	set nocount on;
	SELECT * FROM [Personnel] ORDER BY Name

END
