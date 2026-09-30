using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using AcademicManagementSystem.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace AcademicManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private const int PageSize = 10;

        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserService _userService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(
            ApplicationDbContext dbContext,
            IMapper mapper,
            UserService userService,
            IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userService = userService;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AccessDenied(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.User.FirstOrDefault(x => x.Email == model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(nameof(model.Password), "You account is not found!");
                    return View(model);
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError(nameof(model.Password), "You are not allowed to login!");
                    return View(model);
                }

                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError(nameof(model.Password), "Invalid Email or Password!");
                    return View(model);
                }
                else
                {
                    var role = await _dbContext.Role.FindAsync(user.RoleId);
                    
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, role?.Name ?? string.Empty)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddMinutes(30) : DateTimeOffset.UtcNow.AddMinutes(10)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                    if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);

                    return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
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
        public async Task<IActionResult> Create(UserModel model)
        {
            await ValidateUserAsync(model);

            if (!ModelState.IsValid)
            {
                await PrepareAvailableRolesAsync(model);
                return View(model);
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
        public async Task<IActionResult> Detail(UserModel model)
        {
            await ValidateUserAsync(model, model.Id);

            var user = await _dbContext.User.FindAsync(model.Id);
            if (user == null)
                return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid)
            {
                await PrepareAvailableRolesAsync(model);
                return View(model);
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
