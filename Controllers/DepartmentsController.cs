using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RingoMediaTask.Data;
using RingoMediaTask.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RingoMediaTask.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Departments
        public IActionResult Index()
        {
            return View(_context.Departments.ToList());
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            ViewBag.ParentDepartments = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,ParentDepartmentId,SubDepartments")] 
        Department department, IFormFile logo, List<IFormFile> subDepartmentLogos)
        {
            // Remove validation errors for optional fields //bypassing asp actions
            ModelState.Remove("Logo");
            ModelState.Remove("ParentDepartment");
            //ModelState.Remove("SubDepartments[0].Logo");//we can add a loop depends on count after testing it
            //ModelState.Remove("SubDepartments[0].ParentDepartment");
            for (int i = 0; i < department.SubDepartments.Count; i++)
            {
                ModelState.Remove("SubDepartments["+ i +"].Logo");
                ModelState.Remove("SubDepartments["+ i +"].ParentDepartment");
            }


            if (!ModelState.IsValid)
            {
                // Debugging validation errors
                foreach (var error in ModelState)
                {
                    var key = error.Key;
                    var errors = error.Value.Errors;
                    foreach (var e in errors)
                    {
                        Console.WriteLine($"{key}: {e.ErrorMessage}");
                    }
                }

                // Repopulate ViewBag for dropdown
                ViewBag.Departments = new SelectList(_context.Departments, "Id", "Name", department.ParentDepartmentId);
                return View(department);
            }

            // Process department logo
            if (logo != null && logo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await logo.CopyToAsync(ms);
                    department.Logo = ms.ToArray();
                }
            }

            // Process sub-department logos
            if (department.SubDepartments != null && subDepartmentLogos != null)
            {
                for (int i = 0; i < department.SubDepartments.Count; i++)
                {
                    if (i < subDepartmentLogos.Count && subDepartmentLogos[i] != null && subDepartmentLogos[i].Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await subDepartmentLogos[i].CopyToAsync(ms);
                            department.SubDepartments[i].Logo = ms.ToArray();
                        }
                    }
                }
            }

            // Add department to the context and save changes
            _context.Add(department);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            ViewBag.ParentDepartments = new SelectList(_context.Departments, "Id", "Name", department.ParentDepartmentId);
            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ParentDepartmentId")] Department department, IFormFile Logo)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (Logo != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await Logo.CopyToAsync(memoryStream);
                            department.Logo = memoryStream.ToArray();
                        }
                    }
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ParentDepartments = new SelectList(_context.Departments, "Id", "Name", department.ParentDepartmentId);
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var department = await _context.Departments
                .Include(d => d.SubDepartments)
                .Include(d => d.ParentDepartment)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            var parentDepartments = new List<Department>();
            var current = department;
            while (current.ParentDepartment != null)
            {
                parentDepartments.Add(current.ParentDepartment);
                current = current.ParentDepartment;
            }

            ViewBag.ParentDepartments = parentDepartments;

            return View(department);
        }
        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
