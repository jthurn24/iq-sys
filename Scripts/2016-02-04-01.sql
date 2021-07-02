/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.SystemSMSNotification
	(
	Id bigint NOT NULL,
	SendTo varchar(500) NULL,
	Message varchar(MAX) NULL,
	AllowAfterHours bit NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.SystemSMSNotification ADD CONSTRAINT
	PK_SystemSMSNotification PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.SystemSMSNotification SET (LOCK_ESCALATION = TABLE)
GO
COMMIT



/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.AccountUser ADD
	CellPhone nvarchar(500) NULL
GO
ALTER TABLE dbo.AccountUser SET (LOCK_ESCALATION = TABLE)
GO
COMMIT


/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.AccountUser ADD
	NotificationMethod int NULL
GO
ALTER TABLE dbo.AccountUser SET (LOCK_ESCALATION = TABLE)
GO
COMMIT



CREATE TABLE [dbo].[WarningRuleNotification](
	[Id] [bigint] NOT NULL,
	[WarningRuleId] [int] NULL,
	[AccountUserId] [int] NULL,
 CONSTRAINT [PK_WarningRuleNotification] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WarningRuleNotification]  WITH CHECK ADD  CONSTRAINT [FK_AccountUserToWarningRule_AccountUser] FOREIGN KEY([AccountUserId])
REFERENCES [dbo].[AccountUser] ([Id])
GO

ALTER TABLE [dbo].[WarningRuleNotification] CHECK CONSTRAINT [FK_AccountUserToWarningRule_AccountUser]
GO

ALTER TABLE [dbo].[WarningRuleNotification]  WITH CHECK ADD  CONSTRAINT [FK_AccountUserToWarningRule_WarningRule] FOREIGN KEY([WarningRuleId])
REFERENCES [dbo].[WarningRule] ([Id])
GO

ALTER TABLE [dbo].[WarningRuleNotification] CHECK CONSTRAINT [FK_AccountUserToWarningRule_WarningRule]
GO


