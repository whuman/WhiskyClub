CREATE TABLE [dbo].[Whisky] (
    [WhiskyId]     INT           IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (50)  NOT NULL,
    [Country]      VARCHAR (50)  NOT NULL,
    [Region]       VARCHAR (50)  NOT NULL,
    [Brand]        VARCHAR (50)  NOT NULL,
    [Age]          INT           NOT NULL,
    [Description]  VARCHAR (MAX) NOT NULL,
    [Price]        MONEY         NULL,
    [Volume]       INT           NULL,
    [Version]      ROWVERSION    NOT NULL,
    [InsertedDate] DATETIME2 (7) CONSTRAINT [DF_Whisky_InsertedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedDate]  DATETIME2 (7) CONSTRAINT [DF_Whisky_UpdatedDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Whisky] PRIMARY KEY CLUSTERED ([WhiskyId] ASC)
);

