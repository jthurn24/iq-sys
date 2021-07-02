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
ALTER TABLE dbo.Facts_InfectionVerification ADD
	SupportingDetail varchar(255) NULL
GO
ALTER TABLE dbo.Facts_InfectionVerification SET (LOCK_ESCALATION = TABLE)
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
CREATE TABLE dbo.Cubes_FacilityMonthSCAUTI
	(
	Id int NOT NULL IDENTITY (1, 1),
	FacilityId int NULL,
	AccountId int NULL,
	Total int NULL,
	DeviceDays int NULL,
	Rate decimal(19, 5) NULL,
	MonthId int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Cubes_FacilityMonthSCAUTI ADD CONSTRAINT
	PK_Cubes_FacilityMonthSCAUTI PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Cubes_FacilityMonthSCAUTI SET (LOCK_ESCALATION = TABLE)
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
ALTER TABLE dbo.Cubes_FacilityMonthSCAUTI ADD
	Change decimal(19, 5) NULL
GO
ALTER TABLE dbo.Cubes_FacilityMonthSCAUTI SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

