CREATE TABLE [dbo].[Events] (
    [EventId]      INT           IDENTITY (1, 1) NOT NULL,
    [MemberId]     INT           NOT NULL,
    [Description]  VARCHAR (200) NOT NULL,
    [HostedDate]   DATETIME      NOT NULL,
    [Version]      BINARY (8)    NOT NULL,
    [InsertedDate] DATETIME      NOT NULL,
    [UpdatedDate]  DATETIME      NOT NULL,
    CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED ([EventId] ASC),
    CONSTRAINT [FK_Event_Member] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Members] ([MemberId])
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_Event_Member]
    ON [dbo].[Events]([MemberId] ASC);

