using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

public partial class frmLogin : Form
{
    public frmLogin()
    {
        InitializeComponent();
        this.Draggable(true); // Assuming this is a custom extension method
    }

    // Placeholder for your database access class (use dependency injection or config-based constructor in real app)
    private readonly Class1 login = new Class1("SERVER_NAME", "DATABASE_NAME", "USERNAME", "PASSWORD");

    private void btnlogin_Click(object sender, EventArgs e)
    {
        errorProvider1.Clear();
    
        bool hasError = false;
    
        if (string.IsNullOrWhiteSpace(txtusername.Text))
        {
            errorProvider1.SetError(txtusername, "Input is empty");
            hasError = true;
        }
    
        if (string.IsNullOrWhiteSpace(txtpassword.Text))
        {
            errorProvider1.SetError(txtpassword, "Input is empty");
            hasError = true;
        }
    
        if (hasError) return;
    
        try
        {
            //Parameterized query
            string query = "SELECT * FROM tblaccounts WHERE username = @username AND password = @password AND status = 'ACTIVE'";
    
            var parameters = new Dictionary<string, object>
            {
                { "@username", txtusername.Text },
                { "@password", txtpassword.Text }
            };
    
            DataTable dt = login.GetData(query, parameters); // assumes overload
    
            if (dt.Rows.Count > 0)
            {
                string username = dt.Rows[0]["username"].ToString();
                string usertype = dt.Rows[0]["usertype"].ToString();
    
                var mainForm = new frmMain(username, usertype);
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Incorrect account information or account is inactive",
                    "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error on login",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void chkboxShow_CheckedChanged(object sender, EventArgs e)
    {
        txtpassword.PasswordChar = chkboxShow.Checked ? '\0' : '*';
    }

    private void btnreset_Click(object sender, EventArgs e)
    {
        txtusername.Clear();
        txtpassword.Clear();
        errorProvider1.Clear();
        txtusername.Focus();
    }

    private void txtpassword_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            btnlogin_Click(sender, e);
        }
    }

    // -- DESIGN BUTTON EVENTS --

    private void btn_close_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void btn_minimize_Click(object sender, EventArgs e)
    {
        this.WindowState = FormWindowState.Minimized;
    }

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

    private void btnlogin_MouseHover(object sender, EventArgs e)
    {
        btnlogin.BackColor = Color.DodgerBlue;
        pictureBoxbtnlogin.BackColor = Color.DodgerBlue;
    }

    private void btnlogin_MouseLeave(object sender, EventArgs e)
    {
        btnlogin.BackColor = Color.FromArgb(0, 103, 184);
        pictureBoxbtnlogin.BackColor = Color.FromArgb(0, 103, 184);
    }

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
