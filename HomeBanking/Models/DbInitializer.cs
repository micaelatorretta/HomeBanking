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
                context.SaveChanges();
            }
            #endregion

            #region Accounts
            if (!context.Accounts.Any())
            {
                var clients = context.Clients.ToList();
                int cuentaIndex = 1;

                foreach (Client client in clients)
                {
                    int cantidadDeCuentas = GenerarCantidadAleatoria(); // Generar cantidad aleatoria entre 1 y 3

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
                context.SaveChanges();
            }
            #endregion

            #region Transaction
            if (!context.Transactions.Any())
            {
                var accounts = context.Accounts.ToList();
                var transactions = new List<Transaction>();

                foreach (Account account in accounts)
                {
                    int cantidadDeCuentas = GenerarCantidadAleatoria(); // Generar cantidad aleatoria entre 1 y 3

                    for (int i = 0; i < cantidadDeCuentas; i++) 
                    {
                        var tn = new Transaction
                        {
                            AccountId = account.Id,
                            Account = account,
                            Amount = random.Next(-1000, 5000),
                            Date = DateTime.Now.AddHours(-random.Next(1, 24)),
                            Description = GetRandomDescription(),
                            Type = (random.Next(2) == 0) ? TransactionType.CREDIT.ToString() : TransactionType.DEBIT.ToString()
                        };
                        context.Transactions.Add(tn);
                    }
                }

                context.Transactions.AddRange(transactions);
                context.SaveChanges();
            }
            #endregion
            #region Loans
            if (!context.Loans.Any())
            {
                // Crear préstamos utilizando el enum LoanType
                var loans = new Loan[]
                {
                new Loan { Name = LoanType.HIPOTECARIO.ToString(), MaxAmount = 500000, Payments = "12,24,36,48,60" },
                new Loan { Name = LoanType.PERSONAL.ToString(), MaxAmount = 100000, Payments = "6,12,24" },
                new Loan { Name = LoanType.AUTOMOTRIZ.ToString(), MaxAmount = 300000, Payments = "6,12,24,36" },
                };

                // Agregar los préstamos a la base de datos
                context.Loans.AddRange(loans);
                context.SaveChanges();
            }

            if(!context.ClientLoans.Any()) { 
                // Obtener los préstamos creados
                var hipotecarioLoan = context.Loans.FirstOrDefault(l => l.Name == LoanType.HIPOTECARIO.ToString());
                var personalLoan = context.Loans.FirstOrDefault(l => l.Name == LoanType.PERSONAL.ToString());
                var automotrizLoan = context.Loans.FirstOrDefault(l => l.Name == LoanType.AUTOMOTRIZ.ToString());

                // Buscar todos los clientes (o filtrar según tus necesidades)
                var clients = context.Clients.ToList();

                var random = new Random();

                foreach (var client in clients)
                {
                    // Seleccionar un préstamo aleatorio para el cliente actual
                    var randomLoan = random.Next(3); // Genera un número aleatorio entre 0 y 2

                    ClientLoan clientLoan = null;
                    switch (randomLoan)
                    {
                        case 0:
                            clientLoan = new ClientLoan
                            {
                                Amount = random.Next(50000, 100001), // Monto aleatorio entre 50000 y 100000
                                ClientId = client.Id,
                                LoanId = hipotecarioLoan.Id,
                                Payments = "60"
                            };
                            break;
                        case 1:
                            clientLoan = new ClientLoan
                            {
                                Amount = random.Next(10000, 50001), // Monto aleatorio entre 10000 y 50000
                                ClientId = client.Id,
                                LoanId = personalLoan.Id,
                                Payments = "12"
                            };
                            break;
                        case 2:
                            clientLoan = new ClientLoan
                            {
                                Amount = random.Next(20000, 80001), // Monto aleatorio entre 20000 y 80000
                                ClientId = client.Id,
                                LoanId = automotrizLoan.Id,
                                Payments = "24"
                            };
                            break;
                    }

                    // Agregar el préstamo aleatorio al cliente
                    context.ClientLoans.Add(clientLoan);
                    context.SaveChanges();
                }
                
            }
                #endregion
                
        }

   
        private static int GenerarCantidadAleatoria()
        {
            Random random = new Random();
            return random.Next(1, 4); 
        }


        private static double GenerarSaldoInicialAleatorio()
        {
            Random random = new Random();
            return random.Next(1000, 50001);
        }

        private static readonly Random random = new Random();

        private static string[] descriptions = {
        "Compra en tienda A",
        "Pago de servicios",
        "Transferencia recibida",
        "Retiro de efectivo",
        "Compra en línea",
        "Gasto en restaurant",
        "Depósito de cheque",
        "Gasto en gasolina",
        "Pago de factura",
        "Transferencia enviada"
        };
        private static string GetRandomDescription()
        {
            int index = random.Next(descriptions.Length);
            return descriptions[index];
        }
    }
}

