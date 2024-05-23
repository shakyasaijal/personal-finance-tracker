using System;
using System.Windows.Forms;

namespace PersonalFinanceTracker
{
    public partial class EditTransactionForm : Form
    {
        public Transaction EditedTransaction { get; private set; }
        private int transactionId; // Store the original transaction ID

        public EditTransactionForm(Transaction transaction)
        {
            InitializeComponent();

            // Populate form fields with existing transaction data
            dateTimePickerDate.Value = transaction.Date;
            textBoxDescription.Text = transaction.Description;
            comboBoxCategory.SelectedItem = transaction.Category;
            textBoxAmount.Text = transaction.Amount.ToString();

            // Store the original transaction ID
            transactionId = transaction.Id;
        }

        private void buttonModify_Click(object sender, EventArgs e)
        {
            // Validate input
            if (!ValidateInput())
                return;

            // Update transaction
            DateTime date = dateTimePickerDate.Value;
            string description = textBoxDescription.Text;
            string category = comboBoxCategory.SelectedItem.ToString();
            decimal amount;
            if (!decimal.TryParse(textBoxAmount.Text, out amount))
            {
                MessageBox.Show("Invalid amount. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            EditedTransaction = new Transaction
            {
                Id = transactionId, // Id will be set in the main form after adding
                Date = date,
                Description = description,
                Category = category,
                Amount = amount
            };

            // Close the form
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private bool ValidateInput()
        {
            // Validate amount
            if (!decimal.TryParse(textBoxAmount.Text, out decimal amount))
            {
                MessageBox.Show("Invalid amount. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void EditTransactionForm_Load(object sender, EventArgs e)
        {
            // Optional: Code that runs when the form loads
        }

        private void labelDescription_Click(object sender, EventArgs e)
        {

        }

        private void textBoxAmount_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
