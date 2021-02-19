# PersonalBudgetWPF

This README file explains how to compile and run the Personal Budget programme 

Note that the this program requires Visual studio to run and uses Windows Forms application with c#.
Please unzip the programme and select PersonalBudget.sln. The Programme will then launch.

Please change the connectionString at the top of each windows.cs to your own connection string name or else the program will not recognise the server name. Check recordings for help. We use SQL server management studio to create the tables adn their entires.

----WHAT THE PROGRAMME DOES----

The user can enter the following values:
a. Gross monthly income (before deductions).
b. Estimated monthly tax deducted.
c. Estimated monthly expenditures in each of the following categories:
i. Groceries
ii. Water and lights
iii. Travel costs (including petrol)
iv. Cell phoneand& telephone
v. Other expenses

2. The user can choose between renting accommodation or buying a property.

3. The user can select to rent and is able to enter the monthly rental amount.

4. The user can select to buy a property and is able to enter the following
values for a home loan:
a. Purchase price of property. 
b. Total deposit.
c. Interest rate (percentage).
d. Number of months to repay (between 240 and 360).

5. The website calculates the monthly home loan repayment for buying a property
based on the values that the user entered. 

6. The website shall alert the user that approval of the home loan is unlikely if 
the monthly home loan repayment is more than a third of the user’s gross monthly.

7. The user can choose whether to buy a vehicle.

8. The user can select to buy a vehicle and is able to enter the following values
for vehicle financing:
a. Model and make.
b. Purchase price.
c. Total deposit.
d. Interest rate (percentage).
e. Estimated insurance premium.

9. The website calculates the total monthly cost of buying the car (insurance plus loan
repayment).

10. The website can calculate the available monthly money after all the specified deductions
have been made.

11. The user shall be able to choose to save up a specified amount by a certain date for a
specified reason, e.g. save R100 000 for an honours degree over five years. Given the interest
rate that will be earned on the savings, calculate how much the monthly saving should be to
reach the goal. 

---- DATABASING ----
All of these input are written to the database and retrieved when called. In the folder is a SQL script that you can use to create the neccessary tables
needed for the program to run


---- RUN IN AN IDE ----

If you want to run the website in an IDE, such as Visual studio you should
be able to copy-and-paste the entire contents of any one of the chapter folders
into a project in the IDE, and then run the program.  

We use SQL server management studio to create the tables and store the users credientials and financial details.


---- COMPILING AND RUNNING ----

Once you have opened the Personal Budget programme, you can click the green play button or press F5 or run the programme
PersonalExpenses.cs[Design]
                  
                  
As long as your compiler C# and Windows Form Application, there should be no errors.  


© The Independent Institute of Education (Pty) Ltd 2020
