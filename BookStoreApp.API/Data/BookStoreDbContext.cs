using System;
using System.Collections.Generic;
using BookStoreApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Data;

public partial class BookStoreDbContext : IdentityDbContext<ApiUser>
{
    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) : base(options){    }

    public virtual DbSet<Author> Authors { get; set; }
    public virtual DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Authors__3214EC077F062FDD");
            entity.Property(e => e.Bio).HasMaxLength(250);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Books__3214EC0720D153C0");
            entity.HasIndex(e => e.Isbn, "UQ__Books__447D36EA688317B2").IsUnique();
            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.Image).HasMaxLength(50);
            entity.Property(e => e.Isbn)
                .HasMaxLength(50)
                .HasColumnName("ISBN");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Summary).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_Books_ToTable");
        });

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "13c8c669-4782-4cd7-a15a-2926b9ef33bc",
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole
            {
                Id = "ae9ca92c-e19c-4277-bfa4-2fed2c30837c",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            }
        );

        var Hasher = new PasswordHasher<ApiUser>();

        modelBuilder.Entity<ApiUser>().HasData(
            new ApiUser
            {
                Id = "be5f6c59-8395-4b2b-b96f-54a9e1bbfd11",
                Email = "nickpsal@gmail.com",
                NormalizedEmail = "NICKPSAL@GMAIL.COM",
                UserName = "nickpsal@gmail.com",
                NormalizedUserName = "NICKPSAL@GMAIL.COM",
                FirstName = "Nick",
                LastName = "PSal",
                PasswordHash = Hasher.HashPassword(new ApiUser(), "gbrCBRM2908")
            },
            new ApiUser
            {
                Id = "c3ec77b4-3df1-43d2-8067-42b8458e9a10",
                Email = "user@gmail.com",
                NormalizedEmail = "USER@GMAIL.COM",
                UserName = "user@gmail.com",
                NormalizedUserName = "USER@GMAIL.COM",
                FirstName = "User",
                LastName = "User",
                PasswordHash = Hasher.HashPassword(new ApiUser(), "1234567890")
            }
        );

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = "ae9ca92c-e19c-4277-bfa4-2fed2c30837c",
                UserId = "be5f6c59-8395-4b2b-b96f-54a9e1bbfd11"
            },
            new IdentityUserRole<string>
            {
                RoleId = "13c8c669-4782-4cd7-a15a-2926b9ef33bc",
                UserId = "c3ec77b4-3df1-43d2-8067-42b8458e9a10"
            }
        );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
