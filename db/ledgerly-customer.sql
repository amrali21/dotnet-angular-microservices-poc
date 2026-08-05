/* ============================================================
   Portable version — creates the ledgerly-customer database wherever
   this script is run, without hardcoding file paths or relying
   on Express-specific instance folders.
   ============================================================ */

USE [master]
GO

IF DB_ID(N'ledgerly-customer') IS NULL
BEGIN
    CREATE DATABASE [ledgerly-customer];
END
GO

ALTER DATABASE [ledgerly-customer] SET RECOVERY SIMPLE;
GO

USE [ledgerly-customer]
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

USE [master]
GO
ALTER DATABASE [ledgerly-customer] SET READ_WRITE;
GO
