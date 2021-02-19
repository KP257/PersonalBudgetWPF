using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PersonalBudgetWPF
{
    /// <summary>
    /// Interaction logic for ViewExpenses.xaml
    /// </summary>
    public partial class ViewExpenses : Window
    {
        //Connection string
        string conString = "Data Source=DESKTOP-FT09O6T\\SQLEXPRESS;Initial Catalog=PersonalBudget;Integrated Security=True";
        string userName = Login.userName;

        int grossIncome, personalExpense, propertyExpense, vehicleExpense, amountRemaining, totalExpenses;

        // Compiles when the windows launches
        public ViewExpenses()
        {
            InitializeComponent();
            Loaded += UserExpenes_Load;
        }

        // Checking if the user is returning to re check their expenses
        private void UserExpenes_Load(object sender, RoutedEventArgs e)
        {
            int amount;

            // MULTI-THREADING
            Thread checkProperty = new Thread(CheckPropertyCost);
            Thread checkVehicle = new Thread(CheckVehiclePurchase); 
            Thread ConvertOne = new Thread(ConvertNullsToZeros);
            Thread ConvertTwo = new Thread(ConvertNullsToZerosTwo);

            Thread loadExpenses = new Thread(LoadExpenses);
            Thread loadProperty = new Thread(LoadProperty);
            Thread loadVehicle = new Thread(LoadVehicle);


            checkProperty.Start();
            checkVehicle.Start();
            ConvertOne.Start();
            ConvertTwo.Start();

            //Opening and retrieving data from database
            SqlConnection con = new SqlConnection(conString);
            con.Open();

            // checks if the value is 0 to decide if it is a new user or not
            SqlCommand cmd = new SqlCommand("Select AMOUNT_REMAINING from PERSONAL_BUDGET where USER_NAMES = @USER_NAMES", con);

            cmd.Parameters.AddWithValue("@USER_NAMES", userName);
            SqlDataReader myReader = cmd.ExecuteReader();
            while (myReader.Read())
            {
                try
                {
                    amount = Convert.ToInt32(myReader["AMOUNT_REMAINING"]);
                }
                catch (Exception)
                {
                    amount = 0;
                }
               
                if (amount == 0)
                {
                    CreateExpenses();
                    loadExpenses.Start();
                    loadProperty.Start();
                    loadVehicle.Start();
                }
                else
                {
                    loadExpenses.Start();
                    loadProperty.Start();
                    loadVehicle.Start();
                }
            }
        }

        // places users expenses into personal_budget table in sql
        public void CreateExpenses()
        {
            MyLibrary.FetchAmounts amounts = new MyLibrary.FetchAmounts();

            // Save details to database
            SqlConnection con = new SqlConnection(conString);

            // adding data to database table 
            con.Open();
            using (var cmd = new SqlCommand(@"SELECT GROSS_MONTHLY,MONTHLY_PERSONAL_EXPENSES,MONTHLY_REPAYMENT,MONTHLY_CAR_REPAYMENT FROM PERSONAL_BUDGET WHERE USER_NAMES = @USER_NAMES", con))
            {
                cmd.Parameters.AddWithValue("@USER_NAMES", userName);
                SqlDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    grossIncome = Convert.ToInt32(myReader["GROSS_MONTHLY"]);
                    personalExpense = Convert.ToInt32(myReader["MONTHLY_PERSONAL_EXPENSES"]);
                    propertyExpense = Convert.ToInt32(myReader["MONTHLY_REPAYMENT"]);
                    vehicleExpense = Convert.ToInt32(myReader["MONTHLY_CAR_REPAYMENT"]);

                    totalExpenses = amounts.MonthlyExpenses(personalExpense, propertyExpense, vehicleExpense);
                    amountRemaining = amounts.AmountRemaining(grossIncome, totalExpenses);
                }
                
            }
            con.Close();

            // adding data to database table 
            con.Open();
            using (var cmd = new SqlCommand(@"UPDATE PERSONAL_BUDGET SET MONTHLY_EXPENSES = @MONTHLY_EXPENSES, AMOUNT_REMAINING = @AMOUNT_REMAINING WHERE USER_NAMES = @USER_NAMES", con))
            {
                cmd.Parameters.AddWithValue("@USER_NAMES", userName);
                cmd.Parameters.Add("@MONTHLY_EXPENSES", SqlDbType.Int).Value = totalExpenses;
                cmd.Parameters.Add("@AMOUNT_REMAINING", SqlDbType.Int).Value = amountRemaining;

                cmd.ExecuteNonQuery();

            }
            con.Close();



        }

        // Loads users data into the list
        public void LoadExpenses() 
        {
            // Save details to database
            SqlConnection con = new SqlConnection(conString);

            // adding data to database table 
            con.Open();
            using (var cmd = new SqlCommand(@"SELECT * FROM PERSONAL_BUDGET WHERE USER_NAMES = @USER_NAMES", con))
            {
                cmd.Parameters.AddWithValue("@USER_NAMES", userName);

                DataTable dt = new DataTable();
                //Fills datatable
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                //populates rows and populats the listbox
                foreach (DataRow dr in dt.Rows)
                {
                    // writes all to listbox
                    overView.Items.Add("Your Income per month: R" + dr["GROSS_MONTHLY"].ToString());
                    overView.Items.Add("Your total monthly Personal expense: R" + dr["MONTHLY_PERSONAL_EXPENSES"].ToString());
                    overView.Items.Add("How much you have to pay each month for property: R" + dr["MONTHLY_REPAYMENT"].ToString());
                    overView.Items.Add("How much you have to pay each month for a car: R" + dr["MONTHLY_CAR_REPAYMENT"].ToString());
                    overView.Items.Add("Total Expenses each month: R" + dr["MONTHLY_EXPENSES"].ToString());
                    overView.Items.Add("Amount of money remaining after deductions per month: R" + dr["AMOUNT_REMAINING"]);
                }

            }
            con.Close();
        }

        public void LoadProperty() 
        {
           // Save details to database
            SqlConnection con = new SqlConnection(conString);

            // adding data to database table 
            con.Open();
            using (var cmd = new SqlCommand(@"SELECT * FROM PROPERTY_PURCHASE WHERE USER_NAMES = @USER_NAMES", con))
            {
                cmd.Parameters.AddWithValue("@USER_NAMES", userName);

                DataTable dt = new DataTable();
                //Fills datatable
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                //populates rows and populats the listbox
                foreach (DataRow dr in dt.Rows)
                {
                    // writes all to listbox
                    propertyListView.Items.Add("Property Cost: R" + dr["PURCHASE_PRICE"].ToString());
                    propertyListView.Items.Add("Deposit Amount: R" + dr["TOTAL_DEPOSIT"].ToString());
                    propertyListView.Items.Add("Interest Rate: " + dr["INTEREST_RATE"].ToString() + "%");
                    propertyListView.Items.Add("Over how many Months?: " + dr["MONTHS"].ToString());
                    propertyListView.Items.Add("Monthly property Repayments: R" + dr["MONTHLY_REPAYMENT"].ToString());
                }

            }
            con.Close();

            if (propertyListView.Items.Count == 0)
            {
                propertyListView.Items.Add("You did not select the option to purchase property or you selcted rent");
            }
        }

        public void LoadVehicle() 
        {
            // Save details to database
            SqlConnection con = new SqlConnection(conString);

            // adding data to database table 
            con.Open();
            using (var cmd = new SqlCommand(@"SELECT * FROM PURCHASE_CAR WHERE USER_NAMES = @USER_NAMES", con))
            {
                cmd.Parameters.AddWithValue("@USER_NAMES", userName);

                DataTable dt = new DataTable();
                //Fills datatable
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                //populates rows and populats the listbox
                foreach (DataRow dr in dt.Rows)
                {
                    // writes all to listbox
                    carListView.Items.Add("Model and Make: " + dr["MODEL_AND_MAKE"].ToString());
                    carListView.Items.Add("Purchase Price: R" + dr["PURCHASE_PRICE"].ToString());
                    carListView.Items.Add("Total Deposit: " + dr["TOTAL_DEPOSIT"].ToString());
                    carListView.Items.Add("Interest Rate: " + dr["INTEREST_RATE"].ToString() + "%");
                    carListView.Items.Add("Insurance per month: R" + dr["INSURANCE_PREMIUM"].ToString());
                    carListView.Items.Add("Months: " + dr["MONTHS"].ToString());
                    carListView.Items.Add("Monthly vehicle repayments: R" + dr["MONTHLY_CAR_REPAYMENT"].ToString());
                }

            }
            con.Close();

            if (carListView.Items.Count == 0)
            {
                carListView.Items.Add("You did not select the option to purchase a vehicle");
            }
        }


        // Changing monthly property cost to 0 if it null
        public void CheckPropertyCost() 
        {
            //Opening and retrieving data from database
            SqlConnection con = new SqlConnection(conString);
            con.Open();

            SqlCommand cmd = new SqlCommand("Select MONTHLY_REPAYMENT from PERSONAL_BUDGET where USER_NAMES = @USER_NAMES", con);

            cmd.Parameters.AddWithValue("@USER_NAMES", userName);
            object exists =  (object)cmd.ExecuteScalar();

            if (DBNull.Value.Equals(exists))
            {

                con.Close();

                int monthlyPropertCost = 0;
                con.Open();

                // uploads data into table where the usenrame matches
                using (var newCmd = new SqlCommand(@"UPDATE PERSONAL_BUDGET SET MONTHLY_REPAYMENT = @MONTHLY_REPAYMENT WHERE USER_NAMES = @USER_NAMES", con))
                {
                    newCmd.Parameters.AddWithValue("@USER_NAMES", userName);
                    newCmd.Parameters.Add("@MONTHLY_REPAYMENT", SqlDbType.Int).Value = monthlyPropertCost;
                    newCmd.ExecuteNonQuery();
                }
                con.Close();


            }


        }

        // Changing monthly vehicle cost to 0 if it null
        public void CheckVehiclePurchase()
        {
            //Opening and retrieving data from database
            SqlConnection con = new SqlConnection(conString);
            con.Open();

            SqlCommand cmd = new SqlCommand("Select MONTHLY_CAR_REPAYMENT from PERSONAL_BUDGET where USER_NAMES = @USER_NAMES", con);

            cmd.Parameters.AddWithValue("@USER_NAMES", userName);
            object exists = (object)cmd.ExecuteScalar();

            if (DBNull.Value.Equals(exists))
            {

                con.Close();

                int monthlyVehicleCost = 0;
                con.Open();

                // uploads data into table where the usenrame matches
                using (var newCmd = new SqlCommand(@"UPDATE PERSONAL_BUDGET SET MONTHLY_CAR_REPAYMENT = @MONTHLY_CAR_REPAYMENT WHERE USER_NAMES = @USER_NAMES", con))
                {
                    newCmd.Parameters.AddWithValue("@USER_NAMES", userName);
                    newCmd.Parameters.Add("@MONTHLY_CAR_REPAYMENT", SqlDbType.Int).Value = monthlyVehicleCost;
                    newCmd.ExecuteNonQuery();
                }
                con.Close();


            }


        }

        // Converting total monthly expenses null to 0
        public void ConvertNullsToZeros()
        {

            // Save details to database
            SqlConnection con = new SqlConnection(conString);
            con.Open();

            SqlCommand cmd = new SqlCommand("Select MONTHLY_EXPENSES from PERSONAL_BUDGET where USER_NAMES = @USER_NAMES", con);

            cmd.Parameters.AddWithValue("@USER_NAMES", userName);
            object totalExpense = (object)cmd.ExecuteScalar();

            if (DBNull.Value.Equals(totalExpense))
            {
                int monthlyExpense = 0;

                // uploads data into table where the usenrame matches

                using (var newCmd = new SqlCommand(@" UPDATE PERSONAL_BUDGET SET MONTHLY_EXPENSES = @MONTHLY_EXPENSES where USER_NAMES = @USER_NAMES ", con))
                {
                    newCmd.Parameters.AddWithValue("@USER_NAMES", userName);
                    newCmd.Parameters.Add("@MONTHLY_EXPENSES", SqlDbType.Int).Value = monthlyExpense;
                    newCmd.ExecuteNonQuery();
                }
            }

            con.Close();

        }

        // Converting amount of money reamaining from null to 0
        public void ConvertNullsToZerosTwo() 
        {
            // Save details to database
            SqlConnection con = new SqlConnection(conString);
            
            con.Open();
            SqlCommand cmd = new SqlCommand("Select AMOUNT_REMAINING from PERSONAL_BUDGET where USER_NAMES = @USER_NAMES", con);
            cmd.Parameters.AddWithValue("@USER_NAMES", userName);
            object exists = (object)cmd.ExecuteScalar();

            if (DBNull.Value.Equals(exists))
            {
                int amountRemaining = 0;

                // uploads data into table where the usenrame matches

                using (var newCmd = new SqlCommand(@"UPDATE PERSONAL_BUDGET SET AMOUNT_REMAINING = @AMOUNT_REMAINING where USER_NAMES = @USER_NAMES ", con))
                {
                    newCmd.Parameters.AddWithValue("@USER_NAMES", userName);
                    newCmd.Parameters.Add("@AMOUNT_REMAINING", SqlDbType.Int).Value = amountRemaining;
                    newCmd.ExecuteNonQuery();
                }
            }
            con.Close();
        }

        // goes back
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            //Opens up Menu form
            this.Hide();
            var newForm = new UserMenu();

            newForm.Closed += (s, args) => this.Close();
            newForm.Show();
        }
    }

}

