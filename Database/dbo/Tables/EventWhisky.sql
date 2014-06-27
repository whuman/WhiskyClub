CREATE TABLE [dbo].[EventWhisky] (
    [EventWhiskyId] INT           IDENTITY (1, 1) NOT NULL,
    [EventId]       INT           NOT NULL,
    [WhiskyId]      INT           NOT NULL,
    [Version]       ROWVERSION    NOT NULL,
    [InsertedDate]  DATETIME2 (7) CONSTRAINT [DF_EventWhisky_InsertedDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_EventWhisky] PRIMARY KEY CLUSTERED ([EventWhiskyId] ASC),
    CONSTRAINT [FK_EventWhisky_Event] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Event] ([EventId]),
    CONSTRAINT [FK_EventWhisky_Whisky] FOREIGN KEY ([WhiskyId]) REFERENCES [dbo].[Whisky] ([WhiskyId])
);

