using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseReport
{
    class Transaction 
    {

        public Transaction()
        {

        }

        public Transaction(string memo, string type, string date, string category, decimal amount)
        {
            this.Memo = memo;
            this.Type = type;
            this.Date = DateTime.Parse(date);
            this.Category = category;
            this.Amount = amount;
            
        }
        public decimal Amount { get; set; }
        public String Memo { get; set; }
        public String Category { get; set; }
        public String Type { get; set; }
        public DateTime Date { get; set; }
        

    }
}
