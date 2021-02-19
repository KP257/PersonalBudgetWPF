using System;
using System.Collections.Generic;
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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            //Opens up form
            this.Hide();
            var newForm = new MainWindow();

            newForm.Closed += (s, args) => this.Close();
            newForm.Show();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            //Opens up form
            this.Hide();
            var newForm = new Login();

            newForm.Closed += (s, args) => this.Close();
            newForm.Show();
        }
    }
}
