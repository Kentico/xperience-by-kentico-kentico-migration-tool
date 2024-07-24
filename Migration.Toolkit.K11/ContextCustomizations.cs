using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.K11;

public partial class K11Context
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .EnableDetailedErrors()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        base.OnConfiguring(optionsBuilder);
    }
}
