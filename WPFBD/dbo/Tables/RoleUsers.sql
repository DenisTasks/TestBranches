CREATE TABLE [dbo].[RoleUsers] (
    [Role_RoleId] INT NOT NULL,
    [User_UserId] INT NOT NULL,
    CONSTRAINT [PK_dbo.RoleUsers] PRIMARY KEY CLUSTERED ([Role_RoleId] ASC, [User_UserId] ASC),
    CONSTRAINT [FK_dbo.RoleUsers_dbo.Roles_Role_RoleId] FOREIGN KEY ([Role_RoleId]) REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.RoleUsers_dbo.Users_User_UserId] FOREIGN KEY ([User_UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Role_RoleId]
    ON [dbo].[RoleUsers]([Role_RoleId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_User_UserId]
    ON [dbo].[RoleUsers]([User_UserId] ASC);

