using System;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;

public class UsuarioModel
{
    //Donde queremos guardar los usuario
    private readonly string _usersFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Usuarios");

    public UsuarioModel()
    {
        if (!Directory.Exists(_usersFolderPath))
        {
            // Crea la carpeta 
            Directory.CreateDirectory(_usersFolderPath);
        }

    }
    public bool RegisterUser(string username, string encryptedPrivateKey, string publicKey, string kLoginBcrypt)
    {
        var userPath = Path.Combine(_usersFolderPath, username);
        if (Directory.Exists(userPath))
        {
            // El usuario ya existe
            return false;
        }

        Directory.CreateDirectory(userPath);

        // Crear archivo usuario.json con el KLogin encriptado
        var userData = new
        {
            NombreUsuario = username,
            KLogin = kLoginBcrypt
        };

        var userFilePath = Path.Combine(userPath, "usuario.json");
        File.WriteAllText(userFilePath, JsonSerializer.Serialize(userData));

        // Guardar la clave pública y privada cifrada
        File.WriteAllText(Path.Combine(userPath, "publicKey.xml"), publicKey);
        File.WriteAllText(Path.Combine(userPath, "privateKeyEncrypted.xml"), encryptedPrivateKey);

        ComprobarCarpeta(userPath);

        return true;
    }

   


    public void ComprobarCarpeta(string path)
    {
        string carpetaPractica = Path.Combine(path, "CSarchivosENC");
        string carpetaPractica2 = Path.Combine(path, "CSarchivosDESENC");

        if (!Directory.Exists(carpetaPractica))
        {
            // La carpeta "CSarchivosENC" no existe, créala
            Directory.CreateDirectory(carpetaPractica);
        }

        if (!Directory.Exists(carpetaPractica2))
        {
            // La carpeta "CSarchivosENC" no existe, créala
            Directory.CreateDirectory(carpetaPractica2);
        }
    }


    public bool AuthenticateUser(string username, string password)
    {
        var userPath = Path.Combine(_usersFolderPath, username);
        var userFilePath = Path.Combine(userPath, "usuario.json");
        if (!File.Exists(userFilePath))
        {
            // El usuario no existe
            return false;
        }

        var userDataJson = File.ReadAllText(userFilePath);
        using (JsonDocument doc = JsonDocument.Parse(userDataJson))
        {
            var root = doc.RootElement;

            var nombreUsuario = root.GetProperty("NombreUsuario").GetString();
            var kLogin = root.GetProperty("KLogin").GetString();

            return BCrypt.Net.BCrypt.Verify(password, kLogin);

        }
        // Verificar hash de la contraseña


    }

    public object GetUserDetails(string username)
    {
        var userPath = Path.Combine(_usersFolderPath, username);
        var userFilePath = Path.Combine(userPath, "usuario.json");
        if (!File.Exists(userFilePath))
        {
            return null;
        }

        var userDataJson = File.ReadAllText(userFilePath);
        using (JsonDocument doc = JsonDocument.Parse(userDataJson))
        {
            var root = doc.RootElement;

            var nombreUsuario = root.GetProperty("NombreUsuario").GetString();
            var kLogin = root.GetProperty("KLogin").GetString();

            var publicKeyPath = Path.Combine(userPath, "publicKey.xml");
            var publicKey = File.Exists(publicKeyPath) ? File.ReadAllText(publicKeyPath) : null;

            var privateKeyEncryptedPath = Path.Combine(userPath, "privateKeyEncrypted.xml");
            var privateKeyEncrypted = File.Exists(privateKeyEncryptedPath) ? File.ReadAllText(privateKeyEncryptedPath) : null;

            return new
            {
                NombreUsuario = nombreUsuario,
                KLogin = kLogin,
                PublicKey = publicKey,
                EncryptedPrivateKey = privateKeyEncrypted
            };
        }
    }

    //COMPRESION DE LA CARPETA DEL USUARIO PARA QUE LA PODAMOS MANDAR
    public void ComprimirCarpeta(string carpetaOrigen, string archivoDestino)
    {
        // Crear una carpeta temporal
        var tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempFolder);

        // Copiar todos los archivos y carpetas de 'carpetaOrigen' a 'tempFolder', excluyendo 'usuario.json'
        foreach (var dirPath in Directory.GetDirectories(carpetaOrigen, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(carpetaOrigen, tempFolder));
        }

        foreach (var file in Directory.GetFiles(carpetaOrigen, "*.*", SearchOption.AllDirectories))
        {
            if (!file.EndsWith("usuario.json"))
            {
                var tempFilePath = file.Replace(carpetaOrigen, tempFolder);
                File.Copy(file, tempFilePath, true);
            }
        }

        // Comprimir la carpeta temporal
        ZipFile.CreateFromDirectory(tempFolder, archivoDestino);

        // Eliminar la carpeta temporal
        Directory.Delete(tempFolder, true);
    }

    public string GetUsersFolderPath()
    {
        return _usersFolderPath;
    }
}






//public bool RegisterUser(string username, string password)
//{
//    var userPath = Path.Combine(_usersFolderPath, username);
//    if (Directory.Exists(userPath))
//    {
//        // El usuario ya existe
//        return false;
//    }

//    Directory.CreateDirectory(userPath);

//    // Hash de la contraseña
//    var hashPassword = GetSHA256Hash(password);

//    // Dividir el hash en               
//    var kDatos = hashPassword.Substring(0, hashPassword.Length / 2);
//    var kLogin = hashPassword.Substring(hashPassword.Length / 2);

//    kLogin = BCrypt.Net.BCrypt.HashPassword(kLogin);

//    // Crear archivo usuario.json
//    var userData = new
//    {
//        NombreUsuario = username,
//        KLogin = kLogin
//    };

//    var userFilePath = Path.Combine(userPath, "usuario.json");
//    File.WriteAllText(userFilePath, JsonSerializer.Serialize(userData));

//    // Generar claves pública y privada
//    using (var rsa = new RSACryptoServiceProvider(2048))
//    {
//        try
//        {
//            // Obtener la clave pública y privada
//            var publicKey = rsa.ToXmlString(false); // Solo la clave pública
//            var privateKey = rsa.ToXmlString(true); // La clave privada 


//            // Cifrar la clave privada con KDatos
//            var encryptedPrivateKey = EncryptWithPassword(privateKey, kDatos);

//            // Guardar la clave pública y privada cifrada
//            File.WriteAllText(Path.Combine(userPath, "publicKey.xml"), publicKey);
//            File.WriteAllText(Path.Combine(userPath, "privateKeyEncrypted.xml"), encryptedPrivateKey);
//        }
//        finally
//        {
//            rsa.PersistKeyInCsp = false;
//        }
//    }

//    ComprobarCarpeta(userPath);

//    return true;
//}


//private string GetSHA256Hash(string input)
//{
//    using (SHA256 sha256 = SHA256.Create())
//    {
//        // Convierte la cadena en bytes y calcula el hash
//        byte[] bytes = Encoding.UTF8.GetBytes(input);
//        byte[] hashBytes = sha256.ComputeHash(bytes);

//        // Convierte el hash en una cadena hexadecimal
//        StringBuilder stringBuilder = new StringBuilder();
//        for (int i = 0; i < hashBytes.Length; i++)
//        {
//            stringBuilder.Append(hashBytes[i].ToString("x2"));
//        }

//        return stringBuilder.ToString();
//    }
//}