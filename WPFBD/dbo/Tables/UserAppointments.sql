CREATE TABLE [dbo].[UserAppointments] (
    [UserId]        INT NOT NULL,
    [AppointmentId] INT NOT NULL,
    CONSTRAINT [PK_dbo.UserAppointments] PRIMARY KEY CLUSTERED ([UserId] ASC, [AppointmentId] ASC),
    CONSTRAINT [FK_dbo.UserAppointments_dbo.Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [dbo].[Appointments] ([AppointmentId]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.UserAppointments_dbo.Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[UserAppointments]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AppointmentId]
    ON [dbo].[UserAppointments]([AppointmentId] ASC);

