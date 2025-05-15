using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventory_System___AppsDev
{
    public partial class Dashboard : Form
    {
        private readonly string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\AppsDev-IT2\Inventory System\InventorySystem.accdb;Persist Security Info=True;";
        private DataGridView productGrid;
        private Button addButton;
        private Button updateButton;
        private Button deleteButton;
        private Button refreshButton;

        // Category controls
        private DataGridView categoryGrid;
        private Button addCategoryButton;
        private Button updateCategoryButton;
        private Button deleteCategoryButton;
        private Button refreshCategoryButton;

        // Supplier controls
        private DataGridView supplierGrid;
        private Button addSupplierButton;
        private Button updateSupplierButton;
        private Button deleteSupplierButton;
        private Button refreshSupplierButton;

        // STOCK LOGS SECTION
        private DataGridView stockLogsGrid;
        private Button nextStockLogsButton;
        private Button prevStockLogsButton;
        private Label stockLogsPageLabel;
        private int stockLogsPage = 1;
        private int stockLogsPageSize = 20;
        private int stockLogsTotalPages = 1;

        public Dashboard()
        {
            InitializeComponent();
            InitializeProductComponents();
            InitializeEventHandlers();
            LoadProducts(); // Load products when dashboard starts
        }

        private void InitializeProductComponents()
        {
            // Initialize DataGridView
            productGrid = new DataGridView
            {
                Name = "productGrid",
                Dock = DockStyle.Bottom,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                Location = new Point(10, 100),
                Size = new Size(mainPanel.Width - 20, mainPanel.Height - 150)
            };

            // Initialize Buttons
            addButton = new Button
            {
                Text = "Add Product",
                Size = new Size(120, 40),
                Location = new Point(10, 20),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            addButton.FlatAppearance.BorderSize = 0;

            updateButton = new Button
            {
                Text = "Update Product",
                Size = new Size(120, 40),
                Location = new Point(140, 20),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            updateButton.FlatAppearance.BorderSize = 0;

            deleteButton = new Button
            {
                Text = "Delete Product",
                Size = new Size(120, 40),
                Location = new Point(270, 20),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            deleteButton.FlatAppearance.BorderSize = 0;

            refreshButton = new Button
            {
                Text = "Refresh",
                Size = new Size(120, 40),
                Location = new Point(400, 20),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            refreshButton.FlatAppearance.BorderSize = 0;

            // Add controls to mainPanel
            mainPanel.Controls.Add(productGrid);
            mainPanel.Controls.Add(addButton);
            mainPanel.Controls.Add(updateButton);
            mainPanel.Controls.Add(deleteButton);
            mainPanel.Controls.Add(refreshButton);
        }

        private void InitializeEventHandlers()
        {
            // Wire up button click events
            addButton.Click += AddButton_Click;
            updateButton.Click += UpdateButton_Click;
            deleteButton.Click += DeleteButton_Click;
            refreshButton.Click += RefreshButton_Click;
            productsButton.Click += ProductsButton_Click;
            suppliersButton.Click += SuppliersButton_Click;
            categoriesButton.Click += CategoriesButton_Click;
            stockLogsButton.Click += StockLogsButton_Click;
            logoutButton.Click += LogoutButton_Click;

            // Handle form closing
            this.FormClosing += Dashboard_FormClosing;
        }

        private void LoadProducts()
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Products";
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        productGrid.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method to generate a unique barcode
        private string GenerateBarcode()
        {
            // Example: PRD20250508123456789 (PRD + yyyyMMdd + random 5 digits)
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string randomPart = new Random().Next(10000, 99999).ToString();
            return $"PRD{datePart}{randomPart}";
        }

        // Helper method to show input dialog for product details (with ComboBoxes for Category and Supplier)
        private bool ShowProductInputDialog(string title, ref string productName, ref int categoryId, ref int supplierId, ref int quantity, ref decimal unitPrice, ref int reorderLevel, bool isAdd = false)
        {
            using (Form form = new Form())
            {
                form.Text = title;
                int y = 20;
                int spacing = 35;
                Label lblName = new Label() { Left = 10, Top = y, Text = "Product Name", Width = 90 };
                TextBox txtName = new TextBox() { Left = 110, Top = y, Width = 200, Text = productName };
                y += spacing;
                Label lblCategory = new Label() { Left = 10, Top = y, Text = "Category", Width = 90 };
                ComboBox cmbCategory = new ComboBox() { Left = 110, Top = y, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
                y += spacing;
                Label lblSupplier = new Label() { Left = 10, Top = y, Text = "Supplier", Width = 90 };
                ComboBox cmbSupplier = new ComboBox() { Left = 110, Top = y, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
                y += spacing;
                Label lblQty = new Label() { Left = 10, Top = y, Text = "Quantity", Width = 90 };
                NumericUpDown numQty = new NumericUpDown() { Left = 110, Top = y, Width = 200, Value = quantity, Minimum = 0, Maximum = 100000 };
                y += spacing;
                Label lblUnitPrice = new Label() { Left = 10, Top = y, Text = "UnitPrice", Width = 90 };
                NumericUpDown numUnitPrice = new NumericUpDown() { Left = 110, Top = y, Width = 200, DecimalPlaces = 2, Minimum = 0, Maximum = 1000000, Value = unitPrice };
                y += spacing;
                Label lblReorder = new Label() { Left = 10, Top = y, Text = "ReorderLevel", Width = 90 };
                NumericUpDown numReorder = new NumericUpDown() { Left = 110, Top = y, Width = 200, Value = reorderLevel, Minimum = 0, Maximum = 100000 };
                y += spacing + 10;
                Button btnOk = new Button() { Text = "OK", Left = 110, Width = 80, Top = y, DialogResult = DialogResult.OK };
                Button btnCancel = new Button() { Text = "Cancel", Left = 230, Width = 80, Top = y, DialogResult = DialogResult.Cancel };

                // Load categories
                DataTable categories = new DataTable();
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT CategoryID, Name FROM Categories", conn))
                    {
                        adapter.Fill(categories);
                    }
                }
                cmbCategory.DataSource = categories;
                cmbCategory.DisplayMember = "Name";
                cmbCategory.ValueMember = "CategoryID";
                if (categoryId > 0)
                    cmbCategory.SelectedValue = categoryId;

                // Load suppliers
                DataTable suppliers = new DataTable();
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT SupplierID, Name FROM Suppliers", conn))
                    {
                        adapter.Fill(suppliers);
                    }
                }
                cmbSupplier.DataSource = suppliers;
                cmbSupplier.DisplayMember = "Name";
                cmbSupplier.ValueMember = "SupplierID";
                if (supplierId > 0)
                    cmbSupplier.SelectedValue = supplierId;

                form.Controls.AddRange(new Control[] { lblName, txtName, lblCategory, cmbCategory, lblSupplier, cmbSupplier, lblQty, numQty, lblUnitPrice, numUnitPrice, lblReorder, numReorder, btnOk, btnCancel });
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;
                form.ClientSize = new Size(340, y + 50);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Input validation
                    string nameVal = txtName.Text.Trim();
                    if (string.IsNullOrWhiteSpace(nameVal) || nameVal.Length > 50 || !System.Text.RegularExpressions.Regex.IsMatch(nameVal, @"^[a-zA-Z0-9\s\-]+$"))
                    {
                        MessageBox.Show("Product Name is required, max 50 chars, and must not contain special characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (cmbCategory.SelectedValue == null || Convert.ToInt32(cmbCategory.SelectedValue) <= 0)
                    {
                        MessageBox.Show("Please select a valid category.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (cmbSupplier.SelectedValue == null || Convert.ToInt32(cmbSupplier.SelectedValue) <= 0)
                    {
                        MessageBox.Show("Please select a valid supplier.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (numQty.Value < 0)
                    {
                        MessageBox.Show("Quantity cannot be negative.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (numUnitPrice.Value < 0)
                    {
                        MessageBox.Show("Unit Price cannot be negative.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (numReorder.Value < 0)
                    {
                        MessageBox.Show("Reorder Level cannot be negative.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    productName = nameVal;
                    categoryId = Convert.ToInt32(cmbCategory.SelectedValue);
                    supplierId = Convert.ToInt32(cmbSupplier.SelectedValue);
                    quantity = (int)numQty.Value;
                    unitPrice = numUnitPrice.Value;
                    reorderLevel = (int)numReorder.Value;
                    return true;
                }
                return false;
            }
        }

        // Helper method to show a simple input dialog for remarks
        private string ShowRemarksInputDialog(string title, string defaultRemark = "")
        {
            using (Form form = new Form())
            {
                form.Text = title;
                Label lbl = new Label() { Left = 10, Top = 20, Text = "Remarks", Width = 80 };
                TextBox txt = new TextBox() { Left = 100, Top = 20, Width = 250, Text = defaultRemark };
                Button btnOk = new Button() { Text = "OK", Left = 100, Width = 80, Top = 60, DialogResult = DialogResult.OK };
                Button btnCancel = new Button() { Text = "Cancel", Left = 200, Width = 80, Top = 60, DialogResult = DialogResult.Cancel };
                form.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;
                form.ClientSize = new Size(370, 110);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    return txt.Text.Trim();
                }
                return null;
            }
        }

        private int GetLastInsertedProductId(OleDbConnection conn)
        {
            using (OleDbCommand cmd = new OleDbCommand("SELECT @@IDENTITY", conn))
            {
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string productName = "";
            int categoryId = 0;
            int supplierId = 0;
            int quantity = 0;
            decimal unitPrice = 0;
            int reorderLevel = 0;
            if (ShowProductInputDialog("Add Product", ref productName, ref categoryId, ref supplierId, ref quantity, ref unitPrice, ref reorderLevel, isAdd: true))
            {
                try
                {
                    if (categoryId <= 0 || supplierId <= 0)
                    {
                        MessageBox.Show("Please select a valid category and supplier.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();
                        string barcode = GenerateBarcode();
                        DateTime dateAdded = DateTime.Now;
                        string query = "INSERT INTO Products ([ProductName], [CategoryID], [SupplierID], [Quantity], [UnitPrice], [ReorderLevel], [Barcode], [DateAdded]) VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
                        int productId;
                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProductName", productName);
                            cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                            cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                            cmd.Parameters.AddWithValue("@Quantity", quantity);
                            var unitPriceParam = cmd.Parameters.Add("@UnitPrice", OleDbType.Currency);
                            unitPriceParam.Value = Convert.ToDouble(unitPrice);
                            cmd.Parameters.AddWithValue("@ReorderLevel", reorderLevel);
                            cmd.Parameters.AddWithValue("@Barcode", barcode);
                            var dateAddedParam = cmd.Parameters.Add("@DateAdded", OleDbType.Date);
                            dateAddedParam.Value = dateAdded;
                            cmd.ExecuteNonQuery();
                        }
                        // Get the last inserted ProductID
                        productId = GetLastInsertedProductId(conn);
                        // Prompt for remarks
                        string remarks = ShowRemarksInputDialog("Stock IN Remarks", "Initial stock on add");
                        if (remarks == null) remarks = "Initial stock on add";
                        // Log IN to StockLogs
                        string logQuery = "INSERT INTO StockLogs ([ProductID], [Date], [Type], [Quantity], [Remarks]) VALUES (?, ?, ?, ?, ?)";
                        using (OleDbCommand logCmd = new OleDbCommand(logQuery, conn))
                        {
                            logCmd.Parameters.AddWithValue("@ProductID", productId);
                            var dateParam = logCmd.Parameters.Add("@Date", OleDbType.Date);
                            dateParam.Value = dateAdded;
                            logCmd.Parameters.AddWithValue("@Type", "IN");
                            logCmd.Parameters.AddWithValue("@Quantity", quantity);
                            logCmd.Parameters.AddWithValue("@Remarks", remarks);
                            logCmd.ExecuteNonQuery();
                        }
                    }
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (productGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow row = productGrid.SelectedRows[0];
                int productId = Convert.ToInt32(row.Cells["ProductID"].Value);
                string productName = row.Cells["ProductName"].Value.ToString();
                int categoryId = Convert.ToInt32(row.Cells["CategoryID"].Value);
                int supplierId = Convert.ToInt32(row.Cells["SupplierID"].Value);
                int oldQuantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                decimal unitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value);
                int reorderLevel = Convert.ToInt32(row.Cells["ReorderLevel"].Value);
                int newQuantity = oldQuantity;
                if (ShowProductInputDialog("Update Product", ref productName, ref categoryId, ref supplierId, ref newQuantity, ref unitPrice, ref reorderLevel))
                {
                    try
                    {
                        using (OleDbConnection conn = new OleDbConnection(connectionString))
                        {
                            conn.Open();
                            string query = "UPDATE Products SET ProductName=?, CategoryID=?, SupplierID=?, Quantity=?, UnitPrice=?, ReorderLevel=? WHERE ProductID=?";
                            using (OleDbCommand cmd = new OleDbCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ProductName", productName);
                                cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                                cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                                cmd.Parameters.AddWithValue("@Quantity", newQuantity);
                                cmd.Parameters.AddWithValue("@UnitPrice", Convert.ToDouble(unitPrice));
                                cmd.Parameters.AddWithValue("@ReorderLevel", reorderLevel);
                                cmd.Parameters.AddWithValue("@ProductID", productId);
                                cmd.ExecuteNonQuery();
                            }
                            // Log IN or OUT if quantity changed
                            if (newQuantity != oldQuantity)
                            {
                                string type = newQuantity > oldQuantity ? "IN" : "OUT";
                                int diff = Math.Abs(newQuantity - oldQuantity);
                                if (type == "IN" || (type == "OUT" && newQuantity < oldQuantity))
                                {
                                    string defaultRemark = type == "IN" ? "Stock increased on update" : "Stock decreased on update";
                                    string remarks = ShowRemarksInputDialog($"Stock {type} Remarks", defaultRemark);
                                    if (remarks == null) remarks = defaultRemark;
                                    string logQuery = "INSERT INTO StockLogs ([ProductID], [Date], [Type], [Quantity], [Remarks]) VALUES (?, ?, ?, ?, ?)";
                                    using (OleDbCommand logCmd = new OleDbCommand(logQuery, conn))
                                    {
                                        logCmd.Parameters.AddWithValue("@ProductID", productId);
                                        var dateParam = logCmd.Parameters.Add("@Date", OleDbType.Date);
                                        dateParam.Value = DateTime.Now;
                                        logCmd.Parameters.AddWithValue("@Type", type);
                                        logCmd.Parameters.AddWithValue("@Quantity", diff);
                                        logCmd.Parameters.AddWithValue("@Remarks", remarks);
                                        logCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        LoadProducts();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a product to update.", "Update Product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (productGrid.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this product? All related stock logs will also be deleted.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DataGridViewRow row = productGrid.SelectedRows[0];
                    int productId = Convert.ToInt32(row.Cells["ProductID"].Value);
                    int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                    try
                    {
                        using (OleDbConnection conn = new OleDbConnection(connectionString))
                        {
                            conn.Open();
                            // No OUT log on delete
                            // First, delete related stock logs
                            string deleteLogsQuery = "DELETE FROM StockLogs WHERE ProductID=?";
                            using (OleDbCommand cmdLogs = new OleDbCommand(deleteLogsQuery, conn))
                            {
                                cmdLogs.Parameters.AddWithValue("@ProductID", productId);
                                cmdLogs.ExecuteNonQuery();
                            }
                            // Then, delete the product
                            string deleteProductQuery = "DELETE FROM Products WHERE ProductID=?";
                            using (OleDbCommand cmdProduct = new OleDbCommand(deleteProductQuery, conn))
                            {
                                cmdProduct.Parameters.AddWithValue("@ProductID", productId);
                                cmdProduct.ExecuteNonQuery();
                            }
                        }
                        LoadProducts();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a product to delete.", "Delete Product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void ProductsButton_Click(object sender, EventArgs e)
        {
            headerLabel.Text = "Products";
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(productGrid);
            mainPanel.Controls.Add(addButton);
            mainPanel.Controls.Add(updateButton);
            mainPanel.Controls.Add(deleteButton);
            mainPanel.Controls.Add(refreshButton);
            LoadProducts();
        }

        private void SuppliersButton_Click(object sender, EventArgs e)
        {
            headerLabel.Text = "Suppliers";
            ShowSuppliers();
        }

        private void CategoriesButton_Click(object sender, EventArgs e)
        {
            headerLabel.Text = "Categories";
            ShowCategories();
        }

        private void StockLogsButton_Click(object sender, EventArgs e)
        {
            headerLabel.Text = "Stock Logs";
            ShowStockLogs();
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                Login loginForm = new Login();
                loginForm.Show();
            }
        }

        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show("Are you sure you want to exit?", "Exit",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        private void ShowSection(string sectionName)
        {
            mainPanel.Controls.Clear();
            Label placeholder = new Label
            {
                Text = $"{sectionName} Section",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(30, 30)
            };
            mainPanel.Controls.Add(placeholder);
        }

        // CATEGORY SECTION
        private void InitializeCategoryComponents()
        {
            categoryGrid = new DataGridView
            {
                Name = "categoryGrid",
                Dock = DockStyle.Bottom,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                Location = new Point(10, 100),
                Size = new Size(mainPanel.Width - 20, mainPanel.Height - 150)
            };
            addCategoryButton = new Button { Text = "Add Category", Size = new Size(120, 40), Location = new Point(10, 20), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            addCategoryButton.FlatAppearance.BorderSize = 0;
            updateCategoryButton = new Button { Text = "Update Category", Size = new Size(120, 40), Location = new Point(140, 20), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            updateCategoryButton.FlatAppearance.BorderSize = 0;
            deleteCategoryButton = new Button { Text = "Delete Category", Size = new Size(120, 40), Location = new Point(270, 20), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            deleteCategoryButton.FlatAppearance.BorderSize = 0;
            refreshCategoryButton = new Button { Text = "Refresh", Size = new Size(120, 40), Location = new Point(400, 20), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            refreshCategoryButton.FlatAppearance.BorderSize = 0;
            addCategoryButton.Click += AddCategoryButton_Click;
            updateCategoryButton.Click += UpdateCategoryButton_Click;
            deleteCategoryButton.Click += DeleteCategoryButton_Click;
            refreshCategoryButton.Click += RefreshCategoryButton_Click;
        }
        private void ShowCategories()
        {
            mainPanel.Controls.Clear();
            InitializeCategoryComponents();
            mainPanel.Controls.Add(categoryGrid);
            mainPanel.Controls.Add(addCategoryButton);
            mainPanel.Controls.Add(updateCategoryButton);
            mainPanel.Controls.Add(deleteCategoryButton);
            mainPanel.Controls.Add(refreshCategoryButton);
            LoadCategories();
        }
        private void LoadCategories()
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Categories";
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        categoryGrid.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool ShowCategoryInputDialog(string title, ref string name, ref string description)
        {
            using (Form form = new Form())
            {
                form.Text = title;
                Label lblName = new Label() { Left = 10, Top = 20, Text = "Name", Width = 80 };
                TextBox txtName = new TextBox() { Left = 100, Top = 20, Width = 200, Text = name };
                Label lblDesc = new Label() { Left = 10, Top = 60, Text = "Description", Width = 80 };
                TextBox txtDesc = new TextBox() { Left = 100, Top = 60, Width = 200, Text = description };
                Button btnOk = new Button() { Text = "OK", Left = 100, Width = 80, Top = 110, DialogResult = DialogResult.OK };
                Button btnCancel = new Button() { Text = "Cancel", Left = 220, Width = 80, Top = 110, DialogResult = DialogResult.Cancel };
                form.Controls.AddRange(new Control[] { lblName, txtName, lblDesc, txtDesc, btnOk, btnCancel });
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;
                form.ClientSize = new Size(320, 160);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string nameVal = txtName.Text.Trim();
                    if (string.IsNullOrWhiteSpace(nameVal) || nameVal.Length > 50 || !System.Text.RegularExpressions.Regex.IsMatch(nameVal, @"^[a-zA-Z0-9\s\-]+$"))
                    {
                        MessageBox.Show("Category Name is required, max 50 chars, and must not contain special characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (txtDesc.Text.Length > 100)
                    {
                        MessageBox.Show("Description must be 100 characters or less.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    name = nameVal;
                    description = txtDesc.Text.Trim();
                    return true;
                }
                return false;
            }
        }
        private void AddCategoryButton_Click(object sender, EventArgs e)
        {
            string name = "";
            string description = "";
            if (ShowCategoryInputDialog("Add Category", ref name, ref description))
            {
                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();
                        string query = "INSERT INTO Categories (Name, Description) VALUES (?, ?)";
                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Name", name);
                            cmd.Parameters.AddWithValue("@Description", description);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadCategories();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void UpdateCategoryButton_Click(object sender, EventArgs e)
        {
            if (categoryGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow row = categoryGrid.SelectedRows[0];
                int categoryId = Convert.ToInt32(row.Cells["CategoryID"].Value);
                string name = row.Cells["Name"].Value.ToString();
                string description = row.Cells["Description"].Value.ToString();
                if (ShowCategoryInputDialog("Update Category", ref name, ref description))
                {
                    try
                    {
                        using (OleDbConnection conn = new OleDbConnection(connectionString))
                        {
                            conn.Open();
                            string query = "UPDATE Categories SET Name=?, Description=? WHERE CategoryID=?";
                            using (OleDbCommand cmd = new OleDbCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@Name", name);
                                cmd.Parameters.AddWithValue("@Description", description);
                                cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        LoadCategories();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a category to update.", "Update Category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void DeleteCategoryButton_Click(object sender, EventArgs e)
        {
            if (categoryGrid.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this category? All related products and their stock logs will also be deleted.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DataGridViewRow row = categoryGrid.SelectedRows[0];
                    int categoryId = Convert.ToInt32(row.Cells["CategoryID"].Value);
                    try
                    {
                        using (OleDbConnection conn = new OleDbConnection(connectionString))
                        {
                            conn.Open();
                            // Get all products for this category
                            string getProductsQuery = "SELECT ProductID FROM Products WHERE CategoryID=?";
                            List<int> productIds = new List<int>();
                            using (OleDbCommand getProductsCmd = new OleDbCommand(getProductsQuery, conn))
                            {
                                getProductsCmd.Parameters.AddWithValue("@CategoryID", categoryId);
                                using (OleDbDataReader reader = getProductsCmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        productIds.Add(reader.GetInt32(0));
                                    }
                                }
                            }
                            // Delete stock logs for each product
                            foreach (int productId in productIds)
                            {
                                string deleteLogsQuery = "DELETE FROM StockLogs WHERE ProductID=?";
                                using (OleDbCommand cmdLogs = new OleDbCommand(deleteLogsQuery, conn))
                                {
                                    cmdLogs.Parameters.AddWithValue("@ProductID", productId);
                                    cmdLogs.ExecuteNonQuery();
                                }
                            }
                            // Delete products for this category
                            string deleteProductsQuery = "DELETE FROM Products WHERE CategoryID=?";
                            using (OleDbCommand cmdProducts = new OleDbCommand(deleteProductsQuery, conn))
                            {
                                cmdProducts.Parameters.AddWithValue("@CategoryID", categoryId);
                                cmdProducts.ExecuteNonQuery();
                            }
                            // Delete the category
                            string deleteCategoryQuery = "DELETE FROM Categories WHERE CategoryID=?";
                            using (OleDbCommand cmd = new OleDbCommand(deleteCategoryQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        LoadCategories();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a category to delete.", "Delete Category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void RefreshCategoryButton_Click(object sender, EventArgs e)
        {
            LoadCategories();
        }

        // SUPPLIER SECTION
        private void InitializeSupplierComponents()
        {
            supplierGrid = new DataGridView
            {
                Name = "supplierGrid",
                Dock = DockStyle.Bottom,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                Location = new Point(10, 100),
                Size = new Size(mainPanel.Width - 20, mainPanel.Height - 150)
            };
            addSupplierButton = new Button { Text = "Add Supplier", Size = new Size(120, 40), Location = new Point(10, 20), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            addSupplierButton.FlatAppearance.BorderSize = 0;
            updateSupplierButton = new Button { Text = "Update Supplier", Size = new Size(120, 40), Location = new Point(140, 20), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            updateSupplierButton.FlatAppearance.BorderSize = 0;
            deleteSupplierButton = new Button { Text = "Delete Supplier", Size = new Size(120, 40), Location = new Point(270, 20), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            deleteSupplierButton.FlatAppearance.BorderSize = 0;
            refreshSupplierButton = new Button { Text = "Refresh", Size = new Size(120, 40), Location = new Point(400, 20), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            refreshSupplierButton.FlatAppearance.BorderSize = 0;
            addSupplierButton.Click += AddSupplierButton_Click;
            updateSupplierButton.Click += UpdateSupplierButton_Click;
            deleteSupplierButton.Click += DeleteSupplierButton_Click;
            refreshSupplierButton.Click += RefreshSupplierButton_Click;
        }
        private void ShowSuppliers()
        {
            mainPanel.Controls.Clear();
            InitializeSupplierComponents();
            mainPanel.Controls.Add(supplierGrid);
            mainPanel.Controls.Add(addSupplierButton);
            mainPanel.Controls.Add(updateSupplierButton);
            mainPanel.Controls.Add(deleteSupplierButton);
            mainPanel.Controls.Add(refreshSupplierButton);
            LoadSuppliers();
        }
        private void LoadSuppliers()
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Suppliers";
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        supplierGrid.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading suppliers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool ShowSupplierInputDialog(string title, ref string name, ref string contact, ref string email, ref string address)
        {
            using (Form form = new Form())
            {
                form.Text = title;
                Label lblName = new Label() { Left = 10, Top = 20, Text = "Name", Width = 80 };
                TextBox txtName = new TextBox() { Left = 100, Top = 20, Width = 200, Text = name };
                Label lblContact = new Label() { Left = 10, Top = 60, Text = "Contact", Width = 80 };
                TextBox txtContact = new TextBox() { Left = 100, Top = 60, Width = 200, Text = contact };
                Label lblEmail = new Label() { Left = 10, Top = 100, Text = "Email", Width = 80 };
                TextBox txtEmail = new TextBox() { Left = 100, Top = 100, Width = 200, Text = email };
                Label lblAddress = new Label() { Left = 10, Top = 140, Text = "Address", Width = 80 };
                TextBox txtAddress = new TextBox() { Left = 100, Top = 140, Width = 200, Text = address };
                Button btnOk = new Button() { Text = "OK", Left = 100, Width = 80, Top = 190, DialogResult = DialogResult.OK };
                Button btnCancel = new Button() { Text = "Cancel", Left = 220, Width = 80, Top = 190, DialogResult = DialogResult.Cancel };
                form.Controls.AddRange(new Control[] { lblName, txtName, lblContact, txtContact, lblEmail, txtEmail, lblAddress, txtAddress, btnOk, btnCancel });
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;
                form.ClientSize = new Size(320, 240);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string nameVal = txtName.Text.Trim();
                    if (string.IsNullOrWhiteSpace(nameVal) || nameVal.Length > 50 || !System.Text.RegularExpressions.Regex.IsMatch(nameVal, @"^[a-zA-Z0-9\s\-]+$"))
                    {
                        MessageBox.Show("Supplier Name is required, max 50 chars, and must not contain special characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (txtContact.Text.Length > 50)
                    {
                        MessageBox.Show("Contact must be 50 characters or less.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (txtAddress.Text.Length > 100)
                    {
                        MessageBox.Show("Address must be 100 characters or less.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    name = nameVal;
                    contact = txtContact.Text.Trim();
                    email = txtEmail.Text.Trim();
                    address = txtAddress.Text.Trim();
                    return true;
                }
                return false;
            }
        }
        private void AddSupplierButton_Click(object sender, EventArgs e)
        {
            string name = "";
            string contact = "";
            string email = "";
            string address = "";
            if (ShowSupplierInputDialog("Add Supplier", ref name, ref contact, ref email, ref address))
            {
                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();
                        string query = "INSERT INTO Suppliers (Name, Contact, Email, Address) VALUES (?, ?, ?, ?)";
                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Name", name);
                            cmd.Parameters.AddWithValue("@Contact", contact);
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@Address", address);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadSuppliers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding supplier: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void UpdateSupplierButton_Click(object sender, EventArgs e)
        {
            if (supplierGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow row = supplierGrid.SelectedRows[0];
                int supplierId = Convert.ToInt32(row.Cells["SupplierID"].Value);
                string name = row.Cells["Name"].Value.ToString();
                string contact = row.Cells["Contact"].Value.ToString();
                string email = row.Cells["Email"].Value.ToString();
                string address = row.Cells["Address"].Value.ToString();
                if (ShowSupplierInputDialog("Update Supplier", ref name, ref contact, ref email, ref address))
                {
                    try
                    {
                        using (OleDbConnection conn = new OleDbConnection(connectionString))
                        {
                            conn.Open();
                            string query = "UPDATE Suppliers SET Name=?, Contact=?, Email=?, Address=? WHERE SupplierID=?";
                            using (OleDbCommand cmd = new OleDbCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@Name", name);
                                cmd.Parameters.AddWithValue("@Contact", contact);
                                cmd.Parameters.AddWithValue("@Email", email);
                                cmd.Parameters.AddWithValue("@Address", address);
                                cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        LoadSuppliers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating supplier: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a supplier to update.", "Update Supplier", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void DeleteSupplierButton_Click(object sender, EventArgs e)
        {
            if (supplierGrid.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this supplier? All related products and their stock logs will also be deleted.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DataGridViewRow row = supplierGrid.SelectedRows[0];
                    int supplierId = Convert.ToInt32(row.Cells["SupplierID"].Value);
                    try
                    {
                        using (OleDbConnection conn = new OleDbConnection(connectionString))
                        {
                            conn.Open();
                            // Get all products for this supplier
                            string getProductsQuery = "SELECT ProductID FROM Products WHERE SupplierID=?";
                            List<int> productIds = new List<int>();
                            using (OleDbCommand getProductsCmd = new OleDbCommand(getProductsQuery, conn))
                            {
                                getProductsCmd.Parameters.AddWithValue("@SupplierID", supplierId);
                                using (OleDbDataReader reader = getProductsCmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        productIds.Add(reader.GetInt32(0));
                                    }
                                }
                            }
                            // Delete stock logs for each product
                            foreach (int productId in productIds)
                            {
                                string deleteLogsQuery = "DELETE FROM StockLogs WHERE ProductID=?";
                                using (OleDbCommand cmdLogs = new OleDbCommand(deleteLogsQuery, conn))
                                {
                                    cmdLogs.Parameters.AddWithValue("@ProductID", productId);
                                    cmdLogs.ExecuteNonQuery();
                                }
                            }
                            // Delete products for this supplier
                            string deleteProductsQuery = "DELETE FROM Products WHERE SupplierID=?";
                            using (OleDbCommand cmdProducts = new OleDbCommand(deleteProductsQuery, conn))
                            {
                                cmdProducts.Parameters.AddWithValue("@SupplierID", supplierId);
                                cmdProducts.ExecuteNonQuery();
                            }
                            // Delete the supplier
                            string deleteSupplierQuery = "DELETE FROM Suppliers WHERE SupplierID=?";
                            using (OleDbCommand cmd = new OleDbCommand(deleteSupplierQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        LoadSuppliers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting supplier: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a supplier to delete.", "Delete Supplier", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void RefreshSupplierButton_Click(object sender, EventArgs e)
        {
            LoadSuppliers();
        }

        private void InitializeStockLogsComponents()
        {
            stockLogsGrid = new DataGridView
            {
                Name = "stockLogsGrid",
                Dock = DockStyle.Bottom,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                Location = new Point(10, 100),
                Size = new Size(mainPanel.Width - 20, mainPanel.Height - 150)
            };
            prevStockLogsButton = new Button { Text = "Previous", Size = new Size(100, 35), Location = new Point(10, 20), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            prevStockLogsButton.FlatAppearance.BorderSize = 0;
            nextStockLogsButton = new Button { Text = "Next", Size = new Size(100, 35), Location = new Point(120, 20), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            nextStockLogsButton.FlatAppearance.BorderSize = 0;
            stockLogsPageLabel = new Label { Text = "Page 1", Location = new Point(240, 28), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(0, 122, 204) };
            prevStockLogsButton.Click += PrevStockLogsButton_Click;
            nextStockLogsButton.Click += NextStockLogsButton_Click;
        }
        private void ShowStockLogs()
        {
            mainPanel.Controls.Clear();
            InitializeStockLogsComponents();
            mainPanel.Controls.Add(stockLogsGrid);
            mainPanel.Controls.Add(prevStockLogsButton);
            mainPanel.Controls.Add(nextStockLogsButton);
            mainPanel.Controls.Add(stockLogsPageLabel);
            stockLogsPage = 1;
            LoadStockLogs();
        }
        private void LoadStockLogs()
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    // Get total count for pagination
                    string countQuery = "SELECT COUNT(*) FROM StockLogs";
                    int totalRecords = 0;
                    using (OleDbCommand countCmd = new OleDbCommand(countQuery, conn))
                    {
                        totalRecords = (int)countCmd.ExecuteScalar();
                    }
                    stockLogsTotalPages = (int)Math.Ceiling(totalRecords / (double)stockLogsPageSize);
                    if (stockLogsPage < 1) stockLogsPage = 1;
                    if (stockLogsPage > stockLogsTotalPages) stockLogsPage = stockLogsTotalPages;
                    int offset = (stockLogsPage - 1) * stockLogsPageSize;
                    // Access SQL does not support OFFSET, so use a subquery for pagination
                    string query = $@"
                        SELECT * FROM (
                            SELECT TOP {stockLogsPageSize} * FROM (
                                SELECT TOP {offset + stockLogsPageSize} * FROM StockLogs ORDER BY StockID DESC
                            ) ORDER BY StockID ASC
                        ) ORDER BY StockID DESC";
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        stockLogsGrid.DataSource = dataTable;
                    }
                    stockLogsPageLabel.Text = $"Page {stockLogsPage} of {Math.Max(stockLogsTotalPages,1)}";
                    prevStockLogsButton.Enabled = stockLogsPage > 1;
                    nextStockLogsButton.Enabled = stockLogsPage < stockLogsTotalPages;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading stock logs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PrevStockLogsButton_Click(object sender, EventArgs e)
        {
            if (stockLogsPage > 1)
            {
                stockLogsPage--;
                LoadStockLogs();
            }
        }
        private void NextStockLogsButton_Click(object sender, EventArgs e)
        {
            if (stockLogsPage < stockLogsTotalPages)
            {
                stockLogsPage++;
                LoadStockLogs();
            }
        }
    }
}
