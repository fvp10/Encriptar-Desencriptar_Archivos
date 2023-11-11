# Memoria del Proyecto de Encriptación y Desencriptación

## Índice 📑
* [Introducción](#introducción) 🚀
* [Fases del Proyecto](#fases-del-proyecto) 📈
* [Manual de Usuario](#manual-de-usuario) 📖
* [Software Utilizado](#software) 💻
* [Librerías](#librerías) 📚
* [Detalles de Implementación](#detalles-de-implementación) 🔨
  - [Fase 1](#fase-1)
    * [Interfaz Dinámica](#interfaz-dinámica) 🖥️
    * [Encriptar](#encriptar) 🔒
    * [Desencriptar](#desencriptar) 🔓
  - [Fase 2](#fase-2)
    * [Lado Cliente](#lado-cliente) 🖥️
      - [Interfaz Login](#interfaz-login)
      - [Interfaz de Cifrado/Descifrado](#interfaz-de-cifradodescifrado)
    * [Lado Servidor](#lado-servidor) 🖥️
      - [Gestión de Usuarios y Claves](#gestión-de-usuarios-y-claves)
* [Team](#team) 👫


## Introducción 🚀
Esto es una práctica dentro de la carrera __Ing.Multimedia__ en la __Universidad de Alicante__. En este proyecto se realiza un trabajo en grupo (4 alumnos) relacionado con la seguridad y donde se pretende que desarrollemos conocimientos en el ambito de seguridad. 

El proyecto irá cambiando en función con las fases hasta tener un programa completo con todas las funcionalidades.

## Fases del proyecto 📈

| Fase | Descripción | Requisitos | Estado |
|------|-------------|------------|-------|
| Fase 1 | Implementación de un sistema que cifra y descifra archivos multimedia con AES256 | - Cifrado de al menos 6 archivos multimedia con claves diferentes <br> - Almacenamiento de claves en un archivo o base de datos | ✅ Completado |
| Fase 2 | Diseño e implementación de un sistema para autentificar al administrador y encriptar claves AES con RSA | - Autenticación del administrador <br> - Generación de claves RSA (pública y privada) <br> - Encriptación de claves AES con clave pública | ✅ Completado |
| Fase 3 | Sistema para el acceso de usuarios a los contenidos cifrados | - Generación de claves RSA para cada usuario <br> - Sistema de acceso a contenidos cifrados <br> - Mejoras en el sistema de autenticación | ❌ Pendiente |

## Manual de Usuario 📖

__Iniciar Sesión__
1. Introducir usuario y contraseña.
2. Haz click en el botón login para poder encriptar y desencriptar archivos.

__Encriptar un Archivo__ 🔒
1. Abre la aplicación.
2. Haz clic en el botón "Seleccionar Archivo" y elije el archivo que deseas encriptar.
3. Haz clic en "Encriptar". El archivo encriptado se guardará en una carpeta específica, y se mostrará un mensaje de confirmación.

__Desencriptar un Archivo__ 🔓
1. En la lista de archivos encriptados, haz clic en "Desencriptar" al lado del archivo que desea desencriptar.
2. El archivo desencriptado se guardará en otra carpeta, se borrara en la que se encontraba actualmente, y se mostrará un mensaje de confirmación.

## Software 💻
* __Visual Studio:__ Entorno de desarrollo integrado de Microsoft utilizado para desarrollar aplicaciones, sitios web, servicios web y aplicaciones móviles.

* __.NET Framework__ Tecnología que soporta la construcción y ejecución de aplicaciones y servicios web, simplifica el desarrollo y la implementación de aplicaciones.

__Lenguaje: C#__

## Librerías 📚

<span style="font-size: larger; font-weight: bold; color:#ED9B40 ">Fase 1</span>

```
System: Se usa para operaciones generales y básicas

System.Collections.Generic: Almacenado y manipulación de dátos, como "botonesYFilas".

System.IO: Para la lectura y escritura de archivos.

System.Windows.Forms: Construye y manipula la interfaz gráfica, como botones y paneles.

System.Security.Cryptography:  Se utiliza para encriptar y desencriptar archivos.

System.Linq: Se usa para manipular y consultar datos a la hora del uso del "any".

``` 
<span style="font-size: larger; font-weight: bold; color:#ED9B40 ">Fase 2</span>

```
Microsoft.AspNetCore.Mvc: Librería de ASP.NET Core que facilita la creación de aplicaciones web siguiendo el patrón Modelo-Vista-Controlador (MVC).

System.IO.Compression: Proporciona clases y métodos para trabajar con compresión y descompresión de datos.

System.Net.Http: Contiene clases para realizar solicitudes y recibir respuestas HTTP.

System.Threading.Tasks: Incluye clases y tipos que admiten programación asincrónica basada en tareas.

System.Text.Json: Facilita la serialización y deserialización de objetos en formato JSON
```

<!-- ### Fase 2
```
```
### Fase 3
```
``` -->

## Detalles de implementación 🔨

<span style="font-size: larger; font-weight: bold; color:#ED9B40 ">Fase 1</span>

#### Interfaz Dinámica 🖥️
El código utiliza un diseño dinámico donde los archivos encriptados se muestran en tiempo real. Cuando un archivo se encripta, se agrega automáticamente a la lista visual en la interfaz de usuario. Esto se logra a través del método <span style="color:#9FD1F5">CrearNuevaFila()</span> que crea una nueva fila en la interfaz para cada archivo encriptado.

Para borrar un archivo, se usa el método <span style="color:#9FD1F5">EliminarFila()</span> que remueve la fila de la interfaz y elimina el archivo encriptado, así como sus claves asociadas, del sistema de archivos.

La actualización de la lista visual se realiza a través del método <span style="color:#9FD1F5">ActualizarLista()</span> , que limpia y reconstruye la lista de archivos encriptados basándose en los archivos presentes en el directorio de almacenamiento.

Para verificar y listar los archivos encriptados al iniciar la aplicación, se utiliza el método <span style="color:#9FD1F5">ComprobarArchivosEncriptados()</span> .

#### Encriptar 🔒
El proceso de encriptación se inicia cuando el usuario selecciona un archivo para encriptar y hace clic en "Encriptar". El evento asociado __(encriptar__Click)__ llama al método  <span style="color:#9FD1F5">MetodoDeEncriptado()</span>.

1. <span style="color:#9FD1F5">MetodoDeEncriptado()</span>: Este método se asegura de que un archivo esté seleccionado, genera una clave y un IV aleatorios, y procede a encriptar el archivo seleccionado. Utiliza AES para la encriptación.

2. <span style="color:#9FD1F5">GenerarClaveAleatoria() y GenerarIVAleatorio()</span>: Estos métodos usan RNGCryptoServiceProvider para generar una clave y un IV aleatorios.

3. El archivo encriptado, junto con la clave y el IV, se almacenan en archivos separados dentro de un directorio específico.

4. La interfaz se actualiza automáticamente para listar el nuevo archivo encriptado mediante el <span style="color:#9FD1F5">ActualizarLista()</span>.

#### Desencriptar 🔓
El  proceso de desencriptación inicia cuando el usuario hace clic en "Desencriptar" al lado de un archivo encriptado listado en la interfaz. El evento asociado __(desencriptar__Click)__ llama al método <span style="color:#9FD1F5">Desencriptado()</span> .

1. <span style="color:#9FD1F5">Desencriptado(String nombre)</span>: Este método recupera la clave y el IV asociados con el archivo encriptado desde sus archivos respectivos. Luego, utiliza estos para desencriptar el archivo.

2. Utiliza AES para la desencriptación, especificando la clave y el IV, y el modo CBC.

3. El archivo desencriptado se almacena en un directorio específico, y el usuario recibe una notificación de que la desencriptación fue exitosa.

4. La interfaz se actualiza automáticamente para reflejar los cambios con el <span style="color:#9FD1F5">ActualizarLista()</span>, y el archivo desencriptado se puede acceder desde el directorio especificado.

<span style="font-size: larger; font-weight: bold; color:#ED9B40 ">Fase 2</span>
<p>Esta segunda parte consiste en la implementación de un servidor para recibir los archivos pertenecientes al usuario ya esten encriptados o no. Al hacer diferenciación por usuarios esta fase implementa una nueva interfaz para que un usuario pueda hacer Login y acceder a sus archivos alojados en el servidor.</P>

#### Lado Cliente🖥️

#### Interfaz Login
La siguiente imagen representa la interfaz que tendrá el usuario para poder iniciar sesión: 
    ![](/imgs/login.png)

1.  Incicio de sesión: El usuario ingrsará sus credenciales para poder acceder a sus archivos. La constraseña que ingresa el usuario la utilizaremos para crear un <span style="color:#9FD1F5">Hash</span> y dividirla en dos parte: <span style="color:#9FD1F5">Kdatos</span> y <span style="color:#9FD1F5">Klogin</span>. Aqui podemos ver como se hace el <span style="color:#9FD1F5">Hash</span> de la contraseña del usuario y como se parten en <span style="color:#9FD1F5">Kdatos</span> y <span style="color:#9FD1F5">Klogin</span>.

    ![](/imgs/inicioSesion.png)
2.  Autentificación: el cliente cuando ingresa sus credenciales envia una solicitud de autenticación al servidor donde envia su nombre de usuario y el <span style="color:#9FD1F5">Klogin</span> generado a partir de su contraseña. Con este metodo creamos un objeto json con el <span style="color:#9FD1F5">nombre del usuario</span> y con el <span style="color:#9FD1F5">Klogin</span> y se lo mandamos al servidor para que haga las comprobaciones pertinentes.
    ![](/imgs/autenticarUserCliente.PNG)

#### Interfaz de Cifrado/Descifrado
La interfaz que dispondrá el usuario al hacer el login será: 
    ![](/imgs/encDesenc.PNG)
1. Obtención de datos: Una vez el ususario ha sido autenticado, el servido comprime la carpeta perteneciente al usuario que contiene la <span style="color:#9FD1F5">clave pública</span> con la que encriptaremos la <span style="color:#9FD1F5">calve aleatoria</span>,  la <span style="color:#9FD1F5">calve privada </span>que ha sido encriptada en el servidor con el <span style="color:#9FD1F5">Kdatos</span>. Tras esto se muestra por pantalla los archivos que el usuario tiene encriptados y desencriptados.

2. Manejo de datos: A diferencia de la fase 1, ahora el cliente utiliza <span style="color:#9FD1F5">Kdatos</span> para descifrar la <span style="color:#9FD1F5">clave privada</span> que recibe del servidor. 
Utilizamos el siguiente metodo para descifrar la clave privada: 
    ![](/imgs/descPrivada.PNG)

Con esta <span style="color:#9FD1F5">clave privada</span>, el usuario descifra la <span style="color:#9FD1F5">clave aleatoria</span> que se usa para cifrar y descifrar los archivos junto con su <span style="color:#9FD1F5">IV</span>.
    ![](/imgs/procesoEncPrivada.PNG)

3. Cifrado/descifrado de archivos: tras el paso anterior, el usuario puede cifrar o descifrar los archivos utilizando la <span style="color:#9FD1F5">clave aleatoria</span> y el <span style="color:#9FD1F5">IV</span> proporcionados.
4. Cierre de sesión: Una vez el usuario cierra la app, el cliente comprime los archivos encriptados y desencriptados y se los manda al servidor para que los almacene

#### Lado Servidor🖥️
#### GEstión de Usuarios y Claves
1. Atenticación del usuario: el servidor recive una solicitud del login y busca al usuario por su nombre. Una vez que se ha encontrado el usuario, se utiliza el <span style="color:#9FD1F5">Klogin</span> que nos pasa el cliente y lo comporamos con el <span style="color:#9FD1F5">Klogin</span> almacenado y encriptado asociado a ese usuario para validar al usuario.<p>
Con el siguiente metodo<span style="color:#9FD1F5">AuthenticateUser()</span> podemos ver los pasos para autenticar el usuario: 
    ![](/imgs/autentificacionUsuario.png)

2. Envío de Datos al Cliente: una vez el usuario es autenticado, el servidor comprime en una carpeta para mandarsela al cliente: la <span style="color:#9FD1F5">clave publica</span>, la <span style="color:#9FD1F5">clave privada </span>encriptada con <span style="color:#9FD1F5">Kdatos</span> y los archivos encriptados y desencriptados. Con el siguiente metodo, comprimos la carpeta que le vas a mandar al usuario exeptuando el archio <span style="color:#9FD1F5">usuario.json</span> ya que contiente información del usuario y queremos que se mantenga en el servidor.
    ![](/imgs/compresionArchivos.PNG)

3. Recepción y Almacenamiento de Datos: una vez el cliente cierra sesión, el usuario recibe una carpeta con los archivos cifrados y descifrados con los que ha terminado el usuario en su interfaz. Tras recibirlos, el servidor actualiza la carpeta del usuario con los archivos proporcionados. 


## Team 👨🏻‍🤝‍👨🏻
🧑 Yohannes Befikadu - ybbb2@alu.ua.es

🧑 Valentino Quiles - vqmq1@alu.ua.es

🧑 Felix Valois - fvp10@alu.ua.es

🧑 Jose Angel - jasg25@alu.ua.es
