using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bitnesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bitnesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DigitalFormats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalFormats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reissues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reissues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Samplings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Samplings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SourceFormats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceFormats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Storages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VinylStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VinylStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Years",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Years", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufacturerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adces_Manufacturer_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Amplifiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufacturerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amplifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Amplifiers_Manufacturer_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Cartridges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufacturerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cartridges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cartridges_Manufacturer_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufacturerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Manufacturer_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Wires",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManufacturerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wires_Manufacturer_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PostCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostCategories_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormatInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Size = table.Column<double>(type: "float", nullable: true),
                    BitnessId = table.Column<int>(type: "int", nullable: true),
                    SamplingId = table.Column<int>(type: "int", nullable: true),
                    DigitalFormatId = table.Column<int>(type: "int", nullable: true),
                    SourceFormatId = table.Column<int>(type: "int", nullable: true),
                    VinylStateId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormatInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormatInfo_Bitnesses_BitnessId",
                        column: x => x.BitnessId,
                        principalTable: "Bitnesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FormatInfo_DigitalFormats_DigitalFormatId",
                        column: x => x.DigitalFormatId,
                        principalTable: "DigitalFormats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FormatInfo_Samplings_SamplingId",
                        column: x => x.SamplingId,
                        principalTable: "Samplings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FormatInfo_SourceFormats_SourceFormatId",
                        column: x => x.SourceFormatId,
                        principalTable: "SourceFormats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FormatInfo_VinylStates_VinylStateId",
                        column: x => x.VinylStateId,
                        principalTable: "VinylStates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    ArtistId = table.Column<int>(type: "int", nullable: false),
                    YearId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Albums_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Albums_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Albums_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EquipmentInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: true),
                    CartridgeId = table.Column<int>(type: "int", nullable: true),
                    AmplifierId = table.Column<int>(type: "int", nullable: true),
                    AdcId = table.Column<int>(type: "int", nullable: true),
                    WireId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentInfo_Adces_AdcId",
                        column: x => x.AdcId,
                        principalTable: "Adces",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentInfo_Amplifiers_AmplifierId",
                        column: x => x.AmplifierId,
                        principalTable: "Amplifiers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentInfo_Cartridges_CartridgeId",
                        column: x => x.CartridgeId,
                        principalTable: "Cartridges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentInfo_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentInfo_Wires_WireId",
                        column: x => x.WireId,
                        principalTable: "Wires",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Digitizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlbumId = table.Column<int>(type: "int", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discogs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFirstPress = table.Column<bool>(type: "bit", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    LabelId = table.Column<int>(type: "int", nullable: true),
                    ReissueId = table.Column<int>(type: "int", nullable: true),
                    YearId = table.Column<int>(type: "int", nullable: true),
                    StorageId = table.Column<int>(type: "int", nullable: true),
                    FormatId = table.Column<int>(type: "int", nullable: true),
                    EquipmentId = table.Column<int>(type: "int", nullable: true),
                    Size = table.Column<double>(type: "float", nullable: true),
                    AdcId = table.Column<int>(type: "int", nullable: true),
                    AmplifierId = table.Column<int>(type: "int", nullable: true),
                    BitnessId = table.Column<int>(type: "int", nullable: true),
                    CartridgeId = table.Column<int>(type: "int", nullable: true),
                    DigitalFormatId = table.Column<int>(type: "int", nullable: true),
                    PlayerId = table.Column<int>(type: "int", nullable: true),
                    SamplingId = table.Column<int>(type: "int", nullable: true),
                    SourceFormatId = table.Column<int>(type: "int", nullable: true),
                    VinylStateId = table.Column<int>(type: "int", nullable: true),
                    WireId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Digitizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Digitizations_Adces_AdcId",
                        column: x => x.AdcId,
                        principalTable: "Adces",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Digitizations_Amplifiers_AmplifierId",
                        column: x => x.AmplifierId,
                        principalTable: "Amplifiers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_Bitnesses_BitnessId",
                        column: x => x.BitnessId,
                        principalTable: "Bitnesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_Cartridges_CartridgeId",
                        column: x => x.CartridgeId,
                        principalTable: "Cartridges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_DigitalFormats_DigitalFormatId",
                        column: x => x.DigitalFormatId,
                        principalTable: "DigitalFormats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_EquipmentInfo_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "EquipmentInfo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_FormatInfo_FormatId",
                        column: x => x.FormatId,
                        principalTable: "FormatInfo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_Labels_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Labels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_Reissues_ReissueId",
                        column: x => x.ReissueId,
                        principalTable: "Reissues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_Samplings_SamplingId",
                        column: x => x.SamplingId,
                        principalTable: "Samplings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_SourceFormats_SourceFormatId",
                        column: x => x.SourceFormatId,
                        principalTable: "SourceFormats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_Storages_StorageId",
                        column: x => x.StorageId,
                        principalTable: "Storages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_VinylStates_VinylStateId",
                        column: x => x.VinylStateId,
                        principalTable: "VinylStates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_Wires_WireId",
                        column: x => x.WireId,
                        principalTable: "Wires",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Digitizations_Years_YearId",
                        column: x => x.YearId,
                        principalTable: "Years",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Bitnesses",
                columns: new[] { "Id", "Value" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 24 },
                    { 3, 32 },
                    { 4, 64 }
                });

            migrationBuilder.InsertData(
                table: "DigitalFormats",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "FLAC" },
                    { 2, "DSD64" },
                    { 3, "DSD128" },
                    { 4, "DSD256" },
                    { 5, "DSD512" },
                    { 6, "WV" }
                });

            migrationBuilder.InsertData(
                table: "Samplings",
                columns: new[] { "Id", "Value" },
                values: new object[,]
                {
                    { 1, 96.0 },
                    { 2, 192.0 },
                    { 3, 384.0 },
                    { 4, 2.7999999999999998 },
                    { 5, 5.5999999999999996 },
                    { 6, 11.199999999999999 },
                    { 7, 22.5 }
                });

            migrationBuilder.InsertData(
                table: "SourceFormats",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "LP 12'' 33RPM" },
                    { 2, "EP 10'' 45RPM" },
                    { 3, "EP 12'' 45RPM" },
                    { 4, "SINGLE 7'' 45RPM" },
                    { 5, "SINGLE 12'' 45RPM" },
                    { 6, "SHELLAC 10'' 78RPM" }
                });

            migrationBuilder.InsertData(
                table: "VinylStates",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Mint" },
                    { 2, "Near Mint" },
                    { 3, "Very Good+" },
                    { 4, "Very Good" },
                    { 5, "Good" },
                    { 6, "Unknown" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adces_ManufacturerId",
                table: "Adces",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_ArtistId",
                table: "Albums",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_GenreId",
                table: "Albums",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_YearId",
                table: "Albums",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_Amplifiers_ManufacturerId",
                table: "Amplifiers",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Cartridges_ManufacturerId",
                table: "Cartridges",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_AdcId",
                table: "Digitizations",
                column: "AdcId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_AlbumId",
                table: "Digitizations",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_AmplifierId",
                table: "Digitizations",
                column: "AmplifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_BitnessId",
                table: "Digitizations",
                column: "BitnessId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_CartridgeId",
                table: "Digitizations",
                column: "CartridgeId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_CountryId",
                table: "Digitizations",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_DigitalFormatId",
                table: "Digitizations",
                column: "DigitalFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_EquipmentId",
                table: "Digitizations",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_FormatId",
                table: "Digitizations",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_LabelId",
                table: "Digitizations",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_PlayerId",
                table: "Digitizations",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_ReissueId",
                table: "Digitizations",
                column: "ReissueId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_SamplingId",
                table: "Digitizations",
                column: "SamplingId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_SourceFormatId",
                table: "Digitizations",
                column: "SourceFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_StorageId",
                table: "Digitizations",
                column: "StorageId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_VinylStateId",
                table: "Digitizations",
                column: "VinylStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_WireId",
                table: "Digitizations",
                column: "WireId");

            migrationBuilder.CreateIndex(
                name: "IX_Digitizations_YearId",
                table: "Digitizations",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentInfo_AdcId",
                table: "EquipmentInfo",
                column: "AdcId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentInfo_AmplifierId",
                table: "EquipmentInfo",
                column: "AmplifierId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentInfo_CartridgeId",
                table: "EquipmentInfo",
                column: "CartridgeId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentInfo_PlayerId",
                table: "EquipmentInfo",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentInfo_WireId",
                table: "EquipmentInfo",
                column: "WireId");

            migrationBuilder.CreateIndex(
                name: "IX_FormatInfo_BitnessId",
                table: "FormatInfo",
                column: "BitnessId");

            migrationBuilder.CreateIndex(
                name: "IX_FormatInfo_DigitalFormatId",
                table: "FormatInfo",
                column: "DigitalFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_FormatInfo_SamplingId",
                table: "FormatInfo",
                column: "SamplingId");

            migrationBuilder.CreateIndex(
                name: "IX_FormatInfo_SourceFormatId",
                table: "FormatInfo",
                column: "SourceFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_FormatInfo_VinylStateId",
                table: "FormatInfo",
                column: "VinylStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_ManufacturerId",
                table: "Players",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_PostCategories_CategoryId",
                table: "PostCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PostCategories_PostId",
                table: "PostCategories",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Wires_ManufacturerId",
                table: "Wires",
                column: "ManufacturerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Digitizations");

            migrationBuilder.DropTable(
                name: "PostCategories");

            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "EquipmentInfo");

            migrationBuilder.DropTable(
                name: "FormatInfo");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.DropTable(
                name: "Reissues");

            migrationBuilder.DropTable(
                name: "Storages");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Artists");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Years");

            migrationBuilder.DropTable(
                name: "Adces");

            migrationBuilder.DropTable(
                name: "Amplifiers");

            migrationBuilder.DropTable(
                name: "Cartridges");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Wires");

            migrationBuilder.DropTable(
                name: "Bitnesses");

            migrationBuilder.DropTable(
                name: "DigitalFormats");

            migrationBuilder.DropTable(
                name: "Samplings");

            migrationBuilder.DropTable(
                name: "SourceFormats");

            migrationBuilder.DropTable(
                name: "VinylStates");

            migrationBuilder.DropTable(
                name: "Manufacturer");
        }
    }
}
