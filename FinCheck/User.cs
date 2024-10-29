using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinCheck
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public User(int id, string username, string password)
        {
            ID = id;
            Username = username;
            Password = password;
        }

        public override string ToString()
        {
            string result = Username + " " + Password + " " + ID.ToString();
            return result;
        }
    }
}

