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
    /// Interaction logic for PurchaseVehicle.xaml
    /// </summary>
    public partial class PurchaseVehicle : Window
    {

        //Connection string
        string conString = "Data Source=DESKTOP-FT09O6T\\SQLEXPRESS;Initial Catalog=PersonalBudget;Integrated Security=True";

        public PurchaseVehicle()
        {
            InitializeComponent();
        }


        //Purchasing a car
        public int PurchaseCar()
        {
            //Model and make variable 
            string modelAndMake = txtMakeAndModel.Text;

            // string text fields into decimal
            int carPurchase = Convert.ToInt32(txtVehiclePrice.Text),
                carDeposit = Convert.ToInt32(txtVehicleDeposit.Text),
                carRate = Convert.ToInt32(txtVehicleRate.Text),
                insurance = Convert.ToInt32(txtCarInsurance.Text),
                monthlyCarCost;

            int months = 60;

            string userName = Login.userName;

            // Save details to database
            SqlConnection con = new SqlConnection(conString);

            // Calls abstract class override 
            // passes text box values into a method which is calculated and sent back
            MyLibrary.PurchaseVehicle e = new MyLibrary.PurchaseVehicle(carPurchase, carDeposit, carRate, insurance);
            monthlyCarCost = e.GetAnswer();


            // adding data to database table 
            con.Open();
            using (var cmd = new SqlCommand(@"INSERT INTO PURCHASE_CAR (USER_NAMES, MODEL_AND_MAKE, PURCHASE_PRICE , TOTAL_DEPOSIT, INTEREST_RATE, INSURANCE_PREMIUM, MONTHS,  MONTHLY_CAR_REPAYMENT) VALUES (@USER_NAMES, @MODEL_AND_MAKE, @PURCHASE_PRICE, @TOTAL_DEPOSIT, @INTEREST_RATE, @INSURANCE_PREMIUM, @MONTHS, @MONTHLY_CAR_REPAYMENT)", con))
            {
                cmd.Parameters.Add("@USER_NAMES", SqlDbType.VarChar).Value = userName;
                cmd.Parameters.Add("@MODEL_AND_MAKE", SqlDbType.VarChar).Value = modelAndMake;
                cmd.Parameters.Add("@PURCHASE_PRICE", SqlDbType.Int).Value = carPurchase;
                cmd.Parameters.Add("@TOTAL_DEPOSIT", SqlDbType.Int).Value = carDeposit;
                cmd.Parameters.Add("@INTEREST_RATE", SqlDbType.Int).Value = carRate;
                cmd.Parameters.Add("@INSURANCE_PREMIUM", SqlDbType.Int).Value = insurance;
                cmd.Parameters.Add("@MONTHS", SqlDbType.Int).Value = months;
                cmd.Parameters.Add("@MONTHLY_CAR_REPAYMENT", SqlDbType.Int).Value = monthlyCarCost;

                cmd.ExecuteNonQuery();
            }
            using (var newCmd = new SqlCommand(@"UPDATE PERSONAL_BUDGET SET MONTHLY_CAR_REPAYMENT = @MONTHLY_CAR_REPAYMENT Where USER_NAMES = @USER_NAMES", con))
            {
                newCmd.Parameters.AddWithValue("@USER_NAMES", userName);
                newCmd.Parameters.Add("@MONTHLY_CAR_REPAYMENT", SqlDbType.Int).Value = monthlyCarCost;
                newCmd.ExecuteNonQuery();

            }
            con.Close();



            // Displaying how much the user will pay per month for car expenses
            MessageBox.Show("Car Model and Make: " + modelAndMake + " \n Monthly Car Costs: R" + monthlyCarCost.ToString("#.##"), "Monthly Car Cost");

            //Opens up form
            this.Hide();
            var newForm = new UserMenu();

            newForm.Closed += (s, args) => this.Close();
            newForm.Show();

            // returns  monthly expenses
            return monthlyCarCost;

        }




        private void BtnSaveCar_Click(object sender, RoutedEventArgs e)
        {
            PurchaseCar();
        }
    }
}
