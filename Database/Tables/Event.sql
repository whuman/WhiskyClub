USE [WhiskyClub]
GO
/****** Object:  Table [dbo].[Event]    Script Date: 2014-05-27 10:14:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Event](
	[EventId] [int] IDENTITY(1,1) NOT NULL,
	[MemberId] [int] NOT NULL,
	[Description] [varchar](200) NOT NULL,
	[HostedDate] [datetime2](7) NOT NULL,
	[Version] [timestamp] NOT NULL,
	[InsertedDate] [datetime2](7) NOT NULL,
	[UpdatedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[Event] ADD  CONSTRAINT [DF_Event_HostedDate]  DEFAULT (getdate()) FOR [HostedDate]
GO
ALTER TABLE [dbo].[Event] ADD  CONSTRAINT [DF_Event_InsertedDate]  DEFAULT (getdate()) FOR [InsertedDate]
GO
ALTER TABLE [dbo].[Event] ADD  CONSTRAINT [DF_Event_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_Member] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Member] ([MemberId])
GO
ALTER TABLE [dbo].[Event] CHECK CONSTRAINT [FK_Event_Member]
GO
