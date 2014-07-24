using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using WindyCityUniversity.DAL;
using WindyCityUniversity.Models;

namespace WindyCityUniversity.Controllers
{
    public class ReportController : Controller
    {
        private SchoolContext db = new SchoolContext();
        public ActionResult Index()
        {
            return View();
        }

        // GET: Report
        public ActionResult SharedCourses(int? page)
        {
            var students = GetSharedStudents().OrderBy(s => s.LastName);
           
            int pageSize = 15;
            int pageNumber = (page ?? 1);

            return View(students.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult SharedCoursesPdf()
        {
            var students = GetSharedStudents();

            //return new PdfActionResult(students);
            return View(students);
        }

        private List<Student> GetSharedStudents()
        {
            var students = new List<Student>();

            var results = (from e1 in db.Enrollments
                           join cmte1 in db.Enrollments on e1.CourseId equals cmte1.CourseId
                           join s in
                               (
                                   from e2 in db.Enrollments
                                   join cmte2 in db.Enrollments on e2.CourseId equals cmte2.CourseId
                                   where e2.StudentId != cmte2.StudentId
                                   group new { e2, cmte2 } by new { StudentId = e2.StudentId, ClassmateId = cmte2.StudentId }
                                       into grp
                                       where grp.Count() > 1
                                       select new { StudentId = grp.Key.StudentId, ClassmateId = grp.Key.ClassmateId }
                                   ) on new { StudentId = e1.StudentId, ClassmateId = cmte1.StudentId } equals
                               new { s.StudentId, s.ClassmateId }
                           where e1.StudentId != cmte1.StudentId
                           select new { Student = e1.Student, Classmate = cmte1.Student, SharedCourse = cmte1.Course }).ToList();

            foreach (var result in results)
            {
                var student = students.FirstOrDefault(s => s.Id == result.Student.Id);
                if (student == null)
                {
                    students.Add(result.Student);
                }

                student = students.FirstOrDefault(s => s.Id == result.Student.Id);

                var classmate = student.Classmates.FirstOrDefault(c => c.Student != null && c.Student.Id == result.Classmate.Id);

                if (classmate == null)
                {
                    student.Classmates.Add(new Classmate { Student = result.Classmate });
                }

                classmate = student.Classmates.FirstOrDefault(c => c.Student != null && c.Student.Id == result.Classmate.Id);

                var sharedCourse = classmate.SharedCourses.FirstOrDefault(sc => sc.Id == result.SharedCourse.Id);
                if (sharedCourse == null)
                {
                    classmate.SharedCourses.Add(result.SharedCourse);
                }
            }

            return students;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
