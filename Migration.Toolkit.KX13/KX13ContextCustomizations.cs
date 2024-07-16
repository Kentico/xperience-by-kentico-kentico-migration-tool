using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KX13.Context;

public partial class KX13Context
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .EnableDetailedErrors()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        base.OnConfiguring(optionsBuilder);
    }
}
