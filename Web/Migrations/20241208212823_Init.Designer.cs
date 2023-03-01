﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Web.Db;

#nullable disable

namespace Web.Migrations
{
    [DbContext(typeof(DMADbContext))]
    [Migration("20241208212823_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Web.Models.Adc", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AdcManufacturerId")
                        .HasColumnType("int");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AdcManufacturerId");

                    b.ToTable("Adces");
                });

            modelBuilder.Entity("Web.Models.AdcManufacturer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AdcManufacturers");
                });

            modelBuilder.Entity("Web.Models.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AddedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ArtistId")
                        .HasColumnType("int");

                    b.Property<int?>("CountryId")
                        .HasColumnType("int");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discogs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GenreId")
                        .HasColumnType("int");

                    b.Property<int?>("LabelId")
                        .HasColumnType("int");

                    b.Property<int?>("ReissueId")
                        .HasColumnType("int");

                    b.Property<double?>("Size")
                        .HasColumnType("float");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StorageId")
                        .HasColumnType("int");

                    b.Property<int>("YearId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.HasIndex("CountryId");

                    b.HasIndex("GenreId");

                    b.HasIndex("LabelId");

                    b.HasIndex("ReissueId");

                    b.HasIndex("StorageId");

                    b.HasIndex("YearId");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("Web.Models.Amplifier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AmplifierManufacturerId")
                        .HasColumnType("int");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AmplifierManufacturerId");

                    b.ToTable("Amplifiers");
                });

            modelBuilder.Entity("Web.Models.AmplifierManufacturer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AmplifierManufacturers");
                });

            modelBuilder.Entity("Web.Models.Artist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("Web.Models.Bitness", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Data")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Bitnesses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Data = 1
                        },
                        new
                        {
                            Id = 2,
                            Data = 24
                        },
                        new
                        {
                            Id = 3,
                            Data = 32
                        },
                        new
                        {
                            Id = 4,
                            Data = 64
                        });
                });

            modelBuilder.Entity("Web.Models.Cartrige", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CartrigeManufacturerId")
                        .HasColumnType("int");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CartrigeManufacturerId");

                    b.ToTable("Cartriges");
                });

            modelBuilder.Entity("Web.Models.CartrigeManufacturer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CartrigeManufacturers");
                });

            modelBuilder.Entity("Web.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("Web.Models.DigitalFormat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DigitalFormats");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Data = "FLAC"
                        },
                        new
                        {
                            Id = 2,
                            Data = "DSD64"
                        },
                        new
                        {
                            Id = 3,
                            Data = "DSD128"
                        },
                        new
                        {
                            Id = 4,
                            Data = "DSD256"
                        },
                        new
                        {
                            Id = 5,
                            Data = "DSD512"
                        },
                        new
                        {
                            Id = 6,
                            Data = "WV"
                        });
                });

            modelBuilder.Entity("Web.Models.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("Web.Models.Label", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Labels");
                });

            modelBuilder.Entity("Web.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlayerManufacturerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlayerManufacturerId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Web.Models.PlayerManufacturer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PlayerManufacturers");
                });

            modelBuilder.Entity("Web.Models.Processing", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OpeationCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Processings");
                });

            modelBuilder.Entity("Web.Models.Reissue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("Data")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Reissues");
                });

            modelBuilder.Entity("Web.Models.Sampling", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Data")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Samplings");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Data = 96.0
                        },
                        new
                        {
                            Id = 2,
                            Data = 192.0
                        },
                        new
                        {
                            Id = 3,
                            Data = 384.0
                        },
                        new
                        {
                            Id = 4,
                            Data = 2.7999999999999998
                        },
                        new
                        {
                            Id = 5,
                            Data = 5.5999999999999996
                        },
                        new
                        {
                            Id = 6,
                            Data = 11.199999999999999
                        },
                        new
                        {
                            Id = 7,
                            Data = 22.5
                        });
                });

            modelBuilder.Entity("Web.Models.SourceFormat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SourceFormats");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Data = "LP 12'' 33RPM"
                        },
                        new
                        {
                            Id = 2,
                            Data = "EP 10'' 45RPM"
                        },
                        new
                        {
                            Id = 3,
                            Data = "EP 12'' 45RPM"
                        },
                        new
                        {
                            Id = 4,
                            Data = "SINGLE 7'' 45RPM"
                        },
                        new
                        {
                            Id = 5,
                            Data = "SINGLE 12'' 45RPM"
                        },
                        new
                        {
                            Id = 6,
                            Data = "SHELLAC 10'' 78RPM"
                        });
                });

            modelBuilder.Entity("Web.Models.Storage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Storages");
                });

            modelBuilder.Entity("Web.Models.TechnicalInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AdcId")
                        .HasColumnType("int");

                    b.Property<int?>("AlbumId")
                        .HasColumnType("int");

                    b.Property<int?>("AmplifierId")
                        .HasColumnType("int");

                    b.Property<int?>("BitnessId")
                        .HasColumnType("int");

                    b.Property<int?>("CartrigeId")
                        .HasColumnType("int");

                    b.Property<int?>("DigitalFormatId")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerId")
                        .HasColumnType("int");

                    b.Property<int?>("ProcessingId")
                        .HasColumnType("int");

                    b.Property<int?>("SamplingId")
                        .HasColumnType("int");

                    b.Property<int?>("SourceFormatId")
                        .HasColumnType("int");

                    b.Property<int?>("VinylStateId")
                        .HasColumnType("int");

                    b.Property<int?>("WireId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AdcId");

                    b.HasIndex("AlbumId")
                        .IsUnique()
                        .HasFilter("[AlbumId] IS NOT NULL");

                    b.HasIndex("AmplifierId");

                    b.HasIndex("BitnessId");

                    b.HasIndex("CartrigeId");

                    b.HasIndex("DigitalFormatId");

                    b.HasIndex("PlayerId");

                    b.HasIndex("ProcessingId");

                    b.HasIndex("SamplingId");

                    b.HasIndex("SourceFormatId");

                    b.HasIndex("VinylStateId");

                    b.HasIndex("WireId");

                    b.ToTable("TechnicalInfos");
                });

            modelBuilder.Entity("Web.Models.VinylState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("VinylStates");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Data = "Mint"
                        },
                        new
                        {
                            Id = 2,
                            Data = "Near Mint"
                        },
                        new
                        {
                            Id = 3,
                            Data = "Very Good+"
                        },
                        new
                        {
                            Id = 4,
                            Data = "Very Good"
                        },
                        new
                        {
                            Id = 5,
                            Data = "Good"
                        },
                        new
                        {
                            Id = 6,
                            Data = "Unknown"
                        });
                });

            modelBuilder.Entity("Web.Models.Wire", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WireManufacturerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WireManufacturerId");

                    b.ToTable("Wires");
                });

            modelBuilder.Entity("Web.Models.WireManufacturer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("WireManufacturers");
                });

            modelBuilder.Entity("Web.Models.Year", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Data")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Years");
                });

            modelBuilder.Entity("Web.Models.Adc", b =>
                {
                    b.HasOne("Web.Models.AdcManufacturer", "AdcManufacturer")
                        .WithMany("Adcs")
                        .HasForeignKey("AdcManufacturerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AdcManufacturer");
                });

            modelBuilder.Entity("Web.Models.Album", b =>
                {
                    b.HasOne("Web.Models.Artist", "Artist")
                        .WithMany("Albums")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Web.Models.Country", "Country")
                        .WithMany("Albums")
                        .HasForeignKey("CountryId");

                    b.HasOne("Web.Models.Genre", "Genre")
                        .WithMany("Albums")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Web.Models.Label", "Label")
                        .WithMany("Albums")
                        .HasForeignKey("LabelId");

                    b.HasOne("Web.Models.Reissue", "Reissue")
                        .WithMany("Albums")
                        .HasForeignKey("ReissueId");

                    b.HasOne("Web.Models.Storage", "Storage")
                        .WithMany("Albums")
                        .HasForeignKey("StorageId");

                    b.HasOne("Web.Models.Year", "Year")
                        .WithMany("Albums")
                        .HasForeignKey("YearId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");

                    b.Navigation("Country");

                    b.Navigation("Genre");

                    b.Navigation("Label");

                    b.Navigation("Reissue");

                    b.Navigation("Storage");

                    b.Navigation("Year");
                });

            modelBuilder.Entity("Web.Models.Amplifier", b =>
                {
                    b.HasOne("Web.Models.AmplifierManufacturer", "AmplifierManufacturer")
                        .WithMany("Amplifiers")
                        .HasForeignKey("AmplifierManufacturerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AmplifierManufacturer");
                });

            modelBuilder.Entity("Web.Models.Cartrige", b =>
                {
                    b.HasOne("Web.Models.CartrigeManufacturer", "CartrigeManufacturer")
                        .WithMany("Cartriges")
                        .HasForeignKey("CartrigeManufacturerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CartrigeManufacturer");
                });

            modelBuilder.Entity("Web.Models.Player", b =>
                {
                    b.HasOne("Web.Models.PlayerManufacturer", "PlayerManufacturer")
                        .WithMany("Players")
                        .HasForeignKey("PlayerManufacturerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PlayerManufacturer");
                });

            modelBuilder.Entity("Web.Models.TechnicalInfo", b =>
                {
                    b.HasOne("Web.Models.Adc", "Adc")
                        .WithMany("TechnicalInfos")
                        .HasForeignKey("AdcId");

                    b.HasOne("Web.Models.Album", "Album")
                        .WithOne("TechnicalInfo")
                        .HasForeignKey("Web.Models.TechnicalInfo", "AlbumId");

                    b.HasOne("Web.Models.Amplifier", "Amplifier")
                        .WithMany("TechnicalInfos")
                        .HasForeignKey("AmplifierId");

                    b.HasOne("Web.Models.Bitness", "Bitness")
                        .WithMany("TechnicalInfos")
                        .HasForeignKey("BitnessId");

                    b.HasOne("Web.Models.Cartrige", "Cartrige")
                        .WithMany("TechnicalInfos")
                        .HasForeignKey("CartrigeId");

                    b.HasOne("Web.Models.DigitalFormat", "DigitalFormat")
                        .WithMany("TechnicalInfos")
                        .HasForeignKey("DigitalFormatId");

                    b.HasOne("Web.Models.Player", "Player")
                        .WithMany("TechnicalInfos")
                        .HasForeignKey("PlayerId");

                    b.HasOne("Web.Models.Processing", "Processing")
                        .WithMany("TechnicalInfos")
                        .HasForeignKey("ProcessingId");

                    b.HasOne("Web.Models.Sampling", "Sampling")
                        .WithMany("TechnicalInfos")
                        .HasForeignKey("SamplingId");

                    b.HasOne("Web.Models.SourceFormat", "SourceFormat")
                        .WithMany("TechnicalInfos")
                        .HasForeignKey("SourceFormatId");

                    b.HasOne("Web.Models.VinylState", "VinylState")
                        .WithMany("TechnicalInfos")
                        .HasForeignKey("VinylStateId");

                    b.HasOne("Web.Models.Wire", "Wire")
                        .WithMany("TechnicalInfos")
                        .HasForeignKey("WireId");

                    b.Navigation("Adc");

                    b.Navigation("Album");

                    b.Navigation("Amplifier");

                    b.Navigation("Bitness");

                    b.Navigation("Cartrige");

                    b.Navigation("DigitalFormat");

                    b.Navigation("Player");

                    b.Navigation("Processing");

                    b.Navigation("Sampling");

                    b.Navigation("SourceFormat");

                    b.Navigation("VinylState");

                    b.Navigation("Wire");
                });

            modelBuilder.Entity("Web.Models.Wire", b =>
                {
                    b.HasOne("Web.Models.WireManufacturer", "WireManufacturer")
                        .WithMany("Wires")
                        .HasForeignKey("WireManufacturerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WireManufacturer");
                });

            modelBuilder.Entity("Web.Models.Adc", b =>
                {
                    b.Navigation("TechnicalInfos");
                });

            modelBuilder.Entity("Web.Models.AdcManufacturer", b =>
                {
                    b.Navigation("Adcs");
                });

            modelBuilder.Entity("Web.Models.Album", b =>
                {
                    b.Navigation("TechnicalInfo");
                });

            modelBuilder.Entity("Web.Models.Amplifier", b =>
                {
                    b.Navigation("TechnicalInfos");
                });

            modelBuilder.Entity("Web.Models.AmplifierManufacturer", b =>
                {
                    b.Navigation("Amplifiers");
                });

            modelBuilder.Entity("Web.Models.Artist", b =>
                {
                    b.Navigation("Albums");
                });

            modelBuilder.Entity("Web.Models.Bitness", b =>
                {
                    b.Navigation("TechnicalInfos");
                });

            modelBuilder.Entity("Web.Models.Cartrige", b =>
                {
                    b.Navigation("TechnicalInfos");
                });

            modelBuilder.Entity("Web.Models.CartrigeManufacturer", b =>
                {
                    b.Navigation("Cartriges");
                });

            modelBuilder.Entity("Web.Models.Country", b =>
                {
                    b.Navigation("Albums");
                });

            modelBuilder.Entity("Web.Models.DigitalFormat", b =>
                {
                    b.Navigation("TechnicalInfos");
                });

            modelBuilder.Entity("Web.Models.Genre", b =>
                {
                    b.Navigation("Albums");
                });

            modelBuilder.Entity("Web.Models.Label", b =>
                {
                    b.Navigation("Albums");
                });

            modelBuilder.Entity("Web.Models.Player", b =>
                {
                    b.Navigation("TechnicalInfos");
                });

            modelBuilder.Entity("Web.Models.PlayerManufacturer", b =>
                {
                    b.Navigation("Players");
                });

            modelBuilder.Entity("Web.Models.Processing", b =>
                {
                    b.Navigation("TechnicalInfos");
                });

            modelBuilder.Entity("Web.Models.Reissue", b =>
                {
                    b.Navigation("Albums");
                });

            modelBuilder.Entity("Web.Models.Sampling", b =>
                {
                    b.Navigation("TechnicalInfos");
                });

            modelBuilder.Entity("Web.Models.SourceFormat", b =>
                {
                    b.Navigation("TechnicalInfos");
                });

            modelBuilder.Entity("Web.Models.Storage", b =>
                {
                    b.Navigation("Albums");
                });

            modelBuilder.Entity("Web.Models.VinylState", b =>
                {
                    b.Navigation("TechnicalInfos");
                });

            modelBuilder.Entity("Web.Models.Wire", b =>
                {
                    b.Navigation("TechnicalInfos");
                });

            modelBuilder.Entity("Web.Models.WireManufacturer", b =>
                {
                    b.Navigation("Wires");
                });

            modelBuilder.Entity("Web.Models.Year", b =>
                {
                    b.Navigation("Albums");
                });
#pragma warning restore 612, 618
        }
    }
}
