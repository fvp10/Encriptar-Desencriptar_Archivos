using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Practica_CS_encriptar_desencriptar_F1
{
    public partial class LoginForm : Form
    {
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

            // No aplicar hash al nombre de usuario
            string hashedPassword = GetSHA256Hash(password);

            // Divide el hash en dos partes iguales
            int halfLength = hashedPassword.Length / 2;
            string kdatos = hashedPassword.Substring(0, halfLength);
            string klogin = hashedPassword.Substring(halfLength);

            // Aplica Bcrypt a klogin
            //string hashedKlogin = BCrypt.Net.BCrypt.HashPassword(password);

            MessageBox.Show($"Inicio correcto\nUsername: {username}\nkdatos: {kdatos}\nklogin: {klogin}");

            MessageBox.Show("inicio correcto");

            this.Hide(); // OCULTAMOS EL LOGIN
            Form1 main = new Form1();   //CREAMOS UN NUEVO FORM 
            main.Show(); //LO MOSTRAMOS 
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
