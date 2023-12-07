using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.IO;

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

    [HttpGet("{username}/getAllUsers")]
    public IActionResult GetAllUserInfoShare(string username) {
        try
        {
            var userShareInfo = _userManager.GetAllUserPublicKeys(username);
            return Ok(userShareInfo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al obtener las carpetas de usuario: {ex.Message}" });
        }
    }


    [HttpGet("{username}/get-folder")]
    public IActionResult GetFolder(string username)
    {
        var userFolderPath = Path.Combine(_userManager.GetUsersFolderPath(), username);

        if (!Directory.Exists(userFolderPath))
        {
            return NotFound(new { message = "Usuario no encontrado." });
        }

        var carpetaComprimidaPath = Path.Combine(_userManager.GetUsersFolderPath(), $"{username}_comprimida.zip");

        try
        {
            _userManager.ComprimirCarpeta(userFolderPath, carpetaComprimidaPath);

            // Envía el archivo ZIP al cliente
            var fileBytes = System.IO.File.ReadAllBytes(carpetaComprimidaPath);
            return File(fileBytes, "application/zip", $"{username}_comprimida.zip");
        }
        catch (Exception ex)
        {
            // Manejar la excepción (registros, devolución de error, etc.)
            return BadRequest(new { message = $"Error al comprimir la carpeta: {ex.Message}" });
        }
        finally
        {
            // Elimina el archivo ZIP solo si no hay errores
            if (System.IO.File.Exists(carpetaComprimidaPath))
            {
                System.IO.File.Delete(carpetaComprimidaPath);
            }
        }
    }

    [HttpPost("{username}/refrescarDatos")]
    public IActionResult UploadZip(string username, IFormFile file)
    {

        if (file == null || file.Length == 0)
        {
            return BadRequest("Archivo no proporcionado o vacío.");
        }

        var userFolderPath = Path.Combine(_userManager.GetUsersFolderPath(), username);


        
        var archivosENCPath = Path.Combine(userFolderPath, "CSarchivosENC");
        var archivosDESENCPath = Path.Combine(userFolderPath, "CSarchivosDESENC");

        _userManager.DeleteFilesInFolder(archivosENCPath);
        _userManager.DeleteFilesInFolder(archivosDESENCPath);

        if (!Directory.Exists(userFolderPath))
        {
            return NotFound("Usuario no encontrado.");
        }

        var tempZipPath = Path.GetTempFileName();

        try
        {
            // Guardar el archivo ZIP temporalmente
            using (var stream = new FileStream(tempZipPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Descomprimir el archivo ZIP

            ZipFile.ExtractToDirectory(tempZipPath, userFolderPath, false);

            return Ok("Archivo cargado y descomprimido exitosamente.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al procesar el archivo: {ex.Message}");
        }
        finally
        {
            // Eliminar el archivo ZIP temporal
            if (System.IO.File.Exists(tempZipPath))
            {
                System.IO.File.Delete(tempZipPath);
            }
        }
    }

    [HttpPost("{username}/uploadSharedFiles")]
    public IActionResult UploadSharedFiles(string username, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Archivo no proporcionado o vacío.");
        }

        var userEncryptedFolderPath = Path.Combine(_userManager.GetUsersFolderPath(), username, "CSarchivosENC");

        if (!Directory.Exists(userEncryptedFolderPath))
        {
            Directory.CreateDirectory(userEncryptedFolderPath);
        }

        var tempZipPath = Path.GetTempFileName(); // Asegúrate de que esta línea esté generando una ruta de archivo válida

        try
        {
            // Guardar el archivo ZIP temporalmente
            using (var stream = new FileStream(tempZipPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Descomprimir el archivo ZIP directamente en la carpeta de archivos encriptados del usuario
            ZipFile.ExtractToDirectory(tempZipPath, userEncryptedFolderPath, true);

            return Ok("Archivos compartidos cargados exitosamente en la carpeta de archivos encriptados.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al procesar el archivo: {ex.Message}");
        }
        finally
        {
            // Eliminar el archivo ZIP temporal
            if (System.IO.File.Exists(tempZipPath)) 
            {
                System.IO.File.Delete(tempZipPath);
            }
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