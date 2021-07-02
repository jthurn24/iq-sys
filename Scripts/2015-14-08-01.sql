

INSERT INTO [dbo].[VaccineType] ([Id],[CVXCode],[CVXShortDescription],[FullVaccineName],[VaccineStatus],[InternalID],[NonVaccine],[CreatedAt],[CreatedBy]) VALUES
(170,165,'HPV9',	'Human Papillomavirus 9-valent vaccine',	2	,184,	0, GETDATE() ,'system')

INSERT INTO [dbo].[VaccineType] ([Id],[CVXCode],[CVXShortDescription],[FullVaccineName],[VaccineStatus],[InternalID],[NonVaccine],[CreatedAt],[CreatedBy]) VALUES
(171,161,'Influenza, injectable,quadrivalent, preservative free, pediatric','Influenza, injectable,quadrivalent, preservative free, pediatric',	2,	180,	0, GETDATE() ,'system')

INSERT INTO [dbo].[VaccineType] ([Id],[CVXCode],[CVXShortDescription],[FullVaccineName],[VaccineStatus],[InternalID],[NonVaccine],[CreatedAt],[CreatedBy]) VALUES
(172,166,'influenza, intradermal, quadrivalent, preservative free','influenza, intradermal, quadrivalent, preservative free, injectable',	2,	185,	0, GETDATE() ,'system')

INSERT INTO [dbo].[VaccineType] ([Id],[CVXCode],[CVXShortDescription],[FullVaccineName],[VaccineStatus],[InternalID],[NonVaccine],[CreatedAt],[CreatedBy]) VALUES
(173,163,'meningococcal B, OMV','meningococcal B vaccine, recombinant, OMV, adjuvanted',	2	,182,	0, GETDATE() ,'system')

INSERT INTO [dbo].[VaccineType] ([Id],[CVXCode],[CVXShortDescription],[FullVaccineName],[VaccineStatus],[InternalID],[NonVaccine],[CreatedAt],[CreatedBy]) VALUES
(174,162,'meningococcal B, recombinant','meningococcal B vaccine, fully recombinant',	2,	181	,0, GETDATE() ,'system')

INSERT INTO [dbo].[VaccineType] ([Id],[CVXCode],[CVXShortDescription],[FullVaccineName],[VaccineStatus],[InternalID],[NonVaccine],[CreatedAt],[CreatedBy]) VALUES
(175,164,'meningococcal B, unspecified','meningococcal B, unspecified formulation',	2,	183,	0, GETDATE() ,'system')



DELETE
  FROM [VaccineEntry]

DELETE
  FROM [VaccineTradeName]



INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (1, 113, N'ACAM2000', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (2, 113, N'ACAM2000', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (3, 82, N'ACEL-IMUNE', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (4, 55, N'ACTHIB', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (5, 118, N'ADACEL', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (6, 23, N'Adenovirus types 4 and 7', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (7, 13, N'AFLURIA', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (8, 11, N'Afluria, preservative free', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (9, 11, N'AGRIFLU', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (10, 49, N'ATTENUVAX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (11, 173, N'Bexsero', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (12, 160, N'BIAVAX II', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (13, 74, N'BIOTHRAX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (14, 118, N'BOOSTRIX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (15, 82, N'CERTIVA', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (16, 61, N'CERVARIX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (17, 59, N'COMVAX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (18, 83, N'DAPTACEL', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (19, 46, N'DECAVAC', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (20, 113, N'DRYVAX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (21, 81, N'DT(GENERIC)', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (22, 92, N'ENGERIX B-PEDS', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (23, 90, N'ENGERIX-B-ADULT', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (24, 11, N'FLUARIX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (25, 8, N'Fluarix, quadrivalent, preservative free', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (26, 15, N'Flublok', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (27, 14, N'Flucelvax', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (28, 13, N'FLULAVAL', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (29, 5, N'Flulaval quadrivalent', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (30, 8, N'Flulaval, quadrivalent, preservative free', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (31, 9, N'FLUMIST', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (32, 12, N'Flumist quadrivalent', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (33, 13, N'FLUVIRIN', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (34, 11, N'FLUVIRIN-PRESERVATIVE FREE', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (35, 13, N'FLUZONE', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (36, 172, N'Fluzone Quad Intradermal', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (37, 171, N'Fluzone Quadrivalent, pediatric', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (38, 10, N'Fluzone, intradermal', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (39, 5, N'Fluzone, Quadrivalent', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (40, 8, N'Fluzone, quadrivalent, preservative free', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (41, 129, N'FLUZONE-HIGH DOSE', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (42, 11, N'FLUZONE-PRESERVATIVE FREE', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (43, 62, N'GARDASIL', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (44, 170, N'Gardasil 9', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (45, 86, N'HAVRIX-ADULT', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (46, 96, N'HAVRIX-PEDS', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (47, 55, N'HIBERIX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (48, 57, N'HIBTITER', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (49, 150, N'IMOVAX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (50, 99, N'IMOVAX ID', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (51, 82, N'INFANRIX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (52, 2, N'Influenza A (H5N1) -2013', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (53, 71, N'IPOL', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (54, 127, N'IXIARO', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (55, 69, N'JE-VAX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (56, 95, N'KINRIX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (57, 142, N'MENACTRA', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (58, 19, N'MENHIBRIX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (59, 140, N'MENOMUNE', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (60, 128, N'MENVEO', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (61, 159, N'MERUVAX II', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (62, 135, N'M-M-R II', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (63, 143, N'MUMPSVAX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (64, 75, N'MYCOBAX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (65, 55, N'OMNIHIB', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (66, 68, N'ORIMUNE', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (67, 84, N'PEDIARIX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (68, 58, N'PEDVAXHIB', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (69, 93, N'PENTACEL', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (70, 147, N'PNEUMOVAX 23', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (71, 126, N'PREVNAR 13', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (72, 126, N'PREVNAR 13', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (73, 133, N'PREVNAR 7', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (74, 56, N'PROHIBIT', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (75, 137, N'PROQUAD', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (76, 95, N'Quadracel', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (77, 150, N'RABAVERT', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (78, 99, N'RabAvert', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (79, 90, N'RECOMBIVAX-ADULT', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (80, 89, N'RECOMBIVAX-DIALYSIS', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (81, 92, N'RECOMBIVAX-PEDS', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (82, 154, N'ROTARIX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (83, 155, N'ROTATEQ', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (84, 54, N'TD(GENERIC)', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (85, 54, N'Td, (adult)', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (86, 46, N'Tenivac', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (87, 24, N'TETANUS TOXOID (GENERIC)', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (88, 100, N'TETRAMUNE', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (89, 75, N'TICE BCG', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (90, 85, N'TRIHIBIT', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (91, 82, N'TRIPEDIA', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (92, 174, N'Trumenba', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (93, 94, N'TWINRIX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (94, 112, N'TYPHIM VI', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (95, 111, N'TYPHOID-AKD', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (96, 86, N'VAQTA-ADULT', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (97, 96, N'VAQTA-PEDS', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (98, 132, N'VARIVAX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (99, 109, N'VIVOTIF BERNA', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (100, 121, N'YF-VAX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

INSERT [dbo].[VaccineTradeName] ([Id], [VaccineTypeId], [Name], [LastUpdatedAt], [LastUpdatedBy], [CreatedAt], [CreatedBy]) VALUES (101, 122, N'ZOSTAVAX', NULL, NULL, CAST(N'2015-08-14 21:25:03.940' AS DateTime), N'system')

