USE [NubiDB]
GO

/****** Object:  Table [dbo].[Alert]    Script Date: 1/9/2019 2:12:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Alert](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AlertText] [nvarchar](1024) NULL,
	[Link] [nchar](10) NULL,
	[MsgSmsId] [int] NULL,
	[MsgEmailId] [int] NULL,
	[MsgSmsText] [nvarchar](256) NULL,
	[MsgEmailText] [nvarchar](4000) NULL,
	[IsSendSms] [bit] NOT NULL,
	[IsSendEmail] [bit] NOT NULL,
	[IsOpen] [bit] NOT NULL,
	[DateSendSms] [datetime] NULL,
	[DateSendEmail] [datetime] NULL,
	[CreationDate] [datetime] NULL,
	[IsHiddenInNavbar] [bit] NOT NULL,
	[TemplateId] [int] NULL,
	[UserId] [nvarchar](128) NULL,
	[Email] [nvarchar](256) NULL,
	[MobilePhone] [nvarchar](max) NULL,
	[EmailSubject] [nvarchar](128) NULL,
	[AttachmentPath] [nvarchar](1024) NULL,
 CONSTRAINT [PK_Alert] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Alert] ADD  CONSTRAINT [DF_Alert_IsSendSms]  DEFAULT ((0)) FOR [IsSendSms]
GO

ALTER TABLE [dbo].[Alert] ADD  CONSTRAINT [DF_Alert_IsSendEmail]  DEFAULT ((0)) FOR [IsSendEmail]
GO

ALTER TABLE [dbo].[Alert] ADD  CONSTRAINT [DF_Alert_IsOpen]  DEFAULT ((0)) FOR [IsOpen]
GO

ALTER TABLE [dbo].[Alert] ADD  CONSTRAINT [DF_Alert_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO

ALTER TABLE [dbo].[Alert] ADD  CONSTRAINT [DF_Alert_IsHiddenInNavbar]  DEFAULT ((0)) FOR [IsHiddenInNavbar]
GO

ALTER TABLE [dbo].[Alert]  WITH CHECK ADD  CONSTRAINT [FK_Alert_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO

ALTER TABLE [dbo].[Alert] CHECK CONSTRAINT [FK_Alert_AspNetUsers]
GO




