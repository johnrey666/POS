using System.Windows.Controls;
using System.Windows.Input;
using POSSystem.Desktop.ViewModels;

namespace POSSystem.Desktop.Views;

public partial class ProductsView : UserControl
{
    public ProductsView()
    {
        InitializeComponent();
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is ProductsViewModel vm && vm.SelectedProduct != null)
        {
            vm.ViewProductCommand.Execute(vm.SelectedProduct);
        }
    }
}