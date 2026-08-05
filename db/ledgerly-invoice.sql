/* ============================================================
   Portable version — creates the ledgerly-invoice database wherever
   this script is run, without hardcoding file paths or relying
   on Express-specific instance folders.
   ============================================================ */

USE [master]
GO

IF DB_ID(N'ledgerly-invoice') IS NULL
BEGIN
    CREATE DATABASE [ledgerly-invoice];
END
GO

ALTER DATABASE [ledgerly-invoice] SET RECOVERY SIMPLE;
GO

USE [ledgerly-invoice]
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

USE [master]
GO
ALTER DATABASE [ledgerly-invoice] SET READ_WRITE;
GO
