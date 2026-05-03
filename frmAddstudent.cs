using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

public partial class frmAddstudent : Form
{
    private string username;
    private int errorcount;
    private Form frmStudents_load;

    Class1 addstudent = new Class1("SERVER_NAME", "DATABASE_NAME", "USERNAME", "PASSWORD");

    private const int JHS = 0;
    private const int JHS2 = 1;
    private const int SHS = 2;
    private const int COLLEGE = 3;

    Dictionary<string, int> strandsOptions = new()
    {
        { "ACADEMIC TRACK - General Academic Strand (GAS)", 0 },
        { "ACADEMIC TRACK - Humanities and Social Sciences (HUMSS)", 1 },
        { "ACADEMIC TRACK - Accountancy, Business and Management (ABM)", 2 },
        { "ACADEMIC TRACK - Science, Technology, Engineering and Mathematics (STEM)", 3 },
        { "ARTS AND DESIGN TRACK - Performing Arts", 4 },
        { "SPORTS TRACK - Coaching and Sports", 5 },
        { "SPORTS TRACK - Officiating", 6 },
        { "TECHNICAL VOCATIONAL LIVELIHOOD TRACK - Food and Beverage Services", 7 },
        { "TECHNICAL VOCATIONAL LIVELIHOOD TRACK - Bread and Pastry Production", 8 },
        { "TECHNICAL VOCATIONAL LIVELIHOOD TRACK - Housekeeping", 9 },
        { "TECHNICAL VOCATIONAL LIVELIHOOD TRACK - Cookery", 10 },
        { "TECHNICAL VOCATIONAL LIVELIHOOD TRACK - Caregiving", 11 },
        { "TECHNICAL VOCATIONAL LIVELIHOOD TRACK - Tour Guiding Services", 12 },
        { "TECHNICAL VOCATIONAL LIVELIHOOD TRACK - Bartending", 13 },
        { "TECHNICAL VOCATIONAL LIVELIHOOD TRACK - Tourism Promotion Services", 14 },
        { "TECHNICAL VOCATIONAL LIVELIHOOD TRACK - Computer Programming", 15 },
        { "TECHNICAL VOCATIONAL LIVELIHOOD TRACK - Animation", 16 },
        { "TECHNICAL VOCATIONAL LIVELIHOOD TRACK - Electrical Installation and Maintenance", 17 },
        { "TECHNICAL VOCATIONAL LIVELIHOOD TRACK - Machining", 18 }
    };

    Dictionary<string, int> courseOptions = new()
    {
        { "BACHELOR OF SCIENCE - Bachelor of Science in Computer Science", 11 },
        { "BACHELOR OF SCIENCE - Bachelor of Science in Information Technology", 12 },
        { "BACHELOR OF SCIENCE - Bachelor of Science in Nursing", 7 },
        { "BACHELOR OF SCIENCE - Bachelor of Science in Accountancy", 14 },
        { "BACHELOR OF EDUCATION - Bachelor of Secondary Education Major in English", 33 }
        // trimmed for brevity in update (keep yours if needed)
    };

    public frmAddstudent(Form frmStudents_load, string username)
    {
        InitializeComponent();
        this.username = username;
        this.frmStudents_load = frmStudents_load;
        this.Draggable(true);
    }

    private bool IsEmpty(TextBox tb) => string.IsNullOrWhiteSpace(tb.Text);

