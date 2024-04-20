﻿// <auto-generated />
using CodigoDelSurApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CodigoDelSurApi.Infrastructure.Migrations
{
    [DbContext(typeof(CodigoDelSurDbContext))]
    [Migration("20240420150324_Update_DB_1")]
    partial class Update_DB_1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.18");

            modelBuilder.Entity("CodigoDelSurApi.Infrastructure.DataEntities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UserPreferences", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Genre")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("UserPreferences");
                });

            modelBuilder.Entity("UserPreferences", b =>
                {
                    b.HasOne("CodigoDelSurApi.Infrastructure.DataEntities.User", "User")
                        .WithOne("Preferences")
                        .HasForeignKey("UserPreferences", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CodigoDelSurApi.Infrastructure.DataEntities.User", b =>
                {
                    b.Navigation("Preferences")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
