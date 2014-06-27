CREATE TABLE [dbo].[Event] (
    [EventId]      INT           IDENTITY (1, 1) NOT NULL,
    [MemberId]     INT           NOT NULL,
    [Description]  VARCHAR (200) NOT NULL,
    [HostedDate]   DATETIME2 (7) CONSTRAINT [DF_Event_HostedDate] DEFAULT (getdate()) NOT NULL,
    [Version]      ROWVERSION    NOT NULL,
    [InsertedDate] DATETIME2 (7) CONSTRAINT [DF_Event_InsertedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedDate]  DATETIME2 (7) CONSTRAINT [DF_Event_UpdatedDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED ([EventId] ASC),
    CONSTRAINT [FK_Event_Member] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Member] ([MemberId])
);

