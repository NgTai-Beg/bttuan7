using DAL.database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class FacultyService
    {
        public List<Faculty> GetAll()
        {
            Model1 context = new Model1();
            return context.Faculties.ToList();
        }

        public Faculty FindById(int facultyId)
        {
            Model1 context = new Model1();
            return context.Faculties.FirstOrDefault(f => f.FacultyID == facultyId);
        }

        public void Insert(Faculty faculty)
        {
            Model1 context = new Model1();
            context.Faculties.Add(faculty);
            context.SaveChanges();
        }

        public void Update(Faculty faculty)
        {
            Model1 context = new Model1();
            var existingFaculty = context.Faculties.FirstOrDefault(f => f.FacultyID == faculty.FacultyID);
            if (existingFaculty != null)
            {
                existingFaculty.FacultyName = faculty.FacultyName;
                context.SaveChanges();
            }
        }

        public void Delete(int facultyId)
        {
            Model1 context = new Model1();
            var faculty = context.Faculties.FirstOrDefault(f => f.FacultyID == facultyId);
            if (faculty != null)
            {
                context.Faculties.Remove(faculty);
                context.SaveChanges();
            }
        }
        public List<Major> GetAllMajors()
        {
            using (var context = new Model1())
            {
                return context.Majors.ToList();
            }
        }


    }
}
