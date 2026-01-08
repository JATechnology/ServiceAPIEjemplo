using ServiceAPIEjemplo.Classes;
using ServiceAPIEjemplo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServiceAPIEjemplo.Controllers
{
  
    public class ValuesController : ApiController
    {
        readonly EmpleadoEntities ES = new EmpleadoEntities();


        /*-------------------------------------------------Verificacion de usuario y contraseña----------------------------*/
        /// <summary>
        /// Logueo de usuario y contraseña y validación en caso de que el usuario no este registrado
        /// </summary>
        /// <param name="user">Nombre de usuario</param>
        /// <param name="pass">Contraseña</param>
        /// <returns></returns>
        [HttpGet]
        public string Login(string user, string pass)
        {
            //Mensaje base inicial 
            string msg = "Contraseña incorrecta.";

            //Manejador de errores , reponsable que la aplicación no falle o sea no funcional
            try
            {
                //Mensaje de validación inicial 

                //Validación inicial de usuario en base de datos con solo usuario 
                var usuario = ES.InfoEmpleadoes.Where(X => X.Usuario == user.ToUpper()).ToList();

                //condicional en caso de que no exista el usuario que intenta loguearse 
                if (usuario.Count==0)
                {
                    //Mensaje que recibira el usuario 
                    msg = "Usuario no registrado";
                    return msg;
                }

            //Eliminación de espacios en blanco 
            string passBD = usuario[0].pass.Trim();
           
            // Se usa contraseña capturada por el usuario y la cadena almacenada en base de datos.
            bool passwordIsValid = Classes.Userhash.VerifyPassword(pass.Trim(), passBD);

            //Si el password es valido enviara el mensaje de bienvenido al usuario 
            if (passwordIsValid==true)
            {
                msg = "Bienvenido";
               
            }
            }catch(Exception e)
            {
                //En caso de que haya alguna eventualidad se deja la excepción para analizar errores y asi mismo envía un mensaje base de error
                msg = "Error de conexón";
            }

            //Regresa la variable msn-mensaje con el texto obtenido anteriormente 
            return msg;
        }

        /*-------------------------------------------------Verificacion de usuario y contraseña----------------------------*/
        /// <summary>
        /// Clase encargada del registro de usuario con validación de repeticion de usuarios
        /// </summary>
        /// <param name="noempleado">Num de Nomina</param>
        /// <param name="nombre">Num de Nomina</param>
        /// <param name="puesto">Num de Nomina</param>
        /// <param name="rol">Num de Nomina</param>
        /// <param name="usuario">Num de Nomina</param>
        /// <param name="contraseña">Num de Nomina</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Values/RegistroUsuario")]
        public string RegistroUsuario(int? noempleado,string nombre,string puesto, int? rol,string usuario,string contraseña)
        {
            try {
                //Mensaje base inicial 
                string msg = "Registro exitoso";

                //Validación en base de datos con el usuario registrado y el ingresado por el usuario
            int existe = ES.InfoEmpleadoes.Where(s => s.Usuario == usuario).ToList().Count();

                //Conficional encargado de evitar registros repetidos 
            if (existe>=1)
            {
                    msg = "Registro ya existente";
                    return msg;
            }

            // HASHING (Seguridad)
            // Llama a la clase estática PasswordHasher para realizar el hash(Encriptado) del password 
            string hashedPassword = ServiceAPIEjemplo.Classes.Userhash.HashPassword(contraseña.Trim());

            //Llama al procedimiento almacenado que se encarga de registrar a los nuevos usuario 
            ES.CreateUsuario(noempleado,nombre,puesto,rol,usuario, hashedPassword);

            return msg;
            }catch(Exception e)
            {
                string msg = "Error de conexión";
                return msg;
            }
        }



        /*-------------------------------------------------Eliminación  de usuario y contraseña----------------------------*/
        /// <summary>
        /// Clase encargada del registro de usuario con validación de repeticion de usuarios
        /// </summary>
        /// <param name="noempleado">Num de Nomina</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Values/DeleteUsuario")]
        public string DeleteUsuario(int id)
        {
            try
            {
                //Mensaje base inicial 
                string msg = "Eliminación exitosa";



                InfoEmpleado city = ES.InfoEmpleadoes.Find(id);
                ES.InfoEmpleadoes.Remove(city);
                ES.SaveChanges();

                return msg;
            }
            catch (Exception e)
            {
                string msg = "Error de conexión";
                return msg;
            }
        }


        /*-------------------------------------------------Listado de usuario y info----------------------------*/
        /// <summary>
        /// Clase de listar usuarios
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Values/ListarUsuarios")]
        public List<InfoEmpleadosos> ListarUsuarios()
        {
            try
            {
              //Lista donde se contendran los usuarios
                List<InfoEmpleadosos> existe = new List<InfoEmpleadosos>();

                //cilco encargado de asignado lo obtenido en BD a una lista 
                foreach(var tui in ES.InfoEmpleadoes.ToList())
                {
                    existe.Add(new InfoEmpleadosos {RowID=tui.RowID, NoEmpleado = tui.NoEmpleado, Nombre = tui.Nombre, Puesto = tui.Puesto, Rol = tui.Rol, Usuario = tui.Usuario });
                }

                //Condicional de exitencia 
                if (existe.Count >= 1)
                {
                    
                    return existe;
                }
                
                return null;
            }
            //Captura de excepciones 
            catch (Exception e)
            {
                
                return null;
            }
        }



     

        /*-------------------------------------------------Listado de usuario y info----------------------------*/
        /// <summary>
        /// Clase de listar Roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Values/ListarRoles")]
        public List<Cat_Roles> ListarRoles()
        {
            try
            {
                //Lista donde se contendran los usuarios
                List<Cat_Roles> existe = new List<Cat_Roles>();

                //cilco encargado de asignado lo obtenido en BD a una lista 
                foreach (var tui in ES.Cat_Roles.ToList())
                {
                    existe.Add(new Cat_Roles {RowId=tui.RowId, IdRol = tui.IdRol, Rol = tui.Rol});
                }

                //Condicional de exitencia 
                if (existe.Count >= 1)
                {

                    return existe;
                }

                return null;
            }
            //Captura de excepciones 
            catch (Exception e)
            {

                return null;
            }
        }




    }
}

