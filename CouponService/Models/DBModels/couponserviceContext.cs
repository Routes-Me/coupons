﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CouponService.Models.DBModels
{
    public partial class couponserviceContext : DbContext
    {
        public couponserviceContext()
        {
        }

        public couponserviceContext(DbContextOptions<couponserviceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Authorities> Authorities { get; set; }
        public virtual DbSet<Coupons> Coupons { get; set; }
        public virtual DbSet<Places> Places { get; set; }
        public virtual DbSet<Promotions> Promotions { get; set; }
        public virtual DbSet<PromotionsPlaces> PromotionsPlaces { get; set; }
        public virtual DbSet<Redemptions> Redemptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Authorities>(entity =>
            {
                entity.HasKey(e => e.AuthorityId)
                    .HasName("PRIMARY");

                entity.ToTable("authorities");

                entity.Property(e => e.AuthorityId).HasColumnName("authority_id");

                entity.Property(e => e.InstitutionId).HasColumnName("institution_id");

                entity.Property(e => e.Pin)
                    .HasColumnName("pin")
                    .HasColumnType("char(64)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Coupons>(entity =>
            {
                entity.HasKey(e => e.CouponId)
                    .HasName("PRIMARY");

                entity.ToTable("coupons");

                entity.HasIndex(e => e.PromotionId)
                    .HasName("promotion_id");

                entity.Property(e => e.CouponId).HasColumnName("coupon_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.PromotionId).HasColumnName("promotion_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.Coupons)
                    .HasForeignKey(d => d.PromotionId)
                    .HasConstraintName("coupons_ibfk_1");
            });

            modelBuilder.Entity<Places>(entity =>
            {
                entity.HasKey(e => e.PlaceId)
                    .HasName("PRIMARY");

                entity.ToTable("places");

                entity.Property(e => e.PlaceId).HasColumnName("place_id");

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasColumnType("decimal(10,8)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasColumnType("decimal(11,8)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Promotions>(entity =>
            {
                entity.HasKey(e => e.PromotionId)
                    .HasName("PRIMARY");

                entity.ToTable("promotions");

                entity.Property(e => e.PromotionId).HasColumnName("promotion_id");

                entity.Property(e => e.AdvertisementId).HasColumnName("advertisement_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.EndAt)
                    .HasColumnName("end_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.ExpieryDate)
                    .HasColumnName("expiery_date")
                    .HasColumnType("date");

                entity.Property(e => e.InstitutionId).HasColumnName("institution_id");

                entity.Property(e => e.IsSharable).HasColumnName("is_sharable");

                entity.Property(e => e.LogoUrl)
                    .HasColumnName("logo_url")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.QrCodeUrl)
                    .HasColumnName("qr_code_url")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.StartAt)
                    .HasColumnName("start_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Subtitle)
                    .HasColumnName("subtitle")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.UsageLimit).HasColumnName("usage_limit");
            });

            modelBuilder.Entity<PromotionsPlaces>(entity =>
            {
                entity.HasKey(e => new { e.PromotionId, e.PlaceId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("promotions_places");

                entity.HasIndex(e => e.PlaceId)
                    .HasName("place_id");

                entity.Property(e => e.PromotionId).HasColumnName("promotion_id");

                entity.Property(e => e.PlaceId).HasColumnName("place_id");

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.PromotionsPlaces)
                    .HasForeignKey(d => d.PlaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("promotions_places_ibfk_1");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionsPlaces)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("promotions_places_ibfk_2");
            });

            modelBuilder.Entity<Redemptions>(entity =>
            {
                entity.HasKey(e => e.RedemptionId)
                    .HasName("PRIMARY");

                entity.ToTable("redemptions");

                entity.HasIndex(e => e.CouponId)
                    .HasName("coupon_id");

                entity.Property(e => e.RedemptionId).HasColumnName("redemption_id");

                entity.Property(e => e.CouponId).HasColumnName("coupon_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.OfficerId).HasColumnName("officer_id");

                entity.HasOne(d => d.Coupon)
                    .WithMany(p => p.Redemptions)
                    .HasForeignKey(d => d.CouponId)
                    .HasConstraintName("redemptions_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}