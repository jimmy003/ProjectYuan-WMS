--USE master;  
--GO  
--EXEC sp_clean_db_file_free_space   
--@dbname = N'PROJECT.FC2J', @fileid = 1 ; 


-- Truncate the log by changing the database recovery model to SIMPLE.  
ALTER DATABASE [PROJECT.FC2J]  
SET RECOVERY SIMPLE;  
GO  
-- Shrink the truncated log file to 1 MB.  
DBCC SHRINKFILE ([PROJECT.FC2J_Log], 1);  
GO  
-- Reset the database recovery model.  
ALTER DATABASE [PROJECT.FC2J] 
SET RECOVERY FULL;  
GO  