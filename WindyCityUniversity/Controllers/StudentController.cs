using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using WindyCityUniversity.DAL;
using WindyCityUniversity.Models;
using WindyCityUniversity.Service;

namespace WindyCityUniversity.Controllers
{
    public class StudentController : Controller
    {
        private SchoolContext db = new SchoolContext();
        private List<Course> validCourses = new List<Course>();

        // GET: /Student/
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var students = from s in db.Students
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.ToUpper().Contains(searchString.ToUpper())
                                       || s.FirstName.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                default:  // Name ascending 
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(students.ToPagedList(pageNumber, pageSize));
        }

        // GET: /Student/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: /Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName, FirstName")]Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(student);
        }

        // GET: /Student/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: /Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExternalID, LastName, FirstName")]Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(student).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(student);
        }
        // GET: /Student/Delete/5
        public ActionResult Delete(Guid? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            Student student = db.Students.Find(id);

            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: /Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult BulkLoad()
        {
            var students = new List<Student>();
            var _bulkLoadService = new BulkLoadService();

            _bulkLoadService.Load(students);

            return View();
        }

        [HttpPost]
        public ActionResult UploadStudentsFile()
        {
            if (Request.Files.Count > 0)
            {
                var students = new List<Student>();
                
                validCourses = db.Courses.ToList();

                HttpPostedFileBase file = Request.Files[0];

                var reader = new StreamReader(file.InputStream);
                do
                {
                    string line = reader.ReadLine();
                    var studentInfo = line.Split('|');

                    if (studentInfo.Count() == 4)
                    {
                        var student = new Student
                        {
                            ExternalId = int.Parse(studentInfo[0]),
                            FirstName = studentInfo[1],
                            LastName = studentInfo[2]
                        };
                        
                        var enrollementInfo = studentInfo[3].Split(',');
                        if (enrollementInfo.Length > 0)
                        {
                            student.Enrollments = new List<Enrollment>();

                            foreach (var enrollementItem in enrollementInfo)
                            {
                                var enrollement = PraseEnrollement(enrollementItem);
                                if (enrollement != null)
                                {
                                    student.Enrollments.Add(enrollement);    
                                }
                            }
                        }

                        students.Add(student);
                    }

                } while (reader.Peek() != -1);

                reader.Close();

                var _bulkLoadService = new BulkLoadService();

                _bulkLoadService.Load(students);
            }

            return RedirectToAction("Index");
        }

        private Enrollment PraseEnrollement(string enrollementString)
        {
            enrollementString = enrollementString.TrimEnd(')');

            var values = enrollementString.Split('(');
            Enrollment enrollement = null;
            var course = validCourses.FirstOrDefault(c => c.Code == values[0]);

            if (course != null)
            {
                enrollement = new Enrollment
                {
                    CourseId = course.Id,
                    GPA = float.Parse(values[1])
                };
            }

            return enrollement;
        }
    }
}
