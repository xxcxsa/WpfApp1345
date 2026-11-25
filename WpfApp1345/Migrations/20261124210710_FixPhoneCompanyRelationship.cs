
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WpfApp1345.Migrations
{
    public partial class FixPhoneCompanyRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Добавляем временный столбец CompanyId.
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Phones",
                type: "integer",
                nullable: true);

            // 2. Добавляем производителей, которых нет в Companies.
            migrationBuilder.Sql(@"
                INSERT INTO ""Companies"" (""Title"", ""CEO"", ""Capital"")
                SELECT DISTINCT
                    COALESCE(NULLIF(BTRIM(p.""Company""), ''), 'Unknown'),
                    'Unknown',
                    0
                FROM ""Phones"" p
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM ""Companies"" c
                    WHERE BTRIM(c.""Title"") =
                        COALESCE(NULLIF(BTRIM(p.""Company""), ''), 'Unknown')
                );
            ");

            // 3. Связываем телефоны с производителями.
            // При наличии дубликатов выбираем минимальный Id.
            migrationBuilder.Sql(@"
                UPDATE ""Phones"" p
                SET ""CompanyId"" = (
                    SELECT MIN(c.""Id"")
                    FROM ""Companies"" c
                    WHERE BTRIM(c.""Title"") =
                        COALESCE(NULLIF(BTRIM(p.""Company""), ''), 'Unknown')
                );
            ");

            // 4. Проверяем правильность всех связей.
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM ""Phones"" p
                        WHERE p.""CompanyId"" IS NULL
                           OR NOT EXISTS (
                               SELECT 1
                               FROM ""Companies"" c
                               WHERE c.""Id"" = p.""CompanyId""
                           )
                    ) THEN
                        RAISE EXCEPTION
                            'Не удалось связать все телефоны с производителями.';
                    END IF;
                END $$;
            ");

            // 5. Делаем CompanyId обязательным.
            migrationBuilder.Sql(@"
                ALTER TABLE ""Phones""
                ALTER COLUMN ""CompanyId"" SET NOT NULL;
            ");

            // 6. Создаём индекс.
            migrationBuilder.CreateIndex(
                name: "IX_Phones_CompanyId",
                table: "Phones",
                column: "CompanyId");

            // 7. Создаём внешний ключ.
            migrationBuilder.AddForeignKey(
                name: "FK_Phones_Companies_CompanyId",
                table: "Phones",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // 8. Удаляем старый текстовый столбец.
            migrationBuilder.DropColumn(
                name: "Company",
                table: "Phones");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Удаляем внешний ключ.
            migrationBuilder.DropForeignKey(
                name: "FK_Phones_Companies_CompanyId",
                table: "Phones");

            // 2. Удаляем индекс.
            migrationBuilder.DropIndex(
                name: "IX_Phones_CompanyId",
                table: "Phones");

            // 3. Возвращаем старый столбец Company.
            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "Phones",
                type: "text",
                nullable: true);

            // 4. Восстанавливаем названия производителей.
            migrationBuilder.Sql(@"
                UPDATE ""Phones"" p
                SET ""Company"" = c.""Title""
                FROM ""Companies"" c
                WHERE p.""CompanyId"" = c.""Id"";
            ");

            // 5. Возвращаем обязательность Company.
            migrationBuilder.Sql(@"
                ALTER TABLE ""Phones""
                ALTER COLUMN ""Company"" SET NOT NULL;
            ");

            // 6. Удаляем CompanyId.
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Phones");
        }
    }
}
