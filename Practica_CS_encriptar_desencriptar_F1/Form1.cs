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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

        private void encriptar__Click(object sender, EventArgs e)
        {
            //Felix Valentino
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
