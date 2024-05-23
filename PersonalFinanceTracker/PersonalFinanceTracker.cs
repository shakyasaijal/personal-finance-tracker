using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PersonalFinanceTracker
{
    public partial class PersonalFinanceTracker : Form
    {
        private List<Transaction> transactions = new List<Transaction>();
        private const string DataFilePath = "transactions.csv";

        public PersonalFinanceTracker()
        {
            InitializeComponent();
            LoadTransactions();
        }


        private void LoadTransactions()
        {
            if (File.Exists(DataFilePath))
            {
                var lines = File.ReadAllLines(DataFilePath);
                foreach (var line in lines)
                {
                    var data = line.Split(',');
                    transactions.Add(new Transaction
                    {
                        Id = int.Parse(data[0]),
                        Date = DateTime.Parse(data[1]),
                        Description = data[2],
                        Category = data[3],
                        Amount = decimal.Parse(data[4])
                    });
                }
            }
            RefreshDataGridView();
            LoadPieChartFromDataTable();
        }

        private void SaveTransactions()
        {
            var lines = transactions.Select(t => $"{t.Id},{t.Date},{t.Description},{t.Category},{t.Amount}").ToList();
            File.WriteAllLines(DataFilePath, lines);
            LoadPieChartFromDataTable();
        }

       
        private void DeleteTransaction()
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                // Confirm deletion
                DialogResult result = MessageBox.Show("Are you sure you want to delete this transaction?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if(result == DialogResult.Yes)
                {
                    int selectedRowIndex = dataGridView.SelectedRows[0].Index;
                    transactions.RemoveAt(selectedRowIndex);
                    ReorderTransactionIds();
                    RefreshDataGridView();
                    SaveTransactions();
                    LoadPieChartFromDataTable();
                }
                
            }
            else
            {
                MessageBox.Show("Please select a transaction to delete.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void RefreshDataGridView()
        {
            dataGridView.DataSource = null;
            dataGridView.DataSource = transactions;
        }

        private void SearchTransactions()
        {
            var keyword = txtSearch.Text.ToLower();
            var result = transactions.Where(t =>
                t.Id.ToString().Contains(keyword) ||
                t.Date.ToString().Contains(keyword) ||
                t.Description.ToLower().Contains(keyword) ||
                t.Category.ToLower().Contains(keyword) ||
                t.Amount.ToString().Contains(keyword)).ToList();
            dataGridView.DataSource = null;
            dataGridView.DataSource = result;
            LoadPieChartFromDataTable();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            using (var form = new AddTransactionForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    form.Transaction.Id = transactions.Count > 0 ? transactions.Max(t => t.Id) + 1 : 1;
                    transactions.Add(form.Transaction);
                    UpdateDataGridView();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteTransaction();
            LoadPieChartFromDataTable();
        }

        private void UpdateDataGridView(List<Transaction> data = null)
        {
            dataGridView.DataSource = null;
            dataGridView.DataSource = data ?? transactions;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchTransactions();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    transactions.Clear();
                    var lines = File.ReadAllLines(openFileDialog.FileName);
                    foreach (var line in lines)
                    {
                        var data = line.Split(',');
                        transactions.Add(new Transaction
                        {
                            Id = int.Parse(data[0]),
                            Date = DateTime.Parse(data[1]),
                            Description = data[2],
                            Category = data[3],
                            Amount = decimal.Parse(data[4])
                        });
                    }
                    RefreshDataGridView();
                    LoadPieChartFromDataTable();
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var lines = transactions.Select(t => $"{t.Id},{t.Date},{t.Description},{t.Category},{t.Amount}").ToList();
                    File.WriteAllLines(saveFileDialog.FileName, lines);
                }
            }
        }

        private void PersonalFinanceTracker_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Do you want to save changes before closing?", "Save Changes", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {
                SaveTransactions();
            }
            else if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void ReorderTransactionIds()
        {
            for (int i = 0; i < transactions.Count; i++)
            {
                transactions[i].Id = i + 1;
            }
        }

        private void panelTop_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DeleteTransaction();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DeleteTransaction();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridView.SelectedRows[0].Index;
                Transaction selectedTransaction = transactions[selectedIndex];

                using (EditTransactionForm editForm = new EditTransactionForm(selectedTransaction))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        Transaction editedTransaction = editForm.EditedTransaction;
                        transactions[selectedIndex] = editedTransaction;

                        // Update DataGridView
                        UpdateDataGridView();
                    }
                }
                LoadPieChartFromDataTable();
            }
            else
            {
                MessageBox.Show("Select a transaction to modify.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void chart_Click(object sender, EventArgs e)
        {

        }

        private void panelTop_Paint_1(object sender, PaintEventArgs e)
        {

        }
        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Reload pie chart when dataGridView data changes
            LoadPieChartFromDataTable();
        }

        private void dataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            LoadPieChartFromDataTable();
        }
    }
}
