using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PersonalBudgetWPF
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        //Declaration
        string conString = "Data Source=DESKTOP-FT09O6T\\SQLEXPRESS;Initial Catalog=PersonalBudget;Integrated Security=True";
        public static string userName;
        string userPass;

        public Login()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // user text to strings
            userName = txtUsername.Text;
            userPass = txtPassword.Password.ToString();


            //Declaration
            SqlConnection con = new SqlConnection(conString);
            con.Open();

            SqlCommand cmd = new SqlCommand("Select USER_NAMES,USER_PASSWORDS from USER_REG where USER_NAMES = @USER_NAMES AND USER_PASSWORDS = @USER_PASSWORDS", con);
            cmd.Parameters.AddWithValue("@USER_NAMES", userName);
            cmd.Parameters.AddWithValue("@USER_PASSWORDS", GetShaData(userPass));

            SqlDataReader da = cmd.ExecuteReader();


            //List
            List<string> userList = new List<string>();
            List<string> passList = new List<string>();
            while (da.Read())
            {
                string name = da["USER_NAMES"].ToString();
                string pass = da["USER_PASSWORDS"].ToString();

                userList.Add(name);
                passList.Add(pass);

            }

            
            con.Close();


            string[] userNameArray = userList.ToArray();
            string[] userPassArray = passList.ToArray();

            if (userNameArray.Contains(userName) && userPassArray.Contains(GetShaData(userPass)))
            {
                MessageBox.Show("Welcome " + txtUsername.Text);
                //Opens up Menu form
                this.Hide();
                var newForm = new UserMenu();

                newForm.Closed += (s, args) => this.Close();
                newForm.Show();


            }
            if (!userNameArray.Contains(userName) && !userPassArray.Contains(GetShaData(userPass)))
            {
                MessageBox.Show("DOES NOT EXIST OR INCORRECT DETAILS", "Incorrect");
            }


        }
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
