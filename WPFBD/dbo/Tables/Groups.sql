CREATE TABLE [dbo].[Groups] (
    [GroupId]        INT            IDENTITY (1, 1) NOT NULL,
    [GroupName]      NVARCHAR (MAX) NULL,
    [ParentId]       INT            NULL,
    [CreatorId]      INT            NOT NULL,
    [Creator_UserId] INT            NULL,
    [Parent_GroupId] INT            NULL,
    CONSTRAINT [PK_dbo.Groups] PRIMARY KEY CLUSTERED ([GroupId] ASC),
    CONSTRAINT [FK_dbo.Groups_dbo.Groups_Parent_GroupId] FOREIGN KEY ([Parent_GroupId]) REFERENCES [dbo].[Groups] ([GroupId]),
    CONSTRAINT [FK_dbo.Groups_dbo.Users_Creator_UserId] FOREIGN KEY ([Creator_UserId]) REFERENCES [dbo].[Users] ([UserId])
);


GO
CREATE NONCLUSTERED INDEX [IX_Creator_UserId]
    ON [dbo].[Groups]([Creator_UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Parent_GroupId]
    ON [dbo].[Groups]([Parent_GroupId] ASC);

