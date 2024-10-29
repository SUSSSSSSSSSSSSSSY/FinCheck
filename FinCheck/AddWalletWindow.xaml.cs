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
    public partial class AddWalletWindow : Window
    {
        public string WalletName { get; private set; }
        public string WalletCurrency { get; private set; }

        public AddWalletWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            WalletName = WalletNameTextBox.Text;
            WalletCurrency = (CurrencyComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(WalletName) || string.IsNullOrEmpty(WalletCurrency))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}