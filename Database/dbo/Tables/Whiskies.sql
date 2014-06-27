CREATE TABLE [dbo].[Whiskies] (
    [WhiskyId]     INT             IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (50)    NOT NULL,
    [Brand]        VARCHAR (50)    NOT NULL,
    [Age]          INT             NOT NULL,
    [Country]      VARCHAR (50)    NOT NULL,
    [Region]       VARCHAR (50)    NOT NULL,
    [Description]  VARCHAR (MAX)   NOT NULL,
    [Image]        VARBINARY (MAX) NULL,
    [Price]        DECIMAL (19, 4) NULL,
    [Volume]       INT             NULL,
    [Version]      BINARY (8)      NOT NULL,
    [InsertedDate] DATETIME        NOT NULL,
    [UpdatedDate]  DATETIME        NOT NULL,
    CONSTRAINT [PK_Whiskies] PRIMARY KEY CLUSTERED ([WhiskyId] ASC)
);

