using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinCheck
{
    public class Transaction
    {
        public int ID { get; set; }
        public int WalletID { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string DateTime {  get; set; }

        public Transaction(int iD, int walletID, string type, string category, decimal amount, string description, string dateTime)
        {
            ID = iD;
            WalletID = walletID;
            Type = type;
            Category = category;
            Amount = amount;
            Description = description;
            DateTime = dateTime;
        }

        public Transaction(string transactionString) 
        {
            var parts = transactionString.Split(' ', 8);

            if (parts.Length != 8)
            {
                throw new FormatException("Строка не соответствует ожидаемому формату: 'ID WalletID Type Category Amount DateTime Recurring Description'");
            }

            if (!int.TryParse(parts[0], out int id) || !int.TryParse(parts[1], out int walletID))
            {
                throw new FormatException("ID и WalletID должны быть целыми числами.");
            }

            ID = id;
            WalletID = walletID;

            Type = parts[2];
            Category = parts[3];

            if (!decimal.TryParse(parts[4], out decimal amount))
            {
                throw new FormatException("Amount должен быть числом с плавающей запятой.");
            }

            Amount = amount;

            DateTime = parts[5] + " " + parts[6];


            Description = parts[7];
        }

        public override string ToString()
        {
            return ID.ToString() + " " + WalletID.ToString() + " " + Type + " " + Category + " " + Amount.ToString() + " " + DateTime.ToString() + " " + Description;
        }
    }
}
