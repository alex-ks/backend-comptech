using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Comptech.Backend.Data;

namespace Comptech.Backend.Data.Migrations
{
    [DbContext(typeof(PsqlContext))]
    [Migration("20170207012512_DataPsql")]
    partial class DataPsql
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("Comptech.Backend.Data.DbEntities.DbPhoto", b =>
                {
                    b.Property<int>("PhotoId")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Image")
                        .IsRequired();

                    b.Property<int>("SessionId");

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("PhotoId");

                    b.HasIndex("SessionId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("Comptech.Backend.Data.DbEntities.DbPulse", b =>
                {
                    b.Property<int>("SessionId");

                    b.Property<DateTime>("timestamp");

                    b.Property<int>("Bpm");

                    b.HasKey("SessionId", "timestamp");

                    b.ToTable("Pulse");
                });

            modelBuilder.Entity("Comptech.Backend.Data.DbEntities.DbResult", b =>
                {
                    b.Property<int>("PhotoId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsValid");

                    b.Property<int?>("X1");

                    b.Property<int?>("X2");

                    b.Property<int?>("Y1");

                    b.Property<int?>("Y2");

                    b.HasKey("PhotoId");

                    b.ToTable("Results");
                });

            modelBuilder.Entity("Comptech.Backend.Data.DbEntities.DbSession", b =>
                {
                    b.Property<int>("SessionId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ExpiresAt");

                    b.Property<DateTime>("Start");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<int>("UserId");

                    b.HasKey("SessionId");

                    b.HasIndex("UserId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("Comptech.Backend.Data.DbEntities.DbUser", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Login");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Comptech.Backend.Data.DbEntities.DbPhoto", b =>
                {
                    b.HasOne("Comptech.Backend.Data.DbEntities.DbResult")
                        .WithOne("Photo")
                        .HasForeignKey("Comptech.Backend.Data.DbEntities.DbPhoto", "PhotoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Comptech.Backend.Data.DbEntities.DbSession", "Session")
                        .WithMany()
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Comptech.Backend.Data.DbEntities.DbPulse", b =>
                {
                    b.HasOne("Comptech.Backend.Data.DbEntities.DbSession", "Session")
                        .WithMany()
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Comptech.Backend.Data.DbEntities.DbSession", b =>
                {
                    b.HasOne("Comptech.Backend.Data.DbEntities.DbUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
