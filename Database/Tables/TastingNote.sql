USE [WhiskyClub]
GO
/****** Object:  Table [dbo].[TastingNote]    Script Date: 2014-06-27 11:39:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TastingNote](
	[TastingNoteId] [int] IDENTITY(1,1) NOT NULL,
	[WhiskyId] [int] NOT NULL,
	[MemberId] [int] NOT NULL,
	[Comment] [varchar](max) NOT NULL,
	[Image] [image] NULL,
	[Version] [timestamp] NOT NULL,
	[InsertedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_TastingNote] PRIMARY KEY CLUSTERED 
(
	[TastingNoteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[TastingNote] ADD  CONSTRAINT [DF_TastingNote_InsertedDate]  DEFAULT (getdate()) FOR [InsertedDate]
GO
ALTER TABLE [dbo].[TastingNote] ADD  CONSTRAINT [DF_TastingNote_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[TastingNote]  WITH CHECK ADD  CONSTRAINT [FK_TastingNote_Member] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Member] ([MemberId])
GO
ALTER TABLE [dbo].[TastingNote] CHECK CONSTRAINT [FK_TastingNote_Member]
GO
ALTER TABLE [dbo].[TastingNote]  WITH CHECK ADD  CONSTRAINT [FK_TastingNote_Whisky] FOREIGN KEY([WhiskyId])
REFERENCES [dbo].[Whisky] ([WhiskyId])
GO
ALTER TABLE [dbo].[TastingNote] CHECK CONSTRAINT [FK_TastingNote_Whisky]
GO
