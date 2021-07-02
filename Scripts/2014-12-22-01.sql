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
ALTER TABLE dbo.Room SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Patient SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.PatientRoomChange
	(
	Id bigint NOT NULL,
	PatientId bigint NULL,
	RoomId bigint NULL,
	RoomChangedAt datetime NULL,
	CreatedAt datetime NULL,
	CreatedBy nvarchar(255) NULL,
	LastUpdatedAt datetime NULL,
	LastUpdatedBy nvarchar(255) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.PatientRoomChange ADD CONSTRAINT
	PK_PatientRoomChange PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.PatientRoomChange ADD CONSTRAINT
	FK_PatientRoomChange_Patient FOREIGN KEY
	(
	PatientId
	) REFERENCES dbo.Patient
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PatientRoomChange ADD CONSTRAINT
	FK_PatientRoomChange_Room FOREIGN KEY
	(
	RoomId
	) REFERENCES dbo.Room
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PatientRoomChange SET (LOCK_ESCALATION = TABLE)
GO
COMMIT



INSERT INTO [dbo].[Pathogen] ([Id],[Name])
     VALUES (103,'C Diff')


INSERT INTO [dbo].[PathogensToPathogenSets]
           ([Id],[PathogenSetId],[PathogenId]) VALUES
           (131,4,103)


UPDATE [PsychotropicFrequency]
SET DosageFrequencyDefinitionTypeName = REPLACE(DosageFrequencyDefinitionTypeName,'Implementation','BusinessLogic')

