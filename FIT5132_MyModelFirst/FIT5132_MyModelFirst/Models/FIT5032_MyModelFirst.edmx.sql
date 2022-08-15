
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/15/2022 12:14:47
-- Generated from EDMX file: C:\Users\Nicol\Desktop\Monash\FIT5032-Internet-Apps-Dev\FIT5132_MyModelFirst\FIT5132_MyModelFirst\Models\FIT5032_MyModelFirst.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [FIT5032_MyModelFirst];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Students'
CREATE TABLE [dbo].[Students] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [LastName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Units'
CREATE TABLE [dbo].[Units] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'StudentUnit'
CREATE TABLE [dbo].[StudentUnit] (
    [Students_Id] int  NOT NULL,
    [Units_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Students'
ALTER TABLE [dbo].[Students]
ADD CONSTRAINT [PK_Students]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Units'
ALTER TABLE [dbo].[Units]
ADD CONSTRAINT [PK_Units]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Students_Id], [Units_Id] in table 'StudentUnit'
ALTER TABLE [dbo].[StudentUnit]
ADD CONSTRAINT [PK_StudentUnit]
    PRIMARY KEY CLUSTERED ([Students_Id], [Units_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Students_Id] in table 'StudentUnit'
ALTER TABLE [dbo].[StudentUnit]
ADD CONSTRAINT [FK_StudentUnit_Student]
    FOREIGN KEY ([Students_Id])
    REFERENCES [dbo].[Students]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Units_Id] in table 'StudentUnit'
ALTER TABLE [dbo].[StudentUnit]
ADD CONSTRAINT [FK_StudentUnit_Unit]
    FOREIGN KEY ([Units_Id])
    REFERENCES [dbo].[Units]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StudentUnit_Unit'
CREATE INDEX [IX_FK_StudentUnit_Unit]
ON [dbo].[StudentUnit]
    ([Units_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------