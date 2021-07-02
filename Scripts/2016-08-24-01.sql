

INSERT INTO [dbo].[PatientPrecaution]
           ([Id]
           ,[PatientId]
           ,[PrecautionTypeId]
           ,[StartDate]
           ,[EndDate]
           ,[AdditionalDescription]
           ,[Guid]
           ,[Deleted])
SELECT distinct
([InfectionVerification].Id *3) + PrecautionType.Id ,
PatientId,
PrecautionType.Id,
FirstNotedOn,
ResolvedOn,
'',
NEWID(),
0
  FROM [InfectionVerification] 
  inner JOIN PrecautionsToInfectionVerifications on PrecautionsToInfectionVerifications.InfectionVerificationId = [InfectionVerification].Id
  LEFT JOIN [InfectionPrecaution] on [InfectionPrecaution].Id = PrecautionsToInfectionVerifications.InfectionPrecautionId
  LEFT JOIN PrecautionType on PrecautionType.Name = InfectionPrecaution.Name