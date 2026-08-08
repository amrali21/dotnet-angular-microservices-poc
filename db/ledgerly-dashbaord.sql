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
        [year]    [int]        NOT NULL,
        [revenue] [int]        NOT NULL,
        CONSTRAINT [PK_revenue] PRIMARY KEY CLUSTERED ([month] ASC, [year] ASC)
    );
END
GO


/****** Table: kpis ******/
IF OBJECT_ID(N'dbo.kpis', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[kpis](
	[ID] [INT] not null,
	[kpiname] nvarchar(200) not null,
	[kpidesc] nvarchar(2000) null,
	[kpivalue] int not null,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

END
GO

USE [master]
GO
ALTER DATABASE [ledgerly-dashbaord] SET READ_WRITE;
GO

