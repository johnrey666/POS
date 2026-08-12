namespace POSSystem.Domain.Security;

public static class PermissionCodes
{
    public const string DashboardView = "dashboard.view";
    public const string PosAccess = "pos.access";

    public const string SalesCreate = "sales.create";
    public const string SalesViewOwn = "sales.view_own";
    public const string SalesViewBranch = "sales.view_branch";
    public const string SalesViewAll = "sales.view_all";
    public const string SalesHold = "sales.hold";
    public const string SalesVoid = "sales.void";
    public const string SalesRefund = "sales.refund";
    public const string SalesReprint = "sales.reprint";

    public const string DiscountsApply = "discounts.apply";
    public const string DiscountsApprove = "discounts.approve";

    public const string ReportsView = "reports.view";

    public const string ProductsView = "products.view";
    public const string ProductsEdit = "products.edit";

    public const string UsersManage = "users.manage";
    public const string RolesManage = "roles.manage";
    public const string PermissionsManage = "permissions.manage";

    public const string SettingsManage = "settings.manage";

    public static IReadOnlyList<(string Code, string Name, string Category, string Description)> All { get; } =
    [
        (DashboardView, "View Dashboard", "General", "Access the admin dashboard"),
        (PosAccess, "Access POS", "POS", "Open the point-of-sale terminal"),
        (SalesCreate, "Create Sales", "Sales", "Process new sales transactions"),
        (SalesViewOwn, "View Own Sales", "Sales", "View own transaction history"),
        (SalesViewBranch, "View Branch Sales", "Sales", "View sales for assigned branch"),
        (SalesViewAll, "View All Sales", "Sales", "View sales across all branches"),
        (SalesHold, "Hold & Resume Sales", "Sales", "Hold and resume in-progress sales"),
        (SalesVoid, "Void Sales", "Sales", "Void completed sales"),
        (SalesRefund, "Refund Sales", "Sales", "Process sale refunds"),
        (SalesReprint, "Reprint Receipts", "Sales", "Reprint transaction receipts"),
        (DiscountsApply, "Apply Discounts", "Discounts", "Apply permitted discounts"),
        (DiscountsApprove, "Approve Discounts", "Discounts", "Approve supervisor-level discounts"),
        (ReportsView, "View Reports", "Reports", "Access sales and operational reports"),
        (ProductsView, "View Products", "Products", "View product catalog"),
        (ProductsEdit, "Edit Products", "Products", "Create and edit products"),
        (UsersManage, "Manage Users", "Administration", "Create and manage user accounts"),
        (RolesManage, "Manage Roles", "Administration", "Create and manage roles"),
        (PermissionsManage, "Manage Permissions", "Administration", "Assign permissions to roles"),
        (SettingsManage, "Manage Settings", "Administration", "Change system settings"),
    ];
}
