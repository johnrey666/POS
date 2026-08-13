//AppServices.cs
using POSSystem.Domain.Services;
using POSSystem.Infrastructure.Services;

namespace POSSystem.Desktop;

public static class AppServices
{
    public static IAuthService Auth { get; } = new AuthService();
    public static IAuthorizationService Authorization { get; } = new AuthorizationService(Auth);
    public static IPermissionAdminService PermissionAdmin { get; } = new PermissionAdminService(Auth, Authorization);
    public static IProductCatalogService ProductCatalog { get; } = new ProductCatalogService();
    public static IProductManagementService ProductManagement { get; } = new ProductManagementService();
    public static IBranchService Branches { get; } = new BranchService();
}
