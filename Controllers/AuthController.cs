using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskScheduler.Data;
using TaskScheduler.DTOs;
using TaskScheduler.Models;
using TaskScheduler.Services;

namespace TaskScheduler.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenGenerator _tokenGenerator;

        public AuthController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TokenGenerator tokenGenerator)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                    Phone = model.Phone,
                    IsEmailVerified = false,
                    IsPhoneVerified = false
                };
                var result = await _userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                    return Ok(new
                    {
                        message = "User registered successfully. Please ensure to verify your email and phone number",
                        user = new
                        {
                            name = user.Name,
                            email = user.Email,
                            phone = user.Phone,
                        }
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "User registration failed",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }
            }
            return BadRequest(new
            {
                message = "Validation failed",
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            // redirect to phone/email verification if not verified
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Request failed",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            if (!(user.IsPhoneVerified || user.IsEmailVerified))
            {
                return BadRequest(new
                {
                    message = "Please verify your email or phone number before logging in.",
                    emailVerification = "api/auth/verify-email",
                    phoneVerification = "api/auth/verify-phone"
                });
            }


            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = await _tokenGenerator.GenerateToken(user);
            return Ok(new
            {
                message = "Login successful",
                token = token,
                user = new
                {
                    name = user.Name,
                    email = user.Email,
                    phone = user.Phone
                }
            });

        }

    }
}
