CREATE TABLE [dbo].[Operators] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Identity]     NVARCHAR (50)  NULL,
    [FirstName]    NVARCHAR (100) NULL,
    [LastName]     NVARCHAR (100) NULL,
    [Email]        NVARCHAR (100) NULL,
    [Type]         NVARCHAR (50)  NULL,
    [AccountId]    INT            NOT NULL,
    [LastActivity] DATETIME       NULL,
    CONSTRAINT [PK_dbo.Operators] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Operators_dbo.Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Accounts] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AccountId]
    ON [dbo].[Operators]([AccountId] ASC);

