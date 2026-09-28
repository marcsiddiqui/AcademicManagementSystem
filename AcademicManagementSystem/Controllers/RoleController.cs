using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using AcademicManagementSystem.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AcademicManagementSystem.Controllers
{
    public class RoleController : Controller
    {
        private const int PageSize = 10;

        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly RoleService _roleService;

        public RoleController(ApplicationDbContext dbContext, IMapper mapper, RoleService roleService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _roleService = roleService;
        }

        public async Task<IActionResult> Index(RoleListModel model)
        {
            if (model.PageNumber < 1)
                model.PageNumber = 1;

            var pagedData = await _roleService.GetAllRolesAsync(
                search: model.SearchText,
                status: model.StatusId,
                sortById: model.SortById,
                pageNumber: model.PageNumber,
                pageSize: PageSize);

            model.Roles = _mapper.Map<List<RoleModel>>(pagedData.Records);
            model.PageSize = PageSize;
            model.TotalPages = pagedData.TotalPages;
            model.TotalRecords = pagedData.TotalRecords;
            model.HasNextPage = model.PageNumber < model.TotalPages;
            model.HasPreviousPage = model.PageNumber > 1;
            model.ShowingFrom = model.TotalRecords == 0 ? 0 : ((model.PageNumber - 1) * model.PageSize) + 1;
            model.ShowingTo = Math.Min(model.PageNumber * model.PageSize, model.TotalRecords);

            return View(model);
        }

        public IActionResult Create()
        {
            return View(new RoleModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleModel model)
        {
            await ValidateRoleNameAsync(model);

            if (!ModelState.IsValid)
                return View(model);

            var role = _mapper.Map<Role>(model);
            _dbContext.Role.Add(role);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0)
                return RedirectToAction(nameof(Index));

            var role = await _dbContext.Role.FindAsync(id);
            if (role == null)
                return RedirectToAction(nameof(Index));

            return View(_mapper.Map<RoleModel>(role));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detail(RoleModel model)
        {
            await ValidateRoleNameAsync(model, model.Id);

            var role = await _dbContext.Role.FindAsync(model.Id);
            if (role == null)
                return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid)
                return View(model);

            _mapper.Map(model, role);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var role = await _dbContext.Role.FindAsync(id);
            if (role == null)
                return RedirectToAction(nameof(Index));

            return View(_mapper.Map<RoleModel>(role));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(RoleModel model)
        {
            var role = await _dbContext.Role.FindAsync(model.Id);
            if (role == null)
                return RedirectToAction(nameof(Index));

            _dbContext.Role.Remove(role);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task ValidateRoleNameAsync(RoleModel model, int? excludedId = null)
        {
            if (!string.IsNullOrWhiteSpace(model.Name) &&
                !Regex.IsMatch(model.Name, @"^[\p{L}]+(?:[ .'-][\p{L}]+)*$"))
            {
                ModelState.AddModelError(nameof(model.Name), "Invalid role name!");
            }

            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                var normalizedName = model.Name.Trim();
                var exists = await _dbContext.Role.AnyAsync(x =>
                    x.Name == normalizedName && (!excludedId.HasValue || x.Id != excludedId.Value));

                if (exists)
                    ModelState.AddModelError(nameof(model.Name), "Role already exists!");

                model.Name = normalizedName;
            }
        }
    }
}
