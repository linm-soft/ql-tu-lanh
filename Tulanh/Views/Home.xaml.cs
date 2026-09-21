using System.Windows;
using System.Windows.Controls;

namespace Tulanh.Views
{
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }

        private void OpenFoodManagement_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);

            if (window is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new FoodManagement();
            }
        }
    }
}