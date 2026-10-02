using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using AcademicManagementSystem.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AcademicManagementSystem.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private const int PageSize = 10;

        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserService _userService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(
            ApplicationDbContext dbContext,
            IMapper mapper,
            UserService userService,
            IPasswordHasher<User> passwordHasher,
            IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userService = userService;
            _passwordHasher = passwordHasher;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(UserListModel model)
        {
            if (model.PageNumber < 1)
                model.PageNumber = 1;

            var pagedData = await _userService.GetAllUsersAsync(
                search: model.SearchText,
                status: model.StatusId,
                sortById: model.SortById,
                pageNumber: model.PageNumber,
                pageSize: PageSize);

            model.Users = _mapper.Map<List<UserModel>>(pagedData.Records);
            model.PageSize = PageSize;
            model.TotalPages = pagedData.TotalPages;
            model.TotalRecords = pagedData.TotalRecords;
            model.HasNextPage = model.PageNumber < model.TotalPages;
            model.HasPreviousPage = model.PageNumber > 1;
            model.ShowingFrom = model.TotalRecords == 0 ? 0 : ((model.PageNumber - 1) * model.PageSize) + 1;
            model.ShowingTo = Math.Min(model.PageNumber * model.PageSize, model.TotalRecords);

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = new UserModel
            {
                CreatedOnUtc = DateTime.UtcNow
            };

            await PrepareAvailableRolesAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel model, IFormFile? imageFile)
        {
            var imageExtension = string.Empty;
            if (imageFile is { Length: > 0 })
            {
                imageExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

                if (!allowedExtensions.Contains(imageExtension))
                    ModelState.AddModelError(nameof(imageFile), "Only JPG, JPEG, and PNG images are allowed.");

                const long maximumFileSize = 2 * 1024 * 1024;
                if (imageFile.Length > maximumFileSize)
                    ModelState.AddModelError(nameof(imageFile), "The image must be 2 MB or smaller.");
            }

            await ValidateUserAsync(model);

            if (!ModelState.IsValid)
            {
                await PrepareAvailableRolesAsync(model);
                return View(model);
            }

            if (imageFile is { Length: > 0 })
            {
                var fileName = $"{Guid.NewGuid():N}{imageExtension}";

                var webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");

                var uploadDirectory = Path.Combine(webRootPath, "uploads", "users");
                Directory.CreateDirectory(uploadDirectory);

                var filePath = Path.Combine(uploadDirectory, fileName);
                await using var stream = System.IO.File.Create(filePath);
                await imageFile.CopyToAsync(stream);

                model.ImagePath = $"/uploads/users/{fileName}";
            }

            model.CreatedOnUtc = DateTime.UtcNow;
            var user = _mapper.Map<User>(model);

            if (!string.IsNullOrWhiteSpace(model.Password) && !string.IsNullOrWhiteSpace(model.ConfirmPassword) && model.Password == model.ConfirmPassword)
                user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            _dbContext.User.Add(user);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0)
                return RedirectToAction(nameof(Index));

            var user = await _dbContext.User.FindAsync(id);
            if (user == null)
                return RedirectToAction(nameof(Index));

            var model = _mapper.Map<UserModel>(user);
            await PrepareAvailableRolesAsync(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detail(UserModel model, IFormFile? imageFile)
        {
            var imageExtension = string.Empty;
            if (imageFile is { Length: > 0 })
            {
                imageExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

                if (!allowedExtensions.Contains(imageExtension))
                    ModelState.AddModelError(nameof(imageFile), "Only JPG, JPEG, and PNG images are allowed.");

                const long maximumFileSize = 2 * 1024 * 1024;
                if (imageFile.Length > maximumFileSize)
                    ModelState.AddModelError(nameof(imageFile), "The image must be 2 MB or smaller.");
            }

            await ValidateUserAsync(model, model.Id);

            var user = await _dbContext.User.FindAsync(model.Id);
            if (user == null)
                return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid)
            {
                await PrepareAvailableRolesAsync(model);
                return View(model);
            }

            if (imageFile is { Length: > 0 })
            {
                var fileName = $"{Guid.NewGuid():N}{imageExtension}";

                var webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");

                var uploadDirectory = Path.Combine(webRootPath, "uploads", "users");
                Directory.CreateDirectory(uploadDirectory);

                var filePath = Path.Combine(uploadDirectory, fileName);
                await using var stream = System.IO.File.Create(filePath);
                await imageFile.CopyToAsync(stream);

                model.ImagePath = $"/uploads/users/{fileName}";
            }

            var createdOnUtc = user.CreatedOnUtc;
            _mapper.Map(model, user);
            user.CreatedOnUtc = createdOnUtc;

            if (!string.IsNullOrWhiteSpace(model.Password) && !string.IsNullOrWhiteSpace(model.ConfirmPassword) && model.Password == model.ConfirmPassword)
                user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var user = await _dbContext.User.FindAsync(id);
            if (user == null)
                return RedirectToAction(nameof(Index));

            return View(_mapper.Map<UserModel>(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(UserModel model)
        {
            var user = await _dbContext.User.FindAsync(model.Id);
            if (user == null)
                return RedirectToAction(nameof(Index));

            _dbContext.User.Remove(user);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task ValidateUserAsync(UserModel model, int? excludedId = null)
        {
            model.FullName = model.FullName.Trim();
            model.Email = model.Email.Trim();

            if (!string.IsNullOrWhiteSpace(model.FullName) &&
                !Regex.IsMatch(model.FullName, @"^[\p{L}]+(?:[ .'-][\p{L}]+)*$"))
            {
                ModelState.AddModelError(nameof(model.FullName), "Invalid Full Name!");
            }

            if (!string.IsNullOrWhiteSpace(model.Email) &&
                await _dbContext.User.AnyAsync(x => x.Email == model.Email && (!excludedId.HasValue || x.Id != excludedId.Value)))
            {
                ModelState.AddModelError(nameof(model.Email), "Email already exists!");
            }

            if (model.RoleId > 0 && !await _dbContext.Role.AnyAsync(x => x.Id == model.RoleId))
                ModelState.AddModelError(nameof(model.RoleId), "Selected role does not exist!");
        }

        private async Task PrepareAvailableRolesAsync(UserModel model)
        {
            var roles = await _dbContext.Role
                .Where(x => x.IsActive || x.Id == model.RoleId)
                .OrderBy(x => x.Name)
                .ToListAsync();

            model.AvailableRoles.Clear();
            model.AvailableRoles.Add(new SelectListItem
            {
                Value = "0",
                Text = "Select Role",
                Selected = model.RoleId == 0
            });

            model.AvailableRoles.AddRange(roles.Select(role => new SelectListItem
            {
                Value = role.Id.ToString(),
                Text = role.Name,
                Selected = role.Id == model.RoleId
            }));
        }
    }
}
