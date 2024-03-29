﻿using System;
using System.Text;
using System.Windows.Forms;


using System.Security.Cryptography;
//Librerías para las petición HTTP REQUEST
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;


namespace Practica_CS_encriptar_desencriptar_F1
{



    public partial class LoginForm : Form
    {
        //private AuthClient _authClient;
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string ServerUrl = "https://localhost:7045/Users/authenticate";
        public LoginForm()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        //POST: El usuario se intenta autentificar
        private async void button1_Click(object sender, EventArgs e)
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


            bool authenticated = await AuthenticateUserAsync(username, klogin);

            if (authenticated)
            {
                MessageBox.Show($"Inicio de sesión exitoso\nUsername: {username}\nkdatos: {kdatos}\nklogin: {klogin}");
                this.Hide(); // OCULTAMOS EL LOGIN
                Form1 main = new Form1(username,kdatos);   //CREAMOS UN NUEVO FORM 
                main.Show(); //LO MOSTRAMOS 
            }
            else
            {
                MessageBox.Show("Inicio de sesión fallido");
            }
        }

        private async Task<bool> AuthenticateUserAsync(string usuario, string klogin)
        {
            // Crear objeto con los datos del usuario
            var userObject = new
            {
                username = usuario,
                password = klogin
            };

            // Convertir objeto a JSON
            string jsonContent = JsonConvert.SerializeObject(userObject);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(ServerUrl, content);

                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    return responseObject?.message == "Autenticación exitosa.";
                }
                else
                {
                    // Manejo de diferentes respuestas no exitosas
                    MessageBox.Show($"Error en la autenticación: {response.StatusCode}\n{responseContent}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de comunicación con el servidor: " + ex.Message);
            }

            return false;
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide(); // OCULTAMOS EL LOGIN
            RegistroForm registro = new RegistroForm();   //CREAMOS UN NUEVO FORM 
            registro.Show(); //LO MOSTRAMOS 
        }
    }
}


