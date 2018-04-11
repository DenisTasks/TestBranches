CREATE TABLE [dbo].[UserNotifications] (
    [UserId]         INT NOT NULL,
    [NotificationId] INT NOT NULL,
    CONSTRAINT [PK_dbo.UserNotifications] PRIMARY KEY CLUSTERED ([UserId] ASC, [NotificationId] ASC),
    CONSTRAINT [FK_dbo.UserNotifications_dbo.Notifications_NotificationId] FOREIGN KEY ([NotificationId]) REFERENCES [dbo].[Notifications] ([NotificationId]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.UserNotifications_dbo.Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_NotificationId]
    ON [dbo].[UserNotifications]([NotificationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[UserNotifications]([UserId] ASC);