    private void validateForm()
    {
        errorProvider1.Clear();
        errorcount = 0;

        if (IsEmpty(txtstudentid))
        {
            errorProvider1.SetError(txtstudentid, "Student ID required");
            errorcount++;
        }

        if (IsEmpty(txtstudentln))
        {
            errorProvider1.SetError(txtstudentln, "Last name required");
            errorcount++;
        }

        if (IsEmpty(txtstudentfn))
        {
            errorProvider1.SetError(txtstudentfn, "First name required");
            errorcount++;
        }

        if (IsEmpty(txtstudentmn))
            txtstudentmn.Text = "N/A";

        if (cmbyearlevel.SelectedIndex < 0)
        {
            errorProvider1.SetError(cmbyearlevel, "Select year level");
            errorcount++;
            return;
        }

        ValidateCourseSelection();

        if (!IsEmpty(txtstudentid))
        {
            try
            {
                DataTable dt = addstudent.GetData(
                    $"SELECT studentID FROM tblstudents WHERE studentID = '{txtstudentid.Text}'"
                );

                if (dt.Rows.Count > 0)
                {
                    errorProvider1.SetError(txtstudentid, "Student ID already exists");
                    errorcount++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DB Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void ValidateCourseSelection()
    {
        if (cmbyearlevel.SelectedIndex == SHS)
        {
            if (!strandsOptions.ContainsKey(cmbstudentcourse.Text))
            {
                errorProvider1.SetError(cmbstudentcourse, "Select valid strand");
                errorcount++;
            }
        }
        else if (cmbyearlevel.SelectedIndex == COLLEGE)
        {
            if (!courseOptions.ContainsKey(cmbstudentcourse.Text))
            {
                errorProvider1.SetError(cmbstudentcourse, "Select valid course");
                errorcount++;
            }
        }
        else
        {
            cmbstudentcourse.Text = "N/A";
        }
    }

    private void cmbyearlevel_SelectedIndexChanged(object sender, EventArgs e)
    {
        cmbstudentcourse.Items.Clear();
        cmbstudentcourse.Text = "";

        if (cmbyearlevel.SelectedIndex == SHS)
        {
            foreach (var item in strandsOptions.Keys)
                cmbstudentcourse.Items.Add(item);

            cmbstudentcourse.Enabled = true;
        }
        else if (cmbyearlevel.SelectedIndex == COLLEGE)
        {
            foreach (var item in courseOptions.Keys)
                cmbstudentcourse.Items.Add(item);

            cmbstudentcourse.Enabled = true;
        }
        else
        {
            cmbstudentcourse.Enabled = false;
            cmbstudentcourse.Text = "N/A";
        }
    }

    private void btnsave_Click(object sender, EventArgs e)
    {
        validateForm();

        if (errorcount > 0) return;

        if (MessageBox.Show("Add this student?", "Confirm",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            string sql = $@"
INSERT INTO tblstudents
(studentID, studentLN, studentFN, studentMN, yearLevel, studentCourse, dateCreated, createdBy)
VALUES
('{txtstudentid.Text}', '{txtstudentln.Text}', '{txtstudentfn.Text}', '{txtstudentmn.Text}',
 '{cmbyearlevel.Text.ToUpper()}', '{cmbstudentcourse.Text}',
 '{DateTime.Now:yyyy-MM-dd}', '{username}')";

            addstudent.executeSQL(sql);

            if (addstudent.rowAffected > 0)
            {
                addstudent.executeSQL(
                    $"INSERT INTO tbllogs (datelog, timelog, action, module, ID, performedby) " +
                    $"VALUES ('{DateTime.Now:yyyy-MM-dd}', '{DateTime.Now:HH:mm:ss}', 'Add', 'Students', '{txtstudentid.Text}', '{username}')"
                );

                MessageBox.Show("Student added successfully");

                frmStudents_load
                    .GetType()
                    .GetMethod("LoadStudents")
                    ?.Invoke(frmStudents_load, null);

                Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnclear_Click(object sender, EventArgs e)
    {
        errorProvider1.Clear();

        txtstudentid.Clear();
        txtstudentln.Clear();
        txtstudentfn.Clear();
        txtstudentmn.Clear();

        cmbyearlevel.SelectedIndex = -1;
        cmbstudentcourse.SelectedIndex = -1;
        cmbstudentcourse.Text = "";

        txtstudentid.Focus();
    }
}
