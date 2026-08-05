/* ============================================================
   ASP.NET Core Identity schema for the new auth-service
   (IdentityDbContext<ApplicationUser>, ApplicationUser adds a
   DisplayName column on top of the stock IdentityUser).
   Run against the same [ledgerly-test] database the other services
   use. Idempotent — safe to re-run.
   ============================================================ */

IF DB_ID(N'ledgerly-auth') IS NULL
BEGIN
    CREATE DATABASE [ledgerly-auth];
END
GO

USE [ledgerly-auth]
GO

/****** Table: AspNetRoles ******/
IF OBJECT_ID(N'dbo.AspNetRoles', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetRoles](
        [Id]               [nvarchar](450) NOT NULL,
        [Name]             [nvarchar](256) NULL,
        [NormalizedName]   [nvarchar](256) NULL,
        [ConcurrencyStamp] [nvarchar](max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    CREATE UNIQUE INDEX [RoleNameIndex] ON [dbo].[AspNetRoles] ([NormalizedName] ASC) WHERE [NormalizedName] IS NOT NULL;
END
GO

/****** Table: AspNetUsers ******/
IF OBJECT_ID(N'dbo.AspNetUsers', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetUsers](
        [Id]                   [nvarchar](450)        NOT NULL,
        [DisplayName]          [nvarchar](max)        NOT NULL,
        [UserName]             [nvarchar](256)        NULL,
        [NormalizedUserName]   [nvarchar](256)        NULL,
        [Email]                [nvarchar](256)        NULL,
        [NormalizedEmail]      [nvarchar](256)        NULL,
        [EmailConfirmed]       [bit]                  NOT NULL DEFAULT 0,
        [PasswordHash]         [nvarchar](max)        NULL,
        [SecurityStamp]        [nvarchar](max)        NULL,
        [ConcurrencyStamp]     [nvarchar](max)        NULL,
        [PhoneNumber]          [nvarchar](max)        NULL,
        [PhoneNumberConfirmed] [bit]                  NOT NULL DEFAULT 0,
        [TwoFactorEnabled]     [bit]                  NOT NULL DEFAULT 0,
        [LockoutEnd]           [datetimeoffset](7)    NULL,
        [LockoutEnabled]       [bit]                  NOT NULL DEFAULT 0,
        [AccessFailedCount]    [int]                  NOT NULL DEFAULT 0,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    CREATE UNIQUE INDEX [UserNameIndex] ON [dbo].[AspNetUsers] ([NormalizedUserName] ASC) WHERE [NormalizedUserName] IS NOT NULL;
    CREATE INDEX [EmailIndex] ON [dbo].[AspNetUsers] ([NormalizedEmail] ASC);
END
GO

/****** Table: AspNetRoleClaims ******/
IF OBJECT_ID(N'dbo.AspNetRoleClaims', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetRoleClaims](
        [Id]         [int] IDENTITY(1,1) NOT NULL,
        [RoleId]     [nvarchar](450)     NOT NULL,
        [ClaimType]  [nvarchar](max)     NULL,
        [ClaimValue] [nvarchar](max)     NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims] ([RoleId] ASC);
END
GO

/****** Table: AspNetUserClaims ******/
IF OBJECT_ID(N'dbo.AspNetUserClaims', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetUserClaims](
        [Id]         [int] IDENTITY(1,1) NOT NULL,
        [UserId]     [nvarchar](450)     NOT NULL,
        [ClaimType]  [nvarchar](max)     NULL,
        [ClaimValue] [nvarchar](max)     NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims] ([UserId] ASC);
END
GO

/****** Table: AspNetUserLogins ******/
IF OBJECT_ID(N'dbo.AspNetUserLogins', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetUserLogins](
        [LoginProvider]       [nvarchar](450) NOT NULL,
        [ProviderKey]         [nvarchar](450) NOT NULL,
        [ProviderDisplayName] [nvarchar](max) NULL,
        [UserId]              [nvarchar](450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins] ([UserId] ASC);
END
GO

/****** Table: AspNetUserRoles ******/
IF OBJECT_ID(N'dbo.AspNetUserRoles', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetUserRoles](
        [UserId] [nvarchar](450) NOT NULL,
        [RoleId] [nvarchar](450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles] ([RoleId] ASC);
END
GO

/****** Table: AspNetUserTokens ******/
IF OBJECT_ID(N'dbo.AspNetUserTokens', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetUserTokens](
        [UserId]        [nvarchar](450) NOT NULL,
        [LoginProvider] [nvarchar](450) NOT NULL,
        [Name]          [nvarchar](450) NOT NULL,
        [Value]         [nvarchar](max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [Name] ASC),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END
GO
