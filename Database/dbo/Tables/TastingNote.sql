CREATE TABLE [dbo].[TastingNote] (
    [TastingNoteId] INT           IDENTITY (1, 1) NOT NULL,
    [WhiskyId]      INT           NOT NULL,
    [MemberId]      INT           NOT NULL,
    [Comment]       VARCHAR (MAX) NOT NULL,
    [Image]         IMAGE         NULL,
    [Version]       ROWVERSION    NOT NULL,
    [InsertedDate]  DATETIME2 (7) CONSTRAINT [DF_TastingNote_InsertedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedDate]   DATETIME2 (7) CONSTRAINT [DF_TastingNote_UpdatedDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_TastingNote] PRIMARY KEY CLUSTERED ([TastingNoteId] ASC),
    CONSTRAINT [FK_TastingNote_Member] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Member] ([MemberId]),
    CONSTRAINT [FK_TastingNote_Whisky] FOREIGN KEY ([WhiskyId]) REFERENCES [dbo].[Whisky] ([WhiskyId])
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_TastingNote_Whisky]
    ON [dbo].[TastingNote]([WhiskyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_TastingNote_Member]
    ON [dbo].[TastingNote]([MemberId] ASC);

