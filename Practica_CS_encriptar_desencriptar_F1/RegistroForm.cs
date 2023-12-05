using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Practica_CS_encriptar_desencriptar_F1
{
    public partial class RegistroForm : Form
    {
        public RegistroForm()
        {
            InitializeComponent();
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            registroMetodo();
        }

        public void registroMetodo()
        {
            string username = txtUsername.Text;
            string password = txtContrasenya.Text;

            // Asumiendo que tienes métodos similares a los de UsuarioModel.cs en tu cliente
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Usuarios", username)))
            {
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Usuarios", username));

                string hashedPassword = GetSHA256Hash(password);

                // Divide el hash en dos partes iguales
                int halfLength = hashedPassword.Length / 2;
                string kdatos = hashedPassword.Substring(0, halfLength);
                string klogin = hashedPassword.Substring(halfLength);

                kLogin = BCrypt.Net.BCrypt.HashPassword(klogin);

                var userData = new
                {
                    NombreUsuario = username,
                    KLogin = kLogin
                };

                // Guarda el usuario.json en el directorio del usuario
                File.WriteAllText(Path.Combine(Application.StartupPath, "Usuarios", username, "usuario.json"), JsonSerializer.Serialize(userData));

                // Genera las claves criptográficas RSA
                var (publicKey, privateKey) = GenerateRSAKeys(); // Debes definir este método
                var encryptedPrivateKey = EncryptWithPassword(privateKey, kDatos); // Debes definir este método

                // Guarda las claves en archivos
                File.WriteAllText(Path.Combine(Application.StartupPath, "Usuarios", username, "publicKey.xml"), publicKey);
                File.WriteAllText(Path.Combine(Application.StartupPath, "Usuarios", username, "privateKeyEncrypted.xml"), encryptedPrivateKey);

                MessageBox.Show("Registro exitoso.");
                // Aquí puedes cerrar el formulario de registro o redirigir al usuario
            }
            else
            {
                MessageBox.Show("El nombre de usuario ya existe.");
            }
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
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

