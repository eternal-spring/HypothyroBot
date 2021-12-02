using HypothyroBot.Models;
using Microsoft.EntityFrameworkCore;

namespace HypothyroBot
{
    public class DataBaseContext : DbContext
    {
        public static DataBaseContext Me;
        public DbSet<User> Users { get; set; }
        public DataBaseContext(DbContextOptions<DataBaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}