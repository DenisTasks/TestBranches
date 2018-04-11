CREATE TABLE [dbo].[Locations] (
    [LocationId] INT            IDENTITY (1, 1) NOT NULL,
    [Room]       NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.Locations] PRIMARY KEY CLUSTERED ([LocationId] ASC)
);

