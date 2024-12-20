﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Web_Ban_Hang.Models;

#nullable disable

namespace Web_Ban_Hang.Migrations
{
    [DbContext(typeof(WebShopDbContext))]
    [Migration("20241121050059_m")]
    partial class m
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.35")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Web_Ban_Hang.Models.Cart", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.CartDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CartId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CartItemsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ItemsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.HasIndex("CartItemsId");

                    b.HasIndex("UserId");

                    b.ToTable("CartDetails");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.CartItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CartId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.OrderDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("OrderItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("orderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("productId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OrderItemId");

                    b.HasIndex("orderId");

                    b.HasIndex("productId");

                    b.ToTable("OrderDetail");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.OrderItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("OrderItem");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.Product", b =>
                {
                    b.Property<Guid>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ProductId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.CartDetail", b =>
                {
                    b.HasOne("Web_Ban_Hang.Models.Cart", "Carts")
                        .WithMany("CartDetails")
                        .HasForeignKey("CartId");

                    b.HasOne("Web_Ban_Hang.Models.CartItem", "CartItems")
                        .WithMany("Details")
                        .HasForeignKey("CartItemsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Web_Ban_Hang.Models.User", "Users")
                        .WithMany("CartDetails")
                        .HasForeignKey("UserId");

                    b.Navigation("CartItems");

                    b.Navigation("Carts");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.OrderDetail", b =>
                {
                    b.HasOne("Web_Ban_Hang.Models.OrderItem", null)
                        .WithMany("Details")
                        .HasForeignKey("OrderItemId");

                    b.HasOne("Web_Ban_Hang.Models.Order", "orders")
                        .WithMany("Details")
                        .HasForeignKey("orderId");

                    b.HasOne("Web_Ban_Hang.Models.Product", "products")
                        .WithMany("Details")
                        .HasForeignKey("productId");

                    b.Navigation("orders");

                    b.Navigation("products");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.Cart", b =>
                {
                    b.Navigation("CartDetails");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.CartItem", b =>
                {
                    b.Navigation("Details");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.Order", b =>
                {
                    b.Navigation("Details");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.OrderItem", b =>
                {
                    b.Navigation("Details");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.Product", b =>
                {
                    b.Navigation("Details");
                });

            modelBuilder.Entity("Web_Ban_Hang.Models.User", b =>
                {
                    b.Navigation("CartDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
