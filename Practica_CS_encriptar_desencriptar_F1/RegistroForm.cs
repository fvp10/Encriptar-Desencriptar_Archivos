using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
                string hashedPassword = GetSHA256Hash(password); // Debes definir este método en tu RegistroForm o mejor en una clase aparte

                // Divide el hash para generar las claves y el hash de login
                var (kDatos, kLogin) = SplitHash(hashedPassword); // Debes definir este método
                kLogin = BCrypt.Net.BCrypt.HashPassword(kLogin);

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
    }
}

