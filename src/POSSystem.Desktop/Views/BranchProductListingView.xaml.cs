using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace POSSystem.Desktop.Views;

public partial class BranchProductListingView : UserControl
{
    public BranchProductListingView()
    {
        InitializeComponent();
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is ViewModels.BranchProductListingViewModel vm)
        {
            vm.SavePriceCommand.Execute(null);
        }
    }
}
