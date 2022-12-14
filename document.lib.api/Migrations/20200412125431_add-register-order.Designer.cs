// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using document.lib.api;

namespace document.lib.api.Migrations
{
    [DbContext(typeof(DocumentlibContext))]
    [Migration("20200412125431_add-register-order")]
    partial class addregisterorder
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("document.lib.api.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("newsequentialid()");

                    b.Property<string>("Abbreviation");

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<DateTimeOffset>("LastUpdatedAt");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("document.lib.api.Models.DocumentTag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("newsequentialid()");

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<DateTimeOffset>("LastUpdatedAt");

                    b.Property<Guid>("LibDocumentId");

                    b.Property<Guid>("TagId");

                    b.HasKey("Id");

                    b.HasIndex("LibDocumentId");

                    b.HasIndex("TagId");

                    b.ToTable("DocumentTags");
                });

            modelBuilder.Entity("document.lib.api.Models.Folder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("newsequentialid()");

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<DateTimeOffset>("LastUpdatedAt");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Folders");
                });

            modelBuilder.Entity("document.lib.api.Models.LibDocument", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("newsequentialid()");

                    b.Property<string>("Blobname");

                    b.Property<Guid?>("CategoryId");

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<DateTimeOffset>("Date")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(new DateTimeOffset(new DateTime(2020, 4, 12, 14, 54, 31, 634, DateTimeKind.Unspecified).AddTicks(4105), new TimeSpan(0, 2, 0, 0, 0)));

                    b.Property<DateTimeOffset>("LastUpdatedAt");

                    b.Property<string>("Name");

                    b.Property<Guid>("RegisterId");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("RegisterId");

                    b.ToTable("LibDocuments");
                });

            modelBuilder.Entity("document.lib.api.Models.Register", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("newsequentialid()");

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<string>("DisplayName")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("");

                    b.Property<int>("DocumentCount");

                    b.Property<Guid>("FolderId");

                    b.Property<DateTimeOffset>("LastUpdatedAt");

                    b.Property<string>("Name");

                    b.Property<int>("Order")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.HasIndex("FolderId");

                    b.ToTable("Registers");
                });

            modelBuilder.Entity("document.lib.api.Models.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("newsequentialid()");

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<DateTimeOffset>("LastUpdatedAt");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("document.lib.api.Models.DocumentTag", b =>
                {
                    b.HasOne("document.lib.api.Models.Tag", "Tag")
                        .WithMany("Documents")
                        .HasForeignKey("LibDocumentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("document.lib.api.Models.LibDocument", "LibDocument")
                        .WithMany("Tags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("document.lib.api.Models.LibDocument", b =>
                {
                    b.HasOne("document.lib.api.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("document.lib.api.Models.Register", "Register")
                        .WithMany("Documents")
                        .HasForeignKey("RegisterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("document.lib.api.Models.Register", b =>
                {
                    b.HasOne("document.lib.api.Models.Folder", "Folder")
                        .WithMany("Registers")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
