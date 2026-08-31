using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace eShopPorted.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogBrand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Brand = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogBrand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Catalog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    PictureFileName = table.Column<string>(nullable: false),
                    CatalogTypeId = table.Column<int>(nullable: false),
                    CatalogBrandId = table.Column<int>(nullable: false),
                    AvailableStock = table.Column<int>(nullable: false),
                    RestockThreshold = table.Column<int>(nullable: false),
                    MaxStockThreshold = table.Column<int>(nullable: false),
                    OnReorder = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_CatalogBrand_CatalogBrandId",
                        column: x => x.CatalogBrandId,
                        principalTable: "CatalogBrand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Catalog_CatalogType_CatalogTypeId",
                        column: x => x.CatalogTypeId,
                        principalTable: "CatalogType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CatalogBrand",
                columns: new[] { "Id", "Brand" },
                values: new object[,]
                {
                    { 1, "Ferrari" },
                    { 2, "Lamborghini" },
                    { 3, "Porsche" },
                    { 4, "Maserati" },
                    { 5, "Other" }
                });

            migrationBuilder.InsertData(
                table: "CatalogType",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { 1, "Sports Car" },
                    { 2, "GT" },
                    { 3, "SUV" },
                    { 4, "Spare Part" }
                });

            migrationBuilder.InsertData(
                table: "Catalog",
                columns: new[] { "Id", "AvailableStock", "CatalogBrandId", "CatalogTypeId", "Description", "MaxStockThreshold", "Name", "OnReorder", "PictureFileName", "Price", "RestockThreshold" },
                values: new object[,]
                {
                    { 2, 100, 1, 1, "Ferrari 296 GTB with 663 cv V6 hybrid powertrain", 0, "Ferrari 296 GTB", false, "2.png", 320000m, 0 },
                    { 9, 100, 4, 1, "Maserati MC20 with Nettuno twin-turbo V6", 0, "Maserati MC20", false, "9.png", 240000m, 0 },
                    { 1, 100, 1, 1, "Ferrari SF90 Stradale plug-in hybrid supercar with 1000 cv", 0, "Ferrari SF90 Stradale", false, "1.png", 507000m, 0 },
                    { 3, 100, 1, 2, "Ferrari Roma front-engined V8 grand tourer", 0, "Ferrari Roma", false, "3.png", 222000m, 0 },
                    { 4, 100, 1, 3, "Ferrari Purosangue four-door four-seater V12", 0, "Ferrari Purosangue", false, "4.png", 390000m, 0 },
                    { 6, 100, 2, 3, "Lamborghini Urus Performante super SUV", 0, "Lamborghini Urus Performante", false, "6.png", 260000m, 0 },
                    { 7, 100, 3, 1, "Porsche 911 GT3 RS track-focused flat-six", 0, "Porsche 911 GT3 RS", false, "7.png", 241000m, 0 },
                    { 8, 100, 3, 3, "Porsche Cayenne Turbo GT performance SUV", 0, "Porsche Cayenne Turbo GT", false, "8.png", 198000m, 0 },
                    { 12, 100, 5, 4, "Forged alloy wheel set 20/21 inch staggered", 0, "Forged Alloy Wheel Set", false, "12.png", 8900m, 0 },
                    { 5, 100, 2, 1, "Lamborghini Revuelto V12 hybrid flagship", 0, "Lamborghini Revuelto", false, "5.png", 517000m, 0 },
                    { 10, 100, 4, 2, "Maserati GranTurismo Trofeo V6 grand tourer", 0, "Maserati GranTurismo Trofeo", false, "10.png", 175000m, 0 },
                    { 11, 100, 5, 4, "Carbon ceramic brake kit for track use", 0, "Carbon Ceramic Brake Kit", false, "11.png", 12500m, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_CatalogBrandId",
                table: "Catalog",
                column: "CatalogBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_CatalogTypeId",
                table: "Catalog",
                column: "CatalogTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Catalog");

            migrationBuilder.DropTable(
                name: "CatalogBrand");

            migrationBuilder.DropTable(
                name: "CatalogType");
        }
    }
}
