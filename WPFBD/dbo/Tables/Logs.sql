CREATE TABLE [dbo].[Logs] (
    [LogId]               INT            IDENTITY (1, 1) NOT NULL,
    [Action]              NVARCHAR (MAX) NULL,
    [CreatorId]           INT            NOT NULL,
    [EventTime]           DATETIME       NOT NULL,
    [Creator_UserId]      INT            NULL,
    [AppointmentName]     NVARCHAR (MAX) NULL,
    [ActionAuthorId]      INT            DEFAULT ((0)) NOT NULL,
    [ActionAuthor_UserId] INT            NULL,
    CONSTRAINT [PK_dbo.Logs] PRIMARY KEY CLUSTERED ([LogId] ASC),
    CONSTRAINT [FK_dbo.Logs_dbo.Users_ActionAuthor_UserId] FOREIGN KEY ([ActionAuthor_UserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_dbo.Logs_dbo.Users_Creator_UserId] FOREIGN KEY ([Creator_UserId]) REFERENCES [dbo].[Users] ([UserId])
);


GO
CREATE NONCLUSTERED INDEX [IX_Creator_UserId]
    ON [dbo].[Logs]([Creator_UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ActionAuthor_UserId]
    ON [dbo].[Logs]([ActionAuthor_UserId] ASC);

