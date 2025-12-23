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
CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [AIInteractions] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Input] nvarchar(max) NOT NULL,
    [AIResponse] nvarchar(max) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    CONSTRAINT [PK_AIInteractions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AIInteractions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Courses] (
    [courseID] int NOT NULL IDENTITY,
    [courseName] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [TeacherId] int NOT NULL,
    [CreatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_Courses] PRIMARY KEY ([courseID]),
    CONSTRAINT [FK_Courses_Users_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Feedbacks] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Comment] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Feedbacks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Feedbacks_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Lessons] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [CourseId] int NOT NULL,
    CONSTRAINT [PK_Lessons] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Lessons_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([courseID]) ON DELETE CASCADE
);

CREATE TABLE [Progresses] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [CourseId] int NOT NULL,
    [LessonId] int NOT NULL,
    [IsCompleted] bit NOT NULL,
    [CompletedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Progresses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Progresses_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([courseID]),
    CONSTRAINT [FK_Progresses_Lessons_LessonId] FOREIGN KEY ([LessonId]) REFERENCES [Lessons] ([Id]),
    CONSTRAINT [FK_Progresses_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Quizzes] (
    [Id] int NOT NULL IDENTITY,
    [QuizQuestion] nvarchar(max) NOT NULL,
    [Answer] nvarchar(max) NOT NULL,
    [QuizOptions] nvarchar(max) NOT NULL,
    [LessonId] int NOT NULL,
    CONSTRAINT [PK_Quizzes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Quizzes_Lessons_LessonId] FOREIGN KEY ([LessonId]) REFERENCES [Lessons] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_AIInteractions_UserId] ON [AIInteractions] ([UserId]);

CREATE INDEX [IX_Courses_TeacherId] ON [Courses] ([TeacherId]);

CREATE INDEX [IX_Feedbacks_UserId] ON [Feedbacks] ([UserId]);

CREATE INDEX [IX_Lessons_CourseId] ON [Lessons] ([CourseId]);

CREATE INDEX [IX_Progresses_CourseId] ON [Progresses] ([CourseId]);

CREATE INDEX [IX_Progresses_LessonId] ON [Progresses] ([LessonId]);

CREATE INDEX [IX_Progresses_UserId] ON [Progresses] ([UserId]);

CREATE INDEX [IX_Quizzes_LessonId] ON [Quizzes] ([LessonId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241202234441_InitialCreate', N'9.0.0');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241203012827_UpdateUserRoles', N'9.0.0');

ALTER TABLE [Users] ADD [IsLoggedIn] bit NOT NULL DEFAULT CAST(0 AS bit);

ALTER TABLE [Users] ADD [LastActivity] datetime2 NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241230221541_AddIsLoggedInAndLastActivityToUsers', N'9.0.0');

ALTER TABLE [Courses] DROP CONSTRAINT [FK_Courses_Users_TeacherId];

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Courses]') AND [c].[name] = N'CreatedOn');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Courses] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Courses] DROP COLUMN [CreatedOn];

EXEC sp_rename N'[Courses].[courseName]', N'Status', 'COLUMN';

EXEC sp_rename N'[Courses].[Description]', N'Name', 'COLUMN';

EXEC sp_rename N'[Courses].[courseID]', N'Id', 'COLUMN';

CREATE TABLE [Teachers] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Teachers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Teachers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Teachers_UserId] ON [Teachers] ([UserId]);

ALTER TABLE [Courses] ADD CONSTRAINT [FK_Courses_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE CASCADE;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250320224646_AddTeachersTable', N'9.0.0');

COMMIT;
GO

