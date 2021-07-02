
GO
/****** Object:  Table [dbo].[PatientPrecaution]    Script Date: 8/20/2016 4:23:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PatientPrecaution](
	[Id] [bigint] NOT NULL,
	[PatientId] [bigint] NULL,
	[PrecautionTypeId] [bigint] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[AdditionalDescription] [varchar](max) NULL,
	[Guid] [uniqueidentifier] NULL,
	[Deleted] [bit] NULL,
 CONSTRAINT [PK_PatientPrecaution] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PrecautionType]    Script Date: 8/20/2016 4:23:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PrecautionType](
	[Id] [bigint] NOT NULL,
	[SystemProductId] [int] NULL,
	[Name] [varchar](500) NULL,
	[SubProductTypeKey] [varchar](50) NULL,
 CONSTRAINT [PK_PrecautionType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (1, 1, N'Airborne', NULL)
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (2, 1, N'Contact', NULL)
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (3, 1, N'Droplet', NULL)
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (11, 2, N'15 minute checks', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (12, 2, N'30 minute checks', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (13, 2, N'Neuro checks', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (14, 2, N'Hipsters', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (15, 2, N'Transfer bar', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (16, 2, N'Floor mat', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (17, 2, N'Floor mattress', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (18, 2, N'Labs', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (19, 2, N'Check devices', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (20, 2, N'Check environment', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (21, 2, N'Low bed', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (22, 2, N'Anti tippers', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (23, 2, N'Auto lock brakes', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (24, 2, N'Reminder signs', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (25, 2, N'Resident family re-education', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (26, 2, N'Soft touch call light', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (27, 2, N'Scoop mattress', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (28, 2, N'Dehydration assessment', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (29, 2, N'Activitiy involvement', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (30, 2, N'Furniture rearrangement', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (31, 2, N'Bedside commode', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (32, 2, N'Nap/rest periods', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (33, 2, N'Check footwear', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (34, 2, N'Speciality chairs', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (35, 2, N'Restorative program', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (36, 2, N'Common area', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (37, 2, N'Medication review', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (38, 2, N'B&B patterning', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (39, 2, N'Night light', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (40, 2, N'Repositioning', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (41, 2, N'Psych consult', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (42, 2, N'Optometrist consult', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (43, 2, N'Audiologist consult', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (44, 2, N'Urologist consult Pt therapy', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (45, 2, N'OT therapy', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (46, 2, N'Sp therapy', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (47, 2, N'Dietary consult', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (48, 2, N'Bed alarm', N'Fall')
INSERT [dbo].[PrecautionType] ([Id], [SystemProductId], [Name], [SubProductTypeKey]) VALUES (49, 2, N'Chair alarm', N'Fall')
ALTER TABLE [dbo].[PatientPrecaution]  WITH CHECK ADD  CONSTRAINT [FK_PatientPrecaution_Patient] FOREIGN KEY([PatientId])
REFERENCES [dbo].[Patient] ([Id])
GO
ALTER TABLE [dbo].[PatientPrecaution] CHECK CONSTRAINT [FK_PatientPrecaution_Patient]
GO
ALTER TABLE [dbo].[PatientPrecaution]  WITH CHECK ADD  CONSTRAINT [FK_PatientPrecaution_PrecautionType] FOREIGN KEY([PrecautionTypeId])
REFERENCES [dbo].[PrecautionType] ([Id])
GO
ALTER TABLE [dbo].[PatientPrecaution] CHECK CONSTRAINT [FK_PatientPrecaution_PrecautionType]
GO
ALTER TABLE [dbo].[PrecautionType]  WITH CHECK ADD  CONSTRAINT [FK_PrecautionType_SystemProduct] FOREIGN KEY([SystemProductId])
REFERENCES [dbo].[SystemProduct] ([Id])
GO
ALTER TABLE [dbo].[PrecautionType] CHECK CONSTRAINT [FK_PrecautionType_SystemProduct]
GO
