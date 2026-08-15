using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace POSSystem.Desktop.Views;

public partial class PromoProductsView : UserControl
{
    public PromoProductsView()
    {
        InitializeComponent();
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is ViewModels.PromoProductsViewModel vm && vm.SelectedPromoProduct != null)
        {
            vm.EditPromoCommand.Execute(vm.SelectedPromoProduct);
        }
    }
}
