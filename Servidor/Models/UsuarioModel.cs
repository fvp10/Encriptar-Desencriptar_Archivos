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


    public void ComprobarCarpeta(string path)
    {
        string carpetaPractica = Path.Combine(path, "CSarchivosENC");
        string carpetaPractica2 = Path.Combine(path, "CSarchivosDESENC");

        if (!Directory.Exists(carpetaPractica))
        {
            Directory.CreateDirectory(carpetaPractica);
        }

        if (!Directory.Exists(carpetaPractica2))
        {
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
        


    }
    // Devuelve todo de un usuario para visualizar sus datos
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

    public void DeleteFilesInFolder(string folderPath)
    {
        DirectoryInfo di = new DirectoryInfo(folderPath);

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
    }


    public List<object> GetAllUserPublicKeys(string usuRemitente)
    {
        var usersFolderPath = GetUsersFolderPath();
        var directories = Directory.GetDirectories(usersFolderPath);

        var userPublicKeys = new List<object>();

        foreach (var directory in directories)
        {
            var username = Path.GetFileName(directory);

            if (username != usuRemitente)
            {


                var userDetails = GetUserPublicKey(username);
                if (userDetails != null)
                {
                    userPublicKeys.Add(userDetails);
                }
            }
        }

        return userPublicKeys;
    }

    public object GetUserPublicKey(string username)
    {
        var userPath = Path.Combine(_usersFolderPath, username);
        var publicKeyPath = Path.Combine(userPath, "publicKey.xml");

        if (!File.Exists(publicKeyPath))
        {
            return null;
        }

        var publicKey = File.ReadAllText(publicKeyPath);

        return new
        {
            NombreUsuario = username,
            PublicKey = publicKey
        };
    }


    public string GetUsersFolderPath()
    {
        return _usersFolderPath;
    }
}