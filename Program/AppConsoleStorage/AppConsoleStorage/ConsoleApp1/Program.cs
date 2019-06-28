using ConsoleApp1.AccesoDatos;
using ConsoleApp1.Negocio;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Program
    {
        private const string EndpointUrl = "https://empleadodb.documents.azure.com:443/";
        private const string PrimaryKey = "FQvtyg9VXqDvrnQsDNKQHvHQySU8qIwLni30GElsgg8BH2b93QMiD93xaDeEiR2vFFZGu6A697JYHF9X6O3XSg==";
        private DocumentClient client;

        static void Main(string[] args)
        {
            string storageConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConString"];
            try
            {
                #region storageTable
                CloudTable cloudTable;
                CloudTableClient cloudTableClient;

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                Console.WriteLine("Se ha conectado correctamente al Storage Acount");
                cloudTableClient = storageAccount.CreateCloudTableClient();
                cloudTable = cloudTableClient.GetTableReference("CLIENTE");
                cloudTable.CreateIfNotExists();

                //1. Adicionar empleado
                AdicionarEmpleado(cloudTable);
                //2. Listar empleado

                listarEmpleados(cloudTable);

                cloudTableClient = storageAccount.CreateCloudTableClient();
                cloudTable = cloudTableClient.GetTableReference("BOOK");
                cloudTable.CreateIfNotExists();
                #endregion

                #region CosmoDB
                try
                {
                    Program p = new Program();
                    p.GetStartedDemo().Wait();
                }
                catch (DocumentClientException de)
                {
                    Exception baseException = de.GetBaseException();
                    Console.WriteLine($"{de.StatusCode} error occurred: {de.Message}, Message: {baseException.Message}");
                }
                catch (Exception e)
                {
                    Exception baseException = e.GetBaseException();
                    Console.WriteLine($"Error: {e.Message}, Message: {baseException.Message}");
                }
                finally
                {
                    Console.WriteLine("End of demo, press any key to exit.");
                    Console.ReadKey();
                }
                #endregion
                //AddBoook();

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public static void AddBoook()
        {
            string storageConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConString"];


            var context = new BookContext("Server=tcp:testcasc.database.windows.net,1433;Initial Catalog=db_books;Persist Security Info=False;User ID={your_username};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");


            Book nuevo = new Book();
            nuevo.BookId = 1;
            nuevo.Url = "http://developingmymind.wordpress.com";
            nuevo.Description = "BLOG DE DESARROLLO";

            context.Books.Add(nuevo);
            context.SaveChanges();
            /*
            List<Book> allBooks = context.Books.ToList();
            IEnumerable<Book> someBooks = context.Books.Where(b => b.Url.Contains("algo"));
            Book specificBook = context.Books.Single(b => b.BookId == 1);
            */

        }

        public static void AdicionarEmpleado(CloudTable cloudTable)
        {
            //1. Se almacena la imagen en el storage y a continuacion se guarda en el table storage
            string url = getImage("foto.png", "imagenes", "blob");

            //1. Se crea la entidad a agregar
            Empleado empleado = new Empleado("CUSTOMER")
            {
                Nombre = "CARLOS",
                Apellido = "PEREZ",
                Email = "CARLOS.SIABATO@OUTLOOK.COM",
                NumeroCelular = "101325684",
                UrlFoto = url

            };
            //2. Se agregar a la entidad
            TableOperation tableOperation = TableOperation.InsertOrReplace(empleado);
            //Cliente insertado
            Console.WriteLine("Cliente Insertado");
            cloudTable.Execute(tableOperation);
        }

        public static void listarEmpleados(CloudTable cloudTable)
        {
            TableQuery<Empleado> tQuery = new TableQuery<Empleado>();
            foreach (var item in cloudTable.ExecuteQuery(tQuery))
            {
                Console.WriteLine("Nombre: {0} \nApellido: {1} \nEmail: {2} \nNumero Celular:{3} \nUrlFoto: {4}", item.Nombre, item.Apellido, item.Email, item.NumeroCelular, item.UrlFoto);
                Console.WriteLine("******************************************************************************");
            }
        }

        public static string getImage(string blobName, string containerName, string contentType)
        {
            string urlImg = "";
            try
            {
                string storageConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConString"];
                string pathImg = @"C:\Users\carlos.siabato\Pictures\Img.png";
                if (File.Exists(pathImg))
                {
                    byte[] contentBlob = File.ReadAllBytes(pathImg);

                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer containerRef = blobClient.GetContainerReference(containerName.ToLower());
                    containerRef.CreateIfNotExists();
                    containerRef.SetPermissions(new BlobContainerPermissions()
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
                    CloudBlockBlob blob = containerRef.GetBlockBlobReference(blobName);
                    blob.DeleteIfExists();
                    blob.Properties.ContentType = contentType;
                    using (MemoryStream stream = new MemoryStream(contentBlob))
                    {
                        blob.UploadFromStream(stream);
                    }
                    urlImg = blob.Uri.ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return urlImg;
        }

        private async Task GetStartedDemo()
        {
            client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "clientedb" });
            await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("clientedb"), new DocumentCollection { Id = "clientecollection" });

            Empleado andersenFamily = new Empleado
            {
                Id = "AndersenFamily",
                LastName = "Andersen",
                Parents = new Parent[]
     {
         new Parent { FirstName = "Thomas" },
         new Parent { FirstName = "Mary Kay" }
     },
                Children = new Child[]
     {
         new Child
         {
             FirstName = "Henriette Thaulow",
             Gender = "female",
             Grade = 5,
             Pets = new Pet[]
             {
                 new Pet { GivenName = "Fluffy" }
             }
         }
     },
                Address = new Address { State = "WA", County = "King", City = "Seattle" },
                IsRegistered = true
            };

            await CreateFamilyDocumentIfNotExists("FamilyDB", "FamilyCollection", andersenFamily);

            Family wakefieldFamily = new Family
            {
                Id = "WakefieldFamily",
                LastName = "Wakefield",
                Parents = new Parent[]
                {
         new Parent { FamilyName = "Wakefield", FirstName = "Robin" },
         new Parent { FamilyName = "Miller", FirstName = "Ben" }
                },
                Children = new Child[]
                {
         new Child
         {
             FamilyName = "Merriam",
             FirstName = "Jesse",
             Gender = "female",
             Grade = 8,
             Pets = new Pet[]
             {
                 new Pet { GivenName = "Goofy" },
                 new Pet { GivenName = "Shadow" }
             }
         },
         new Child
         {
             FamilyName = "Miller",
             FirstName = "Lisa",
             Gender = "female",
             Grade = 1
         }
                },
                Address = new Address { State = "NY", County = "Manhattan", City = "NY" },
                IsRegistered = false
            };

            await CreateFamilyDocumentIfNotExists("FamilyDB", "FamilyCollection", wakefieldFamily);
        }

        private void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private async Task CreateEmployDocumentIfNotExists(string databaseName, string collectionName, Empleado empleado)
        {
            try
            {
                await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, empleado.NoDocumento));
                WriteToConsoleAndPromptToContinue($"Found {empleado.NoDocumento}");
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), empleado);
                    WriteToConsoleAndPromptToContinue($"Created Family {empleado.NoDocumento}");
                }
                else
                {
                    throw;
                }
            }
        }

    }
}
