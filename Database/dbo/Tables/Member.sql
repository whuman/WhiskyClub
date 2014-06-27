CREATE TABLE [dbo].[Member] (
    [MemberId]     INT           IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (50)  NOT NULL,
    [Version]      ROWVERSION    NOT NULL,
    [InsertedDate] DATETIME2 (7) CONSTRAINT [DF_Member_InsertedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedDate]  DATETIME2 (7) CONSTRAINT [DF_Member_UpdatedDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Member] PRIMARY KEY CLUSTERED ([MemberId] ASC)
);

