using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace online_mr_certi.Data;

/// <summary>Design-time factory so <c>dotnet ef</c> does not need to auto-detect server version against a live database.</summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var conn = Environment.GetEnvironmentVariable("MARRIAGE_CERT_DESIGN_CONN")
            ?? "Server=127.0.0.1;Port=3306;Database=marriage_cert_db;User=root;Password=;";
        optionsBuilder.UseMySql(conn, ServerVersion.Parse("8.0.36-mysql"));
        return new AppDbContext(optionsBuilder.Options);
    }
}
