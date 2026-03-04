using System;
using System.Collections.Concurrent;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp1
{
    class Usuario
    {
        public int numeroCuenta {get; set;}
        public string contraseña {get; set;}
        public string nombre {get; set;}
        public decimal saldo {get; set;}
        public List<string> historial {get; set;}
        public Usuario(int numeroCuenta, string nombre, decimal saldo, string contraseña)
        {
            this.numeroCuenta = numeroCuenta;
            this.contraseña = contraseña;
            this.nombre = nombre;
            this.saldo = saldo;
            this.historial = new List<string>();
        }
    }

    class Program
    {
        static void GuardarDatos(List<Usuario> usuarios)
            {
                // Opciones de formato
                var opciones = new JsonSerializerOptions { WriteIndented = true };

                // Serialización (Convertir objeto a texto)
                string jsonString = JsonSerializer.Serialize(usuarios, opciones);

                // Escritura (Guardar en el disco duro)
                File.WriteAllText("usuarios.json", jsonString);
            }

        static List<Usuario> CargarDatos()
        {
            string ruta = "usuarios.json";

            if (File.Exists(ruta))
            {
                string jsonString = File.ReadAllText(ruta);
                return JsonSerializer.Deserialize<List<Usuario>>(jsonString);
            }
            
            // IMPORTANTE: Aplicar CalcularHash aquí también
            return new List<Usuario>
            {
                new Usuario(123, "Juan", 1000, CalcularHash("1234")), 
                new Usuario(1233, "Maria", 2000, CalcularHash("1234"))
            };
        }

        static int LeerEntero(string mensaje)
            {
                int resultado;
                while (true)
                {
                    Console.WriteLine(mensaje);
                    string entrada = Console.ReadLine();

                    if (int.TryParse(entrada, out resultado))
                    {
                        return resultado; // Si es un número válido, lo devuelve y sale del bucle
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error: Debe ingresar un número entero válido.");
                        Console.ResetColor();
                    }
                }
            }

        static decimal LeerDecimal(string mensaje)
            {
                decimal resultado;
                while (true)
                {
                    Console.WriteLine(mensaje);
                    if (decimal.TryParse(Console.ReadLine(), out resultado) && resultado >= 0)
                    {
                        return resultado;
                    }
                    Console.WriteLine("Error: Ingrese un monto válido (ej: 1500.50)");
                }
            }
        static string CalcularHash(string entrada)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convertimos la contraseña en un arreglo de bytes
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(entrada));

                // Convertimos los bytes en una cadena hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        static void Main(string[] args)
        {
            List<Usuario> Listausuarios = CargarDatos();

            Console.WriteLine("Bienvenido a su ATM");
            int numeroCuenta = LeerEntero("Ingrese su numero de cuenta");

            Usuario usuarioLogueado = Listausuarios.Find(u => u.numeroCuenta == numeroCuenta);

            if (usuarioLogueado != null)
            {
                bool loginExitoso = false;

                for (int i = 3; i > 0; i--)
                {
                    Console.WriteLine("Ingrese su contraseña:");
                    string passwordIngresada = Console.ReadLine();
                    string hashIngresado = CalcularHash(passwordIngresada);

                    if (usuarioLogueado.contraseña == hashIngresado)
                    {
                        Console.WriteLine("Acceso concedido");
                        loginExitoso = true;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Contraseña incorrecta");
                        Console.WriteLine("Le quedan " + (i -1) + " intentos restantes"); 
                    }
                }

                if (loginExitoso == true)
                {
                    mostrarMenu(usuarioLogueado, Listausuarios);
                }
                else
                {
                    Console.WriteLine("Se ha bloqueado el acceso");
                }
            }
        }      

        static void mostrarMenu(Usuario usuarioLogueado , List<Usuario> todasLasCuentas)
            {
                while(true)
                {
                    Console.Clear();
                    Console.WriteLine("Bienvenido " + usuarioLogueado.nombre);
                    Console.WriteLine("Que operacion desea realizar?");
                    Console.WriteLine("1. Ingresar dinero");
                    Console.WriteLine("2. Retirar dinero");
                    Console.WriteLine("3. Consultar saldo");
                    Console.WriteLine("4. Transferencia a otra cuenta");
                    Console.WriteLine("5. Consultar historial de transacciones");
                    Console.WriteLine("6. Salir");
                    int opcion = LeerEntero("Ingrese una opcion: ");

                    switch(opcion)
                    {
                        case 1:
                        Console.Clear();
                        decimal cantidadIngresada = LeerDecimal("Ingrese la cantidad de dinero que desea ingresar");
                        
                        if(cantidadIngresada > 0)
                        {
                            usuarioLogueado.saldo += cantidadIngresada;
                            Console.WriteLine("Su saldo actual es de: " + usuarioLogueado.saldo);
                            Console.WriteLine("Presione cualquier tecla para continuar");
                            usuarioLogueado.historial.Add($"Deposito: +{cantidadIngresada:C} (Saldo: {usuarioLogueado.saldo:C})");    
                            GuardarDatos(todasLasCuentas);                        
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("La cantidad ingresada no es valida");
                            Console.WriteLine("Presione cualquier tecla para continuar");
                            Console.ReadKey();

                        }
                        break;

                        case 2:
                        Console.Clear();
                        decimal cantidadRetirada = LeerDecimal("Ingrese la cantidad a retirar");

                        if(cantidadRetirada > 0 && usuarioLogueado.saldo >= cantidadRetirada)
                        {
                            usuarioLogueado.saldo -= cantidadRetirada;
                            Console.WriteLine("Su saldo actual es de: " + usuarioLogueado.saldo);
                            Console.WriteLine("Presione cualquier tecla para continuar");
                            usuarioLogueado.historial.Add($"Retiro: -{cantidadRetirada:C} (Saldo: {usuarioLogueado.saldo:C})");  
                            GuardarDatos(todasLasCuentas);                      
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("La cantidad ingresada no es valida");
                            Console.WriteLine("Presione cualquier tecla para continuar al menu");
                            Console.ReadKey();
                        }
                        break;

                        case 3:
                            Console.Clear();
                            Console.WriteLine($"\nTitular: {usuarioLogueado.nombre}\nSaldo actual: {usuarioLogueado.saldo:C}");                            
                            Console.WriteLine("Presione cualquier tecla para continuar");
                            Console.ReadKey();
                        break;

                        case 4:
                            Console.Clear();
                            Console.WriteLine("Transferir a otra cuenta");
                            int cuentaDestino = LeerEntero("Ingrese el numero de cuenta a la cual desea transferir");

                            Usuario destinatario = todasLasCuentas.Find(x => x.numeroCuenta == cuentaDestino);

                            if (destinatario != null && destinatario != usuarioLogueado)
                            {
                                Console.WriteLine("Transeferir a la cuenta " + destinatario.numeroCuenta);
                                Console.WriteLine("Titular de la cuenta " + destinatario.nombre);
                                Console.WriteLine("Su saldo es de " + usuarioLogueado.saldo);
                                decimal cantidad = LeerDecimal("Ingrese la cantidad a transferir");

                                if (cantidad <= usuarioLogueado.saldo && cantidad > 0)
                                {
                                    usuarioLogueado.saldo -= cantidad;
                                    destinatario.saldo += cantidad;
                                    Console.WriteLine("Transferencia exitosa por la cantidad de " + cantidad + " pesos");
                                    Console.WriteLine("Su nuevo saldo es de: " + usuarioLogueado.saldo);
                                    Console.WriteLine("Presione cualquier tecla para continuar a menu");
                                    usuarioLogueado.historial.Add($"Transferencia enviada a {destinatario.nombre}: -{cantidad:C}");
                                    destinatario.historial.Add($"Transferencia recibida de {usuarioLogueado.nombre}: +{cantidad:C}");   
                                    GuardarDatos(todasLasCuentas);                                 
                                    Console.ReadKey();
                                }
                                else if (cantidad <= 0 )
                                {
                                    Console.WriteLine("No se pudo realizar la transferencia, cantidad no admitida");
                                    Console.WriteLine("Presione cualquier tecla para continuar a menu");
                                    Console.ReadKey();
                                }
                                else if (cantidad > usuarioLogueado.saldo)
                                {
                                    Console.WriteLine("No se pudo realizar la transferencia, saldo insuficiente");
                                    Console.WriteLine("Presione cualquier tecla para continuar a menu");
                                    Console.ReadKey();
                                }
                            }
                            else
                            {
                                Console.WriteLine("No se encontro el usuario");
                                Console.WriteLine("Presione cualquier tecla para continuar a menu");
                                Console.ReadKey();
                            }
                        break;

                        case 5:
                            Console.Clear();
                            Console.WriteLine("Historial de movimientos");
                            Console.WriteLine("-------------------------------------");
                            if (usuarioLogueado.historial.Count == 0) Console.WriteLine("No hay movimientos.");
                            else usuarioLogueado.historial.ForEach(m => Console.WriteLine($"> {m}"));
                            Console.WriteLine("-------------------------------------");
                            Console.WriteLine("Presione cualquier tecla para continuar a menu");
                            Console.ReadKey();
                           
                        break;

                        case 6:
                            Console.WriteLine("Gracias por usar el sistema");
                            GuardarDatos(todasLasCuentas);
                           return;

                        default:
                            Console.Clear();
                            Console.WriteLine("Opcion no valida");
                        break;
                    }
                } 
            }
    }
}



