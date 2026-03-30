using Infrastructure.Identity.Data;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Data;

namespace Infrastructure.Data;

public static class InfrastructureInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        // initialize database
        await PersistenceInitializer.InitializeDatabaseAsync(serviceProvider);

        // initialize default identity roles
        await IdentityInitializer.InitilizeDefaultRolesAsync(serviceProvider);

        // initialize default user accounts
        await IdentityInitializer.InitilizeDefaultAdminAccountsAsync(serviceProvider);
    }
}
