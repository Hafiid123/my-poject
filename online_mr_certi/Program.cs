using Microsoft.EntityFrameworkCore;
using online_mr_certi.Data;
using online_mr_certi.Infrastructure;
using online_mr_certi.Models;
using online_mr_certi.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<ManualPaymentOptions>(


    builder.Configuration.GetSection(ManualPaymentOptions.SectionName));

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".MarriageCert.Session";
});



var conn = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var serverVersion = ServerVersion.Parse(
    builder.Configuration["MySql:ServerVersion"] ?? "8.0.36-mysql");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(conn, serverVersion));

builder.Services.AddScoped<ICertificatePdfService, CertificatePdfService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    // RBAC safety sync: prevent accidental AccessDenied for valid staff roles.
    var existingPermissions = await db.Permissions
        .AsNoTracking()
        .ToDictionaryAsync(p => p.Name, p => p.Id);
    foreach (var permissionName in AppPermissions.All)
    {
        if (!existingPermissions.ContainsKey(permissionName))
            db.Permissions.Add(new Permission { Name = permissionName });
    }
    if (db.ChangeTracker.HasChanges())
        await db.SaveChangesAsync();

    var rolesByName = await db.Roles.ToDictionaryAsync(r => r.Name, r => r);
    if (!rolesByName.ContainsKey(AppRoles.User))
        db.Roles.Add(new Role { Name = AppRoles.User, CreatedAt = DateTime.UtcNow });
    if (!rolesByName.ContainsKey(AppRoles.Admin))
        db.Roles.Add(new Role { Name = AppRoles.Admin, CreatedAt = DateTime.UtcNow });
    if (db.ChangeTracker.HasChanges())
        await db.SaveChangesAsync();

    rolesByName = await db.Roles.ToDictionaryAsync(r => r.Name, r => r);
    existingPermissions = await db.Permissions
        .AsNoTracking()
        .ToDictionaryAsync(p => p.Name, p => p.Id);

    if (rolesByName.TryGetValue(AppRoles.Admin, out var adminRole))
    {
        var adminPermissionIds = await db.RolePermissions
            .Where(rp => rp.RoleId == adminRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();
        foreach (var permissionId in existingPermissions.Values)
        {
            if (!adminPermissionIds.Contains(permissionId))
                db.RolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = permissionId });
        }
    }

    // Every non-public role gets minimum Admin panel entry permission.
    if (existingPermissions.TryGetValue(AppPermissions.ViewDashboard, out var viewDashboardId))
    {
        var nonUserRoleIds = await db.Roles
            .Where(r => r.Name != AppRoles.User)
            .Select(r => r.Id)
            .ToListAsync();
        var roleIdsWithDashboard = await db.RolePermissions
            .Where(rp => rp.PermissionId == viewDashboardId)
            .Select(rp => rp.RoleId)
            .ToListAsync();

        foreach (var roleId in nonUserRoleIds)
        {
            if (!roleIdsWithDashboard.Contains(roleId))
                db.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = viewDashboardId });
        }
    }
    if (db.ChangeTracker.HasChanges())
        await db.SaveChangesAsync();

    if (app.Configuration.GetValue<bool>("SeedAdmin") &&
        !await db.Users.AnyAsync(u => u.Role == AppRoles.Admin))
    {
        var email = (app.Configuration["SeedAdminEmail"] ?? "admin@system.local").Trim().ToLowerInvariant();
        var name = app.Configuration["SeedAdminName"] ?? "System Administrator";
        var password = app.Configuration["SeedAdminPassword"] ?? "Admin@123";
        var adminRoleId = await db.Roles
            .Where(r => r.Name == AppRoles.Admin)
            .Select(r => (int?)r.Id)
            .FirstOrDefaultAsync();
        db.Users.Add(new User
        {
            Name = name,
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Role = AppRoles.Admin,
            RoleId = adminRoleId,
            PaymentStatus = UserPaymentStatuses.Unpaid
        });
        await db.SaveChangesAsync();
    }

    if (!await db.Fees.AnyAsync())
    {
        db.Fees.Add(new Fee
        {
            ServiceName = "Marriage Application",
            Amount = 10.00m,
            Currency = "USD",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    var feeRow = await db.Fees.AsNoTracking()
        .Where(f => f.IsActive)
        .OrderBy(f => f.Id)
        .FirstOrDefaultAsync();
    if (feeRow is not null)
    {
        var appIds = await db.MarriageApplications.Select(a => a.Id).ToListAsync();
        foreach (var aid in appIds)
        {
            if (!await db.Payments.AnyAsync(p => p.ApplicationId == aid))
            {
                var marriageApp = await db.MarriageApplications.AsNoTracking().FirstAsync(a => a.Id == aid);
                db.Payments.Add(new Payment
                {
                    ApplicationId = aid,
                    UserId = marriageApp.UserId,
                    FeeId = feeRow.Id,
                    Amount = feeRow.Amount,
                    PaymentStatus = PaymentStatuses.Pending,
                    PaymentDate = null,
                    ReceiptImage = null,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
        await db.SaveChangesAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
