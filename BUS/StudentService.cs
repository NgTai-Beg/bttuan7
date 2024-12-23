using DAL.database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class StudentService
    {
        public List<Student> GetAll()
        {
            using (var context = new Model1())
            {
                return context.Students
                    .Include(s => s.Faculty) // Nạp thông tin Khoa
                    .Include(s => s.Major) // Nạp thông tin Chuyên Ngành
                    .ToList();
            }
        }

        public List<Student> GetAllHasNoMajor()
        {
            Model1 context = new Model1();
            return context.Students.Where(p => p.MajorID == null).ToList();
        }

        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            Model1 context = new Model1();
            return context.Students.Where(p => p.MajorID == null && p.FacultyID == facultyID).ToList();
        }

        public Student FindById(int studentId) // Thay đổi kiểu dữ liệu từ string sang int
        {
            Model1 context = new Model1();
            return context.Students.FirstOrDefault(p => p.StudentID == studentId); // So sánh int với int
        }



        public void InsertUpdate(Student s)
        {
            Model1 context = new Model1();
            context.Students.AddOrUpdate(s); // Chỉ hoạt động nếu sử dụng Entity Framework
            context.SaveChanges();
        }
        public void Add(Student student)
        {
            using (var context = new Model1())
            {
                context.Students.Add(student); // Thêm sinh viên vào DbContext
                context.SaveChanges(); // Lưu thay đổi vào SQL
            }
        }

        public void Update(Student student)
        {
            using (var context = new Model1())
            {
                var existingStudent = context.Students.FirstOrDefault(s => s.StudentID == student.StudentID);
                if (existingStudent != null)
                {
                    existingStudent.FullName = student.FullName;
                    existingStudent.AverageScore = student.AverageScore;
                    existingStudent.FacultyID = student.FacultyID;
                    existingStudent.MajorID = student.MajorID;
                    context.SaveChanges(); // Lưu thay đổi
                }
            }
        }


        public void Delete(int studentID)
        {
            using (var context = new Model1())
            {
                var student = context.Students.FirstOrDefault(s => s.StudentID == studentID);
                if (student != null)
                {
                    context.Students.Remove(student); // Xóa sinh viên
                    context.SaveChanges(); // Lưu thay đổi
                }
            }
        }


        public Student GetById(int studentID)
        {
            Model1 context = new Model1();
            return context.Students.Find(studentID);
        }

        public void Save()
        {
            Model1 context = new Model1();
            context.SaveChanges();
        }
    }
}
