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
        
        //public void initialize()
        //{
        //    if(this.Memo)
        //}

        public static string GetCSV(List<Transaction> tl)
        {
            string csv = "Month, Category, Amount"+Environment.NewLine;
            tl.ForEach(x =>
            {
                var month = x.Date.Month + "/" + x.Date.Year;
                x.Category= x.Category.Trim(new char[] { ',' });
                var amount = x.Type == "Debit" ?x.Amount*-1 : x.Amount;
                csv += $"{month}, {x.Category}, {amount}{Environment.NewLine}";
            });

            return csv;
        }
    }
}
