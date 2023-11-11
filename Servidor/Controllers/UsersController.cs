using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsuarioModel _userManager;

    public UsersController()
    {
        _userManager = new UsuarioModel();
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterModel model)
    {
        var result = _userManager.RegisterUser(model.Username, model.Password);
        if (result)
        {
            return Ok(new { message = "Registro exitoso." });
        }
        else
        {
            return BadRequest(new { message = "El usuario ya existe." });
        }
    }

    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] LoginModel model)
    {
        var result = _userManager.AuthenticateUser(model.Username, model.Password);
        if (result)
        {
            return Ok(new { message = "Autenticación exitosa." });
        }
        else
        {
            return Unauthorized(new { message = "Usuario o contraseña incorrectos." });
        }
    }

    [HttpGet("{username}")]
    public IActionResult GetUser(string username)
    {
        var userDetails = _userManager.GetUserDetails(username);
        if (userDetails != null)
        {
            return Ok(userDetails);
        }
        else
        {
            return NotFound(new { message = "Usuario no encontrado." });
        }
    }
}



public class RegisterModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}