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
    /// Interaction logic for Budget.xaml
    /// </summary>
    public partial class UsersPersonalExpense : Window
    {
        //Connection string
        string conString = "Data Source=DESKTOP-FT09O6T\\SQLEXPRESS;Initial Catalog=PersonalBudget;Integrated Security=True";

        public UsersPersonalExpense()
        {
            InitializeComponent();
           
        }

        // Personal Expenses with rent if chosen
        public int CalculatePersonalExpenses()
        {
            // string text fields into into 
            int tax = Convert.ToInt32(txtTaxedAmount.Text),
                groceries = Convert.ToInt32(txtGroceries.Text),
                waterAndLights = Convert.ToInt32(txtWaterAndLights.Text),
                travelCosts = Convert.ToInt32(txtTravelCosts.Text),
                phoneBill = Convert.ToInt32(txtPhoneBills.Text),
                other = Convert.ToInt32(txtOther.Text),
                monthlyPersonalExpenses;

            // declaration of rent
            int rent = 0;
            string userName = Login.userName;
            string selectedRent = "False";

            // Save details to database
            SqlConnection con = new SqlConnection(conString);

            // Calls abstract class override 
            // passes text box values into a method which is calculated and sent back
            MyLibrary.PersonalExpenses usersExpenses = new MyLibrary.PersonalExpenses(tax, groceries, waterAndLights, travelCosts, phoneBill, other);
            monthlyPersonalExpenses = usersExpenses.GetAnswer();

            //Checks is user has selected rent or not
            if (checkedRentYes.IsChecked == true)
            {
                // declaration of rent
                rent = Convert.ToInt32(txtRent.Text);

                selectedRent = "True";

                 // adds rent to monthly personal cost
                 monthlyPersonalExpenses += rent;

            }

            // adding data to database table 
            con.Open();
            using (var cmd = new SqlCommand(@"INSERT INTO PERSONAL_EXPENSES (USER_NAMES, TAXED_AMOUNT, GROCERIES, TRAVEL_COSTS, PHONE_BILLS, WATER_AND_LIGHTS, OTHER, RENT_SELECTED, RENT, TOTAL_PERSONAL_EXPENSES) VALUES (@USER_NAMES, @TAXED_AMOUNT, @GROCERIES, @TRAVEL_COSTS, @PHONE_BILLS, @WATER_AND_LIGHTS, @OTHER, @RENT_SELECTED, @RENT, @TOTAL_PERSONAL_EXPENSES)", con))
            {
                cmd.Parameters.Add("@USER_NAMES", SqlDbType.VarChar).Value = userName;
                cmd.Parameters.Add("@TAXED_AMOUNT", SqlDbType.Int).Value = tax;
                cmd.Parameters.Add("@GROCERIES", SqlDbType.Int).Value = groceries;
                cmd.Parameters.Add("@TRAVEL_COSTS", SqlDbType.Int).Value = travelCosts;
                cmd.Parameters.Add("@PHONE_BILLS", SqlDbType.Int).Value = phoneBill;
                cmd.Parameters.Add("@WATER_AND_LIGHTS", SqlDbType.Int).Value = waterAndLights;
                cmd.Parameters.Add("@OTHER", SqlDbType.Int).Value = other;
                cmd.Parameters.Add("@RENT_SELECTED", SqlDbType.VarChar).Value = selectedRent;
                cmd.Parameters.Add("@RENT", SqlDbType.Int).Value = rent;
                cmd.Parameters.Add("@TOTAL_PERSONAL_EXPENSES", SqlDbType.Int).Value = monthlyPersonalExpenses;
               
                cmd.ExecuteNonQuery();

            }
            // Updating personal_budget column matching the user
            using (var newCmd = new SqlCommand(@"UPDATE PERSONAL_BUDGET SET MONTHLY_PERSONAL_EXPENSES = @MONTHLY_PERSONAL_EXPENSES Where USER_NAMES = @USER_NAMES", con))
            {
                newCmd.Parameters.AddWithValue("@USER_NAMES", userName);
                newCmd.Parameters.Add("@MONTHLY_PERSONAL_EXPENSES", SqlDbType.Int).Value = monthlyPersonalExpenses;
                newCmd.ExecuteNonQuery();

            }
            con.Close();


            //Displays mersonal expenses before purchasing property or a car
            MessageBox.Show("Monthly Personal Expenses: R" + monthlyPersonalExpenses.ToString("#.##"));

            //Opens up form
            this.Hide();
            var newForm = new UserMenu();

            newForm.Closed += (s, args) => this.Close();
            newForm.Show();

            // returns personal monthly expenses
            return monthlyPersonalExpenses;

           

        }

        // Ssaving details button
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            CalculatePersonalExpenses();
        }

        // chekcbox on check methods
        private void checkedRentYes_Checked(object sender, RoutedEventArgs e)
        {
            checkedRentNo.IsChecked = false;
            txtRent.Visibility = Visibility.Visible;
            labelRent.Visibility = Visibility.Visible;
        }

        private void checkedRentNo_Checked(object sender, RoutedEventArgs e)
        {
            checkedRentYes.IsChecked = false;
            txtRent.Visibility = Visibility.Collapsed;
            labelRent.Visibility = Visibility.Collapsed;
        }
    }
}
