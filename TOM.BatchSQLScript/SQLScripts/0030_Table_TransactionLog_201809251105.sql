IF OBJECT_ID('[dbo].[TransactionLog]', 'U') IS NOT NULL DROP TABLE [dbo].[TransactionLog]; 

CREATE TABLE [dbo].[TransactionLog]
(
	[IDLog] INT NOT NULL PRIMARY KEY,
	[IDFunction] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterFunction](IDFunction),
	[IDUser] VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES [dbo].[MasterUser](IDUser),
	[CreatedDate] DATETIME NOT NULL,
	[Action] VARCHAR(50) NOT NULL,
	[Remarks] VARCHAR(1000) NOT NULL,
);