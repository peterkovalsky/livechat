CREATE TABLE [dbo].[Accounts] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (500) NULL,
    [Identifier] NVARCHAR (50)  NULL,
    CONSTRAINT [PK_dbo.Accounts] PRIMARY KEY CLUSTERED ([Id] ASC)
);

