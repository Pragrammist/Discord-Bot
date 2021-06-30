using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
namespace OCHKO
{
    public class DiscordUser
    {
        public ulong Id { get; set; } //id is discordId
        public status OptionalStatus { get; set; } //status that player set
    }
    public class DiscrodUsersContext : DbContext
    {
        public DbSet<DiscordUser> Users { get; set; }
        public DiscrodUsersContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=DiscordUsers;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiscordUser>().Property(u => u.Id).ValueGeneratedNever();
            modelBuilder.Entity<DiscordUser>().Property(u => u.OptionalStatus).HasDefaultValue(status.ingame);
            base.OnModelCreating(modelBuilder);
        }
    }
}
