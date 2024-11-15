using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace CompanyManagementApp
{
    public partial class EmployeeWindow : Window
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();

        public EmployeeWindow()
        {
            InitializeComponent();
            LoadEmployeeData();
            LoadBranchData();
            LoadSupervisorData();
        }

        private void LoadEmployeeData()
        {
            string query = "SELECT e.ID, e.Given_Name, e.Family_Name, e.Date_Of_Birth, e.Gender_Identity, e.Gross_Salary, e.Supervisor_Id, e.Branch_Id, CONCAT(s.Given_Name, ' ', s.Family_Name) AS SupervisorName " +
                           "FROM employees e " +
                           "LEFT JOIN employees s ON e.Supervisor_Id = s.ID";

            MySqlDataAdapter adapter = dbHelper.ExecuteQuery(query);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            EmployeeDataGrid.ItemsSource = dt.DefaultView;
        }

        private void LoadBranchData()
        {
            string query = "SELECT id, branch_name FROM branches";
            MySqlDataAdapter adapter = dbHelper.ExecuteQuery(query);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            BranchComboBox.ItemsSource = dt.DefaultView;
        }

        private void LoadSupervisorData()
        {
            string query = "SELECT ID, CONCAT(Given_Name, ' ', Family_Name) AS FullName FROM employees";
            MySqlDataAdapter adapter = dbHelper.ExecuteQuery(query);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            SupervisorComboBox.ItemsSource = dt.DefaultView;
        }

        private void EmployeeDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem != null)
            {
                DataRowView row = (DataRowView)EmployeeDataGrid.SelectedItem;
                GivenNameTextBox.Text = row["Given_Name"].ToString();
                FamilyNameTextBox.Text = row["Family_Name"].ToString();
                DateOfBirthPicker.SelectedDate = DateTime.Parse(row["Date_Of_Birth"].ToString());
                GenderComboBox.Text = row["Gender_Identity"].ToString();
                SalaryTextBox.Text = row["Gross_Salary"].ToString();
                BranchComboBox.SelectedValue = row["Branch_Id"];
                SupervisorComboBox.SelectedValue = row["Supervisor_Id"];
            }
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            string givenName = GivenNameTextBox.Text;
            string familyName = FamilyNameTextBox.Text;
            int grossSalary = int.Parse(SalaryTextBox.Text);
            string gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            DateTime dateOfBirth = DateOfBirthPicker.SelectedDate ?? DateTime.Now;
            int branchId = Convert.ToInt32(BranchComboBox.SelectedValue);
            int supervisorId = SupervisorComboBox.SelectedValue != null ? Convert.ToInt32(SupervisorComboBox.SelectedValue) : 0;

            string query = $"INSERT INTO employees (Given_Name, Family_Name, Date_Of_Birth, Gender_Identity, Gross_Salary, Branch_Id, Supervisor_Id) " +
                           $"VALUES ('{givenName}', '{familyName}', '{dateOfBirth:yyyy-MM-dd}', '{gender}', {grossSalary}, {branchId}, {supervisorId})";
            dbHelper.ExecuteNonQuery(query);
            LoadEmployeeData();
            ClearFields(); // Clear fields after insertion
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem != null)
            {
                DataRowView row = (DataRowView)EmployeeDataGrid.SelectedItem;
                int employeeId = Convert.ToInt32(row["ID"]);

                string givenName = GivenNameTextBox.Text;
                string familyName = FamilyNameTextBox.Text;
                int grossSalary = int.Parse(SalaryTextBox.Text);
                string gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                DateTime dateOfBirth = DateOfBirthPicker.SelectedDate ?? DateTime.Now;
                int branchId = Convert.ToInt32(BranchComboBox.SelectedValue);
                int supervisorId = SupervisorComboBox.SelectedValue != null ? Convert.ToInt32(SupervisorComboBox.SelectedValue) : 0;

                string query = $"UPDATE employees SET Given_Name = '{givenName}', Family_Name = '{familyName}', Date_Of_Birth = '{dateOfBirth:yyyy-MM-dd}', " +
                               $"Gender_Identity = '{gender}', Gross_Salary = {grossSalary}, Branch_Id = {branchId}, Supervisor_Id = {supervisorId} " +
                               $"WHERE ID = {employeeId}";
                dbHelper.ExecuteNonQuery(query);
                LoadEmployeeData();
                ClearFields(); // Clear fields after update
            }
            else
            {
                MessageBox.Show("Please select an employee to update.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem != null)
            {
                DataRowView row = (DataRowView)EmployeeDataGrid.SelectedItem;
                int employeeId = Convert.ToInt32(row["ID"]);
                string query = $"DELETE FROM employees WHERE ID = {employeeId}";
                dbHelper.ExecuteNonQuery(query);
                LoadEmployeeData();
                ClearFields(); // Clear fields after deletion
            }
            else
            {
                MessageBox.Show("Please select an employee to delete.");
            }
        }

        private void ClearFields()
        {
            GivenNameTextBox.Text = string.Empty;
            FamilyNameTextBox.Text = string.Empty;
            DateOfBirthPicker.SelectedDate = null;
            GenderComboBox.SelectedIndex = -1;
            SalaryTextBox.Text = string.Empty;
            BranchComboBox.SelectedIndex = -1;
            SupervisorComboBox.SelectedIndex = -1;
        }
    }
}
