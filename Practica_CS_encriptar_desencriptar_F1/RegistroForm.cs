using Newtonsoft.Json;

using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Practica_CS_encriptar_desencriptar_F1
{
    public partial class RegistroForm : Form
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string url = "https://localhost:7045/Users/register";
        public RegistroForm()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
           Registro();

            
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide(); // OCULTAMOS EL LOGIN
            LoginForm main = new LoginForm();   //CREAMOS UN NUEVO FORM 
            main.Show(); //LO MOSTRAMOS 


        }

        public void Registro()
        {
            string username = txtUsername.Text;
            string password = txtContrasenya.Text;
            if (username == "" || password == "") {
                MessageBox.Show("Pero mete algo no ?");
            }

            else if (!Directory.Exists(Path.Combine(Application.StartupPath, "Usuarios", username)))
            {

                string hashedPassword = GetSHA256Hash(password);

                // Divide el hash en dos partes iguales
                int halfLength = hashedPassword.Length / 2;
                string kdatos = hashedPassword.Substring(0, halfLength);
                string klogin = hashedPassword.Substring(halfLength);

                string kLoginBcrypt = BCrypt.Net.BCrypt.HashPassword(klogin);

                var userData = new
                {
                    NombreUsuario = username,
                    KLogin = kLoginBcrypt
                };

                // Genera las claves criptográficas RSA
                var (publicKey, privateKey) = GenerateRSAKeys(); 
                var encryptedPrivateKey = EncryptWithPassword(privateKey, kdatos); 


                var registrationData = new
                {
                    Username = username,
                    EncryptedPrivateKey = encryptedPrivateKey,
                    PublicKey = publicKey,
                    KLogin = kLoginBcrypt
                };
              EnviarDatosServidor(username,encryptedPrivateKey,publicKey,kLoginBcrypt);


               
            }
            else
            {
                MessageBox.Show("El nombre de usuario ya existe.");
            }
        }

        private async Task EnviarDatosServidor(string username, string encryptedPrivateKey, string publicKey, string kLogin)
        {

            var registrationData = new
            {
                Username = username,
                EncryptedPrivateKey = encryptedPrivateKey,
                PublicKey = publicKey,
                KLogin = kLogin
            };
            var content = new StringContent(JsonConvert.SerializeObject(registrationData), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Registro exitoso (parte Cliente).");
                    this.Hide(); // OCULTAMOS EL LOGIN
                    LoginForm main = new LoginForm();   //CREAMOS UN NUEVO FORM 
                    main.Show(); //LO MOSTRAMOS 
                }
                else
                {
                    MessageBox.Show("Error en el registro (parte Cliente).");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la conexión con el servidor: " + ex.Message);
            }

        }


        private (string, string) GenerateRSAKeys()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                return (rsa.ToXmlString(false), rsa.ToXmlString(true));
            }
        }

        private string EncryptWithPassword(string clearText, string password)
        {
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);

            byte[] key = new byte[32]; 
            byte[] iv = new byte[16]; 
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

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
    }
}

