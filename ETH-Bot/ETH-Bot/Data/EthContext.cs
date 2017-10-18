using System;
using ETH_Bot.Data.Entities;
using ETH_Bot.Data.Entities.SubEntities;
using ETH_Bot.Services;
using Microsoft.EntityFrameworkCore;

namespace ETH_Bot.Data
{
    public class EthContext : DbContext
    {
        //User Database
        public DbSet<User> Users { get; set; }
        public DbSet<Reminder> Reminders{ get; set; }
        
        public EthContext(DbContextOptions<EthContext> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(optionsBuilder.IsConfigured)
                return;

            optionsBuilder.UseMySql(ConfigService.LazyGet("connectionString", true));
        }

        protected override Void OnModelCreating(ModelBuilder modelBuilder)
        {
            //create models
        }
    }
}