﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using document.lib.ef;

#nullable disable

namespace document.lib.ef.Migrations
{
    [DbContext(typeof(DocumentLibContext))]
    partial class DocumentLibContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("document.lib.ef.Entities.EfCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("DateModified")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("document.lib.ef.Entities.EfDocument", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BlobLocation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Company")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("DateModified")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("DateOfDocument")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Digital")
                        .HasColumnType("bit");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhysicalName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RegisterId")
                        .HasColumnType("int");

                    b.Property<bool>("Unsorted")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("UploadDate")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("RegisterId");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("document.lib.ef.Entities.EfFolder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("DateModified")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsFull")
                        .HasColumnType("bit");

                    b.Property<int>("MaxDocumentsFolder")
                        .HasColumnType("int");

                    b.Property<int>("MaxDocumentsRegister")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalDocuments")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Folders");
                });

            modelBuilder.Entity("document.lib.ef.Entities.EfRegister", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("DateModified")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DocumentCount")
                        .HasColumnType("int");

                    b.Property<int?>("FolderId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FolderId");

                    b.ToTable("Registers");
                });

            modelBuilder.Entity("document.lib.ef.Entities.EfTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("DateModified")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("EfDocumentId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EfDocumentId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("document.lib.ef.Entities.EfDocument", b =>
                {
                    b.HasOne("document.lib.ef.Entities.EfCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("document.lib.ef.Entities.EfRegister", "Register")
                        .WithMany("Documents")
                        .HasForeignKey("RegisterId");

                    b.Navigation("Category");

                    b.Navigation("Register");
                });

            modelBuilder.Entity("document.lib.ef.Entities.EfRegister", b =>
                {
                    b.HasOne("document.lib.ef.Entities.EfFolder", "Folder")
                        .WithMany("Registers")
                        .HasForeignKey("FolderId");

                    b.Navigation("Folder");
                });

            modelBuilder.Entity("document.lib.ef.Entities.EfTag", b =>
                {
                    b.HasOne("document.lib.ef.Entities.EfDocument", null)
                        .WithMany("Tags")
                        .HasForeignKey("EfDocumentId");
                });

            modelBuilder.Entity("document.lib.ef.Entities.EfDocument", b =>
                {
                    b.Navigation("Tags");
                });

            modelBuilder.Entity("document.lib.ef.Entities.EfFolder", b =>
                {
                    b.Navigation("Registers");
                });

            modelBuilder.Entity("document.lib.ef.Entities.EfRegister", b =>
                {
                    b.Navigation("Documents");
                });
#pragma warning restore 612, 618
        }
    }
}
