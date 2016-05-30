CREATE TABLE [dbo].[Visitors] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (200) NULL,
    [Email]    NVARCHAR (200) NULL,
    [Location] NVARCHAR (200) NULL,
    CONSTRAINT [PK_dbo.Visitors] PRIMARY KEY CLUSTERED ([Id] ASC)
);

