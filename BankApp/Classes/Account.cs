using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApp.Enums;

namespace BankApp.Classes
{
    public class Account
    {
        public long AccountId { get; set; }
        public Enums.CurrencyType CurrencyType { get; set; }
        public decimal Balance { get; set; }
        public bool AccountLock { get; set; }
        public User User { get; set; }
    }
}
