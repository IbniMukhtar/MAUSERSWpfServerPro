using Microsoft.Data.SqlClient;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using ServerProjectWPF.MVVM;

namespace ServerProjectWPF
{
    public partial class MainWindow : Window
    {
        private User selectedUser;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if any of the required fields are empty
                if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                    string.IsNullOrWhiteSpace(txtUsername.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Password) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtMobileNo.Text))
                {
                    MessageBox.Show("Please fill out all the required fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

                string connectionString = "Data Source=Server3;Initial Catalog=weighdb;User ID=Sa;Password=Dst@system;Trust Server Certificate=True";

                string query = string.Empty;

                if (selectedUser == null)
                {
                    // Insert new user
                    query = "INSERT INTO MAUSERS (FullName, Username, UPasswd, CDate, Status, Email, MobileNo) VALUES (@FullName, @Username, @UPasswd, @CDate, @Status, @Email, @MobileNo)";
                }
                else
                {
                    // Update existing user
                    query = "UPDATE MAUSERS SET FullName = @FullName, Username = @Username, UPasswd = @UPasswd, Status = @Status, Email = @Email, MobileNo = @MobileNo WHERE ID = @ID";
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("@FullName", txtFullName.Text);
                    command.Parameters.AddWithValue("@Username", txtUsername.Text);
                    command.Parameters.AddWithValue("@UPasswd", txtPassword.Password);
                    command.Parameters.AddWithValue("@CDate", currentDate);
                    command.Parameters.AddWithValue("@Status", (chkStatus.IsChecked ?? false) ? 1 : 0);
                    command.Parameters.AddWithValue("@Email", txtEmail.Text);
                    command.Parameters.AddWithValue("@MobileNo", txtMobileNo.Text);

                    if (selectedUser != null)
                    {
                        command.Parameters.AddWithValue("@ID", selectedUser.ID);
                    }

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Data saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearTextBoxes(this);
                        chkStatus.IsChecked = true;
                    }
                    else
                    {
                        MessageBox.Show("Failed to save data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReSetButton_Click(object sender, RoutedEventArgs e)
        {
            ClearTextBoxes(this);
            chkStatus.IsChecked = true;
        }
        private void ClearTextBoxes(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is TextBox textBox)
                {
                    textBox.Text = string.Empty;
                }
                else if (child is PasswordBox passwordBox)
                {
                    passwordBox.Password = string.Empty;
                }
                else
                {
                    ClearTextBoxes(child);
                }
            }
        }


        private void DisplayUsersButton_Click(object sender, RoutedEventArgs e)
        {
            AllUsersWindow allUsersWindow = new AllUsersWindow();
            allUsersWindow.Show();
        }

        public void EditUser(User user)
        {
            selectedUser = user;

            txtID.Text = user.ID.ToString();
            txtFullName.Text = user.FullName;
            txtUsername.Text = user.Username;
            txtEmail.Text = user.Email;
            txtMobileNo.Text = user.MobileNo;
            // Fetch the CDate from the database
            string connectionString = "Data Source=Server3;Initial Catalog=weighdb;User ID=Sa;Password=Dst@system;Trust Server Certificate=True";
            string query = "SELECT CDate FROM MAUSERS WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ID", user.ID);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    txtDate.Text = ((DateTime)result).ToString("dd-MM-yyyy");
                }
            }

            btnSave.Content = "Update";
            btnSave.Click -= btnSave_Click;
            btnSave.Click += UpdateUser_Click;


            Show();
    }

        private void UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            btnSave_Click(sender, e);
            ClearTextBoxes(this);
            chkStatus.IsChecked = true;
        }

    }
}
