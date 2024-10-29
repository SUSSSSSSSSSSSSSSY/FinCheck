using System.Text;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinCheck
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper _databaseHelper;
        private User _currentUser;

        public MainWindow(DatabaseHelper databaseHelper, User user)
        {
            InitializeComponent();
            _databaseHelper = databaseHelper;
            _currentUser = user;
            LoadWallets();
        }

        private void AddWallet_Click(object sender, RoutedEventArgs e)
        {
            AddWalletWindow addWalletWindow = new AddWalletWindow();

            if (addWalletWindow.ShowDialog() == true)
            {
                string walletName = addWalletWindow.WalletName;
                string walletCurrency = addWalletWindow.WalletCurrency;

                _databaseHelper.AddWallet(_currentUser.ID, walletName, walletCurrency);

                LoadWallets();
            }
        }

        private void LoadWallets()
        {
            WalletsListBox.Items.Clear();
            WalletsListBoxTransaction.Items.Clear();
            WalletsStatsListBox.Items.Clear();
            FromWallet.Items.Clear();
            ToWallet.Items.Clear();

            List<Wallet> wallets = _databaseHelper.GetWalletsForUser(_currentUser.ID);

            foreach (var wallet in wallets)
            {
                WalletsListBox.Items.Add(wallet.ToString());
                WalletsListBoxTransaction.Items.Add(wallet.ToString());
                FromWallet.Items.Add(wallet.ToString());
                FromWalletComboBox.Items.Add(wallet.ToString());
                WalletsStatsListBox.Items.Add(wallet.ToString());
            }

            List<Wallet> Allwallets = _databaseHelper.GetAllWallets();

            foreach (var wallet in Allwallets)
            {
                ToWallet.Items.Add(wallet.ToString());
            }
        }


        private void DeleteWallet_Click(object sender, RoutedEventArgs e)
        {
            Wallet selectedWallet = new Wallet (WalletsListBox.SelectedItem.ToString());

            if (selectedWallet != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить кошелек '{selectedWallet.WalletName}'?",
                                             "Подтвердите удаление", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    _databaseHelper.DeleteWallet(selectedWallet.ID);

                    LoadWallets();
                }
            }
            else
            {
                MessageBox.Show("Выберите кошелек для удаления.");
            }
        }



        private void FindTransactions_Click(object sender, RoutedEventArgs e)
        {
            TransactionsListBox.Items.Clear();

            Wallet selectedWallet = new Wallet(WalletsListBoxTransaction.SelectedItem.ToString());
            if (selectedWallet != null)
            {
                List<Transaction> transactions = _databaseHelper.GetTransactionsByWalletID(selectedWallet.ID);

                foreach (var transaction in transactions)
                {
                    TransactionsListBox.Items.Add(transaction.ToString());
                }
            }
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {

            Wallet selectedWallet = new Wallet(WalletsListBoxTransaction.SelectedItem.ToString());

            if (selectedWallet != null)
            {
                AddOrUpdateTransaction transaction = new AddOrUpdateTransaction();
                if(transaction.ShowDialog() == true)
                {
                    _databaseHelper.AddTransaction(selectedWallet.ID, transaction.Type, transaction.Category, transaction.Amount, transaction.Description, transaction.DateTime);
                    FindTransactions_Click(sender, e);
                }
            }
        }

        private void EditTransaction_Click(object sender, RoutedEventArgs e)
        {
            Wallet selectedWallet = new Wallet(WalletsListBoxTransaction.SelectedItem.ToString());
            Transaction selectedTransaction = new Transaction(TransactionsListBox.SelectedItem.ToString());
            if (selectedWallet != null && selectedTransaction != null)
            {
                AddOrUpdateTransaction transaction = new AddOrUpdateTransaction();
                if (transaction.ShowDialog() == true)
                {
                    _databaseHelper.UpdateTransaction(selectedTransaction.ID, selectedWallet.ID, transaction.Type, transaction.Category, transaction.Amount, transaction.Description, transaction.DateTime);
                }
            }
        }

        private void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            Transaction selectedTransaction = new Transaction(TransactionsListBox.SelectedItem.ToString());
            if (selectedTransaction != null)
            {
                _databaseHelper.DeleteTransaction(selectedTransaction.ID);
            }
        }

        private void ShowStats_Click(object sender, RoutedEventArgs e)
        {
            Wallet selWallet;
            try
            {
                if(WalletsStatsListBox.SelectedItem == null)
                {
                    throw new Exception("Вы не выбрали кошелёк");
                }
                selWallet = new Wallet(WalletsStatsListBox.SelectedItem.ToString());
            }
            catch (Exception ex) { MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }

            MessageBox.Show("Количество денег в кошельке: " + _databaseHelper.GetBalance(selWallet.ID).ToString() + selWallet.Currency);
        }

        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            var transactions = _databaseHelper.GetTransactionsForUser(_currentUser.ID);

            if (transactions.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта.", "Экспорт отчета");
                return;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Отчет_по_транзакциям",
                DefaultExt = ".csv",
                Filter = "CSV файлы (*.csv)|*.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var writer = new System.IO.StreamWriter(saveFileDialog.FileName))
                {
                    writer.WriteLine("Дата,Тип,Категория,Сумма,Описание");

                    foreach (var transaction in transactions)
                    {
                        writer.WriteLine($"{transaction.DateTime},{transaction.Type},{transaction.Category},{transaction.Amount},{transaction.Description}");
                    }
                }

                MessageBox.Show("Отчет успешно экспортирован.", "Экспорт отчета");
            }
        }


        private void ConvertCurrency_Click(object sender, RoutedEventArgs e)
        {
            Wallet _fromWallet;
            string _toCurrency;
            try
            {
                _fromWallet = new Wallet(FromWalletComboBox.SelectedItem.ToString());
                _toCurrency = (ToCurrencyComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                MessageBox.Show("FromCurrency: " + _fromWallet.Currency);
                MessageBox.Show("ToCurrency: " + _toCurrency);

            }
            catch (Exception ex) { MessageBox.Show("Не все поля выбраны"); return;  }

            decimal exchangeRateTO = 1m;
            decimal exchangeRateFROM = 1m;

            switch (_fromWallet.Currency)
            {
                case "USD":
                    exchangeRateFROM = 41.37m; break;
                case "EUR":
                    exchangeRateFROM = 44.61m; break;
                case "UAH":
                    exchangeRateFROM = 1m; break;
            }

            switch (_toCurrency.ToString())
            {
                case "USD":
                    exchangeRateTO = 41.37m; break;
                case "EUR":
                    exchangeRateTO = 44.61m; break;
                case "UAH":
                    exchangeRateTO = 1m; break;
            }

            _databaseHelper.UpdateWallet(_fromWallet.ID, _fromWallet.UserID, _fromWallet.WalletName, _toCurrency.ToString());

            List<Transaction> transactions = _databaseHelper.GetTransactionsByWalletID(_fromWallet.ID);

            foreach (var transaction in transactions)
            {
                
                _databaseHelper.UpdateTransaction(transaction.ID, transaction.WalletID, transaction.Type, transaction.Category, transaction.Amount*Convert.ToDecimal(exchangeRateFROM)/ Convert.ToDecimal(exchangeRateTO), transaction.Description, transaction.DateTime);
            }

            LoadWallets();

            MessageBox.Show("Конвертация удалась");
        }

        public async Task<(string updatedCurrency, decimal? exchangeRate)> ExchangeRateToUAH(string baseCurrency)
        {
            var currencyService = new CurrencyService();
            decimal? exchangeRate = await currencyService.GetExchangeRateToUAH(baseCurrency);

            string updatedCurrency = baseCurrency;

            return (updatedCurrency, exchangeRate);
        }



        private void ConfirmTransfer_Click(object sender, RoutedEventArgs e)
        {
            Wallet _fromWallet = new Wallet(FromWallet.SelectedItem.ToString());
            Wallet _toWallet = new Wallet(ToWallet.SelectedItem.ToString());
            decimal Amount;
            try
            {
                Amount = Convert.ToDecimal(AmountTextBoxWallet.Text);
                if(Amount <= 0) { throw new Exception(); }
                if(Amount > _databaseHelper.GetBalance(_fromWallet.ID)) {  throw new Exception(); }
            }
            catch { MessageBox.Show("Правильно введите количество"); return; }

            decimal exchangeRateTO = 1m;
            decimal exchangeRateFROM = 1m;

            switch (_fromWallet.Currency)
            {
                case "USD":
                    exchangeRateFROM = 41.37m; break;
                case "EUR":
                    exchangeRateFROM = 44.61m; break;
                case "UAH":
                    exchangeRateFROM = 1m; break;
            }

            switch (_toWallet.Currency)
            {
                case "USD":
                    exchangeRateTO = 41.37m; break;
                case "EUR":
                    exchangeRateTO = 44.61m; break;
                case "UAH":
                    exchangeRateTO = 1m; break;
            }

            _databaseHelper.AddTransaction(_fromWallet.ID, "Расход", "Перевод", Amount , "Перевод денег", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));



            _databaseHelper.AddTransaction(_toWallet.ID, "Доход", "Перевод", Amount*Convert.ToDecimal(exchangeRateFROM) /Convert.ToDecimal(exchangeRateTO), "Перевод денег", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            MessageBox.Show("Перевод удался");
        }
    }
}
