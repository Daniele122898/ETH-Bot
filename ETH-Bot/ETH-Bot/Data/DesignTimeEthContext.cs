using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ETH_Bot.Data
{
    public class DesignTimeEthContext : IDesignTimeDbContextFactory<EthContext>
    {
        public EthContext CreateDbContext(string[] args)
        {
            return new EthContext(
                new DbContextOptions<EthContext>()
            );
        }
    }
}