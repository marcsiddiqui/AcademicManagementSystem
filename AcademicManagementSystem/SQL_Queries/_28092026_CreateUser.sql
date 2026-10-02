IF OBJECT_ID(N'[dbo].[User]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[User]
    (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [FullName] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL,
        [PasswordHash] NVARCHAR(100) NOT NULL,
        [RoleId] INT NOT NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_User_IsActive] DEFAULT (1),
        [CreatedOnUtc] DATETIME2 NOT NULL,
        CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_User_Email] UNIQUE ([Email]),
        CONSTRAINT [FK_User_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id])
    );

    CREATE INDEX [IX_User_RoleId] ON [dbo].[User] ([RoleId]);
END;


alter table Student
add CreatedBy int not null default 0

alter table Student
add CreatedOnUtc datetime not null default getutcdate()

alter table Student
add UpdatedBy int not null default 0

alter table Student
add UpdatedOnUtc datetime null

alter table [user]
add ImagePath  nvarchar(max) null