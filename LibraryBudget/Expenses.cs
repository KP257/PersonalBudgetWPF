using System;
using System.Dynamic;
using System.Text;

namespace MyLibrary
{

    //ABSTRACT CLASS
    public abstract class Expenses
    {
        // abstract class
        // override method
        abstract public int GetAnswer();
    }

    // Custom Class libraries with using an abstract class to override
    // Custom Classes with calculation
    public class CalculateTax : Expenses
    {
        int deductedTax;

        public CalculateTax(int taxPercentage, int taxIncome)
        {
            taxPercentage /= 100;
            deductedTax = taxIncome * taxPercentage;
        }
        // the abstract method
        public override int GetAnswer()
        {
            // Returns users Expenses 
            return deductedTax;
        }
    }

    // The users personal expenses + constructor with calculation
    public class PersonalExpenses : Expenses
    {
        int monthlyPersonalExpenses;

        // constructor to calculate personal expenses
        public PersonalExpenses(int tax, int groceries, int waterAndLights, int travelCosts, int phoneBill, int other)
        {
            //Calculation for monthly personal expenses
            monthlyPersonalExpenses = tax + groceries + waterAndLights + travelCosts + phoneBill + other;
        }

        // the abstract method  
        public override int GetAnswer()
        {
            // Returns users Expenses 
            return monthlyPersonalExpenses;
        }
    }

    // Purchase Property calculation
    public class PurchaseProperty : Expenses
    {
        int monthlyPropertyCost;

        // constructor to calculate home loan repayment
        public PurchaseProperty(int purchasePrice, int totalDeposit, int interestRate, int numberOfMonths)
        {
            //Calculating home loan repayment
            //Declarations
            int price, interest, numOfMonths;

            price = purchasePrice - totalDeposit;
            interest = interestRate / 100;
            numOfMonths = numberOfMonths / 12;

            monthlyPropertyCost = price * (1 + interest * numOfMonths);

            monthlyPropertyCost /= numberOfMonths;
        }

        // the abstract method  
        public override int GetAnswer()
        {
            // Purhcase property 
            return monthlyPropertyCost;
        }
    }

    // Purchase Vehicle calculation
    public class PurchaseVehicle : Expenses
    {
        int monthlyCarCost;

        public PurchaseVehicle(int purchaseCarPrice, int totalCarDeposit, int interestCarRate, int insurance)
        {
            //Default number of months being 5 years = 60 months
            int numberOfMonths = 60, price, interest;


            //Calculating monthly car payment
            price = purchaseCarPrice - totalCarDeposit;
            interest = interestCarRate / 100;

            monthlyCarCost = price * (1 + interest * numberOfMonths);

            monthlyCarCost /= numberOfMonths;

            // plus insurance amount
            monthlyCarCost += insurance;

        }

        // the abstract method  
        public override int GetAnswer()
        {
            // Purchase Car
            return monthlyCarCost;
        }

    }

    // Calculating amount of money the user had to save each month
    public class SaveMoney 
    {
        double monthlySavings;

        public double AmountToSave(double startingAmount, double goalAmount, double growthInterest, double yearsToSave)
        {
            //Default number of months being 5 years = 60 months
            double rate, principle;


            rate = (growthInterest / 100);

            principle = (rate / 12) * (goalAmount - startingAmount * Math.Pow((1 + (rate / 12)), (yearsToSave * 12)));
            monthlySavings = principle / (Math.Pow(1 + (rate / 12), yearsToSave * 12) - 1);

            return monthlySavings;
        }


    }

    //Calculations for Amount ramining and total expenses
    public class FetchAmounts
    {
        public double AmountRemaining(double grossIncome, double expenses)
        {
            double finalAmount = grossIncome - expenses;
           
            return finalAmount;
        }

        public double MonthlyExpenses(double personalExpenses, double propertyExpenses, double vehicleExpenses, double amountToSave) 
        {
            double monthlyExpenses = personalExpenses + propertyExpenses + vehicleExpenses + amountToSave;

            return monthlyExpenses;
        }
    }

}
