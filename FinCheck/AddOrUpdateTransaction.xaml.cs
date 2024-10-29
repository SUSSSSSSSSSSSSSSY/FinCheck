using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinCheck
{
    /// <summary>
    /// Interaction logic for AddOrUpdateTransaction.xaml
    /// </summary>

    public partial class AddOrUpdateTransaction : Window
    {
        public string Type { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string DateTime { get; set; }

        public AddOrUpdateTransaction()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            MessageBox.Show(Type);
            Category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            MessageBox.Show(Category);
            try
            {
                Amount = Convert.ToDecimal(AmountTextBox.Text);
            }
            catch 
            {
                MessageBox.Show($"Произошла ошибка: Неправильное число", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show(Amount.ToString());

            DateTime = DateCreatedPicker.ToString();
            MessageBox.Show(DateTime);
            Description = DescriptionTextBox.Text;
            MessageBox.Show(Description);

            if (string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(Category) || !decimal.IsPositive(Amount) || string.IsNullOrEmpty(DateTime) || string.IsNullOrEmpty(Description))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
