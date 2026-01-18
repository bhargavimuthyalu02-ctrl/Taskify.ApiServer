using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskify.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimestampsAndRoleColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Role to Users table if it doesn't exist (must happen FIRST)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Role')
                BEGIN
                    ALTER TABLE [dbo].[Users] ADD [Role] nvarchar(50) NOT NULL DEFAULT 'User';
                END
                ELSE
                BEGIN
                    -- If Role exists but is nvarchar(max), alter it to nvarchar(50)
                    DECLARE @var0 sysname;
                    SELECT @var0 = [d].[name]
                    FROM [sys].[default_constraints] [d]
                    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Role');
                    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var0 + '];');
                    ALTER TABLE [Users] ALTER COLUMN [Role] nvarchar(50) NOT NULL;
                END
            ");

            // Add CreatedAt to Tasks table if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND name = 'CreatedAt')
                BEGIN
                    ALTER TABLE [dbo].[Tasks] ADD [CreatedAt] datetime2 NULL DEFAULT GETUTCDATE();
                END
            ");

            // Add UpdatedAt to Tasks table if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND name = 'UpdatedAt')
                BEGIN
                    ALTER TABLE [dbo].[Tasks] ADD [UpdatedAt] datetime2 NULL DEFAULT GETUTCDATE();
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop columns if rolling back
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND name = 'CreatedAt')
                BEGIN
                    DECLARE @var0 sysname;
                    SELECT @var0 = [d].[name]
                    FROM [sys].[default_constraints] [d]
                    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Tasks]') AND [c].[name] = N'CreatedAt');
                    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Tasks] DROP CONSTRAINT [' + @var0 + '];');
                    ALTER TABLE [dbo].[Tasks] DROP COLUMN [CreatedAt];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND name = 'UpdatedAt')
                BEGIN
                    DECLARE @var1 sysname;
                    SELECT @var1 = [d].[name]
                    FROM [sys].[default_constraints] [d]
                    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Tasks]') AND [c].[name] = N'UpdatedAt');
                    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Tasks] DROP CONSTRAINT [' + @var1 + '];');
                    ALTER TABLE [dbo].[Tasks] DROP COLUMN [UpdatedAt];
                END
            ");

            // Revert Role column back to nvarchar(max) if needed
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Role')
                BEGIN
                    ALTER TABLE [Users] ALTER COLUMN [Role] nvarchar(max) NOT NULL;
                END
            ");
        }
    }
}
