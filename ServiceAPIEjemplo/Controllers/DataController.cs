using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ServiceAPIEjemplo.Controllers
{
    public class DataController
    {
        // Identificador único del usuario
        [Key]
        public int Id { get; set; }

        // Datos principales del usuario
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Puesto { get; set; } // Ejemplo: "Gerente de Ventas"

        [Required]
        public string Rol { get; set; } // Ejemplo: "Administrador", "Usuario Estándar"

        [Required]
        // Nombre de usuario único (usado para el login)
        public string Username { get; set; }

        // Campo de seguridad: Aquí se almacena la contraseña hasheada y salada.
        [Required]
        public string HashedPassword { get; set; }
    }

    public class RegisterModel
    {
        // El cliente debe enviar estos datos.
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Puesto { get; set; }

        [Required]
        public string Rol { get; set; }

        [Required]
        public string Username { get; set; }

        // Contraseña en texto plano (solo existe en memoria durante la solicitud)
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public partial class Cat_Roles
    {
        public int RowId { get; set; }
        public string Rol { get; set; }
        public int IdRol { get; set; }

    }


    public partial class InfoEmpleadosos
    {
        public int RowID { get; set; }
        public int NoEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Puesto { get; set; }
        public int Rol { get; set; }
        public string Usuario { get; set; }
        public string pass { get; set; }

        public virtual Cat_Roles Cat_Roles { get; set; }
    }


}