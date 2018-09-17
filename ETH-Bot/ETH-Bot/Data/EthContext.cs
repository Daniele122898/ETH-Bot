using System;
using System.IO;
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
        
        //ScraperData
        public DbSet<AlgDat> AlgDat { get; set; }
        public DbSet<DiscMath> DiscMath { get; set; }
        public DbSet<Eprog> Eprog{ get; set; }
        public DbSet<LinAlg> LinAlg{ get; set; }
        public DbSet<Classes> Classes{ get; set; }
        
        public EthContext() : base()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySql();
            //return;
            
            if (!ConfigService.GetConfig().TryGetValue("connectionString", out var connectionString))
            {
                throw new IOException
                {
                    Source = "Couldn't find a \"connectionString\" entry in the config.json file. Exiting."
                };
            }

            optionsBuilder.UseMySql(connectionString);


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //create models
        }
    }
}