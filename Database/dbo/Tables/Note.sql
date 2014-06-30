CREATE TABLE [dbo].[Note] (
	[NoteId] INT           IDENTITY (1, 1) NOT NULL,
	[WhiskyId]      INT           NOT NULL,
	[EventId]		INT           NOT NULL,
	[MemberId]      INT           NOT NULL,
	[Comment]       VARCHAR (MAX) NOT NULL,
	[Image]         IMAGE         NULL,
	[Version]       ROWVERSION    NOT NULL,
	[InsertedDate]  DATETIME2 (7) CONSTRAINT [DF_Note_InsertedDate] DEFAULT (getdate()) NOT NULL,
	[UpdatedDate]   DATETIME2 (7) CONSTRAINT [DF_Note_UpdatedDate] DEFAULT (getdate()) NOT NULL,
	CONSTRAINT [PK_Note] PRIMARY KEY CLUSTERED ([NoteId] ASC),
	CONSTRAINT [FK_Note_Member] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Member] ([MemberId]),
	CONSTRAINT [FK_Note_Whisky] FOREIGN KEY ([WhiskyId]) REFERENCES [dbo].[Whisky] ([WhiskyId]), 
	CONSTRAINT [FK_Note_Event] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Event] ([EventId])
);

GO
CREATE NONCLUSTERED INDEX [IX_FK_Note_Whisky]
	ON [dbo].[Note]([WhiskyId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_FK_Note_Member]
	ON [dbo].[Note]([MemberId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_FK_Note_Event]
	ON [dbo].[Note]([EventId] ASC);

