using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Tulanh.Data;
using Tulanh.Models;

namespace Tulanh.Views
{
    public partial class FoodManagement : UserControl
    {
        ObservableCollection<Food> foods =
            new ObservableCollection<Food>();

        Food editingFood = null;

        public FoodManagement()
        {
            InitializeComponent();

            DatabaseHelper.CreateDatabase();

            FoodDataGrid.ItemsSource = foods;

            LoadFoods();

            dpImportDate.SelectedDate = DateTime.Today;
        }
        private void LoadFoods()
        {
            foods.Clear();

            var list = DatabaseHelper.GetFoods();

            foreach (Food food in list)
            {
                foods.Add(food);
            }
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
                return;

            Food food = GetFoodFromForm();

            DatabaseHelper.AddFood(food);

            LoadFoods();

            MessageBox.Show("Đã thêm thực phẩm!");

            ClearForm();
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (editingFood == null)
                return;

            if (!CheckInput())
                return;

            Food food = GetFoodFromForm();

            food.Id = editingFood.Id;

            DatabaseHelper.UpdateFood(food);

            LoadFoods();

            MessageBox.Show("Đã cập nhật thực phẩm!");

            editingFood = null;

            ClearForm();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            editingFood = null;

            ClearForm();
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            Food food = button.DataContext as Food;

            if (food == null)
                return;

            MessageBoxResult result =
                MessageBox.Show(
                    "Bạn có chắc muốn xóa thực phẩm này?",
                    "Xác nhận",
                    MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                DatabaseHelper.DeleteFood(food.Id);

                LoadFoods();

                MessageBox.Show("Đã xóa thực phẩm!");
            }
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            Food food = button.DataContext as Food;

            if (food == null)
                return;

            editingFood = food;

            txtName.Text = food.Name;

            txtQuantity.Text =
                food.Quantity.ToString();

            txtWeight.Text =
                food.Weight.ToString();

            txtFreshDays.Text =
                food.FreshDays.ToString();

            dpImportDate.SelectedDate =
                food.ImportDate;

            foreach (ComboBoxItem item in cbCategory.Items)
            {
                if (item.Content.ToString() == food.Category)
                {
                    cbCategory.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in cbCompartment.Items)
            {
                if (item.Content.ToString() == food.Compartment)
                {
                    cbCompartment.SelectedItem = item;
                    break;
                }
            }
            AddButtons.Visibility =
                Visibility.Collapsed;

            EditButtons.Visibility =
                Visibility.Visible;
        }
        private bool CheckInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên thực phẩm!");

                return false;
            }


            if (cbCategory.SelectedItem == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn phân loại!");

                return false;
            }


            if (cbCompartment.SelectedItem == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn ngăn chứa!");

                return false;
            }


            if (!int.TryParse(
                txtQuantity.Text,
                out int quantity))
            {
                MessageBox.Show(
                    "Số lượng không hợp lệ!");

                return false;
            }


            if (!double.TryParse(
                txtWeight.Text,
                out double weight))
            {
                MessageBox.Show(
                    "Trọng lượng không hợp lệ!");

                return false;
            }


            if (!int.TryParse(
                txtFreshDays.Text,
                out int freshDays))
            {
                MessageBox.Show(
                    "Số ngày tươi ngon không hợp lệ!");

                return false;
            }


            if (dpImportDate.SelectedDate == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn ngày nhập!");

                return false;
            }

            return true;
        }
        private Food GetFoodFromForm()
        {
            ComboBoxItem categoryItem =
                (ComboBoxItem)cbCategory.SelectedItem;

            ComboBoxItem compartmentItem =
                (ComboBoxItem)cbCompartment.SelectedItem;


            int.TryParse(
                txtQuantity.Text,
                out int quantity);

            double.TryParse(
                txtWeight.Text,
                out double weight);

            int.TryParse(
                txtFreshDays.Text,
                out int freshDays);


            Food food = new Food
            {
                Name = txtName.Text,
                Category = categoryItem.Content.ToString(),

                Compartment =  compartmentItem.Content.ToString(),
                Quantity = quantity,
                Weight = weight,
                ImportDate = dpImportDate.SelectedDate.Value,
                FreshDays = freshDays
            };

            return food;
        }
        private void ClearForm()
        {
            txtName.Clear();
            txtQuantity.Clear();
            txtWeight.Clear();
            txtFreshDays.Clear();
            cbCategory.SelectedIndex = -1;
            cbCompartment.SelectedIndex = -1;
            dpImportDate.SelectedDate =  DateTime.Today;
            AddButtons.Visibility =
                Visibility.Visible;

            EditButtons.Visibility =
                Visibility.Collapsed;
        }
    }
}