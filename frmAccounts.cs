using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

public partial class frmAccounts : Form
{
    private string username;
    private int row = -1;

    // Replace with actual SQL Server credentials
    SQLHelperClass sqlHelper = new SQLHelperClass("YOUR_SERVER_NAME", "YOUR_DATABASE_NAME", "YOUR_USERNAME", "YOUR_PASSWORD");

    public frmAccounts(string username)
    {
        InitializeComponent();
        this.username = username;

        this.Draggable(true);
        this.Text = " ";
        this.FormBorderStyle = FormBorderStyle.Sizable;
    }

    private void frmAccounts_Load(object sender, EventArgs e)
    {
        LoadAccounts();
    }

    public void LoadAccounts()
    {
        try
        {
            string query = "SELECT username, password, usertype, status, createdby, datecreated " +
                           "FROM tblaccounts " +
                           "WHERE username <> '" + username + "' " +
                           "ORDER BY username";

            dataGridView1.DataSource = sqlHelper.GetData(query);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error on accounts load", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void txtsearch_TextChanged(object sender, EventArgs e)
    {
        try
        {
            string query = "SELECT username, password, usertype, status, createdby, datecreated " +
                           "FROM tblaccounts " +
                           "WHERE username <> '" + username + "' " +
                           "AND (username LIKE '%" + txtsearch.Text + "%' " +
                           "OR usertype LIKE '%" + txtsearch.Text + "%') " +
                           "ORDER BY username";

            dataGridView1.DataSource = sqlHelper.GetData(query);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error on search", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btn_refresh_Click(object sender, EventArgs e)
    {
        LoadAccounts();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        frmNewaccount newaccountform = new frmNewaccount(this, username);
        newaccountform.Show();
    }

    private void btnupdate_Click(object sender, EventArgs e)
    {
        if (row < 0)
        {
            MessageBox.Show("Please select an account first.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string editusername = dataGridView1.Rows[row].Cells[0].Value.ToString();
        string editpassword = dataGridView1.Rows[row].Cells[1].Value.ToString();
        string editype = dataGridView1.Rows[row].Cells[2].Value.ToString();
        string editstatus = dataGridView1.Rows[row].Cells[3].Value.ToString();

        frmUpdateAccount updateaccountfrm =
            new frmUpdateAccount(this, editusername, editpassword, editype, editstatus, username);

        updateaccountfrm.Show();
    }

    private void btndelete_Click(object sender, EventArgs e)
    {
        if (row < 0)
        {
            MessageBox.Show("Please select an account first.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        DialogResult dr = MessageBox.Show(
            "Are you sure you want to delete this account?",
            "Confirmation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (dr == DialogResult.Yes)
        {
            string selecteduser = dataGridView1.Rows[row].Cells[0].Value.ToString();

            try
            {
                sqlHelper.executeSQL("DELETE FROM tblaccounts WHERE username = '" + selecteduser + "'");

                if (sqlHelper.rowAffected > 0)
                {
                    sqlHelper.executeSQL(
                        "INSERT INTO tbllogs (datelog, timelog, action, module, ID, performedby) VALUES ('" +
                        DateTime.Now.ToShortDateString() + "', '" +
                        DateTime.Now.ToShortTimeString() + "', 'Delete', 'Accounts Management', '" +
                        selecteduser + "', '" + username + "')");

                    MessageBox.Show("Account Deleted", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadAccounts();
                    row = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error on delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        try
        {
            if (e.RowIndex >= 0)
            {
                row = e.RowIndex;

                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error on datagrid cellclick", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            dataGridView1.Rows[e.RowIndex].DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }
    }

    // DESIGN UI EVENTS

    private void btnrefresh_Click(object sender, EventArgs e)
    {
        LoadAccounts();
    }

    private void btn_close_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void btn_minimize_Click(object sender, EventArgs e)
    {
        this.WindowState = FormWindowState.Minimized;
    }

    private void btn_minimize_MouseHover(object sender, EventArgs e)
    {
        btn_minimize.BackColor = Color.Silver;
        btn_minimize.BorderStyle = BorderStyle.FixedSingle;
    }

    private void btn_minimize_MouseLeave(object sender, EventArgs e)
    {
        btn_minimize.BackColor = Color.Transparent;
        btn_minimize.BorderStyle = BorderStyle.None;
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

    private void btn_refresh_MouseHover(object sender, EventArgs e)
    {
        btn_refresh.BackColor = Color.LightBlue;
    }

    private void btn_refresh_MouseLeave_1(object sender, EventArgs e)
    {
        btn_refresh.BackColor = Color.Transparent;
    }

    private void btnAdd_MouseHover(object sender, EventArgs e)
    {
        btnAdd.BackColor = Color.LimeGreen;
    }

    private void btnAdd_MouseLeave(object sender, EventArgs e)
    {
        btnAdd.BackColor = Color.Azure;
    }

    private void btnupdate_MouseHover(object sender, EventArgs e)
    {
        btnupdate.BackColor = Color.FromArgb(0, 78, 168);
    }

    private void btnupdate_MouseLeave(object sender, EventArgs e)
    {
        btnupdate.BackColor = Color.Azure;
    }

    private void btndelete_MouseHover(object sender, EventArgs e)
    {
        btndelete.BackColor = Color.Red;
    }

    private void btndelete_MouseLeave(object sender, EventArgs e)
    {
        btndelete.BackColor = Color.Azure;
    }
}
