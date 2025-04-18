using Cumulative01.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cumulative01.Controllers
{
    public class TeacherPageController : Controller
    {
        // Using the API controller directly
        private readonly TeacherAPIController _api;

        public TeacherPageController(TeacherAPIController api)
        {
            _api = api;
        }

        // GET: TeacherPage/List?SearchKey=...
        [HttpGet]
        public IActionResult List(string SearchKey)
        {
            List<Teacher> Teachers = _api.ListTeacherNames(SearchKey);
            return View(Teachers);
        }

        // GET: TeacherPage/Show/{id}
        [HttpGet]
        public IActionResult Show(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }

        // GET: TeacherPage/New
        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        // POST: TeacherPage/Create
        [HttpPost]
        public IActionResult Create(Teacher NewTeacher)
        {
            int TeacherId = _api.AddTeacher(NewTeacher);
            return RedirectToAction("Show", new { id = TeacherId });
        }

        // GET: TeacherPage/DeleteConfirm/{id}
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View(SelectedTeacher);
        }

        // POST: TeacherPage/Delete/{id}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            _api.DeleteTeacher(id);
            return RedirectToAction("List");
        }
        // GET: TeacherPage/Edit/{id}
[HttpGet]
public IActionResult Edit(int id)
{
    Teacher SelectedTeacher = _api.FindTeacher(id);
    return View(SelectedTeacher);
}

// POST: TeacherPage/Update/{id}
[HttpPost]
public IActionResult Update(int id, string TeacherFirstName, string TeacherLastName, string EmployeeID, DateTime HireDate, decimal Salary)
{
    Teacher UpdatedTeacher = new Teacher();
    UpdatedTeacher.TeacherFirstName = TeacherFirstName;
    UpdatedTeacher.TeacherLastName = TeacherLastName;
    UpdatedTeacher.EmployeeID = EmployeeID;
    UpdatedTeacher.HireDate = HireDate;
    UpdatedTeacher.Salary = (double)Salary;

    _api.UpdateTeacher(id, UpdatedTeacher);

    return RedirectToAction("Show", new { id = id });
}

        
    }
}
