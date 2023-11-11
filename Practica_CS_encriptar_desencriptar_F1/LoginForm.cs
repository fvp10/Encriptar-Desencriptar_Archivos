using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Practica_CS_encriptar_desencriptar_F1
{
    public partial class LoginForm : Form
    {
        private const string ServerUrl = "https://localhost:7045"; // 
        public LoginForm()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Obtén el nombre de usuario y la contraseña
            string username = txtUser.Text;
            string password = txtPass.Text;

            // Aplicar hash a la contraseña
            string hashedPassword = GetSHA256Hash(password);

            // Divide el hash en dos partes iguales
            int halfLength = hashedPassword.Length / 2;
            string kdatos = hashedPassword.Substring(0, halfLength);
            string klogin = hashedPassword.Substring(halfLength);

            // Generar un par de claves RSA
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

            // Obtener la clave pública y privada en formato XML
            string publicKeyXml = rsa.ToXmlString(false);
            string privateKeyXml = rsa.ToXmlString(true);

            // Cifrar la clave pública con kdatos
            // string encryptedPublicKey = EncryptWithKDatos(kdatos, Encoding.UTF8.GetBytes(publicKeyXml));

            // Almacena la clave pública cifrada y la clave privada en el servidor de manera segura
            // Aquí debes guardar encryptedPublicKey y privateKeyXml en tu servidor de manera segura

            // Autenticar al usuario en el servidor
            
            bool authenticated = AuthenticateUser(username, klogin);

            if (authenticated)
            {
                MessageBox.Show($"Inicio de sesión exitoso\nUsername: {username}");
                this.Hide(); // OCULTAMOS EL LOGIN
                Form1 main = new Form1(kdatos);   //CREAMOS UN NUEVO FORM 
                main.Show(); //LO MOSTRAMOS 
            }
            else
            {
                MessageBox.Show("Inicio de sesión fallido");
            }

            MessageBox.Show($"Inicio de sesión exitoso\nUsername: {username}");
            this.Hide(); // OCULTAMOS EL LOGIN
            //Form1 main = new Form1(kdatos);   //CREAMOS UN NUEVO FORM 
            //main.Show(); //LO MOSTRAMOS 
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

        /*
        private static string EncryptWithKDatos(string kdatos, byte[] data)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(kdatos); // Clave derivada de kdatos
                aesAlg.IV = new byte[aesAlg.BlockSize / 8]; // Vector de inicialización (IV) vacío

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                        csEncrypt.FlushFinalBlock();
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        */

        
        private bool AuthenticateUser(string username, string klogin)
        {
            try
            {
                // Crea una solicitud HTTP POST
                using (WebClient client = new WebClient())
                {
                    // Configura los datos a enviar
                    byte[] data = Encoding.UTF8.GetBytes($"username={username}&klogin={klogin}");

                    // Configura los encabezados de la solicitud
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    // Realiza la solicitud al servidor
                    byte[] responseBytes = client.UploadData(ServerUrl, "POST", data);

                    // Convierte la respuesta en una cadena
                    string response = Encoding.UTF8.GetString(responseBytes);

                    // Verifica la respuesta del servidor (puedes personalizar esto)
                    if (response == "Success")
                    {
                        return true; // Inicio de sesión exitoso
                    }
                }
            }
            catch (Exception ex)
            {
                // Maneja errores de comunicación con el servidor
                Console.WriteLine("Error de comunicación con el servidor: " + ex.Message);
            }

            return false; // Inicio de sesión fallido
        }
        

        private void txtUser_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPass_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
