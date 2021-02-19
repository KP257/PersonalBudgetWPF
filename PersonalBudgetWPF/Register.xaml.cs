using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Declaration
        string conString = "Data Source=DESKTOP-FT09O6T\\SQLEXPRESS;Initial Catalog=PersonalBudget;Integrated Security=True";

        public static int grossIncome;

        public MainWindow()
        {
            InitializeComponent();
        }

        //Registering the user
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            UsersDetails userModel = new UsersDetails();
            // passing to model class and back
            userModel.Name = txtUsername.Text;
            userModel.Password = txtPassword.Password.ToString();
            userModel.GrossIncome = Convert.ToInt32(txtGrossIncome.Text);

            string userName = userModel.Name;
            string password = userModel.Password;
             grossIncome = userModel.GrossIncome;

            // connection to the database
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            // getting username and checking if it exists 
            SqlCommand cmd = new SqlCommand("Select USER_NAMES from USER_REG where USER_NAMES = @USER_NAMES", con);
            cmd.Parameters.AddWithValue("@USER_NAMES", userName);
            SqlDataReader da = cmd.ExecuteReader();

            //List
            List<string> userList = new List<string>();

            while (da.Read())
            {
                string users = da.GetString(0);
                userList.Add(users);
            }
            con.Close();

            string[] userDetails = userList.ToArray();

            // searching through array for details and handling validation
            if (!System.Text.RegularExpressions.Regex.IsMatch(userName, "^[a-zA-Z ]"))
            {
                MessageBox.Show("This textbox accepts only alphabetical characters");
                txtUsername.Text.Remove(txtUsername.Text.Length - 1);
                return;
            }
            else if (userDetails.Contains(userName))
            {
                MessageBox.Show("User name already exists", "Already Exists");
                return;
            }
            else if (password.Length < 3)
            {
                MessageBox.Show("Password needs to be longer than 3 characters", "Password to short");
                return;
            }
            else if (!userDetails.Contains(userName))
            {
                con.Open();
                //Adding user login details to register user
                using (var cmdC = new SqlCommand(@"INSERT INTO USER_REG(USER_NAMES, USER_PASSWORDS, GROSS_INCOME) VALUES (@USER_NAMES, @USER_PASSWORDS, @GROSS_INCOME)", con))
                {
                    cmdC.Parameters.Add("@USER_NAMES", SqlDbType.VarChar).Value = txtUsername.Text;
                    cmdC.Parameters.Add("@USER_PASSWORDS", SqlDbType.VarChar).Value = GetShaData(txtPassword.Password.ToString());
                    cmdC.Parameters.Add("@GROSS_INCOME", SqlDbType.Int).Value = grossIncome;

                    cmdC.ExecuteNonQuery();
                }
                using (var newCmd = new SqlCommand(@"INSERT INTO PERSONAL_BUDGET(USER_NAMES, GROSS_MONTHLY) VALUES (@USER_NAMES, @GROSS_MONTHLY)", con))
                {
                    newCmd.Parameters.Add("@USER_NAMES", SqlDbType.VarChar).Value = txtUsername.Text;
                    newCmd.Parameters.Add("@GROSS_MONTHLY", SqlDbType.Int).Value = grossIncome;

                    newCmd.ExecuteNonQuery();
                }

                MessageBox.Show("Welcome " + txtUsername.Text + ".");

                //Opens up user menu form
                this.Hide();
                var newForm = new Login();

                newForm.Closed += (s, args) => this.Close();
                newForm.Show();

            }
            con.Close();
        }

        // Security for hasing password into database
        public static string GetShaData(string data) 
        {
            SHA1 sha = SHA1.Create();
            Byte[] hasData = sha.ComputeHash(Encoding.Default.GetBytes(data));
            StringBuilder returnValue = new StringBuilder();
            for (int i = 0; i < hasData.Length - 1; i++)
            {
                returnValue.Append(hasData[i].ToString());
            }
            return returnValue.ToString();
        }
      
    }
}
