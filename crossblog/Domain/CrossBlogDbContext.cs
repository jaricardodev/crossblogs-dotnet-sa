using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace crossblog.Domain
{
    public partial class CrossBlogDbContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public CrossBlogDbContext(DbContextOptions<CrossBlogDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>().HasOne(c => c.Article).WithMany().OnDelete(DeleteBehavior.SetNull);
        }
    }
}
