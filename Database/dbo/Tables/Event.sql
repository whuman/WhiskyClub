﻿CREATE TABLE [dbo].[Event] (
    [EventId]      INT           IDENTITY (1, 1) NOT NULL,
    [MemberId]     INT           NOT NULL,
    [Description]  VARCHAR (200) NOT NULL,
    [HostedDate]   DATETIME      NOT NULL,
    [Version]      BINARY (8)    NOT NULL,
    [InsertedDate] DATETIME      NOT NULL,
    [UpdatedDate]  DATETIME      NOT NULL,
    CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED ([EventId] ASC),
    CONSTRAINT [FK_Event_Member] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Member] ([MemberId])
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_Event_Member]
    ON [dbo].[Event]([MemberId] ASC);

