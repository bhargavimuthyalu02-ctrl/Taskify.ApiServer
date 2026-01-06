using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskify.Data.Migrations
{
    public partial class AddTimestampsToTasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add columns if they do not exist
            migrationBuilder.Sql(@"
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'CreatedAt' AND Object_ID = Object_ID(N'dbo.Tasks'))
BEGIN
    ALTER TABLE [dbo].[Tasks] ADD [CreatedAt] datetime2 NOT NULL CONSTRAINT DF_Tasks_CreatedAt DEFAULT(GETUTCDATE())
END

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'UpdatedAt' AND Object_ID = Object_ID(N'dbo.Tasks'))
BEGIN
    ALTER TABLE [dbo].[Tasks] ADD [UpdatedAt] datetime2 NOT NULL CONSTRAINT DF_Tasks_UpdatedAt DEFAULT(GETUTCDATE())
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop columns if they exist
            migrationBuilder.Sql(@"
IF EXISTS(SELECT * FROM sys.columns WHERE Name = N'CreatedAt' AND Object_ID = Object_ID(N'dbo.Tasks'))
BEGIN
    DECLARE @df nvarchar(200);
    SELECT @df = df.name
    FROM sys.default_constraints df
    INNER JOIN sys.columns c ON c.default_object_id = df.object_id
    WHERE c.object_id = OBJECT_ID('dbo.Tasks') AND c.name = 'CreatedAt';

    IF @df IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE [dbo].[Tasks] DROP CONSTRAINT ' + @df);
    END

    ALTER TABLE [dbo].[Tasks] DROP COLUMN [CreatedAt];
END

IF EXISTS(SELECT * FROM sys.columns WHERE Name = N'UpdatedAt' AND Object_ID = Object_ID(N'dbo.Tasks'))
BEGIN
    DECLARE @df2 nvarchar(200);
    SELECT @df2 = df.name
    FROM sys.default_constraints df
    INNER JOIN sys.columns c ON c.default_object_id = df.object_id
    WHERE c.object_id = OBJECT_ID('dbo.Tasks') AND c.name = 'UpdatedAt';

    IF @df2 IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE [dbo].[Tasks] DROP CONSTRAINT ' + @df2);
    END

    ALTER TABLE [dbo].[Tasks] DROP COLUMN [UpdatedAt];
END
");
        }
    }
}
