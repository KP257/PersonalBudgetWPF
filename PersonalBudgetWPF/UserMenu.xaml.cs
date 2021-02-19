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
using System.Windows.Shapes;

namespace PersonalBudgetWPF
{
    /// <summary>
    /// Interaction logic for UserMenu.xaml
    /// </summary>
    public partial class UserMenu : Window
    {
        //Connection string
        string conString = "Data Source=DESKTOP-FT09O6T\\SQLEXPRESS;Initial Catalog=PersonalBudget;Integrated Security=True";
        string name = Login.userName;

        public UserMenu()
        {
            InitializeComponent();
            Loaded += UserInformation_Loaded;
        }

        // MULTI-THREADING!
        private void UserInformation_Loaded(object sender, RoutedEventArgs e)
        {
            // Creating and initializing threads 
            Thread thr1 = new Thread(AddIncome);
            Thread thr2 = new Thread(CheckExpenses);
            Thread thr3 = new Thread(CheckProperty);
            Thread thr4 = new Thread(CheckVehicle);
            Thread thr5 = new Thread(CheckUserDecision);
            Thread thr6 = new Thread(CheckUserInput);

            thr1.Start();
            thr2.Start();
            thr3.Start();
            thr4.Start();
            thr5.Start();
            thr6.Start();
        }

        // Adds the users income as a display for him/her to see
        private void AddIncome()
        {

            // Whenever you update your UI elements from a thread other than the main thread, you need to use:
            this.Dispatcher.Invoke(() =>
            {
            // Save details to database
            SqlConnection con = new SqlConnection(conString);


            // adding data to database table 
            con.Open();
            using (var cmd = new SqlCommand(@"SELECT GROSS_INCOME FROM USER_REG WHERE USER_NAMES = @USER_NAMES", con))
            {

                cmd.Parameters.AddWithValue("@USER_NAMES", name);
                SqlDataReader myReader = cmd.ExecuteReader();
                while (myReader.Read())
                {
                    
                        lblIncome.Content = "Your Grossly Income is R" + myReader["GROSS_INCOME"].ToString();
                }  


            }
            con.Close();

            lblIncome.Visibility = Visibility.Visible;
            });
        }

        // checking if the user has entered in data before
        public void CheckExpenses() 
        {

            //Opening and retrieving data from database
            SqlConnection con = new SqlConnection(conString);
            con.Open();

            // using SQL to retrieve data 
            SqlCommand cmd = new SqlCommand("Select USER_NAMES from PERSONAL_EXPENSES where USER_NAMES = @USER_NAMES", con);
            cmd.Parameters.AddWithValue("@USER_NAMES", name);

            // readting and retrieving data fromd database
            SqlDataReader da = cmd.ExecuteReader();
            List<string> users = new List<string>();

            while (da.Read())
            {
                string user = da.GetString(0);
                users.Add(user);
            }
            con.Close();

            string[] userArray = users.ToArray();
            for (int i = 0; i < userArray.Length; i++)
            {
                // Whenever you update your UI elements from a thread other than the main thread, you need to use:
                this.Dispatcher.Invoke(() =>
                {
                    // checking
                    if (userArray.Contains(name))
                    {
                        BtnPersonalExpenses.Visibility = Visibility.Collapsed;
                        BtnPersonalExpensesFake.Visibility = Visibility.Visible;
                        lblPmessage.Visibility = Visibility.Visible;
                    }
                });

            }
        }
        // checking if the user has entered in data before
        public void CheckProperty()
        {

            //Opening and retrieving data from database
            SqlConnection con = new SqlConnection(conString);
            con.Open();

            SqlCommand cmd = new SqlCommand("Select USER_NAMES from PROPERTY_PURCHASE where USER_NAMES = @USER_NAMES", con);
            cmd.Parameters.AddWithValue("@USER_NAMES", name);

            SqlDataReader da = cmd.ExecuteReader();
            List<string> users = new List<string>();

            while (da.Read())
            {
                string user = da.GetString(0);
                users.Add(user);
            }
            con.Close();

            string[] userArray = users.ToArray();
            for (int i = 0; i < userArray.Length; i++)
            {
                // Whenever you update your UI elements from a thread other than the main thread, you need to use:
                this.Dispatcher.Invoke(() =>
                {
                    if (userArray.Contains(name))
                    {
                        BtnHomeLoadPayments.Visibility = Visibility.Collapsed;
                        BtnHomeLoadPaymentsFake.Visibility = Visibility.Visible;
                        lblPPmessage.Visibility = Visibility.Visible;
                    }
                });

            }
        }

