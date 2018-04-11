CREATE TABLE [dbo].[Notifications] (
    [NotificationId]   INT            IDENTITY (1, 1) NOT NULL,
    [Subject]          NVARCHAR (MAX) NULL,
    [BeginningDate]    DATETIME2 (7)  NULL,
    [EndingDate]       DATETIME2 (7)  NULL,
    [OrganizerId]      INT            NOT NULL,
    [LocationId]       INT            NOT NULL,
    [Organizer_UserId] INT            NULL,
    CONSTRAINT [PK_dbo.Notifications] PRIMARY KEY CLUSTERED ([NotificationId] ASC),
    CONSTRAINT [FK_dbo.Notifications_dbo.Users_Organizer_UserId] FOREIGN KEY ([Organizer_UserId]) REFERENCES [dbo].[Users] ([UserId])
);




GO
CREATE NONCLUSTERED INDEX [IX_Organizer_UserId]
    ON [dbo].[Notifications]([Organizer_UserId] ASC);

