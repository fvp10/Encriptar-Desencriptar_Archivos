using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
//Librerías para las petición HTTP REQUEST
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO.Compression;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Net;
using System.Net.Http.Headers;
using Practica_CS_encriptar_desencriptar_F1.Modelos;

namespace Practica_CS_encriptar_desencriptar_F1
{
    public partial class Form1 : Form

    {
        private int posicionVertical = 10; // Inicializa la posición vertical
        private int posicionVerticalDESENC = 10;
        private Dictionary<System.Windows.Forms.Button, Panel> botonesYFilas = new Dictionary<System.Windows.Forms.Button, Panel>();
        private string rutaGuardado = Environment.CurrentDirectory;
        private string nombreArc;
        private Random rnd = new Random();
        ///Variables para implementación del servidor
        private string kdatos = "";
        private bool estaDesencriptada = false;
        private string clavePublica = "";
        private string clavePrivada = "";
        private string nombreRemitente;

        private static readonly HttpClient _httpClient = new HttpClient();
        private const string ServerUrl = "https://localhost:7045/Users/";


        public Form1(string username,string kdatosRecibidos)
        {
            InitializeComponent();
            PedirYGuardarDatos(username);

            kdatos = kdatosRecibidos;
            this.nombreRemitente = username;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Puedes preguntar al usuario si realmente desea cerrar la aplicación
            if (MessageBox.Show("¿Estás seguro de que quieres salir y borrar los datos temporales?",
                                "Confirmar Salida",
                                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Comprimir();
                EnviarZipAlServidorAsync(rutaGuardado,nombreRemitente, ServerUrl);
                BorrarCarpetaUsuario();
                this.Hide(); // OCULTAMOS EL LOGIN
                LoginForm main = new LoginForm();   //CREAMOS UN NUEVO FORM 
                main.Show(); //LO MOSTRAMOS
            }
            else
            {
                // Si el usuario decide no salir después de todo, puedes cancelar el cierre del formulario
                e.Cancel = true;
            }
        }


        private static async Task<bool> EnviarZipAlServidorAsync(string zipPath, string username, string baseUrl)
        {
            // Comprobar que el archivo existe
            if (!File.Exists(Path.Combine(zipPath, username + ".zip")))
            {
                Console.WriteLine("El archivo ZIP no existe en la ruta proporcionada.");
                return false;
            }

            // Construir la URL completa usando el nombre de usuario
            string apiUrl = $"{baseUrl}{username}/refrescarDatos";

            try
            {
                using (var client = new HttpClient())
                using (var content = new MultipartFormDataContent())
                using (var fileStream = new FileStream(Path.Combine(zipPath, username + ".zip"), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "file",
                        FileName = Path.GetFileName(Path.Combine(zipPath, username + ".zip"))
                    };
                    content.Add(fileContent);

                    // Enviar la solicitud POST al servidor
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    // Verificar si la respuesta es exitosa
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("El archivo ZIP se ha enviado correctamente.");

                    }
                    else
                    {
                        Console.WriteLine($"Error al enviar el archivo: {response.StatusCode}");
                        return false;
                    }

                }
                File.Delete(Path.Combine(zipPath, username + ".zip"));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al enviar el archivo: {ex.Message}");
                return false;
            }
        }

        public static void AddDirectoryToZip(ZipArchive archive, string directoryPath, string entryNameInZip)
        {
            // Comprobar si el directorio está vacío
            if (Directory.GetFiles(directoryPath, "*", SearchOption.TopDirectoryOnly).Length == 0)
            {
                // Si el directorio está vacío, agregarlo como una entrada de directorio
                archive.CreateEntry(entryNameInZip + "/");
            }
            else
            {
                // Si el directorio contiene archivos, agregar cada archivo al ZIP
                foreach (string file in Directory.GetFiles(directoryPath, "*", SearchOption.TopDirectoryOnly))
                {
                    // Crea el nombre de la entrada en el archivo ZIP
                    string relativePath = file.Substring(directoryPath.Length + 1);
                    string entryName = Path.Combine(entryNameInZip, relativePath).Replace('\\', '/');

                    // Agregar archivo al ZIP
                    archive.CreateEntryFromFile(file, entryName);
                }
            }
        }


