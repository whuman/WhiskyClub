USE [WhiskyClub]
GO
/****** Object:  Table [dbo].[EventWhisky]    Script Date: 2014-06-24 11:16:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventWhisky](
	[EventWhiskyId] [int] IDENTITY(1,1) NOT NULL,
	[EventId] [int] NOT NULL,
	[WhiskyId] [int] NOT NULL,
	[Version] [timestamp] NOT NULL,
	[InsertedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_EventWhisky] PRIMARY KEY CLUSTERED 
(
	[EventWhiskyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[EventWhisky] ADD  CONSTRAINT [DF_EventWhisky_InsertedDate]  DEFAULT (getdate()) FOR [InsertedDate]
GO
ALTER TABLE [dbo].[EventWhisky]  WITH CHECK ADD  CONSTRAINT [FK_EventWhisky_Event] FOREIGN KEY([EventId])
REFERENCES [dbo].[Event] ([EventId])
GO
ALTER TABLE [dbo].[EventWhisky] CHECK CONSTRAINT [FK_EventWhisky_Event]
GO
ALTER TABLE [dbo].[EventWhisky]  WITH CHECK ADD  CONSTRAINT [FK_EventWhisky_Whisky] FOREIGN KEY([WhiskyId])
REFERENCES [dbo].[Whisky] ([WhiskyId])
GO
ALTER TABLE [dbo].[EventWhisky] CHECK CONSTRAINT [FK_EventWhisky_Whisky]
GO
