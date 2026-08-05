/* ============================================================
   Portable version — creates the ledgerly-dashbaord database wherever
   this script is run, without hardcoding file paths or relying
   on Express-specific instance folders.
   ============================================================ */

USE [master]
GO

IF DB_ID(N'ledgerly-dashbaord') IS NULL
BEGIN
    CREATE DATABASE [ledgerly-dashbaord];
END
GO

ALTER DATABASE [ledgerly-dashbaord] SET RECOVERY SIMPLE;
GO

USE [ledgerly-dashbaord]
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

IF OBJECT_ID(N'dbo.kpis', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[kpis](
        [totalbilled]  [int],
        [collected] [int] ,
	[outstanding] [int] ,
        [customers] [int] 
    );
END
GO

USE [master]
GO
ALTER DATABASE [ledgerly-dashbaord] SET READ_WRITE;
GO
