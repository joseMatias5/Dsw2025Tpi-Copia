using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Validations;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticateController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JwtTokenService _jwtTokenService;
    private readonly ILogger<AuthenticateController> _logger;

    public AuthenticateController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        JwtTokenService jwtTokenService,
        ILogger<AuthenticateController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel request)
    {
        _logger.LogInformation("Solicitud de ingreso");
        try
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                _logger.LogInformation("Solicitud de ingreso rechazada");
                return Unauthorized("The username or password is incorrect");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogInformation("Solicitud de ingreso rechazada");
                return Unauthorized("The username or password is incorrect");
            }

            var token = _jwtTokenService.GenerateToken(request.Username);
            _logger.LogInformation("Solicitud de ingreso exitosa");
            return Ok(new { token });
        }
        catch (Exception)
        {
            _logger.LogInformation("Solicitud de ingreso rechazada");
            return Problem("There was a problem logging in");
        }

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        _logger.LogInformation("Solicitud de registro");
        try
        {
            RegisterValidation.ValidateModel(model);
            var user = new IdentityUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            _logger.LogInformation("Solicitud de registro exitosa");
            return Ok("New user successfully created.");
        }
        catch (ArgumentNullException ane)
        {
            return NotFound(ane.Message);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (Exception)
        {
            _logger.LogInformation("Solicitud de registro rechazada");
            return Problem("There was a problem adding new user");
        }
    }
}
