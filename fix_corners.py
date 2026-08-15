import re
import os

files = [
    'src/POSSystem.Desktop/Views/DashboardView.xaml',
    'src/POSSystem.Desktop/Views/PosView.xaml',
    'src/POSSystem.Desktop/Views/ProductsView.xaml',
    'src/POSSystem.Desktop/Views/RolePermissionsView.xaml',
    'src/POSSystem.Desktop/Views/ProductDialog.xaml',
    'src/POSSystem.Desktop/Views/SupervisorLoginDialog.xaml',
    'src/POSSystem.Desktop/Views/MainWindow.xaml',
    'src/POSSystem.Desktop/Views/LoginWindow.xaml',
]

for f in files:
    if os.path.exists(f):
        with open(f, 'r', encoding='utf-8') as file:
            content = file.read()
        content = re.sub(r'CornerRadius="[^"]+"', 'CornerRadius="0"', content)
        with open(f, 'w', encoding='utf-8') as file:
            file.write(content)
        print(f'Updated: {f}')
