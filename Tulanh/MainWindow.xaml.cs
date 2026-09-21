using System.Windows;
using Tulanh.Views;

namespace Tulanh
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = new Home();
        }
        private void OpenFoodManagement_Click(
            object sender,
            RoutedEventArgs e)
        {
            MainContent.Content = new FoodManagement();
        }
        private void OpenHome_Click(
            object sender,
            RoutedEventArgs e)
        {
            MainContent.Content = new Home();
        }
    }
}