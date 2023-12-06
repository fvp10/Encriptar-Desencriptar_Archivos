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

    private string GetSHA256Hash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            // Convierte la cadena en bytes y calcula el hash
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(bytes);

            // Convierte el hash en una cadena hexadecimal
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                stringBuilder.Append(hashBytes[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }

    public bool RegisterUser(string username, string password)
    {
        var userPath = Path.Combine(_usersFolderPath, username);
        if (Directory.Exists(userPath))
        {
            // El usuario ya existe
            return false;
        }

        Directory.CreateDirectory(userPath);

        // Hash de la contraseña
        var hashPassword = GetSHA256Hash(password);


        // Dividir el hash en               
        var kDatos = hashPassword.Substring(0, hashPassword.Length / 2);
        var kLogin = hashPassword.Substring(hashPassword.Length / 2);

        kLogin = BCrypt.Net.BCrypt.HashPassword(kLogin);

        // Crear archivo usuario.json
        var userData = new
        {
            NombreUsuario = username,
            KLogin = kLogin
        };

        var userFilePath = Path.Combine(userPath, "usuario.json");
        File.WriteAllText(userFilePath, JsonSerializer.Serialize(userData));

        // Generar claves pública y privada
        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            try
            {
                // Obtener la clave pública y privada
                var publicKey = rsa.ToXmlString(false); // Solo la clave pública
                var privateKey = rsa.ToXmlString(true); // La clave privada 


                // Cifrar la clave privada con KDatos
                var encryptedPrivateKey = EncryptWithPassword(privateKey, kDatos);

                // Guardar la clave pública y privada cifrada
                File.WriteAllText(Path.Combine(userPath, "publicKey.xml"), publicKey);
                File.WriteAllText(Path.Combine(userPath, "privateKeyEncrypted.xml"), encryptedPrivateKey);
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }

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

    private string EncryptWithPassword(string clearText, string password)
    {
        byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);

        // Utilizar la contraseña directamente; es necesario que tenga la longitud adecuada.
        // Esto es INSEGURO y solo para fines demostrativos.
        // En producción, siempre debe usarse un salt y una función de derivación de clave segura.
        byte[] key = new byte[32]; // AES requiere una clave de 256 bits para AES-256.
        byte[] iv = new byte[16]; // El IV siempre necesita 16 bytes para AES.
        Array.Copy(System.Text.Encoding.UTF8.GetBytes(password.PadRight(key.Length)), key, key.Length);
        Array.Copy(System.Text.Encoding.UTF8.GetBytes(password.PadRight(iv.Length)), iv, iv.Length);

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
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


    // Devuelve todos los usuarios
    public List<string> GetAllUserFolders()
    {
        var usersFolderPath = GetUsersFolderPath();
        var directories = Directory.GetDirectories(usersFolderPath);
        return directories.Select(Path.GetFileName).ToList();
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