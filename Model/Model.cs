using GraphQL.Types;
using System.Data;
using GraphQL;

namespace VulnerableWebApplication.VLAModel
{
    public class Employee
    {
        /*
        Données des employés de l'entreprise
        */
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }

    public class Creds
    {
        /*
        Login et mots de passe des employés de l'entreprise
        */
        public string User { get; set; }
        public string Passwd { get; set; }
    }

    public class Data
    {
        public static string GetLogPage()
        {
            /*
            Structure des journaux d'événements
            */
            return @"<!doctype html><html lang=""fr""><head><meta charset=""utf-8""><title>Application Logs</title></head><body><h1>Application Logs</h1></body></html>";
        }

        public static DataSet GetDataSet()
        {
            /*
            Contenu de la BDD relationnelle (Utilisateurs)
            */
            DataTable table = new DataTable();
            table.Columns.Add("User", typeof(string));
            table.Columns.Add("Passwd", typeof(string));
            table.Rows.Add("root", "ce5ca673d13b36118d54a7cf13aeb0ca012383bf771e713421b4d1fd841f539a");
            table.Rows.Add("admin", "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918");
            table.Rows.Add("User", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
            table.Rows.Add("Alice", "9b510b4af0d9b121f68d5a3400975047cbf38f963963b4c7510842d9d6310e7f");
            table.Rows.Add("Bob", "aed8f2deab14c36eeaa6d9c5c07ac6b586a74c18015dff9ac1cd0fc320f107b2");
            table.Rows.Add("Charlie", "99cdaf24cef97271760d72f0552ff18bb0c53e47d272cc1b3aa2c8b4e7d71b22");
            table.Rows.Add("Diana", "c27ab3e46131d5e15819aa5c919dca2c7d449a13a1293c9963e1a9d6181b51ac");
            table.Rows.Add("Edward", "3b179a52471e65a043a6c2b2dc1cb703165e2f94a8d4d3818b35eb278f730111");
            table.Rows.Add("Fiona", "31b6273952ff5ef238f5ef544a212eb434813782a279de537bf8c02ccc07fa08");
            table.Rows.Add("George", "27730420c3b86d8eb76e568be4e9279f69d5b00d625c2f0742d260ed9cc2ec26");
            table.Rows.Add("Hannah", "dc8fd3ef67d7031e81a8e2d088aceb430972e4ad03bfccafd063b5729ca0a139");
            table.Rows.Add("Ian", "0964e66cc96ed16adb6364caf1d0c80f80b91c9bf49aed3ffc0e51bca4dc0567");
            table.Rows.Add("Julia", "69ccc763a7a99e5ef616c760e8dcc90a96491cfd15ec84d61fbbf222474a9b3d");
            var DataSet = new DataSet();
            DataSet.Tables.Add(table);
            return DataSet;
        }


        public static List<Employee> GetEmployees()
        {
            /*
            Contenu de la BDD non relationnelle (Employés)
            */
            List<Employee> Employees = new List<Employee>() {
                new Employee() { Id = "1", Name = "John", Age = 16, Address = "4 rue jean moulin"},
                new Employee() { Id = "42", Name = "Steve",  Age = 21, Address = "3 rue Victor Hugo" },
                new Employee() { Id = "1000", Name = "Bill",  Age = 18, Address = "4 place du 18 juin" },
                new Employee() { Id = "1001", Name = "Alice", Age = 25, Address = "123 rue de la Paix" },
                new Employee() { Id = "1002", Name = "Bob", Age = 30, Address = "456 avenue des Champs-Élysées" },
                new Employee() { Id = "1003", Name = "Charlie", Age = 28, Address = "789 boulevard Saint-Germain" },
                new Employee() { Id = "1004", Name = "Diana", Age = 32, Address = "1010 rue du Faubourg Saint-Honoré" },
                new Employee() { Id = "1005", Name = "Edward", Age = 45, Address = "2020 avenue de la République" },
                new Employee() { Id = "1006", Name = "Fiona", Age = 29, Address = "3030 place de la Concorde" },
                new Employee() { Id = "1007", Name = "George", Age = 35, Address = "4040 rue de Rivoli" },
                new Employee() { Id = "1008", Name = "Hannah", Age = 27, Address = "5050 avenue Montaigne" },
                new Employee() { Id = "1009", Name = "Ian", Age = 40, Address = "6060 rue de la Boétie" },
                new Employee() { Id = "1010", Name = "Julia", Age = 22, Address = "7070 rue de Vaugirard" }
            };
            return Employees;
        }
    }


    /* 
     Classes et Query GraphQL (clients)
     */
    public record Client(int Id, string Name, int Country);
    public record Country(int Id, string Name);

    public record Bank(int RIB, string Name);

    public class ClientDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }

        public string Bank { get; set; }


    }

    public class ClientDetailsType : ObjectGraphType<ClientDetails>
    {
        public ClientDetailsType()
        {
            Field(x => x.Id);
            Field(x => x.Name);
            Field(x => x.Country);
            Field(x => x.Bank); 
        }
    }

    public interface IClientService
    {
        public List<ClientDetails> GetClients();
        public List<ClientDetails> GetClient(int empId);
        public List<ClientDetails> GetClientsByCountry(int Country);
    }

    public class ClientService : IClientService
    {
        public ClientService(){}

        private List<Client> Clients = new List<Client>
        {
            new Client(1, "NovaSynergy Solutions", 1),
            new Client(2, "EcoVerde Innovations", 1),
            new Client(3, "AstraTech Dynamics", 2),
            new Client(4, "Luminara Creation", 2),
            new Client(5, "ZenithWave Enterprises", 3),
        };

        private List<Country> Countrys = new List<Country>
        {
            new Country(1, "France"),
            new Country(2, "Taïwan"),
            new Country(3, "China"),
        };

        public List<ClientDetails> GetClients()
        {
            return Clients.Select(emp => new ClientDetails
            {
                Id = emp.Id,
                Name = emp.Name,
                Country = Countrys.First(d => d.Id == emp.Country).Name,
            }).ToList();
        }

        public List<ClientDetails> GetClient(int empId)
        {
            return Clients.Where(emp => emp.Id == empId).Select(emp => new ClientDetails
            {
                Id = emp.Id,
                Name = emp.Name,
                Country = Countrys.First(d => d.Id == emp.Country).Name,
            }).ToList();
        }

        public List<ClientDetails> GetClientsByCountry(int Country)
        {
            return Clients.Where(emp => emp.Country == Country).Select(emp => new ClientDetails
            {
                Id = emp.Id,
                Name = emp.Name,
                Country = Countrys.First(d => d.Id == Country).Name,
            }).ToList();
        }
    }

    public class ClientQuery : ObjectGraphType
    {
        public ClientQuery(IClientService ClientService)
        {
            Field<ListGraphType<ClientDetailsType>>(Name = "Clients", resolve: x => ClientService.GetClients());
            Field<ListGraphType<ClientDetailsType>>(Name = "Client", arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }), resolve: x => ClientService.GetClient(x.GetArgument<int>("id")));
        }
    }

    public class ClientDetailsSchema : Schema
    {
        public ClientDetailsSchema(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Query = serviceProvider.GetRequiredService<ClientQuery>();
        }
    }
}

