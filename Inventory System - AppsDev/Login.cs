﻿using System;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Inventory_System___AppsDev
{
    public partial class Login : Form
    {
        private readonly string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\yeems214\Documents\Inventory-System---AppsDev\InventorySystem.accdb;Persist Security Info=True;";

        public Login()
        {
            InitializeComponent();
            comboBox1.Items.Add("admin");
            comboBox1.Items.Add("staff");
            comboBox1.SelectedIndex = 0;

            LoginBtn.Click += LoginBtn_Click;
            clearButton.Click += ClearButton_Click;
            SignInLink.LinkClicked += SignInLink_LinkClicked;
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            string username = UsernameTxt.Text.Trim();
            string password = passwordTxt.Text;
            string selectedRole = comboBox1.SelectedItem?.ToString();


            // Input validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (AuthenticateUser(username, password))
                {
                    MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Form nextForm = null;
                    if (selectedRole == "admin")
                    {
                        nextForm = new Dashboard();
                    }
                    else if (selectedRole == "staff")
                    {
                        nextForm = new Form1();
                    }

                    if (nextForm != null)
                    {
                        this.Hide(); // Hide the login form
                        nextForm.FormClosed += (s, args) => this.Close();
                        nextForm.Show();
                    }

                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Authentication Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    passwordTxt.Clear(); // Clear password field for security
                    passwordTxt.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during login: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            string selectedRole = comboBox1.SelectedItem?.ToString();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string query = "SELECT [Password], [Role] FROM Users WHERE Username = @Username";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    try
                    {
                        conn.Open();
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHashedPassword = reader["Password"].ToString();
                                string storedRole = reader["Role"].ToString();
                                string inputHashedPassword = HashPassword(password);

                                // Check both password and role
                                return storedHashedPassword == inputHashedPassword &&
                                       string.Equals(storedRole, selectedRole, StringComparison.OrdinalIgnoreCase);
                            }
                            else
                            {
                                return false; // Username not found
                            }
                        }
                    }
                    catch (OleDbException ex)
                    {
                        MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            UsernameTxt.Text = string.Empty;
            passwordTxt.Text = string.Empty;
        }

        private void SignInLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SignUp signUpForm = new SignUp();
            signUpForm.Show();
            this.Hide();
        }
    }
}
