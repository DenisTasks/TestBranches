CREATE TABLE [dbo].[Appointments] (
    [AppointmentId]    INT            IDENTITY (1, 1) NOT NULL,
    [Subject]          NVARCHAR (MAX) NULL,
    [BeginningDate]    DATETIME       NOT NULL,
    [EndingDate]       DATETIME       NOT NULL,
    [OrganizerId]      INT            NOT NULL,
    [LocationId]       INT            NOT NULL,
    [Organizer_UserId] INT            NULL,
    CONSTRAINT [PK_dbo.Appointments] PRIMARY KEY CLUSTERED ([AppointmentId] ASC),
    CONSTRAINT [FK_dbo.Appointments_dbo.Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([LocationId]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.Appointments_dbo.Users_Organizer_UserId] FOREIGN KEY ([Organizer_UserId]) REFERENCES [dbo].[Users] ([UserId])
);


GO
CREATE NONCLUSTERED INDEX [IX_LocationId]
    ON [dbo].[Appointments]([LocationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Organizer_UserId]
    ON [dbo].[Appointments]([Organizer_UserId] ASC);

