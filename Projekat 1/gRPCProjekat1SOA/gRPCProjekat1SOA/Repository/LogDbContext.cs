using Microsoft.EntityFrameworkCore;

namespace gRPCProjekat1SOA.Repository
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options) { }

        public virtual DbSet<LogVal> LogVals { get; set; } = null!;
    }
}
