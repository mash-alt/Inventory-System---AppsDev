using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Inventory_System___AppsDev
{
    public partial class SignUp : Form
    {
        private readonly string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\yeems214\Documents\Inventory-System---AppsDev\InventorySystem.accdb;Persist Security Info=True;";

        public SignUp()
        {
            InitializeComponent();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            signUpButton.Click += SignUpButton_Click;
            clearButton.Click += ClearButton_Click;

            // Add KeyPress events to prevent spaces in username and password
            UserNameTxtBox.KeyPress += TextBox_PreventSpaces;
            PasswordTxtBox.KeyPress += TextBox_PreventSpaces;
        }

        private void SignUpButton_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                try
                {
                    if (IsUsernameExists(UserNameTxtBox.Text))
                    {
                        ShowError("Username already exists. Please choose a different username.");
                        UserNameTxtBox.Focus();
                        return;
                    }

                    if (InsertNewUser())
                    {
                        MessageBox.Show("Account created successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFields();

                        // Open Login form and close SignUp form
                        new Login().Show();
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool InsertNewUser()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string query = "INSERT INTO Users (Username, Name, [Password], [Role]) VALUES (@Username, @Name, @Password, @Role)";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", UserNameTxtBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@Name", NameTxtBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@Password", HashPassword(PasswordTxtBox.Text));
                    cmd.Parameters.AddWithValue("@Role", "staff"); // Set Role as 'staff'

                    try
                    {
                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                    catch (OleDbException ex)
                    {
                        MessageBox.Show($"Database error: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
        }

        private bool IsUsernameExists(string username)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username.Trim());

                    try
                    {
                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                    catch (OleDbException ex)
                    {
                        MessageBox.Show($"Database error: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the password string to bytes
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Compute the hash
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Convert the hash bytes to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private bool ValidateInputs()
        {
            // Check for empty fields
            if (string.IsNullOrWhiteSpace(NameTxtBox.Text))
            {
                ShowError("Please enter your name.");
                NameTxtBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(UserNameTxtBox.Text))
            {
                ShowError("Please enter a username.");
                UserNameTxtBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(PasswordTxtBox.Text))
            {
                ShowError("Please enter a password.");
                PasswordTxtBox.Focus();
                return false;
            }

            // Validate name (letters and spaces only)
            if (!Regex.IsMatch(NameTxtBox.Text.Trim(), @"^[a-zA-Z\s]+$"))
            {
                ShowError("Name should contain only letters and spaces.");
                NameTxtBox.Focus();
                return false;
            }

            // Validate username (letters, numbers, and underscores only)
            if (!Regex.IsMatch(UserNameTxtBox.Text, @"^[a-zA-Z0-9_]+$"))
            {
                ShowError("Username should contain only letters, numbers, and underscores.");
                UserNameTxtBox.Focus();
                return false;
            }

            // Validate password length
            if (PasswordTxtBox.Text.Length < 6)
            {
                ShowError("Password must be at least 6 characters long.");
                PasswordTxtBox.Focus();
                return false;
            }

            return true;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            NameTxtBox.Clear();
            UserNameTxtBox.Clear();
            PasswordTxtBox.Clear();
            NameTxtBox.Focus();
        }

        private void TextBox_PreventSpaces(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        private void BackBtn_Click(object sender, EventArgs e)
        {
            this.Close();
            new Login().Show();
        }
    }
}