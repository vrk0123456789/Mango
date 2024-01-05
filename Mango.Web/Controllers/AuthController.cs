using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto();
            return View(loginRequestDto);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            ResponseDto responseDto = await _authService.LoginAsync(loginRequestDto);
            
            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", responseDto.Message);
            }
            
            return View(loginRequestDto);
        }
        [HttpGet]
        public IActionResult Register()
        {
            List<SelectListItem> roleList = new List<SelectListItem>() { 
            new SelectListItem {Text=SD.RoleAdmin, Value=SD.RoleAdmin},
            new SelectListItem {Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };
            ViewBag.RoleList = roleList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
        {
            ResponseDto responseDto = await _authService.RegisterAsync(registrationRequestDto);
            ResponseDto assignRole;
            if(responseDto != null && responseDto.IsSuccess) 
            {
                if (string.IsNullOrEmpty(registrationRequestDto.Role))
                    registrationRequestDto.Role = SD.RoleCustomer;
                assignRole = await _authService.AssignRoleAsync(registrationRequestDto);
                if(assignRole != null && assignRole.IsSuccess) 
                {
                    TempData["success"] = "Registration Successful";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    TempData["error"] = responseDto.Message;
                }
            }
            else
            {
                TempData["error"] = responseDto.Message;
            }
            List<SelectListItem> roleList = new List<SelectListItem>() {
            new SelectListItem {Text=SD.RoleAdmin, Value=SD.RoleAdmin},
            new SelectListItem {Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };
            ViewBag.RoleList = roleList;
            return View(registrationRequestDto);
        }
        public IActionResult Logout()
        {
            return View();
        }
    }
}
