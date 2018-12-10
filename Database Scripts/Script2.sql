USE [NubiDB]
GO
ALTER TABLE [dbo].[EmergencyContact] DROP CONSTRAINT [FK_EmergencyContact_RelationshipType]
GO
ALTER TABLE [dbo].[Attachment] DROP CONSTRAINT [FK_Attachment_Employee]
GO
ALTER TABLE [dbo].[Attachment] DROP CONSTRAINT [DF_Attachment_IsDeleted]
GO

UPDATE [dbo].[Employee] set [AttachmentNo] = null 
GO
ALTER TABLE [dbo].[Employee] DROP COLUMN [AttachmentNo]
GO
/****** Object:  Table [dbo].[TrainingCertificate]    Script Date: 12/5/2018 1:45:43 PM ******/
DROP TABLE [dbo].[TrainingCertificate]
GO
/****** Object:  Table [dbo].[Attachment]    Script Date: 12/5/2018 1:45:43 PM ******/
DROP TABLE [dbo].[Attachment]
GO
/****** Object:  Table [dbo].[Attachment]    Script Date: 12/5/2018 1:45:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Attachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmpNo] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Path] [nvarchar](max) NOT NULL,
	[TypeOfAttachmentNo] [int] NOT NULL,
	[ExpirationDate] [date] NULL,
	[IsExpired] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
	[Date] [date] NOT NULL,
 CONSTRAINT [PK_Attachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmergencyContact]    Script Date: 12/5/2018 1:45:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmergencyContact](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmpNo] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](80) NOT NULL,
	[Mobile] [nvarchar](80) NOT NULL,
	[RelationshipTypeNo] [int] NOT NULL,
	[Type] [nvarchar](80) NULL,
 CONSTRAINT [PK_EmergencyContact] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RelationshipType]    Script Date: 12/5/2018 1:45:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RelationEn] [nvarchar](80) NOT NULL,
	[RelationAr] [nvarchar](80) NOT NULL,
 CONSTRAINT [PK_RelationshipType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Attachment] ADD  CONSTRAINT [DF_Attachment_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Attachment]  WITH CHECK ADD  CONSTRAINT [FK_Attachment_Employee] FOREIGN KEY([EmpNo])
REFERENCES [dbo].[Employee] ([Id])
GO
ALTER TABLE [dbo].[Attachment] CHECK CONSTRAINT [FK_Attachment_Employee]
GO
ALTER TABLE [dbo].[EmergencyContact]  WITH CHECK ADD  CONSTRAINT [FK_EmergencyContact_RelationshipType] FOREIGN KEY([RelationshipTypeNo])
REFERENCES [dbo].[RelationshipType] ([Id])
GO
ALTER TABLE [dbo].[EmergencyContact] CHECK CONSTRAINT [FK_EmergencyContact_RelationshipType]
GO
