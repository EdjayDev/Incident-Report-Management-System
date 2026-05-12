using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

public partial class frmLogin : Form
{
    // Database instance
    private readonly Class1 login =
        new Class1("SERVER_NAME", "DATABASE_NAME", "USERNAME", "PASSWORD");

    public frmLogin()
    {
        InitializeComponent();

        this.Draggable(true); // Custom draggable extension
        this.AcceptButton = btnlogin;

        txtpassword.PasswordChar = '*';
    }

    private void btnlogin_Click(object sender, EventArgs e)
    {
        ValidateLogin();
    }

    private void ValidateLogin()
    {
        errorProvider1.Clear();

        bool hasError = false;

        string username = txtusername.Text.Trim();
        string password = txtpassword.Text;

        // Username validation
        if (string.IsNullOrWhiteSpace(username))
        {
            errorProvider1.SetError(txtusername, "Username is required");
            hasError = true;
        }

        // Password validation
        if (string.IsNullOrWhiteSpace(password))
        {
            errorProvider1.SetError(txtpassword, "Password is required");
            hasError = true;
        }

        if (hasError)
            return;

        try
        {
            string query = @"
                SELECT username, usertype
                FROM tblaccounts
                WHERE username = @username
                AND password = @password
                AND status = 'ACTIVE'";

            var parameters = new Dictionary<string, object>
            {
                { "@username", username },
                { "@password", password }
            };

            DataTable dt = login.GetData(query, parameters);

            if (dt.Rows.Count > 0)
            {
                string accountUsername = dt.Rows[0]["username"].ToString();
                string usertype = dt.Rows[0]["usertype"].ToString();

                frmMain mainForm = new frmMain(accountUsername, usertype);

                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show(
                    "Incorrect username/password or inactive account.",
                    "Login Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtpassword.Clear();
                txtpassword.Focus();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Login Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void chkboxShow_CheckedChanged(object sender, EventArgs e)
    {
        txtpassword.PasswordChar =
            chkboxShow.Checked ? '\0' : '*';
    }

    private void btnreset_Click(object sender, EventArgs e)
    {
        txtusername.Clear();
        txtpassword.Clear();

        chkboxShow.Checked = false;

        errorProvider1.Clear();

        txtusername.Focus();
    }

    private void txtpassword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            ValidateLogin();
        }
    }

    // =========================
    // WINDOW BUTTONS
    // =========================

    private void btn_close_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private void btn_minimize_Click(object sender, EventArgs e)
    {
        this.WindowState = FormWindowState.Minimized;
    }

    // =========================
    // CLOSE BUTTON EFFECTS
    // =========================

    private void btn_close_MouseHover(object sender, EventArgs e)
    {
        btn_close.BackColor = Color.Salmon;
        btn_close.BorderStyle = BorderStyle.FixedSingle;
    }

    private void btn_close_MouseLeave(object sender, EventArgs e)
    {
        btn_close.BackColor = Color.Transparent;
        btn_close.BorderStyle = BorderStyle.None;
    }

    // =========================
    // MINIMIZE BUTTON EFFECTS
    // =========================

    private void btn_minimize_MouseHover(object sender, EventArgs e)
    {
        btn_minimize.BackColor = Color.LightGray;
        btn_minimize.BorderStyle = BorderStyle.FixedSingle;
    }

    private void btn_minimize_MouseLeave(object sender, EventArgs e)
    {
        btn_minimize.BackColor = Color.Transparent;
        btn_minimize.BorderStyle = BorderStyle.None;
    }

    // =========================
    // LOGIN BUTTON EFFECTS
    // =========================

    private void btnlogin_MouseHover(object sender, EventArgs e)
    {
        btnlogin.BackColor = Color.DodgerBlue;
        pictureBoxbtnlogin.BackColor = Color.DodgerBlue;
    }

    private void btnlogin_MouseLeave(object sender, EventArgs e)
    {
        Color defaultColor = Color.FromArgb(0, 103, 184);

        btnlogin.BackColor = defaultColor;
        pictureBoxbtnlogin.BackColor = defaultColor;
    }

    // =========================
    // RESET BUTTON EFFECTS
    // =========================

    private void btnreset_MouseHover(object sender, EventArgs e)
    {
        btnreset.BackColor = Color.Red;
        pictureBoxbtnreset.BackColor = Color.Red;
    }

    private void btnreset_MouseLeave(object sender, EventArgs e)
    {
        btnreset.BackColor = Color.Firebrick;
        pictureBoxbtnreset.BackColor = Color.Firebrick;
    }
}
