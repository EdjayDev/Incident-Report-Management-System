using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

public partial class frmCaseManagement : Form
{
    Database db = new Database("SERVER_NAME", "DATABASE_NAME", "USERNAME", "PASSWORD");

    private string username;
    private string studentnumber, lastname, firstname, middlename, yearlevel, course;
    private string violationcode, violationdescription, status, caseid, resolution, schoolyear, concernlevel, recommendation;
    private int row = -1;

    public frmCaseManagement(string username)
    {
        InitializeComponent();
        this.Draggable(true);
        this.username = username;
        txtstudentid.Focus();
    }

    private void frmCaseManagement_Load(object sender, EventArgs e)
    {
        txtstudentid.Focus();
    }

    private void btn_refresh_Click(object sender, EventArgs e)
    {
        LoadCases();
    }

    public void LoadCases()
    {
        try
        {
            string query =
                "SELECT c.caseID, c.violationID, c.violationCount, v.description, " +
                "c.status, c.resolution, c.schoolyear, c.concernlevel, c.recommendation, c.createdBy, c.dateCreated " +
                "FROM tblcases c " +
                "INNER JOIN tblviolations v ON c.violationID = v.violationcode " +
                "WHERE c.studentID = '" + txtstudentid.Text + "' " +
                "ORDER BY c.caseID DESC";

            DataTable dt = db.GetData(query);
            dataGridView1.DataSource = dt;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error on cases load", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        try
        {
            if (e.RowIndex < 0) return;

            row = e.RowIndex;

            dataGridView1.Rows[e.RowIndex].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error on datagrid cellclick", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.WrapMode = DataGridViewTriState.False;
        dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
    }

    private void txtstudentid_TextChanged(object sender, EventArgs e)
    {
        ClearStudentFields();

        if (string.IsNullOrWhiteSpace(txtstudentid.Text))
        {
            DisableControls();
            dataGridView1.DataSource = null;
            return;
        }

        LoadCases();

        string studentQuery =
            "SELECT studentLN, studentFN, studentMN, yearLevel, studentCourse " +
            "FROM tblstudents " +
            "WHERE studentID = '" + txtstudentid.Text + "'";

        DataTable studentInfo = db.GetData(studentQuery);

        if (studentInfo.Rows.Count > 0)
        {
            EnableControls();

            txtlastname.Text = studentInfo.Rows[0]["studentLN"].ToString();
            txtfirstname.Text = studentInfo.Rows[0]["studentFN"].ToString();
            txtmiddlename.Text = studentInfo.Rows[0]["studentMN"].ToString();
            txtyearlevel.Text = studentInfo.Rows[0]["yearLevel"].ToString();
            txtcourse.Text = studentInfo.Rows[0]["studentCourse"].ToString();
        }
        else
        {
            DisableControls();
        }
    }

    private void ClearStudentFields()
    {
        txtlastname.Clear();
        txtfirstname.Clear();
        txtmiddlename.Clear();
        txtyearlevel.Clear();
        txtcourse.Clear();
    }

    private void EnableControls()
    {
        labelguide.Visible = false;

        btn_refresh.Visible = true;
        btn_refresh.Enabled = true;
        btn_refresh.BackColor = Color.FromArgb(150, 0, 52, 112);

        btnAdd.Visible = true;
        btnupdate.Visible = true;

        paneloptionlist.BackColor = SystemColors.Highlight;

        paneladdcase.Enabled = true;
        panelupdatecase.Enabled = true;

        paneladdcase.BackColor = Color.LimeGreen;
        panelupdatecase.BackColor = Color.FromArgb(89, 133, 225);
    }

    private void DisableControls()
    {
        labelguide.Visible = true;

        btn_refresh.Visible = false;
        btn_refresh.Enabled = false;
        btn_refresh.BackColor = Color.Transparent;

        btnAdd.Visible = false;
        btnupdate.Visible = false;

        paneloptionlist.BackColor = SystemColors.ButtonShadow;

        paneladdcase.Enabled = false;
        panelupdatecase.Enabled = false;

        paneladdcase.BackColor = SystemColors.ButtonShadow;
        panelupdatecase.BackColor = SystemColors.ButtonShadow;
    }

    private void btnclear_Click(object sender, EventArgs e)
    {
        txtstudentid.Clear();
        txtstudentid.Focus();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        studentnumber = txtstudentid.Text;
        lastname = txtlastname.Text;
        firstname = txtfirstname.Text;
        middlename = txtmiddlename.Text;
        yearlevel = txtyearlevel.Text;
        course = txtcourse.Text;

        frmAddCase addCaseForm = new frmAddCase(
            this,
            studentnumber,
            lastname,
            firstname,
            middlename,
            yearlevel,
            course,
            username
        );

        addCaseForm.Show();
    }

    private void btnupdate_Click(object sender, EventArgs e)
    {
        if (row < 0 || dataGridView1.Rows.Count == 0)
        {
            MessageBox.Show("Please select a case first.", "No Selected Case",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        studentnumber = txtstudentid.Text;
        lastname = txtlastname.Text;
        firstname = txtfirstname.Text;
        middlename = txtmiddlename.Text;
        yearlevel = txtyearlevel.Text;
        course = txtcourse.Text;

        caseid = dataGridView1.Rows[row].Cells[0].Value.ToString();
        violationcode = dataGridView1.Rows[row].Cells[1].Value.ToString();
        violationdescription = dataGridView1.Rows[row].Cells[3].Value.ToString();
        status = dataGridView1.Rows[row].Cells[4].Value.ToString();
        resolution = dataGridView1.Rows[row].Cells[5].Value.ToString();
        schoolyear = dataGridView1.Rows[row].Cells[6].Value.ToString();
        concernlevel = dataGridView1.Rows[row].Cells[7].Value.ToString();
        recommendation = dataGridView1.Rows[row].Cells[8].Value.ToString();

        frmUpdateCase updateCaseForm = new frmUpdateCase(
            this,
            studentnumber,
            lastname,
            firstname,
            middlename,
            yearlevel,
            course,
            username,
            violationcode,
            violationdescription,
            status,
            caseid,
            resolution,
            schoolyear,
            concernlevel,
            recommendation
        );

        updateCaseForm.Show();
    }

    // UI Behavior

    private void btnAdd_MouseEnter(object sender, EventArgs e)
    {
        btnAdd.BackColor = Color.LimeGreen;
    }

    private void btnAdd_MouseLeave(object sender, EventArgs e)
    {
        btnAdd.BackColor = Color.Azure;
    }

    private void btnupdate_MouseEnter(object sender, EventArgs e)
    {
        btnupdate.BackColor = Color.FromArgb(73, 116, 225);
    }

    private void btnupdate_MouseLeave(object sender, EventArgs e)
    {
        btnupdate.BackColor = Color.Azure;
    }

    private void btn_refresh_MouseEnter(object sender, EventArgs e)
    {
        btn_refresh.BackColor = Color.LightBlue;
    }

    private void btn_refresh_MouseLeave(object sender, EventArgs e)
    {
        btn_refresh.BackColor = Color.FromArgb(150, 0, 52, 112);
    }

    private void btn_minimize_Click_1(object sender, EventArgs e)
    {
        this.WindowState = FormWindowState.Minimized;
    }

    private void btn_minimize_MouseEnter_1(object sender, EventArgs e)
    {
        btn_minimize.BackColor = Color.Silver;
    }

    private void btn_minimize_MouseLeave_1(object sender, EventArgs e)
    {
        btn_minimize.BackColor = Color.FromArgb(150, 0, 52, 112);
    }

    private void btn_close_Click_1(object sender, EventArgs e)
    {
        this.Close();
    }

    private void btn_close_MouseEnter_1(object sender, EventArgs e)
    {
        btn_close.BackColor = Color.Salmon;
    }

    private void btn_close_MouseLeave_1(object sender, EventArgs e)
    {
        btn_close.BackColor = Color.FromArgb(150, 0, 52, 112);
    }
}
