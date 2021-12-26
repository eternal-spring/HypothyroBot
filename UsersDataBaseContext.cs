using HypothyroBot.Models;
using Microsoft.EntityFrameworkCore;

namespace HypothyroBot
{
    public class UsersDataBaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public UsersDataBaseContext(DbContextOptions<UsersDataBaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}