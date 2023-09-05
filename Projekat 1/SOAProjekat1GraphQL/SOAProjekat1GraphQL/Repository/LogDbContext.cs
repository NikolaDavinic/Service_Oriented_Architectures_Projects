using Microsoft.EntityFrameworkCore;

namespace SOAProjekat1GraphQL.Repository
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<LogVal> LogVals { get; set; }

    }
}
