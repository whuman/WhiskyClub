USE [WhiskyClub]
GO
/****** Object:  Table [dbo].[Whisky]    Script Date: 2014-06-24 11:17:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Whisky](
	[WhiskyId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Country] [varchar](50) NOT NULL,
	[Region] [varchar](50) NOT NULL,
	[Brand] [varchar](50) NOT NULL,
	[Age] [int] NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[Price] [money] NULL,
	[BottleSize] [int] NULL,
	[Version] [timestamp] NOT NULL,
	[InsertedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Whisky] PRIMARY KEY CLUSTERED 
(
	[WhiskyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[Whisky] ADD  CONSTRAINT [DF_Whisky_InsertedDate]  DEFAULT (getdate()) FOR [InsertedDate]
GO
ALTER TABLE [dbo].[Whisky] ADD  CONSTRAINT [DF_Whisky_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
