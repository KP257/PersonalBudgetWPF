using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace PersonalBudgetWPF
{
    /// <summary>
    /// Interaction logic for PurchaseProperty.xaml
    /// </summary>
    public partial class PurchaseProperty : Window
    {

        //Connection string
        string conString = "Data Source=DESKTOP-FT09O6T\\SQLEXPRESS;Initial Catalog=PersonalBudget;Integrated Security=True";

        public PurchaseProperty()
        {
            InitializeComponent();
        }

        public int HomeLoanRepayment()
        {
            // string text fields into decimal
            int purchaseProp = Convert.ToInt32(txtPurchasedPrice.Text),
                dep = Convert.ToInt32(txtPropertyDeposit.Text),
                rate = Convert.ToInt32(txtInterestRate.Text),
                months = Convert.ToInt32(txtPropertyMonths.Text),
                
                third,
                monthlyHomeLoan;

            string userName = Login.userName;

            // Calls abstract class override 
            // passes text box values into a method which is calculated and sent back
            MyLibrary.PurchaseProperty e = new MyLibrary.PurchaseProperty(purchaseProp, dep, rate, months);
            monthlyHomeLoan = e.GetAnswer();

            // Save details to database
            SqlConnection con = new SqlConnection(conString);

            // adding data to database table 
            con.Open();
            using (var cmd = new SqlCommand(@"SELECT GROSS_INCOME FROM USER_REG WHERE USER_NAMES = @USER_NAMES", con))
            {
                
                cmd.Parameters.AddWithValue("@USER_NAMES", userName);
                SqlDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                   int income = Convert.ToInt32(myReader["GROSS_INCOME"]);
                    //A third of users income for error message
                    third = income / 3;

                    // checks if monthly property costs is bigger than a third of users income
                    if (monthlyHomeLoan >= third)
                    {
                        MessageBox.Show("Approval of the home loan is unlikely", "Income to low");
                    }
                }
                

            }

            con.Close();

            

            // adding data to database table 
            con.Open();
            using (var cmd = new SqlCommand(@"INSERT INTO PROPERTY_PURCHASE (USER_NAMES, PURCHASE_PRICE, TOTAL_DEPOSIT, INTEREST_RATE, MONTHS, MONTHLY_REPAYMENT) VALUES (@USER_NAMES, @PURCHASE_PRICE, @TOTAL_DEPOSIT, @INTEREST_RATE, @MONTHS, @MONTHLY_REPAYMENT)", con))
            {
                cmd.Parameters.Add("@USER_NAMES", SqlDbType.VarChar).Value = userName; 
                cmd.Parameters.Add("@PURCHASE_PRICE", SqlDbType.Int).Value = purchaseProp;
                cmd.Parameters.Add("@TOTAL_DEPOSIT", SqlDbType.Int).Value = dep;
                cmd.Parameters.Add("@INTEREST_RATE", SqlDbType.Int).Value = rate;
                cmd.Parameters.Add("@MONTHS", SqlDbType.Int).Value = months;
                cmd.Parameters.Add("@MONTHLY_REPAYMENT", SqlDbType.Int).Value = monthlyHomeLoan;

                cmd.ExecuteNonQuery();
            }
            using (var newCmd = new SqlCommand(@"UPDATE PERSONAL_BUDGET SET MONTHLY_REPAYMENT = @MONTHLY_REPAYMENT Where USER_NAMES = @USER_NAMES", con))
            {
                newCmd.Parameters.AddWithValue("@USER_NAMES", userName);
                newCmd.Parameters.Add("@MONTHLY_REPAYMENT", SqlDbType.Int).Value = monthlyHomeLoan;
                newCmd.ExecuteNonQuery();

            }
            con.Close();

            // Displays monthly home loan repayment
            MessageBox.Show("Monthly Home Loan Repayment: R" + monthlyHomeLoan.ToString("#.##"), "Property Cost");

            //Opens up form
            this.Hide();
            var newForm = new UserMenu();

            newForm.Closed += (s, args) => this.Close();
            newForm.Show();

            // returns personal monthly expenses
            return monthlyHomeLoan;
        }

        private void BtnSaveProperty_Click(object sender, RoutedEventArgs e)
        {
            int numOfMonths = Convert.ToInt32(txtPropertyMonths.Text);

            if (numOfMonths >= 240 && numOfMonths <= 360)
            {
                HomeLoanRepayment();
            }
            else
                MessageBox.Show("Please enter the number of months between 240 and 360");
            {
            }
        }
    }
}
