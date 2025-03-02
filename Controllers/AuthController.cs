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
        private readonly IEmailSender _emailSender;

        public AuthController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TokenGenerator tokenGenerator, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
            _emailSender = emailSender;
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
                        message = "User registered successfully. Please ensure to verify your email and phone number.",
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
                        message = "User registration failed.",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }
            }
            return BadRequest(new
            {
                message = "Validation failed.",
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Request failed.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
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
                return Unauthorized(new { message = "Invalid email or password." });
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

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var verificationLink = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email?email={model.Email}&token={Uri.EscapeDataString(token)}";

            var subject = "Verify Your Email";
            var body = $"Click the following link to verify your email: <a href='{verificationLink}'>Verify Email</a>";


            await _emailSender.SendEmail(model.Email, subject, body);
            return Ok(new { message = "Verification email sent." });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid email." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Invalid token or email confirmation failed." });
            }

            user.IsEmailVerified = true;
            await _userManager.UpdateAsync(user);
            return Ok(new { message = "Email verified successfully." });
        }
    }
}
