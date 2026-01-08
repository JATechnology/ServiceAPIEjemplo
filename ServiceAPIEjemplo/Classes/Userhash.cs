using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Web.UI.WebControls;


namespace ServiceAPIEjemplo.Classes
{

    public static class Userhash
    {
        // --- CONSTANTES CRÍTICAS encargadas del armado del encripado---
        //Tamaño definido para el token de una logitud definida
        private const int SaltSize = 16;
        //Tamaño del encriptado para su armado 
        private const int HashSize = 32;
        //Definición de iteraciones para la generación del la enciptación, esto con el fin de que el encriptado sea mas robusto 
        private const int Iterations = 100000;

        // =========================================================================
        // 1. GENERACIÓN DE HASH INICIAL para las clases de Registro
        // =========================================================================
        public static string HashPassword(string password)
        {

            //Eliminación de espacios en blanco
            password = password.Trim();

            //Definición de variable para token
            byte[] salt;
            //Generación deñ token en base a numeraciones generadas de manera ramdom 
            using (var rng = RandomNumberGenerator.Create())
            {
                //Asignación de los bytes con los valores random 
                rng.GetBytes(salt = new byte[SaltSize]);
            }

            //Proceso de encriptación en base a los parametros definidos anteriormente 
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                //Definicion del tamaño del encriptado
                byte[] hash = pbkdf2.GetBytes(HashSize);

                // CONCATENACIÓN: Salt primero, luego Hash
                byte[] hashBytes = new byte[SaltSize + HashSize];
                //Armado de la encriptación en base a los parametros definidos anteriormente 
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

                //Generación de la encriptacion con una base typo 64 bytes  para su robustes 
                string finalHash = Convert.ToBase64String(hashBytes);
                // Formato de almacenamiento: Un solo bloque Base64
                
                // --PRUEBAS---
                //Console.WriteLine($"[REGISTRO] HASH FINAL GUARDADO: {finalHash}"); 

                //Se regresa el valor con la encriptación 
                return Convert.ToBase64String(hashBytes);
            }
        }

        // =========================================================================
        // 2. VERIFICACIÓN DE HASH (Login)
        // =========================================================================
        public static bool VerifyPassword(string enteredPassword, string storedHashWithSalt)
        {
            //Eliminación de espacios en los parametros ingresados
            enteredPassword = enteredPassword.Trim();
            storedHashWithSalt = storedHashWithSalt.Trim();

            //----- PRUEBAS---------
            //byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(enteredPassword.Trim());

            try
            {
                // Se decodifica el bloque Base64 completo
                byte[] hashBytes = Convert.FromBase64String(storedHashWithSalt);

                // Se Valida la longitud
                if (hashBytes.Length != SaltSize + HashSize)
                {
                    return false;
                }

                // Separar: Leer el Salt(Token) primero, luego el Hash 
                byte[] salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                //SE EXTRAE EL HASH ALMACENADO(Siguientes 32 bytes)
                byte[] storedHash = new byte[HashSize];
                Array.Copy(hashBytes, SaltSize, storedHash, 0, HashSize);

                // Re-hashear (Proceso IDÉNTICO al registro)
                using (var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, Iterations, HashAlgorithmName.SHA256))
                {
                    //Re encriptación de pass inicial para su comparación 
                    byte[] enteredPasswordHash = pbkdf2.GetBytes(HashSize);

                    // Comparación byte a byte en tiempo constante
                    var result = ConstantTimeEquals(storedHash, enteredPasswordHash);

                    //Validación de resultado para la vista de usuario y mensaje definido 
                    if (result == true)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception)
            {
                string msg = "Error de conexión";
                return false;
            }
        }

        // =========================================================================
        // 3. COMPARACIÓN DE TIEMPO CONSTANTE (Auxiliar)
        // =========================================================================

        // Este método implementa la comparación de tiempo constante
        // (Constant-Time Comparison) manualmente.
        private static bool ConstantTimeEquals(byte[] a, byte[] b)
        {
            //Verificar primero si las longitudes son diferentes.
        // Si son diferentes, no son iguales y se sale.
            if (a.Length != b.Length)
            {
                return false;
            }

            int diff = 0;
          //  Iterar sobre todos los bytes, incluso si se encuentra una diferencia.
        // Esto asegura que la función SIEMPRE tome el mismo tiempo para ejecutarse, 
        // independientemente de dónde ocurra la primera diferencia.
            for (int i = 0; i < a.Length; i++)
            {

                // El operador XOR (^) compara los bits de a[i] y b[i].
                // Si son iguales, el resultado es 0. Si son diferentes, es != 0.
                // Acumulamos el resultado de las diferencias en 'diff'.
                diff |= a[i] ^ b[i];
            }
            // Si todos los bytes fueron iguales, 'diff' será 0.
            // Si diff es 0, retorna true; de lo contrario, retorna false.
            return diff == 0;
        }
    }
}