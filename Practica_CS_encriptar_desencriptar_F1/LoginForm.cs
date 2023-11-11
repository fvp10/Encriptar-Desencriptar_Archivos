using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//Librería para la petición HTTP REQUEST
//using Newtonsoft.Json;


namespace Practica_CS_encriptar_desencriptar_F1
{
//    public class AuthClient
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _authEndpoint;

//        public AuthClient(string authEndpoint)
//        {
//            _httpClient = new HttpClient();
//            _authEndpoint = authEndpoint;
//        }

//        public async Task<string> AuthenticateUser(string username, string kLogin)//APUNTADO EN NOTION
//        {
//            var requestData = new
//            {
//                Username = username,
//                KLogin = kLogin
//            };

//            var jsonContent = JsonConvert.SerializeObject(requestData);
//            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

//            try
//            {
//                HttpResponseMessage response = await _httpClient.PostAsync(_authEndpoint, content);

//                if (response.IsSuccessStatusCode)
//                {
//                    string responseContent = await response.Content.ReadAsStringAsync();
//                    return responseContent;
//                }
//                else
//                {
//                    return $"Error: {response.StatusCode}";
//                }
//            }
//            catch (Exception ex)
//            {
//                return $"Exception: {ex.Message}";
//            }
//        }
//    }




    public partial class LoginForm : Form
    {
        //private AuthClient _authClient;

        public LoginForm()
        {
            InitializeComponent();
            //_authClient = new AuthClient("https://tu-servidor.com/api/authenticate"); // Reemplazar con la URL real del servidor
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Obtén el nombre de usuario y la contraseña
            string username = txtUser.Text;
            string password = txtPass.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor, ingrese un nombre de usuario y contraseña.");
                return;
            }

            // No aplicar hash al nombre de usuario
            string hashedPassword = GetSHA256Hash(password);

            // Divide el hash en dos partes iguales
            int halfLength = hashedPassword.Length / 2;
            string kdatos = hashedPassword.Substring(0, halfLength);
            string klogin = hashedPassword.Substring(halfLength);

            // DUDA: Aplica Bcrypt a klogin
            //string hashedKlogin = BCrypt.Net.BCrypt.HashPassword(password);

            // CUANDO HAYA SERVIDOR: Envía kLogin al servidor para autenticación
            //string response = await _authClient.AuthenticateUser(username, hashedKlogin);

            if (AuthenticateUser(username, klogin))
            {
                MessageBox.Show($"Inicio correcto\nUsername: {username}\nkdatos: {kdatos}\nklogin: {klogin}");
                MessageBox.Show("inicio correcto");

                this.Hide(); // OCULTAMOS EL LOGIN
                Form1 main = new Form1(kdatos);   //CREAMOS UN NUEVO FORM 
                main.Show(); //LO MOSTRAMOS 
            }
            else
            {
                MessageBox.Show($"Datos Incorrectos");
            }
        }

        private bool AuthenticateUser(string username, string klogin)
        {
            // Aquí iría la lógica para autenticar al usuario
            // Por ahora, vamos a simular que la autenticación siempre es exitosa
            return true;
        }


        private void txtUser_TextChanged(object sender, EventArgs e)
        {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          

        }

        private void txtPass_TextChanged(object sender, EventArgs e)
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
