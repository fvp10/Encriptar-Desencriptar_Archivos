using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Practica_CS_encriptar_desencriptar_F1
{
    public partial class Form1 : Form

    {
        private int posicionVertical = 10; // Inicializa la posición vertical
        private int posicionVerticalDESENC = 10;
        private Dictionary<System.Windows.Forms.Button, Panel> botonesYFilas = new Dictionary<System.Windows.Forms.Button, Panel>();
        //private string rutaGuardado = Path.Combine(Environment.CurrentDirectory, "CSarchivosENC");
        //private string rutaGuardadoDESENC = Path.Combine(Environment.CurrentDirectory, "CSarchivosDESENC");
        private string rutaGuardado = "";
        private string rutaGuardadoDESENC = "";
        private string nombreArc;
        private Random rnd = new Random();
        ///Quitar cuando se implemente el servidor
        private string kdatos="";
        private RSACryptoServiceProvider rsa;
        private string clavePublica = "";
        private string clavePrivada = "";
        private string clavePrivadaEn = "";
        private string kdatosServidor = "";
        private byte[] kdatosHash;//para pasar a 32bytes
        private byte[] ivGlobal; // Almacena el IV utilizado en la encriptación

        public Form1(string kdatosRecibidos)
        {
            InitializeComponent();
            ComprobarCarpeta();
            ComprobarArchivosEncriptados();

            kdatos = kdatosRecibidos;
            kdatosServidor = kdatos; // Asumo que obtienes este valor de algún lugar
            GenerarClavesRSA();
            
        }

        //private void ComprobarKdatos(string kdatos1, string kdatosServidor1)
        //{
        //    if (kdatos1 != kdatosServidor1)
        //    {
        //        MessageBox.Show("Error al iniciar sesión");
        //        this.Hide(); // Ocultamos el formulario actual
        //        LoginForm main = new LoginForm(); // Creamos un nuevo formulario de inicio de sesión
        //        main.Show(); // Mostramos el nuevo formulario
        //    }
        //}

        private void desencriptar__Click(object sender, EventArgs e)
        {
            MessageBox.Show("Proceso de desencriptar");

        }

        private void encriptar__Click(object sender, EventArgs e)
        {
            MetodoDeEncriptado();
        }

        private void GuardarClavePrivadaEncriptada(string clavePrivadaEncriptada)
        {
            string rutaArchivoClavePrivada = Path.Combine(rutaGuardado, "clavePrivadaEncriptada.txt");
            File.WriteAllText(rutaArchivoClavePrivada, clavePrivadaEncriptada);
        }

        private void GenerarClavesRSA()
        {
            rsa = new RSACryptoServiceProvider(2048);
            clavePrivada = rsa.ToXmlString(true); // Clave privada
            clavePublica = rsa.ToXmlString(false); // Clave pública

            // Encripta la clave privada con kdatos
            clavePrivadaEn = EncriptarClavePrivada(clavePrivada, kdatos);
            GuardarClavePrivadaEncriptada(clavePrivadaEn);
            // Guarda o maneja las claves según sea necesario
        }

        private string EncriptarClavePrivada(string clavePrivada, string kdatos)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                kdatosHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(kdatos));

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = kdatosHash;
                    ivGlobal = aesAlg.IV; // Guarda el IV para la desencriptación

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(clavePrivada); // Escribe la clave privada como string
                            }
                        }

                        // Devuelve la clave privada encriptada y el IV como un único string Base64
                        byte[] encryptedData = msEncrypt.ToArray();
                        return Convert.ToBase64String(ivGlobal.Concat(encryptedData).ToArray());
                    }
                }
            }
        }


        private bool ComprobarKdatos(string kdatosRecibidos, string kdatosServidor)
        {
            // Transforma kdatosRecibidos de la misma manera que kdatos original
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] kdatosRecibidosHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(kdatosRecibidos));

                // Compara las versiones transformadas de kdatos
                if (kdatosRecibidos.SequenceEqual(kdatosServidor))
                {
                    MessageBox.Show("kdatos coincide con la original utilizada para encriptar la clave privada.");
                    return true;
                }
                else
                {
                    MessageBox.Show("kdatos no coincide con la original.");
                    return false;
                }
            }
        }

        //clavePrivadaEncriptada la obtengo del servidor
        private string DesencriptarClavePrivada(string clavePrivadaEncriptada, string kdatos)
        {
            byte[] fullCipher = Convert.FromBase64String(clavePrivadaEncriptada);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(kdatos));
                byte[] iv = fullCipher.Take(16).ToArray(); // Extrae el IV (primeros 16 bytes)
                byte[] cipherText = fullCipher.Skip(16).ToArray(); // Extrae el texto cifrado

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = key;
                    aesAlg.IV = iv;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }


        private void Desencriptado(String nombre)
        {

            string RutaArchivoClave = Path.Combine(rutaGuardado, nombre + "_clave.txt");
            string RutArchivoIV = Path.Combine(rutaGuardado, nombre + "_IV.txt");
            string RutaArchivoENC = Path.Combine(rutaGuardado, nombre + ".enc");

            byte[] claveEncriptadaRSA = File.ReadAllBytes(RutaArchivoClave); // Corregido para leer la clave RSA encriptada correcta


            //DESENCRIPTADO RSA
            //Desencripto clave privada
            if (ComprobarKdatos(kdatos, kdatosServidor))
            {
                string rutaArchivoClavePrivada = Path.Combine(rutaGuardado, "clavePrivadaEncriptada.txt");
                if (File.Exists(rutaArchivoClavePrivada))
                {
                    string clavePrivadaEncriptada = File.ReadAllText(rutaArchivoClavePrivada);
                    clavePrivada = DesencriptarClavePrivada(clavePrivadaEncriptada, kdatos);
                }
                RSACryptoServiceProvider rsaDecryptor = new RSACryptoServiceProvider();
                rsaDecryptor.FromXmlString(clavePrivada); // Cargar la clave privada
                byte[] claveDesencriptadaRSA = rsaDecryptor.Decrypt(claveEncriptadaRSA, false);
                byte[] claveAES = Convert.FromBase64String(Encoding.UTF8.GetString(claveDesencriptadaRSA));

                byte[] iv = File.ReadAllBytes(RutArchivoIV);
                //byte[] key = Convert.FromBase64String(File.ReadAllText(RutaArchivoClave));

                using (Aes objetoAes = Aes.Create())
                {
                    objetoAes.Key = claveAES;
                    objetoAes.IV = iv;
                    objetoAes.Mode = CipherMode.CBC; // Modo CBC

                    // Crear un flujo de descifrado
                    using (ICryptoTransform descifrado = objetoAes.CreateDecryptor())
                    {
                        // Ruta donde se guardará el archivo desencriptado
                        string archivoDesencriptado = Path.Combine(rutaGuardadoDESENC, nombre);

                        // Crear un flujo de escritura para el archivo desencriptado
                        using (FileStream archivoDesenc = new FileStream(archivoDesencriptado, FileMode.Create))
                        {
                            // Crear un flujo de lectura para el archivo encriptado
                            using (FileStream fsIn = new FileStream(RutaArchivoENC, FileMode.Open))
                            {
                                // Leer el IV desde el archivo encriptado
                                fsIn.Read(iv, 0, iv.Length);

                                // Crear un flujo de cifrado para descifrar los datos
                                using (CryptoStream cs = new CryptoStream(fsIn, descifrado, CryptoStreamMode.Read))
                                {
                                    // Leer el archivo encriptado y escribir los datos descifrados en el archivo desencriptado
                                    int data;
                                    while ((data = cs.ReadByte()) != -1)
                                    {
                                        archivoDesenc.WriteByte((byte)data);
                                    }
                                }
                            }
                        }
                    }
                }

                MessageBox.Show("Archivo desencriptado y guardado en: " + rutaGuardadoDESENC);
                BorrarArchivoEncriptado(nombre);
                //crearNuevaFilaDESENC(nombre);
                ActualizarLista();
            }
            else
            {
                MessageBox.Show("Error al desencriptar" );

            }
        }

        private void MetodoDeEncriptado()
        {

            // Verificar si se ha seleccionado un archivo
            if (string.IsNullOrEmpty(txt1.Text))
            {
                MessageBox.Show("Por favor, seleccione un archivo para encriptar.");
                return;
            }

            // Obtener la ruta del archivo seleccionado
            string archivoOriginal = openFileDialog1.FileName;

            // Generar una clave aleatoria
            byte[] clave = GenerarClaveAleatoria();

            // Generar un IV aleatorio
            byte[] iv = GenerarIVAleatorio();
            nombreArc = Path.GetFileName(archivoOriginal);

           

            comprobarRepetidos(nombreArc);

            // Crear una instancia de AES con la clave y el IV generados
            using (Aes objetoAes = Aes.Create())
            {
                objetoAes.Key = clave;
                objetoAes.IV = iv;
                objetoAes.Mode = CipherMode.CBC; // Modo CBC

                // Convertir la clave AES a Base 64
                string claveBase64 = Convert.ToBase64String(clave);
                byte[] claveBase64Bytes = Encoding.UTF8.GetBytes(claveBase64);

                // Encriptar la cadena Base 64 con RSA
                RSACryptoServiceProvider rsaEncryptor = new RSACryptoServiceProvider();
                rsaEncryptor.FromXmlString(clavePublica); // Cargar la clave pública
                byte[] claveEncriptadaRSA = rsaEncryptor.Encrypt(claveBase64Bytes, false);

                // Guardar la clave encriptada RSA como bytes
                string archivoClave = Path.Combine(rutaGuardado, nombreArc + "_clave.txt");
                File.WriteAllBytes(archivoClave, claveEncriptadaRSA);




                string archivoIV = Path.Combine(rutaGuardado, nombreArc + "_IV.txt");
                File.WriteAllBytes(archivoIV, iv);

 
                using (ICryptoTransform cifrado = objetoAes.CreateEncryptor())
                {
                    // Ruta donde se guardará el archivo encriptado
                    string archivoEncriptado = Path.Combine(rutaGuardado, nombreArc + ".enc");

                    using (FileStream archivoenc = new FileStream(archivoEncriptado, FileMode.Create))
                    {

                        archivoenc.Write(iv, 0, iv.Length);

                        using (FileStream fsIn = new FileStream(archivoOriginal, FileMode.Open))
                        {
                            //Crear un flujo de cifrado para cifrar los datos
                            using (CryptoStream cs = new CryptoStream(archivoenc, cifrado, CryptoStreamMode.Write))
                            {
                                // Leer el archivo original y escribir los datos cifrados en el archivo encriptado
                                int data;
                                while ((data = fsIn.ReadByte()) != -1) // va byte a byte escribiendo el cifrado correspondiente, si llega al final devuelve -1 y para el bucle
                                {
                                    cs.WriteByte((byte)data);
                                }
                            }
                        }
                    }
                }
            }

            // Notificar al usuario que la encriptación se ha completado
            MessageBox.Show("Archivo encriptado y guardado en: " + rutaGuardado);

            // Crear una nueva fila en la interfaz gráfica
            CrearNuevaFila();

            //Actualiz la paggina :

            ActualizarLista();
        }

        // Método para generar una clave aleatoria de 32 bytes (256 bits)
        private byte[] GenerarClaveAleatoria()
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] clave = new byte[32]; // 32 bytes para AES-256
                rng.GetBytes(clave);
                return clave;
            }
        }

        // Método para generar un IV aleatorio de 16 bytes (128 bits)
        private byte[] GenerarIVAleatorio()
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] iv = new byte[16]; // 16 bytes para AES
                rng.GetBytes(iv);
                return iv;
            }
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

        private void CrearNuevaFila()
        {
            // CREACION DE LAS FILAS
            Label txtCifrado = new Label();
            
            txtCifrado.Text = nombreArc; //ESTO ES TEMPORAL YA QUE HABRÁ QUE PONER EL ARCHIVO CIFRADO
            txtCifrado.Width = 150;
            txtCifrado.Height = 30;

            System.Windows.Forms.Button btnDesencriptar = new System.Windows.Forms.Button();
            btnDesencriptar.Text = "Desencriptar";
            btnDesencriptar.Width = 80;
            btnDesencriptar.Height = 30;

            System.Windows.Forms.Button btnBorrar = new System.Windows.Forms.Button();
            btnBorrar.Text = "Borrar";
            btnBorrar.Width = 80;
            btnBorrar.Height = 30;

            // Crea un nuevo contenedor para la fila (por ejemplo, un panel)
            Panel nuevaFila = new Panel();
            nuevaFila.Width = panelContenedor.Width;
            nuevaFila.Height = 30;

            // Establece las ubicaciones de los controles dentro de la fila
            txtCifrado.Location = new System.Drawing.Point(0, 0);
            btnDesencriptar.Location = new System.Drawing.Point(txtCifrado.Right + 5, 0);
            btnBorrar.Location = new System.Drawing.Point(btnDesencriptar.Right + 5, 0);

            // Agrega los controles a la fila
            nuevaFila.Controls.Add(txtCifrado);
            nuevaFila.Controls.Add(btnDesencriptar);
            nuevaFila.Controls.Add(btnBorrar);

            // Establece la posición de la fila
            nuevaFila.Location = new System.Drawing.Point(10, posicionVertical);

            // Incrementa la posición vertical para la próxima fila
            posicionVertical += nuevaFila.Height + 5; // Puedes ajustar el espacio entre filas

            // Asigna un manejador de eventos a los botones de la fila
            btnDesencriptar.Click +=  (sender, e ) => Desencriptado(txtCifrado.Text);
            btnBorrar.Click += (sender, e) => EliminarFila(nuevaFila, txtCifrado.Text,1);//he quitado + ".enc"

            // Agrega la nueva fila al panel de contenedor
            panelContenedor.Controls.Add(nuevaFila);

            txt1.Text = "";
        }

        private void EliminarFila(Panel fila,string nombreArchivo,int modo)
        {
            if(modo == 1)
            {
                BorrarArchivoEncriptado(nombreArchivo);
                // Elimina la fila del panel de contenedor y del diccionario
                panelContenedor.Controls.Remove(fila);
                panelContenedor.Controls.Clear();
            }
            else
            {
                string rutaArchivoDESENC = Path.Combine(rutaGuardadoDESENC, nombreArchivo);
                File.Delete(rutaArchivoDESENC);
                panelDESENC.Controls.Remove(fila);
                panelDESENC.Controls.Clear();
            }
           
            // Busca y elimina la entrada correspondiente en el diccionario
            foreach (var kvp in botonesYFilas)
            {
                if (kvp.Value == fila)
                {
                    botonesYFilas.Remove(kvp.Key);
                    break;
                }
            }

            ActualizarLista();
        }

        private void ActualizarLista()
        {
            // Limpiar la lista existente
            panelContenedor.Controls.Clear();
            panelDESENC.Controls.Clear();
            posicionVertical = 10; // Restablecer la posición vertical
            posicionVerticalDESENC = 10;

            // Repoblar la lista de archivos encriptados
            ComprobarArchivosEncriptados();
        }

        private void BorrarArchivoEncriptado(string nombreArchivo)
        {
            
            string rutaArchivoEncriptado = Path.Combine(rutaGuardado, nombreArchivo+".enc");
            string rutaClave = Path.Combine(rutaGuardado, nombreArchivo+"_clave.txt");
            string rutaIV = Path.Combine(rutaGuardado, nombreArchivo + "_IV.txt");
            // Verifica si existen los archivos
            if (File.Exists(rutaArchivoEncriptado) && File.Exists(rutaClave) && File.Exists(rutaIV))
            {
                // Elimina los archivos encriptados
                File.Delete(rutaArchivoEncriptado);
                File.Delete(rutaClave);
                File.Delete(rutaIV);
            }
            ActualizarLista();
        }
        private void panelContenedor_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comprobarRepetidos(String nom)
        {     
            string[] archivos = Directory.GetFiles(rutaGuardado, "*.enc");

            if (archivos.Length != 0)
            {
                bool nombreExiste = archivos.Any(archivo => Path.GetFileNameWithoutExtension(archivo) == nom);

                while (nombreExiste)
                {
                    int random = generadorRandom();
                    nom = random.ToString() + nom;
                    nombreExiste = archivos.Any(archivo => Path.GetFileNameWithoutExtension(archivo) == nom);
                }

                nombreArc = nom;
            }
        }

        private int generadorRandom()
        {
            return rnd.Next(0, 100); // Genera un número aleatorio entre 0 y 99    
        }



        private void crearNuevaFilaDESENC(String nom)
        {

            Label txtDESENC = new Label();

            txtDESENC.Text = nom; //ESTO ES TEMPORAL YA QUE HABRÁ QUE PONER EL ARCHIVO CIFRADO
            txtDESENC.Width = 150;
            txtDESENC.Height = 30;

            System.Windows.Forms.Button btnBorrar = new System.Windows.Forms.Button();
            btnBorrar.Text = "Borrar";
            btnBorrar.Width = 80;
            btnBorrar.Height = 30;

            Panel nuevaFila = new Panel();
            nuevaFila.Width = panelDESENC.Width;
            nuevaFila.Height = 30;

            txtDESENC.Location = new System.Drawing.Point(0, 0);
            btnBorrar.Location = new System.Drawing.Point(txtDESENC.Right + 5, 0);

            nuevaFila.Controls.Add(txtDESENC);
            nuevaFila.Controls.Add(btnBorrar);


            nuevaFila.Location = new System.Drawing.Point(10, posicionVerticalDESENC);

            // Incrementa la posición vertical para la próxima fila
            posicionVerticalDESENC += nuevaFila.Height + 5; // Puedes ajustar el espacio entre filas

            btnBorrar.Click += (sender, e) => EliminarFila(nuevaFila, txtDESENC.Text,2);//he quitado + ".enc"

            panelDESENC.Controls.Add(nuevaFila);

        }

        //Comprueba archivos de las carpetas
        private void ComprobarArchivosEncriptados()
        {
            if (!Directory.Exists(rutaGuardado) || !Directory.Exists(rutaGuardadoDESENC))
            {
                return;
            }

            // Buscar archivos en la carpeta de guardado
            string[] archivos = Directory.GetFiles(rutaGuardado, "*.enc");
            string[] archivosDESENC = Directory.GetFiles(rutaGuardadoDESENC);

            foreach (string archivo in archivos)
            {
                nombreArc = Path.GetFileNameWithoutExtension(archivo);
                string claveFile = Path.Combine(rutaGuardado, nombreArc + "_clave.txt");
                string ivFile = Path.Combine(rutaGuardado, nombreArc + "_IV.txt");

                // Verificar si existen los archivos de clave y IV
                if (File.Exists(claveFile) && File.Exists(ivFile))
                {
                    // Crear una nueva fila en la interfaz gráfica
                    CrearNuevaFila();

                    // Agregar el nombre del archivo encriptado
                    foreach (Panel fila in panelContenedor.Controls)
                    {
                        Label txtCifrado = (Label)fila.Controls[0];
                        if (txtCifrado.Text == "")
                        {
                            txtCifrado.Text = nombreArc;
                            txtCifrado.Width = 150;
                            txtCifrado.Height = 30;
                        }
                    }
                }
            }

            foreach (string arch in archivosDESENC)
            {
                string nombreArchivoDESENC = Path.GetFileName(arch);

                crearNuevaFilaDESENC(nombreArchivoDESENC);

                foreach (Panel fila in panelDESENC.Controls)
                {
                    Label txtDESENC = (Label)fila.Controls[0];
                    if (txtDESENC.Text == "")
                    {
                        txtDESENC.Text = nombreArchivoDESENC;
                        txtDESENC.Width = 150;
                        txtDESENC.Height = 30;
                    }
                }
            }
        }

        public void ComprobarCarpeta()
        {
            string carpetaPractica = Path.Combine(Environment.CurrentDirectory, "CSarchivosENC");
            string carpetaPractica2 = Path.Combine(Environment.CurrentDirectory, "CSarchivosDESENC");

            if (!Directory.Exists(carpetaPractica))
            {
                // La carpeta "CSarchivosENC" no existe, créala
                Directory.CreateDirectory(carpetaPractica);
            }
            rutaGuardado = carpetaPractica;

            if (!Directory.Exists(carpetaPractica2))
            {
                // La carpeta "CSarchivosENC" no existe, créala
                Directory.CreateDirectory(carpetaPractica2);
            }
            rutaGuardadoDESENC = carpetaPractica2;
        }
    }
}
