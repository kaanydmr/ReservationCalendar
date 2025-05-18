IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Classrooms] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Building] nvarchar(50) NOT NULL,
    [Floor] int NOT NULL,
    [Capacity] int NOT NULL,
    [HasProjector] bit NOT NULL,
    [HasComputers] bit NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Classroo__3214EC07F6AADBE3] PRIMARY KEY ([Id])
);

CREATE TABLE [Holidays] (
    [Id] int NOT NULL IDENTITY,
    [Date] date NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Holidays__3214EC07D46BC282] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(100) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [PasswordSalt] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(50) NOT NULL,
    [LastName] nvarchar(50) NOT NULL,
    [Role] nvarchar(20) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Users__3214EC07032C5894] PRIMARY KEY ([Id])
);

CREATE TABLE [AcademicTerms] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [CreatedById] int NOT NULL,
    CONSTRAINT [PK__Academic__3214EC07D7F482A1] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__AcademicT__Creat__403A8C7D] FOREIGN KEY ([CreatedById]) REFERENCES [Users] ([Id])
);

CREATE TABLE [ClassroomFeedback] (
    [Id] int NOT NULL IDENTITY,
    [ClassroomId] int NOT NULL,
    [InstructorId] int NOT NULL,
    [Rating] tinyint NOT NULL,
    [Comment] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Classroo__3214EC0734D017D3] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Classroom__Class__534D60F1] FOREIGN KEY ([ClassroomId]) REFERENCES [Classrooms] ([Id]),
    CONSTRAINT [FK__Classroom__Instr__5441852A] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [ContactMessages] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Subject] nvarchar(100) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [IsRead] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__ContactM__3214EC07703ABA20] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__ContactMe__UserI__59063A47] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [SystemLogs] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NULL,
    [ActionType] nvarchar(50) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Timestamp] datetime2 NOT NULL DEFAULT ((getdate())),
    [IpAddress] nvarchar(50) NULL,
    [IsError] bit NOT NULL,
    CONSTRAINT [PK__SystemLo__3214EC077948A9F8] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__SystemLog__UserI__5DCAEF64] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [ClassroomReservations] (
    [Id] int NOT NULL IDENTITY,
    [ClassroomId] int NOT NULL,
    [InstructorId] int NOT NULL,
    [AcademicTermId] int NOT NULL,
    [Title] nvarchar(100) NOT NULL,
    [Description] nvarchar(max) NULL,
    [DayOfWeek] tinyint NOT NULL,
    [StartTime] time NOT NULL,
    [EndTime] time NOT NULL,
    [StartDate] date NOT NULL,
    [EndDate] date NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [RejectionReason] nvarchar(max) NULL,
    [IsHoliday] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [UpdatedAt] datetime2 NOT NULL DEFAULT ((getdate())),
    [ApprovedRejectedById] int NULL,
    CONSTRAINT [PK__Classroo__3214EC07F30A4887] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Classroom__Acade__4E88ABD4] FOREIGN KEY ([AcademicTermId]) REFERENCES [AcademicTerms] ([Id]),
    CONSTRAINT [FK__Classroom__Appro__4F7CD00D] FOREIGN KEY ([ApprovedRejectedById]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK__Classroom__Class__4CA06362] FOREIGN KEY ([ClassroomId]) REFERENCES [Classrooms] ([Id]),
    CONSTRAINT [FK__Classroom__Instr__4D94879B] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id])
);

CREATE INDEX [IX_AcademicTerms_CreatedById] ON [AcademicTerms] ([CreatedById]);

CREATE INDEX [IX_ClassroomFeedback_ClassroomId] ON [ClassroomFeedback] ([ClassroomId]);

CREATE INDEX [IX_ClassroomFeedback_InstructorId] ON [ClassroomFeedback] ([InstructorId]);

CREATE INDEX [IX_ClassroomReservations_AcademicTermId] ON [ClassroomReservations] ([AcademicTermId]);

CREATE INDEX [IX_ClassroomReservations_ApprovedRejectedById] ON [ClassroomReservations] ([ApprovedRejectedById]);

CREATE INDEX [IX_ClassroomReservations_ClassroomId] ON [ClassroomReservations] ([ClassroomId]);

CREATE INDEX [IX_ClassroomReservations_InstructorId] ON [ClassroomReservations] ([InstructorId]);

CREATE INDEX [IX_ContactMessages_UserId] ON [ContactMessages] ([UserId]);

CREATE UNIQUE INDEX [UQ__Holidays__77387D072F52EC92] ON [Holidays] ([Date]);

CREATE INDEX [IX_SystemLogs_UserId] ON [SystemLogs] ([UserId]);

CREATE UNIQUE INDEX [UQ__Users__A9D105343CC79A8B] ON [Users] ([Email]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250514150744_ChangeAcademicTermDatesToDateTime', N'9.0.4');

COMMIT;
GO

