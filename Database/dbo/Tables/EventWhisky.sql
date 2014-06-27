CREATE TABLE [dbo].[EventWhisky] (
    [EventWhiskyId] INT        IDENTITY (1, 1) NOT NULL,
    [EventId]       INT        NOT NULL,
    [WhiskyId]      INT        NOT NULL,
    [Version]       BINARY (8) NOT NULL,
    [InsertedDate]  DATETIME   NOT NULL,
    CONSTRAINT [PK_EventWhisky] PRIMARY KEY CLUSTERED ([EventWhiskyId] ASC),
    CONSTRAINT [FK_EventWhisky_Event] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Event] ([EventId]),
    CONSTRAINT [FK_EventWhisky_Whisky] FOREIGN KEY ([WhiskyId]) REFERENCES [dbo].[Whisky] ([WhiskyId])
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_EventWhisky_Whisky]
    ON [dbo].[EventWhisky]([WhiskyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_EventWhisky_Event]
    ON [dbo].[EventWhisky]([EventId] ASC);

