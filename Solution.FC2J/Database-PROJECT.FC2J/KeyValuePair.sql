﻿CREATE TABLE [dbo].[KeyValuePair]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[Key] NVARCHAR(50) NOT NULL,
	[Value] NVARCHAR(150) NOT NULL
)