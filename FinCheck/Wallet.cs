using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinCheck
{
    public class Wallet
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string WalletName { get; set; }
        public string Currency { get; set; }

        public Wallet(int id, int userID, string walletName, string currency)
        {
            ID = id;
            UserID = userID;
            WalletName = walletName;
            Currency = currency;
        }

        public Wallet(string walletInfo) 
        {
            var parts = walletInfo.Split(' ');

            if (parts.Length != 4)
            {
                throw new FormatException("Строка не соответствует ожидаемому формату: 'WalletName Currency ID UserID'");
            }

            WalletName = parts[0];
            Currency = parts[1];

            if (!int.TryParse(parts[2], out int id) || !int.TryParse(parts[3], out int userID))
            {
                throw new FormatException("ID и UserID должны быть целыми числами.");
            }

            ID = id;
            UserID = userID;
        }

        public override string ToString()
        {
            return WalletName + " " + Currency + " " + ID.ToString() + " " + UserID.ToString();
        }
    }
}

