using System.Windows;
using ServerProjectWPF.MVVM;
using Microsoft.Data.SqlClient;

namespace ServerProjectWPF
{
    public partial class AllUsersWindow : Window
    {
        public AllUsersWindow()
        {
            InitializeComponent();

            // Populate users when the window loads
            PopulateUsers();
        }

        private void PopulateUsers()
        {
            try
            {
                // Call the method to fetch all users from the database
                List<User> users = GetAllUsersFromDatabase();

                // Bind the fetched user data to the ListView
                lvUsers.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching users: " + ex.Message);
            }
        }

        // Method to fetch all users from the database
        private List<User> GetAllUsersFromDatabase()
        {
            List<User> users = new List<User>();

            // Get the connection string from App.config
            string connectionString = "Data Source=Server3;Initial Catalog=weighdb;User ID=Sa;Password=Dst@system;Trust Server Certificate=True";

            // SQL query to select all users
            string query = "SELECT ID, FullName, Username, Email, MobileNo FROM MAUSERS";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the database connection
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Loop through the result set and create User objects
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                ID = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Username = reader.GetString(2),
                                Email = reader.GetString(3),
                                MobileNo = reader.GetString(4)
                            };

                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the user associated with the Delete button
            var button = sender as FrameworkElement;
            var user = button?.DataContext as User;

            // Check if the user is not null
            if (user != null)
            {
                // Show confirmation dialog
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this user?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // Check user's choice
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Remove the user from the database
                        DeleteUserFromDatabase(user);

                        // Remove the user from the underlying collection
                        if (lvUsers.ItemsSource is ICollection<User> userList)
                        {
                            userList.Remove(user);
                        }
                        PopulateUsers();

                        // Show success message
                        MessageBox.Show("User deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        // Show error message
                        MessageBox.Show("Error deleting user: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }



        // Method to delete a user from the database
        private void DeleteUserFromDatabase(User user)
        {
            // Get the connection string from App.config
            string connectionString = "Data Source=Server3;Initial Catalog=weighdb;User ID=Sa;Password=Dst@system;Trust Server Certificate=True";

            // SQL query to delete the user
            string query = "DELETE FROM MAUSERS WHERE ID = @ID";

            // Create a SqlConnection using the connection string
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create a SqlCommand with the query and connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@ID", user.ID);

                    // Open the connection
                    connection.Open();

                    // Execute the command
                    command.ExecuteNonQuery();
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the user associated with the Edit button
            var button = sender as FrameworkElement;
            var user = button?.DataContext as User;

            // Check if the user is not null
            if (user != null)
            {
                // Create a new instance of the MainWindow
                MainWindow mainWindow = new MainWindow();

                // Pass the selected user's information to the MainWindow
                mainWindow.EditUser(user);

                // Show the MainWindow
                mainWindow.Show();
            }
        }


    }
}
