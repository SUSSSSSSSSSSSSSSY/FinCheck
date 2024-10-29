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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows;

    public partial class LoginWindow : Window
    {
        private readonly DatabaseHelper _dbHelper;

        public LoginWindow()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
            _dbHelper.InitializeDatabase();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }

            _dbHelper.AddUser(username, password);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            User user = _dbHelper.GetUser(username, password);


            if (user != null)
            {
                MessageBox.Show("Login successful!");
                MainWindow main = new MainWindow(_dbHelper, user);
                main.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Invalid credentials");
            }
        }
    }
}
