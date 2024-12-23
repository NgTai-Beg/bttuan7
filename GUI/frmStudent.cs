using BUS;
using DAL.database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class frmStudent : Form
    {
        public frmStudent()
        {
            InitializeComponent();
        }

        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private void FillMajorCombobox(List<Major> listMajors)
        {
            listMajors.Insert(0, new Major { MajorID = 0, Name = "" });
            cmbMajor.DataSource = listMajors;
            cmbMajor.DisplayMember = "Name";
            cmbMajor.ValueMember = "MajorID";
        }

        private void frmStudent_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dgvStudent);

                var listFacultys = facultyService.GetAll();
                if (listFacultys != null)
                    FillFacultyCombobox(listFacultys);

                var listMajors = facultyService.GetAllMajors(); // Lấy danh sách chuyên ngành
                if (listMajors != null)
                    FillMajorCombobox(listMajors);

                var listStudents = studentService.GetAll();
                if (listStudents != null)
                    BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillFacultyCombobox(List<Faculty> listFacultys)
        {
            cmbFaculty.DataSource = listFacultys;
            cmbFaculty.DisplayMember = "FacultyName";
            cmbFaculty.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear(); // Xóa dữ liệu cũ
            foreach (var student in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = student.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = student.FullName;
                dgvStudent.Rows[index].Cells[2].Value = student.Faculty?.FacultyName;
                dgvStudent.Rows[index].Cells[3].Value = (student.AverageScore ?? 0).ToString("0.00"); // Định dạng 2 chữ số thập phân
                dgvStudent.Rows[index].Cells[4].Value = student.Major?.Name;
            }
        }

        public void setGridViewStyle(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.DarkTurquoise;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.BackgroundColor = System.Drawing.Color.White;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void chkUnregisterMajor_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.chkUnregisterMajor.Checked)
                listStudents = studentService.GetAllHasNoMajor() ?? new List<Student>();
            else
                listStudents = studentService.GetAll() ?? new List<Student>();

            BindGrid(listStudents);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var student = new Student
                {
                    StudentID = int.Parse(txtStudentID.Text),
                    FullName = txtFullName.Text,
                    AverageScore = float.Parse(txtAverageScore.Text),
                    FacultyID = (int)cmbFaculty.SelectedValue,
                    MajorID = (int?)cmbMajor.SelectedValue
                };

                studentService.Add(student); // Gọi phương thức thêm
                MessageBox.Show("Them sinh viên thành công!");
                BindGrid(studentService.GetAll()); // Làm mới DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể thêm sinh viên. Lỗi: {ex.Message}");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                int studentID = int.Parse(txtStudentID.Text);
                var student = studentService.GetById(studentID);

                if (student != null)
                {
                    student.FullName = txtFullName.Text;
                    student.AverageScore = float.Parse(txtAverageScore.Text);
                    student.FacultyID = (int)cmbFaculty.SelectedValue;
                    student.MajorID = (int?)cmbMajor.SelectedValue;

                    studentService.Update(student); // Gọi phương thức cập nhật
                    MessageBox.Show("Cập nhật sinh viên thành công!");
                    BindGrid(studentService.GetAll()); // Làm mới DataGridView
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên để cập nhật!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể cập nhật sinh viên. Lỗi: {ex.Message}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int studentID = int.Parse(txtStudentID.Text);
                studentService.Delete(studentID); // Gọi phương thức xóa
                MessageBox.Show("Xóa sinh viên thành công!");
                BindGrid(studentService.GetAll()); // Làm mới DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvStudent.Rows[e.RowIndex];

                txtStudentID.Text = row.Cells[0].Value?.ToString();
                txtFullName.Text = row.Cells[1].Value?.ToString();
                txtAverageScore.Text = row.Cells[3].Value?.ToString();

                // Lấy FacultyID thay vì FacultyName
                var studentID = int.Parse(txtStudentID.Text);
                var student = studentService.GetById(studentID);

                if (student != null)
                {
                    cmbFaculty.SelectedValue = student.FacultyID; // Gán FacultyID vào ComboBox
                }

                // Gán Major nếu có
                if (row.Cells[4].Value == null || string.IsNullOrEmpty(row.Cells[4].Value.ToString()))
                {
                    chkUnregisterMajor.Checked = true;
                    cmbMajor.SelectedIndex = 0;
                }
                else
                {
                    chkUnregisterMajor.Checked = false;
                    cmbMajor.Text = row.Cells[4].Value?.ToString();
                }
            }
        }

        private void chkUnregisterMajor_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkUnregisterMajor.Checked)
            {
                cmbMajor.SelectedIndex = 0;
            }
        }
    }
}
