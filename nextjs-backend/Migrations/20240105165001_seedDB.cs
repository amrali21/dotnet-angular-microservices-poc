using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nextjs_backend.Migrations
{
    public partial class seedDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "customers",
            //    columns: table => new
            //    {
            //        id = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
            //        name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
            //        email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
            //        image_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_customers", x => x.id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "invoices",
            //    columns: table => new
            //    {
            //        id = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
            //        customer_id = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
            //        amount = table.Column<int>(type: "int", nullable: false),
            //        status = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
            //        date = table.Column<DateTime>(type: "date", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_invoices", x => x.id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "revenue",
            //    columns: table => new
            //    {
            //        month = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: false),
            //        revenue = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_revenue", x => new { x.month, x.revenue });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "users",
            //    columns: table => new
            //    {
            //        id = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
            //        name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
            //        email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
            //        password = table.Column<string>(type: "text", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_users", x => x.id);
            //    });

            migrationBuilder.InsertData(
                table: "customers",
                columns: new[] { "id", "email", "image_url", "name" },
                values: new object[,]
                {
                    { "126eed9c-c90c-4ef6-a4a8-fcf7408d3c66", "emil@kowalski.com", "/customers/emil-kowalski.png", "Emil Kowalski" },
                    { "13D07535-C59E-4157-A011-F8D2EF4E0CBB", "balazs@orban.com", "/customers/balazs-orban.png", "Balazs Orban" },
                    { "3958dc9e-712f-4377-85e9-fec4b6a6442a", "delba@oliveira.com", "/customers/delba-de-oliveira.png", "Delba de Oliveira" },
                    { "3958dc9e-737f-4377-85e9-fec4b6a6442a", "hector@simpson.com", "/customers/hector-simpson.png", "Hector Simpson" },
                    { "3958dc9e-742f-4377-85e9-fec4b6a6442a", "lee@robinson.com", "/customers/lee-robinson.png", "Lee Robinson" },
                    { "3958dc9e-787f-4377-85e9-fec4b6a6442a", "steph@dietz.com", "/customers/steph-dietz.png", "Steph Dietz" },
                    { "50ca3e18-62cd-11ee-8c99-0242ac120002", "steven@tey.com", "/customers/steven-tey.png", "Steven Tey" },
                    { "76d65c26-f784-44a2-ac19-586678f7c2f2", "michael@novotny.com", "/customers/michael-novotny.png", "Michael Novotny" },
                    { "CC27C14A-0ACF-4F4A-A6C9-D45682C144B9", "amy@burns.com", "/customers/amy-burns.png", "Amy Burns" },
                    { "d6e15727-9fe1-4961-8c5b-ea44a9bd81aa", "evil@rabbit.com", "/customers/evil-rabbit.png", "Evil Rabbit" }
                });

            migrationBuilder.InsertData(
                table: "invoices",
                columns: new[] { "id", "amount", "customer_id", "date", "status" },
                values: new object[,]
                {
                    { "1", 15795, "3958dc9e-712f-4377-85e9-fec4b6a6442a", new DateTime(2022, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "pending" },
                    { "10", 8546, "76d65c26-f784-44a2-ac19-586678f7c2f2", new DateTime(2023, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "paid" },
                    { "11", 500, "3958dc9e-742f-4377-85e9-fec4b6a6442a", new DateTime(2023, 8, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "paid" },
                    { "12", 8945, "76d65c26-f784-44a2-ac19-586678f7c2f2", new DateTime(2023, 6, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "paid" },
                    { "13", 8945, "3958dc9e-737f-4377-85e9-fec4b6a6442a", new DateTime(2023, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "paid" },
                    { "14", 8945, "3958dc9e-712f-4377-85e9-fec4b6a6442a", new DateTime(2023, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "paid" },
                    { "15", 1000, "3958dc9e-737f-4377-85e9-fec4b6a6442a", new DateTime(2022, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "paid" },
                    { "2", 20348, "3958dc9e-742f-4377-85e9-fec4b6a6442a", new DateTime(2022, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "pending" },
                    { "3", 3040, "3958dc9e-787f-4377-85e9-fec4b6a6442a", new DateTime(2022, 10, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "paid" },
                    { "4", 44800, "50ca3e18-62cd-11ee-8c99-0242ac120002", new DateTime(2023, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "paid" },
                    { "5", 34577, "76d65c26-f784-44a2-ac19-586678f7c2f2", new DateTime(2023, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "pending" },
                    { "6", 54246, "126eed9c-c90c-4ef6-a4a8-fcf7408d3c66", new DateTime(2023, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "pending" },
                    { "7", 666, "d6e15727-9fe1-4961-8c5b-ea44a9bd81aa", new DateTime(2023, 6, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "pending" },
                    { "8", 32545, "50ca3e18-62cd-11ee-8c99-0242ac120002", new DateTime(2023, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "paid" },
                    { "9", 1250, "3958dc9e-787f-4377-85e9-fec4b6a6442a", new DateTime(2023, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "paid" }
                });

            migrationBuilder.InsertData(
                table: "revenue",
                columns: new[] { "month", "revenue" },
                values: new object[,]
                {
                    { "Apr", 2500 },
                    { "Aug", 3700 },
                    { "Dec", 4800 },
                    { "Feb", 1800 },
                    { "Jan", 2000 },
                    { "Jul", 3500 },
                    { "Jun", 3200 },
                    { "Mar", 2200 },
                    { "May", 2300 },
                    { "Nov", 3000 },
                    { "Oct", 2800 },
                    { "Sep", 2500 }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "email", "name", "password" },
                values: new object[] { "410544b2-4001-4271-9855-fec4b6a6442a", "user@nextmail.com", "User", "123456" });

            //migrationBuilder.CreateIndex(
            //    name: "UQ__revenue__0DD75472E02AC0B3",
            //    table: "revenue",
            //    column: "month",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "UQ__users__AB6E6164F19C174A",
            //    table: "users",
            //    column: "email",
            //    unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "invoices");

            migrationBuilder.DropTable(
                name: "revenue");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
