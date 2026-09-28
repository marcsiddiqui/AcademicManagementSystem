IF OBJECT_ID(N'[dbo].[Role]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Role]
    (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(50) NOT NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Role_IsActive] DEFAULT (1),
        CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Role_Name] UNIQUE ([Name])
    );
END;
