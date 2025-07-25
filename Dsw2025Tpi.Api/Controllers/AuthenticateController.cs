using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Validations;
using Dsw2025Tpi.Application.Interfaces;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticateController : ControllerBase
{
    private readonly IAuthenticateService _service;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthenticateController(IAuthenticateService service,
        SignInManager<IdentityUser> signInManager)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _signInManager = signInManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel.RequestLogin request)
    {
        try
        {
            var user = await _service.Login(request);

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                throw new Application.Exceptions.ApplicationException("The username or password is incorrect");
            }
            var token = _service.GenerateToken(request.Username);

            return Ok(token);
        }
        catch (Application.Exceptions.ApplicationException ape)
        {
            return NotFound(ape.Message);
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
            return Problem("There was a problem logging in");
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel.RequestRegister model)
    {
        try
        {
            await _service.Register(model);
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
            return Problem("There was a problem adding new user");
        }
    }
}
