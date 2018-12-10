USE [NubiDB]
GO
/****** Object:  Table [dbo].[TypesOfAttachment]    Script Date: 12/5/2018 11:55:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TypesOfAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttachmentTypeEn] [nvarchar](80) NOT NULL,
	[AttachmentTypeAr] [nvarchar](80) NOT NULL,
 CONSTRAINT [PK_AttachmentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[TypesOfAttachment] ON 
GO
INSERT [dbo].[TypesOfAttachment] ([Id], [AttachmentTypeEn], [AttachmentTypeAr]) VALUES (1, N'National ID', N'بطاقة الهوية')
GO
INSERT [dbo].[TypesOfAttachment] ([Id], [AttachmentTypeEn], [AttachmentTypeAr]) VALUES (2, N'Passport Number', N'رقم الجواز')
GO
INSERT [dbo].[TypesOfAttachment] ([Id], [AttachmentTypeEn], [AttachmentTypeAr]) VALUES (3, N'Last Certificate', N'اخر شهادة')
GO
INSERT [dbo].[TypesOfAttachment] ([Id], [AttachmentTypeEn], [AttachmentTypeAr]) VALUES (4, N'Image', N'صورة شخصية')
GO
INSERT [dbo].[TypesOfAttachment] ([Id], [AttachmentTypeEn], [AttachmentTypeAr]) VALUES (5, N'Training Certificate', N'شهادة تدريب')
GO
SET IDENTITY_INSERT [dbo].[TypesOfAttachment] OFF
GO