        // checking if the user has entered in data before
        public void CheckVehicle()
        {
            //Opening and retrieving data from database
            SqlConnection con = new SqlConnection(conString);
            con.Open();

            SqlCommand cmd = new SqlCommand("Select USER_NAMES from PURCHASE_CAR where USER_NAMES = @USER_NAMES", con);
            cmd.Parameters.AddWithValue("@USER_NAMES", name);

            SqlDataReader da = cmd.ExecuteReader();
            List<string> users = new List<string>();

            while (da.Read())
            {
                string user = da.GetString(0);
                users.Add(user);
            }
            con.Close();

            string[] userArray = users.ToArray();
            for (int i = 0; i < userArray.Length; i++)
            {
                // Whenever you update your UI elements from a thread other than the main thread, you need to use:
                this.Dispatcher.Invoke(() =>
                {
                    if (userArray.Contains(name))
                    {
                        BtnPurchaseVehicle.Visibility = Visibility.Collapsed;
                        BtnPurchaseVehicleFake.Visibility = Visibility.Visible;
                        lblPVmessage.Visibility = Visibility.Visible;
                    }
                });

            }
        }

        // checks if user has entered rent
        public void CheckUserDecision() 
        {
            //Opening and retrieving data from database
            SqlConnection con = new SqlConnection(conString);
            con.Open();

            SqlCommand cmd = new SqlCommand("Select RENT_SELECTED from PERSONAL_EXPENSES where USER_NAMES = @USER_NAMES", con);

            cmd.Parameters.AddWithValue("@USER_NAMES", name);
            string RentExists = (string)cmd.ExecuteScalar();

            this.Dispatcher.Invoke(() =>
            {
                if (RentExists == "True")
                {
                    BtnHomeLoadPayments.Visibility = Visibility.Collapsed;
                    BtnHomeLoadPaymentsFake.Visibility = Visibility.Visible;
                    lblRentMessage.Visibility = Visibility.Visible;
                }
                
            });
            
        }

        // checking if the user has atleeast entered his/her personal expenses
        public void CheckUserInput() 
        {
            //Opening and retrieving data from database
            SqlConnection con = new SqlConnection(conString);
            con.Open();

            SqlCommand cmd = new SqlCommand("Select USER_NAMES from PERSONAL_EXPENSES where USER_NAMES = @USER_NAMES", con);

            cmd.Parameters.AddWithValue("@USER_NAMES", name);
            string UserExist = (string)cmd.ExecuteScalar();

            this.Dispatcher.Invoke(() =>
            {
                if (UserExist == null)
                {
                    BtnShowTotal.Visibility = Visibility.Collapsed;
                    BtnShowTotalFAKE.Visibility = Visibility.Visible;
                    lblSMessage.Visibility = Visibility.Visible;
                }
            });
        }

        // Navigation buttons below
        private void BtnViewBudget_Click(object sender, RoutedEventArgs e)
        {
            //Opens up Menu form
            this.Hide();
            var newForm = new ViewExpenses();

            newForm.Closed += (s, args) => this.Close();
            newForm.Show();
        }

        private void BtnPurchaseVehicle_Click(object sender, RoutedEventArgs e)
        {
            //Opens up Menu form
            this.Hide();
            var newForm = new PurchaseVehicle();

            newForm.Closed += (s, args) => this.Close();
            newForm.Show();
        }

        private void BtnPersonalExpenses_Click(object sender, RoutedEventArgs e)
        {
            //Opens up Menu form
            this.Hide();
            var newForm = new UsersPersonalExpense();

            newForm.Closed += (s, args) => this.Close();
            newForm.Show();
        }

        private void BtnHomeLoanPayments_Click(object sender, RoutedEventArgs e)
        {
            //Opens up Menu form
            this.Hide();
            var newForm = new PurchaseProperty();

            newForm.Closed += (s, args) => this.Close();
            newForm.Show();
        }

        // advanced feature
        private void BtnAdvancedFeature_Click(object sender, RoutedEventArgs e)
        {
            // list of words
            string[] names = { "Keep",
                              "Saving",
                              "Money!!"
            };

            //Linq query  
            IEnumerable<string> namesOfPeople = from name in names
                                                where name.Length <= 16
                                                select name;
            //displaying each word with foreach
            foreach (var name in namesOfPeople)
            {
                MessageBox.Show(name + "\n", "Simple Advice using LINQ");
            }
        }
    }
}
