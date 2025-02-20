using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication10.Models;

namespace WebApplication10.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext appDbContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        public EmployeeController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment)
        {
            this.appDbContext = appDbContext;
            this.webHostEnvironment = webHostEnvironment;
        }

        //public async Task<IActionResult> Index(string search)
        //{
        //    var emp = await appDbContext.Employees
        //        .Where(x => string.IsNullOrEmpty(search) || x.Name.Contains(search))
        //        .ToListAsync();
        //    ViewData["Search"] = search;
        //    return View(emp);
        //}

        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 5)
        {
            var query = appDbContext.Employees
                .Where(x => string.IsNullOrEmpty(search) || x.Name.Contains(search));

            int totalRecords = await query.CountAsync();
            var emp = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["Search"] = search;
            ViewData["Page"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return View(emp);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Employee employee, IFormFile formFile)
        {
            if (formFile != null && formFile.Length > 0)
            {
                string uploadPath = Path.Combine(webHostEnvironment.WebRootPath, "Images");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                string filePath = Path.Combine(uploadPath, fileName);
                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }
                employee.Img = "/Images/" + fileName;
            }
            appDbContext.Employees.Add(employee);
            appDbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var emp = appDbContext.Employees.Find(id);
            return View(emp);
        }
        [HttpPost]
        public IActionResult Edit(Employee employee, IFormFile formFile)
        {
            if (formFile!=null && formFile.Length>0)
            {
                string uploadPath = Path.Combine(webHostEnvironment.WebRootPath, "Images");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                string filePath = Path.Combine(uploadPath, fileName);
                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }
                employee.Img = "/Images/" + fileName;
            }
            appDbContext.Employees.Update(employee);
            appDbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var emp = appDbContext.Employees.Find(id);
            appDbContext.Remove(emp);
            appDbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