        private void Comprimir()
        {
            string zipPath = Path.Combine(rutaGuardado,nombreRemitente + ".zip");

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    string folder1Path = Path.Combine(rutaGuardado, "CSarchivosENC");
                    string folder2Path = Path.Combine(rutaGuardado, "CSarchivosDESENC");

                    // Añade la primera carpeta al archivo ZIP
                    AddDirectoryToZip(archive, folder1Path, "CSarchivosENC");

                    // Añade la segunda carpeta al archivo ZIP
                    AddDirectoryToZip(archive, folder2Path, "CSarchivosDESENC");
                }

                // Guarda el archivo ZIP
                File.WriteAllBytes(zipPath, memoryStream.ToArray());
            }
        }

        // La función para borrar la carpeta del usuario sigue siendo la misma
        private void BorrarCarpetaUsuario()
        {
            string carpetaUsuario = rutaGuardado;
            try
            {
                if (Directory.Exists(rutaGuardado))
                {
                    Directory.Delete(rutaGuardado, true);
                }
            }
            catch (Exception ex)
            {
                // Aquí puedes decidir si quieres mostrar un error o simplemente registrar el problema
                // MessageBox.Show("No se pudo borrar la carpeta del usuario: " + ex.Message);
            }
        }


        private async Task<bool> PedirYGuardarDatos(string usuario)
        {
            if (!Directory.Exists(rutaGuardado))
            {
                Directory.CreateDirectory(rutaGuardado);
            }

            DirectoryInfo rutaanterior = new DirectoryInfo(rutaGuardado);
            

            string zipPath = Path.Combine(rutaanterior.Parent.FullName, usuario + "_datos.zip");

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(ServerUrl + usuario + "/get-folder");

                if (response.IsSuccessStatusCode)
                {
                    // Leer los bytes del archivo ZIP de la respuesta
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();

                    // Guardar los bytes del archivo en el sistema de archivos
                    File.WriteAllBytes(zipPath, fileBytes);

                    MessageBox.Show("Datos recibidos y archivo ZIP guardado en: " + rutaGuardado);

                    // Llamar a la función de descompresión y organización aquí
                    bool descomprimirYOrganizar = await DescomprimirYOrganizarArchivos(zipPath, rutaGuardado,usuario);
                    return descomprimirYOrganizar;
                }
                else
                {
                    // Manejar las respuestas no exitosas aquí
                    MessageBox.Show($"Error al recibir los datos. Código de estado: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de comunicación con el servidor: " + ex.Message);
            }

            return false;
        }

        private async Task<bool> DescomprimirYOrganizarArchivos(string zipPath, string extractPath,string nusuario)
        {
            try
            {
                var rutacarpetanueva = Path.Combine(extractPath, nusuario);

                // Asegúrate de que la ruta de extracción existe
                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }

                if (!Directory.Exists(rutacarpetanueva))
                {
                    Directory.CreateDirectory(rutacarpetanueva);
                }

                rutaGuardado = rutacarpetanueva;

                // Descomprimir el 

                ZipFile.ExtractToDirectory(zipPath, rutacarpetanueva);
                File.Delete(zipPath);
                

                // Mover los archivos a sus ubicaciones correspondientes
               
                string[] nombresArchivos = { "publicKey.xml", "privateKeyEncrypted.xml"};
                foreach (var nombreArchivo in nombresArchivos) 
                {
                    string archivoOrigen = Path.Combine(rutacarpetanueva, nombreArchivo);
                    // Determina la carpeta de destino basada en el nombre del archivo
                    if (nombreArchivo == "publicKey.xml")
                    {
                        clavePublica = File.ReadAllText(archivoOrigen);
                    }
                    else if (nombreArchivo == "privateKeyEncrypted.xml")
                    {
                        clavePrivada = File.ReadAllText(archivoOrigen);
                    }
                }

                MessageBox.Show("Archivo ZIP descomprimido y archivos organizados correctamente.");
                ComprobarArchivosEncriptados();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al descomprimir y organizar los archivos: " + ex.Message);
                return false;
            }
        }

        private void desencriptar__Click(object sender, EventArgs e)
        {
            MessageBox.Show("Proceso de desencriptar");
            

        }
        private void button1_Click(object sender, EventArgs e)
        {
            showModalCompartir();
        }

        private void encriptar__Click(object sender, EventArgs e)
        {
            MetodoDeEncriptado();
        }

        private string DesencriptarClavePrivada(string cipherText, string password)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            byte[] key = new byte[32]; // AES-256 requiere una clave de 32 bytes
            byte[] iv = new byte[16]; // El IV para AES siempre necesita 16 bytes
            Array.Copy(Encoding.UTF8.GetBytes(password.PadRight(key.Length)), key, key.Length);
            Array.Copy(Encoding.UTF8.GetBytes(password.PadRight(iv.Length)), iv, iv.Length);
            estaDesencriptada = true;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                    }
                    return Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            
        }

        private void Desencriptado(String nombre)
        {

            string RutaArchivoClave = Path.Combine(rutaGuardado , "CSarchivosENC", nombre + "_clave.txt");
            string RutArchivoIV = Path.Combine(rutaGuardado , "CSarchivosENC", nombre + "_IV.txt");
            string RutaArchivoENC = Path.Combine(rutaGuardado , "CSarchivosENC", nombre + ".enc");

            byte[] claveEncriptadaRSA = File.ReadAllBytes(RutaArchivoClave); // Corregido para leer la clave RSA encriptada correcta


            //DESENCRIPTADO RSA
            //Desencripto clave privada

            //DESENCRIPTAR CLAVE PRIVADA
            //QUITAR LA RUTA 

            if (!estaDesencriptada) {
                clavePrivada = DesencriptarClavePrivada(clavePrivada, kdatos);
            }
            
            

            //***DESENCRIPTAR CLAVE AES CON CLAVE PRIVADA***
            RSACryptoServiceProvider rsaDecryptor = new RSACryptoServiceProvider();
            rsaDecryptor.FromXmlString(clavePrivada); // Cargar la clave privada
            byte[] claveDesencriptadaRSA = rsaDecryptor.Decrypt(claveEncriptadaRSA, false);
            byte[] claveAES = claveDesencriptadaRSA;

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
                    string archivoDesencriptado = Path.Combine(rutaGuardado ,"CSarchivosDESENC", nombre);

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

            MessageBox.Show("Archivo desencriptado y guardado en: " + rutaGuardado + "CSarchivosDESENC");
            
            BorrarArchivoEncriptado(nombre);
            //crearNuevaFilaDESENC(nombre);
            ActualizarLista();

        }


        private void MetodoDeEncriptado()
        {
            if (!VerificarSeleccionDeArchivo())
            {
                return;
            }

            string rutaArchivoOriginal = openFileDialog1.FileName;
            asignarNombreUnico(rutaArchivoOriginal); // Asignamos a NombreArc un id unico

            byte[] clave = GenerarClaveAleatoria();
            byte[] iv = GenerarIVAleatorio();

            EncriptarArchivo(rutaArchivoOriginal, clave, iv,"");

            MessageBox.Show("Archivo encriptado y guardado en: " + rutaGuardado + "CSarchivosENC");
            CrearNuevaFila();
            ActualizarLista();
        }

        //Verifica si alrchivo ha sido seleccionado en el form
        private bool VerificarSeleccionDeArchivo()
        {
            if (string.IsNullOrEmpty(txt1.Text))
            {
                MessageBox.Show("Por favor, seleccione un archivo.");
                return false;
            }
            return true;
        }

        //Encripta el archivo con la clave y el iv
        private void EncriptarArchivo(string archivoOriginal, byte[] clave, byte[] iv,string rutaCarpetaTemp)
        {
            using (Aes objetoAes = Aes.Create())
            {
                objetoAes.Key = clave;
                objetoAes.IV = iv;
                objetoAes.Mode = CipherMode.CBC;

               

                string archivoEncriptado = "";

                if (rutaCarpetaTemp == "")
                {
                    archivoEncriptado = Path.Combine(rutaGuardado, "CSarchivosENC", nombreArc + ".enc");
                    EncriptarClaveRSA(clave, iv, rutaCarpetaTemp, "");
                }
                else
                {
                    string nombreArchivoEncriptado = $"compartidoPor_{nombreRemitente}_{nombreArc}.enc";
                    archivoEncriptado = Path.Combine(rutaCarpetaTemp,nombreArchivoEncriptado);

                }

                

                using (ICryptoTransform cifrado = objetoAes.CreateEncryptor())
                using (FileStream archivoEnc = new FileStream(archivoEncriptado, FileMode.Create))
                {
                    archivoEnc.Write(iv, 0, iv.Length);
                    CifrarContenidoArchivo(archivoOriginal, cifrado, archivoEnc);
                }
            }
        }


        //Encripta la clave del archivo con la publica y guarda ambos en ../
        private void EncriptarClaveRSA(byte[] clave, byte[] iv, string ruta, string clavePubUs)
        {
            //Cargamos la clave publica
            RSACryptoServiceProvider rsaEncryptor = new RSACryptoServiceProvider();
            if (clavePubUs != "")
            {
                rsaEncryptor.FromXmlString(clavePubUs);
            }
            else
            {
                rsaEncryptor.FromXmlString(clavePublica);
            }
            //Encriptamos la clave publica
            byte[] claveEncriptadaRSA = rsaEncryptor.Encrypt(clave, false);

            string nombreClave = $"{nombreArc}_clave.txt";
            string nombreIV = $"{nombreArc}_IV.txt";

            string rutaeEncriptado = Path.Combine(rutaGuardado, "CSarchivosENC");

            if (ruta != "")
            {
                nombreClave = $"compartidoPor_{nombreRemitente}_{nombreArc}_clave.txt";
                nombreIV = $"compartidoPor_{nombreRemitente}_{nombreArc}_IV.txt";

                rutaeEncriptado = ruta;
            }
            //Creo el archivo con su nombre CLAVE
            string archivoClave = Path.Combine(rutaeEncriptado, nombreClave);
            File.WriteAllBytes(archivoClave, claveEncriptadaRSA);

            //Creo el archivo con su nombre IV
            string archivoIV = Path.Combine(rutaeEncriptado, nombreIV);
            File.WriteAllBytes(archivoIV, iv);
        }

        //Coge el archivo y la instancia de criptostream
        private void CifrarContenidoArchivo(string archivoOriginal, ICryptoTransform cifrado, FileStream archivoEnc)
        {
            using (FileStream fsIn = new FileStream(archivoOriginal, FileMode.Open)) // Abre el archivo y crea otro para volcar lo encriptado
            using (CryptoStream cs = new CryptoStream(archivoEnc, cifrado, CryptoStreamMode.Write)) // Escribe en el archvio caada dato a encriptado
            {
                int data;
                while ((data = fsIn.ReadByte()) != -1)
                {
                    cs.WriteByte((byte)data);
                }
            }
        }



        private async void showModalCompartir()
        {
            if (!VerificarSeleccionDeArchivo())
            {
                return;
            }

            List<Usuario> userList = await GetUserList();
            var modalForm = new ModalCompartir(userList);
            var result = modalForm.ShowDialog();



            if (result == DialogResult.OK)
            {
                var selectedUsers = modalForm.SelectedUsuarios;
                byte[] claveAES = GenerarClaveAleatoria();
                byte[] ivAES = GenerarIVAleatorio();

                string carpetaTemporal = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(carpetaTemporal);

                string rutaArcOriginal = openFileDialog1.FileName;
                asignarNombreUnico(rutaArcOriginal);

                EncriptarArchivo(rutaArcOriginal, claveAES, ivAES, carpetaTemporal);

                foreach (var usuario in selectedUsers)
                {
                   
                    EncriptarClaveRSA(claveAES, ivAES, carpetaTemporal,usuario.PublicKey);

                    
                    string rutaArchivoZip = ComprimirCarpeta(carpetaTemporal);

                    bool exito = await EnviarDatos(usuario.NombreUsuario, rutaArchivoZip);
                    if (!exito)
                    {
                        MessageBox.Show("Error al enviar datos para el usuario: " + usuario.NombreUsuario);
                    }

                    if (File.Exists(rutaArchivoZip))
                    {
                        File.Delete(rutaArchivoZip);
                    }


                }

                Directory.Delete(carpetaTemporal, true);
            }
        }

        private string ComprimirCarpeta(string carpetaTemporal)
        {
            string archivoZip = Path.Combine(Path.GetTempPath(), "compartido_" + Guid.NewGuid().ToString() + ".zip");
            if (File.Exists(archivoZip))
            {
                File.Delete(archivoZip);
            }

            ZipFile.CreateFromDirectory(carpetaTemporal, archivoZip);
            return archivoZip;
        }


        private async Task<bool> EnviarDatos(string nombreUsuario, string rutaArchivoZip)
        {
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new StreamContent(File.OpenRead(rutaArchivoZip));
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "file",
                        FileName = Path.GetFileName(rutaArchivoZip)
                    };
                    content.Add(fileContent);

                    var response = await _httpClient.PostAsync(ServerUrl + $"{nombreUsuario}" + "/uploadSharedFiles", content);
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar datos: " + ex.Message);
                return false;
            }
        }

        private async Task<List<Usuario>> GetUserList()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(ServerUrl + $"{nombreRemitente}" + "/getAllUsers");


            if (response.IsSuccessStatusCode)
            {
                string userListJson = await response.Content.ReadAsStringAsync();
                List<Usuario> usuarios = JsonConvert.DeserializeObject<List<Usuario>>(userListJson);
                return usuarios; // Devuelve la lista de usuarios deserializada
            }
            else
            {
                return new List<Usuario>(); // Retorna una lista vacía si la solicitud no es exitosa
            }
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
            btnDesencriptar.Click += (sender, e) => Desencriptado(txtCifrado.Text);
            btnBorrar.Click += (sender, e) => EliminarFila(nuevaFila, txtCifrado.Text, 1);//he quitado + ".enc"

            // Agrega la nueva fila al panel de contenedor
            panelContenedor.Controls.Add(nuevaFila);

            txt1.Text = "";
        }

        private void EliminarFila(Panel fila, string nombreArchivo, int modo)
        {
            if (modo == 1)
            {
                BorrarArchivoEncriptado(nombreArchivo);
                // Elimina la fila del panel de contenedor y del diccionario
                panelContenedor.Controls.Remove(fila);
                panelContenedor.Controls.Clear();
            }
            else
            {
                string rutaArchivoDESENC = Path.Combine(rutaGuardado , "CSarchivosDESENC", nombreArchivo);
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

            string rutaArchivoEncriptado = Path.Combine(rutaGuardado, "CSarchivosENC", nombreArchivo + ".enc");
            string rutaClave = Path.Combine(rutaGuardado,"CSarchivosENC", nombreArchivo + "_clave.txt");
            string rutaIV = Path.Combine(rutaGuardado,"CSarchivosENC", nombreArchivo + "_IV.txt");
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

        private void asignarNombreUnico(String rutaNom)
        {
            var random = $"{DateTime.UtcNow.Ticks}-{new Random().Next()}";
            nombreArc = random + Path.GetFileName(rutaNom);
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

                btnBorrar.Click += (sender, e) => EliminarFila(nuevaFila, txtDESENC.Text, 2);//he quitado + ".enc"

                panelDESENC.Controls.Add(nuevaFila);

            }

        private void ComprobarArchivosEncriptados()
        {

            // Buscar archivos en la carpeta de guardado
            string[] archivos = Directory.GetFiles(Path.Combine(rutaGuardado, "CSarchivosENC"), "*.enc");
            string[] archivosDESENC = Directory.GetFiles(Path.Combine(rutaGuardado, "CSarchivosDESENC"));

            foreach (string archivo in archivos)
            {
                nombreArc = Path.GetFileNameWithoutExtension(archivo);
                string claveFile = Path.Combine(rutaGuardado, "CSarchivosENC", nombreArc + "_clave.txt");
                string ivFile = Path.Combine(rutaGuardado, "CSarchivosENC", nombreArc + "_IV.txt");

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

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
