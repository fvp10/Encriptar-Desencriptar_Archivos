using System;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace Practica_CS_encriptar_desencriptar_F1
{
    public partial class Form1 : Form
    {
        public Form1()
        {   
            InitializeComponent();
        }

        private void desencriptar__Click(object sender, EventArgs e)
        {
            //Jose Yohn
        }

        private void encriptar__Click(string archivoenc, byte[] clave, byte[] iv)
        {
            /*para generar la clave y el iv de manera aleatoria se hace en un método a parte usando 
             *RNGCryptoServiceProvider que es una clase en el espacio de nombres  de la libreria System.Security.Cryptography(usada para el encriptado y desencriptado
             *en el framework de .NET que se utiliza para generar números aleatorios criptográficamente seguros.
             */
        }

        private void btn_selc_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Busca tu archivo a encriptar";
            openFileDialog1.ShowDialog();
            string texto = openFileDialog1.FileName;

            if (File.Exists(openFileDialog1.FileName))
            {         
                TextReader leer = new StreamReader(texto);        
                leer.Close();
            }
            txt1.Text = Path.GetFileName(texto);
        }
    }
}
