USE [NubiDB]
GO
/****** Object:  Table [dbo].[AttendanceSheet]    Script Date: 12/19/2018 4:18:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttendanceSheet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Number] [int] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[PunchTime] [datetime] NOT NULL,
	[WorkState] [nvarchar](128) NOT NULL,
	[Terminal] [nvarchar](128) NULL,
	[PunchType] [nvarchar](128) NULL,
	[NoOfHours] [nvarchar](50) NULL,
	[RemainingHours] [nvarchar](50) NULL,
	[ImportLogNo] [nvarchar](128) NOT NULL,
	[Date] [date] NOT NULL,
 CONSTRAINT [PK_AttendanceSheet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImportLog]    Script Date: 12/19/2018 4:18:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImportLog](
	[Id] [nvarchar](128) NOT NULL,
	[FileName] [nvarchar](max) NOT NULL,
	[Path] [nvarchar](max) NOT NULL,
	[Date] [date] NOT NULL,
 CONSTRAINT [PK_ImportLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[AttendanceSheet]  WITH CHECK ADD  CONSTRAINT [FK_AttendanceSheet_ImportLog] FOREIGN KEY([ImportLogNo])
REFERENCES [dbo].[ImportLog] ([Id])
GO
ALTER TABLE [dbo].[AttendanceSheet] CHECK CONSTRAINT [FK_AttendanceSheet_ImportLog]
GO
