/* ============================================================
   Portable version — creates the nextjs-test database wherever
   this script is run, without hardcoding file paths or relying
   on Express-specific instance folders.
   ============================================================ */

USE [master]
GO

IF DB_ID(N'nextjs-test') IS NULL
BEGIN
    CREATE DATABASE [nextjs-test];
END
GO

ALTER DATABASE [nextjs-test] SET RECOVERY SIMPLE;
GO

USE [nextjs-test]
GO

/****** Table: __EFMigrationsHistory ******/
IF OBJECT_ID(N'dbo.__EFMigrationsHistory', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory](
        [MigrationId]    [nvarchar](150) NOT NULL,
        [ProductVersion] [nvarchar](32)  NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC)
    );
END
GO

/****** Table: customers ******/
IF OBJECT_ID(N'dbo.customers', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[customers](
        [id]        [varchar](255) NOT NULL,
        [name]      [varchar](255) NOT NULL,
        [email]     [varchar](255) NOT NULL,
        [image_url] [varchar](255) NOT NULL,
        PRIMARY KEY CLUSTERED ([id] ASC)
    );
END
GO

/****** Table: invoices ******/
IF OBJECT_ID(N'dbo.invoices', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[invoices](
        [id]          [varchar](255) NOT NULL,
        [customer_id] [varchar](255) NOT NULL,
        [amount]      [int]          NOT NULL,
        [status]      [varchar](255) NOT NULL,
        [date]        [date]         NOT NULL,
        PRIMARY KEY CLUSTERED ([id] ASC)
    );
END
GO

/****** Table: revenue ******/
IF OBJECT_ID(N'dbo.revenue', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[revenue](
        [month]   [varchar](4) NOT NULL,
        [revenue] [int]        NOT NULL,
        CONSTRAINT [PK_revenue] PRIMARY KEY CLUSTERED ([month] ASC, [revenue] ASC),
        CONSTRAINT [UQ_revenue_month] UNIQUE NONCLUSTERED ([month] ASC)
    );
END
GO

USE [master]
GO
ALTER DATABASE [nextjs-test] SET READ_WRITE;
GO
