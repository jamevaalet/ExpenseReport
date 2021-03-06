﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Collections.Specialized;

namespace ExpenseReport
{
    public partial class Form1 : Form
    {
        string filename;
        DataTable tbSource;
        List<Transaction> Transactions = new List<Transaction>();
      

        Configuration configFile = ConfigurationManager.OpenExeConfiguration(@"c:\users\jamev\source\repos\expensereport\expensereport\configuration.config");
        KeyValueConfigurationCollection appSettings;//= configFile.AppSettings.Settings;

        public Form1()
        {
            InitializeComponent();
            appSettings = configFile.AppSettings.Settings;
        }

        private void LoadExcel(string filename)
        {
            if(string.IsNullOrWhiteSpace(filename))
            {
                filename = @"C:\Users\jamevaa\Desktop\firsttech\ExportedTransactions.csv";
            }
            this.filename = filename;
            OleDbConnection conn = new OleDbConnection
                  ("Provider=Microsoft.Jet.OleDb.4.0; Data Source = " +
                    Path.GetDirectoryName(filename) +
                    "; Extended Properties = \"Text;HDR=YES;FMT=Delimited\"");

            conn.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter
                   ("SELECT * FROM " + Path.GetFileName(filename), conn);

            DataSet ds = new DataSet("Temp");
            adapter.Fill(ds);
            tbSource = ds.Tables[0];

            conn.Close();

            initializeTransactions();

        }

        private void InitializeCategory()
        {
            foreach (DataRow row in tbSource.Rows)
            {
                if (appSettings[row["Memo"].ToString()] == null)
                {
                    memoLabel.Name =row["Memo"].ToString();
                    AddUpdateAppSettings(row["Memo"].ToString(), row["Memo"].ToString());
                }

            }
        }

        private void initializeTransactions()
        {
           // Configuration configFile = ConfigurationManager.OpenExeConfiguration(@"C: \Users\jamevaa\Documents\Visual Studio 2015\Projects\ExpenseReport\ExpenseReport\configuration.config");
            

            foreach (DataRow row in tbSource.Rows)
            {
                if (!appSettings.AllKeys.Contains(row["Memo"].ToString()))
                {

                    AddUpdateAppSettings(row["Memo"].ToString(), "MISC");
                }
                var t = new Transaction()
                {
                    Memo = row["Memo"].ToString(),
                    Category= appSettings[row["Memo"].ToString()].Value,
                    Type = row["Transaction Type"].ToString(),
                    Date = Convert.ToDateTime(row["Effective Date"].ToString()),
                    Amount = Math.Abs(Convert.ToDecimal(row["Amount"].ToString()))
                };

                Transactions.Add(t);

            //    var csv = Transaction.GetCSV(Transactions);

            }

            var csv = Transaction.GetCSV(Transactions);
            var f = $"{Path.GetDirectoryName(filename)}\\Edit.csv";
            //File.Create(f);
            
            File.WriteAllText(f, csv);
            

            //List<string> values = new List<string>();
            //foreach(var key in appSettings.AllKeys)
            //{
            //    if(appSettings[key].Value != "")
            //    values.Add(appSettings[key].Value);
            //}

            //comboBox1.DataSource = values.Distinct().ToList();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            var masterTransactions = Transactions
                .GroupBy(x => x.Category)
                .Select(xg => new Transaction()
                {
                    Category = xg.First().Category,
                    Amount = xg.Sum(y => y.Amount)
                }).ToList();



            ExpenseChart.DataSource = masterTransactions;


            ExpenseChart.Series["Transactions"].XValueMember = "Category";
            ExpenseChart.Series["Transactions"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            ExpenseChart.Series["Transactions"].YValueMembers = "Amount";
            ExpenseChart.Series["Transactions"].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Auto;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadExcel(openFileDialog1.FileName);
            }

        }

   
        private void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var settings = configFile.AppSettings.Settings;
                if (!settings.AllKeys.Contains(key))
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            var masterTransactions = Transactions
                .GroupBy(x => x.Category)
                .Select(xg => new Transaction()
                {
                    Category = xg.First().Category,
                    Amount = xg.Sum(y => y.Amount)
                }).ToList();



            ExpenseChart.DataSource = masterTransactions;


            ExpenseChart.Series["Transactions"].XValueMember = "Category";
            ExpenseChart.Series["Transactions"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            ExpenseChart.Series["Transactions"].YValueMembers = "Amount";
            ExpenseChart.Series["Transactions"].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Auto;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var key = memoLabel.Text;
            var value = comboBox1.SelectedValue.ToString();

            if(key != "Memo")
            AddUpdateAppSettings(key, value);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in tbSource.Rows)
            {
                if (appSettings[row["Memo"].ToString()].Value == "")
                {
                    memoLabel.Text = row["Memo"].ToString();
                    return;
                }

            }
        }


        private void button5_Click(object sender, EventArgs e)
        {
            var key = memoLabel.Text;
            var value = textBox1.Text;

            if (key != "Memo" && value != "New")
                AddUpdateAppSettings(key, value);
        }
    }
}
