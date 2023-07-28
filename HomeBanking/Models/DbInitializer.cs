using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeBanking.Models
{
    public class DbInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            #region Clients
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client { Email = "prueba@gmail.com", FirstName="Prueba", LastName="Prueba", Password="123456"},
                    new Client { Email = "juanito@gmail.com", FirstName = "Juanito", LastName = "González", Password = "qwerty123" },
                    new Client { Email = "laura@example.com", FirstName = "Laura", LastName = "Martínez", Password = "myp@ssw0rd" },
                    new Client { Email = "carlos23@hotmail.com", FirstName = "Carlos", LastName = "López", Password = "abcde987" },
                    new Client { Email = "maria.smith@yahoo.com", FirstName = "María", LastName = "Smith", Password = "pass1234" },
                    new Client { Email = "jose89@gmail.com", FirstName = "José", LastName = "Ramírez", Password = "hola12345" }
                };

                foreach (Client client in clients)
                {
                    context.Clients.Add(client);
                }
            }
            #endregion

            #region Accounts
            if (!context.Accounts.Any())
            {
                var clients = context.Clients.ToList();
                int cuentaIndex = 1;

                foreach (Client client in clients)
                {
                    int cantidadDeCuentas = GenerarCantidadDeCuentasAleatoria(); // Generar cantidad aleatoria entre 1 y 3

                    for (int i = 0; i < cantidadDeCuentas; i++)
                    {
                        var newAccount = new Account
                        {
                            Client = client,
                            CreationDate = DateTime.Now,
                            Number = "VIN" + cuentaIndex.ToString("D3"), 
                            Balance = GenerarSaldoInicialAleatorio() // Generar un saldo inicial aleatorio
                        };

                        if (client.Accounts == null)
                        {
                            client.Accounts = new List<Account>();
                        }

                        client.Accounts.Add(newAccount);
                        context.Accounts.Add(newAccount);
                        cuentaIndex++;
                    }
                }
            }
            #endregion

            context.SaveChanges();
        }

   
        private static int GenerarCantidadDeCuentasAleatoria()
        {
            Random random = new Random();
            return random.Next(1, 4); 
        }


        private static double GenerarSaldoInicialAleatorio()
        {
            Random random = new Random();
            return random.Next(1000, 50001);
        }
    }
}

