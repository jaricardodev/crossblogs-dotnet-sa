using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace crossblog.Domain
{
    public partial class CrossBlogDbContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public static readonly LoggerFactory MyLoggerFactory
    = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });

        public CrossBlogDbContext(DbContextOptions<CrossBlogDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>().HasOne(c => c.Article).WithMany().OnDelete(DeleteBehavior.SetNull);
        }
    }
}
