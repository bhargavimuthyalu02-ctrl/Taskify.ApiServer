using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskify.Data.Migrations
{
    public partial class AddOwnerEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add OwnerEmail column if it doesn't exist (safe for existing DBs)
            migrationBuilder.Sql(@"
IF NOT EXISTS(
    SELECT * FROM sys.columns
    WHERE Name = N'OwnerEmail'
      AND Object_ID = Object_ID(N'dbo.Tasks')
)
BEGIN
    ALTER TABLE [dbo].[Tasks] ADD [OwnerEmail] nvarchar(200) NOT NULL CONSTRAINT DF_Tasks_OwnerEmail DEFAULT('')
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove OwnerEmail column if it exists (drop default constraint first)
            migrationBuilder.Sql(@"
IF EXISTS(
    SELECT * FROM sys.columns
    WHERE Name = N'OwnerEmail'
      AND Object_ID = Object_ID(N'dbo.Tasks')
)
BEGIN
    DECLARE @df nvarchar(200);
    SELECT @df = df.name
    FROM sys.default_constraints df
    INNER JOIN sys.columns c ON c.default_object_id = df.object_id
    WHERE c.object_id = OBJECT_ID('dbo.Tasks') AND c.name = 'OwnerEmail';

    IF @df IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE [dbo].[Tasks] DROP CONSTRAINT ' + @df);
    END

    ALTER TABLE [dbo].[Tasks] DROP COLUMN [OwnerEmail];
END
");
        }
    }
}
