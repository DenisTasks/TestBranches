CREATE TABLE [dbo].[Users] (
    [UserId]   INT            IDENTITY (1, 1) NOT NULL,
    [IsActive] BIT            NOT NULL,
    [Name]     NVARCHAR (MAX) NULL,
    [UserName] NVARCHAR (MAX) NULL,
    [Password] NVARCHAR (MAX) NULL,
    [Salt]     NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED ([UserId] ASC)
);



