using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBudgetWPF
{
    class UsersDetails
    {
        public string Name { get; set; }
        public string Password { get;  set; }
        public int GrossIncome { get; set; }

        public UsersDetails() // Empty constructor
        { }

        public UsersDetails(string name, string password, int grossIncome) 
        {
            Name = name;
            Password = password;
            GrossIncome = grossIncome;
        }

    }
}
