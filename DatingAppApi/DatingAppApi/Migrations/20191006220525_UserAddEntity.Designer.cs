﻿// <auto-generated />
using System;
using DatingAppApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DatingAppApi.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20191006220525_UserAddEntity")]
    partial class UserAddEntity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099");

            modelBuilder.Entity("DatingAppApi.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Password");

                    b.Property<byte[]>("PaswordHash");

                    b.Property<byte[]>("PaswordSalt");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DatingAppApi.Data.Values", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Valor");

                    b.HasKey("Id");

                    b.ToTable("Values");
                });
#pragma warning restore 612, 618
        }
    }
}