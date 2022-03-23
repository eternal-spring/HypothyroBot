using HypothyroBot.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HypothyroBot
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Test> Tests { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        static ApplicationContext()
            => NpgsqlConnection.GlobalTypeMapper
            .MapEnum<DrugType>()
            .MapEnum<ModeType>()
            .MapEnum<GenderType>()
            .MapEnum<PathologyType>()
            .MapEnum<ThyroidType>();
        protected override void OnModelCreating(ModelBuilder builder)
            => builder.HasPostgresEnum<DrugType>()
            .HasPostgresEnum<ModeType>()
            .HasPostgresEnum<GenderType>()
            .HasPostgresEnum<PathologyType>()
            .HasPostgresEnum<ThyroidType>();
    }
}