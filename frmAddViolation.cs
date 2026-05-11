using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

public partial class frmAddViolation : Form
{
    private string username;
    private int errorCount;
    private frmViolations frmViolationsLoad;

    Database db = new Database("SERVER_NAME", "DATABASE_NAME", "USERNAME", "PASSWORD");

    public frmAddViolation(frmViolations frmViolationsLoad, string username)
    {
        InitializeComponent();

        this.username = username;
        this.frmViolationsLoad = frmViolationsLoad;

        this.Draggable(true);

        cmbstatus.SelectedIndex = 0;
        cmbtype.SelectedIndex = -1;
    }

    private void ValidateForm()
    {
        errorProvider1.Clear();
        errorCount = 0;

        string violationCode = txtviolationcode.Text.Trim();
        string description = txtdescription.Text.Trim();

        if (string.IsNullOrWhiteSpace(violationCode))
        {
            errorProvider1.SetError(txtviolationcode, "Violation code is required");
            errorCount++;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            errorProvider1.SetError(txtdescription, "Description is required");
            errorCount++;
        }

        if (cmbstatus.SelectedIndex < 0)
        {
            errorProvider1.SetError(cmbstatus, "Please select a status");
            errorCount++;
        }

        if (cmbtype.SelectedIndex < 0)
        {
            errorProvider1.SetError(cmbtype, "Please select a violation type");
            errorCount++;
        }

        if (errorCount == 0)
        {
            try
            {
                DataTable dt = db.GetData(
                    "SELECT * FROM Violations WHERE ViolationCode = '" + violationCode + "'"
                );

                if (dt.Rows.Count > 0)
                {
                    errorProvider1.SetError(txtviolationcode, "Violation code already exists");
                    errorCount++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }

    private void btnsave_Click(object sender, EventArgs e)
    {
        ValidateForm();

        if (errorCount > 0)
        {
            return;
        }

        DialogResult dr = MessageBox.Show(
            "Are you sure you want to add this violation?",
            "Confirmation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

        if (dr != DialogResult.Yes)
        {
            return;
        }

        try
        {
            string violationCode = txtviolationcode.Text.Trim().ToUpper();
            string description = txtdescription.Text.Trim();
            string type = cmbtype.Text.Trim().ToUpper();
            string status = cmbstatus.Text.Trim().ToUpper();

            db.ExecuteSQL(
                "INSERT INTO Violations " +
                "(ViolationCode, Description, Type, Status, CreatedBy, DateCreated) VALUES (" +
                "'" + violationCode + "', " +
                "'" + description.Replace("'", "''") + "', " +
                "'" + type + "', " +
                "'" + status + "', " +
                "'" + username + "', " +
                "'" + DateTime.Now.ToString("yyyy-MM-dd") + "')"
            );

            if (db.RowAffected > 0)
            {
                db.ExecuteSQL(
                    "INSERT INTO Logs " +
                    "(DateLog, TimeLog, Action, Module, ReferenceID, PerformedBy) VALUES (" +
                    "'" + DateTime.Now.ToString("yyyy-MM-dd") + "', " +
                    "'" + DateTime.Now.ToString("hh:mm tt") + "', " +
                    "'Add', " +
                    "'Violations', " +
                    "'" + violationCode + "', " +
                    "'" + username + "')"
                );

                MessageBox.Show(
                    "Violation added successfully",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                frmViolationsLoad.LoadViolations();

                this.Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Save Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void btnclear_Click(object sender, EventArgs e)
    {
        errorProvider1.Clear();

        txtviolationcode.Clear();
        txtdescription.Clear();

        cmbtype.SelectedIndex = -1;
        cmbstatus.SelectedIndex = 0;

        txtviolationcode.Focus();
    }

    private void btn_close_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void btn_close_MouseEnter(object sender, EventArgs e)
    {
        btn_close.BackColor = Color.Salmon;
    }

    private void btn_close_MouseLeave(object sender, EventArgs e)
    {
        btn_close.BackColor = Color.FromArgb(135, 156, 34, 23);
    }

    private void btn_minimize_Click(object sender, EventArgs e)
    {
        this.WindowState = FormWindowState.Minimized;
    }

    private void btn_minimize_MouseEnter(object sender, EventArgs e)
    {
        btn_minimize.BackColor = Color.Silver;
    }

    private void btn_minimize_MouseLeave(object sender, EventArgs e)
    {
        btn_minimize.BackColor = Color.FromArgb(135, 156, 34, 23);
    }
}
