using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace CompanyManagementApp
{
    public partial class ClientsWindow : Window
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();

        public ClientsWindow()
        {
            InitializeComponent();
            LoadClientData();
            LoadBranchData(); // Load branches for branch selection
        }

        private void LoadClientData()
        {
            string query = "SELECT c.client_id AS ClientID, c.client_name AS ClientName, CONCAT(b.branch_name) AS BranchName " +
                           "FROM clients c " +
                           "LEFT JOIN branches b ON c.branch_id = b.id";

            MySqlDataAdapter adapter = dbHelper.ExecuteQuery(query);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            ClientsDataGrid.ItemsSource = dt.DefaultView;
        }

        private void LoadBranchData()
        {
            string query = "SELECT id, branch_name FROM branches";
            MySqlDataAdapter adapter = dbHelper.ExecuteQuery(query);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            BranchComboBox.ItemsSource = dt.DefaultView;
            BranchComboBox.SelectedIndex = -1; // Set no selection as default
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            string clientName = ClientNameTextBox.Text;

            if (BranchComboBox.SelectedValue == null)
            {
                MessageBox.Show("Please select a branch.");
                return;
            }

            int branchId = Convert.ToInt32(BranchComboBox.SelectedValue);

            string query = $"INSERT INTO clients (client_name, branch_id) VALUES ('{clientName}', {branchId})";
            dbHelper.ExecuteNonQuery(query);
            LoadClientData();
            ClearFields(); // Clear fields after insertion
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem != null)
            {
                DataRowView row = (DataRowView)ClientsDataGrid.SelectedItem;
                int clientId = Convert.ToInt32(row["ClientID"]);

                string clientName = ClientNameTextBox.Text;
                if (BranchComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Please select a branch.");
                    return;
                }
                int branchId = Convert.ToInt32(BranchComboBox.SelectedValue);

                string query = $"UPDATE clients SET client_name = '{clientName}', branch_id = {branchId} WHERE client_id = {clientId}";
                dbHelper.ExecuteNonQuery(query);
                LoadClientData();
                ClearFields(); // Clear fields after update
            }
            else
            {
                MessageBox.Show("Please select a client to update.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem != null)
            {
                DataRowView row = (DataRowView)ClientsDataGrid.SelectedItem;
                int clientId = Convert.ToInt32(row["ClientID"]);

                string query = $"DELETE FROM clients WHERE client_id = {clientId}";
                dbHelper.ExecuteNonQuery(query);
                LoadClientData();
                ClearFields(); // Clear fields after deletion
            }
            else
            {
                MessageBox.Show("Please select a client to delete.");
            }
        }

        private void ClientsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem != null)
            {
                DataRowView row = (DataRowView)ClientsDataGrid.SelectedItem;
                ClientNameTextBox.Text = row["ClientName"].ToString();
                BranchComboBox.SelectedValue = row["BranchName"];
            }
        }

        private void ClearFields()
        {
            ClientNameTextBox.Text = string.Empty;
            BranchComboBox.SelectedIndex = -1; // Clear selection
            ClientsDataGrid.SelectedItem = null; // Deselect any selected row in the DataGrid
        }
    }
}
