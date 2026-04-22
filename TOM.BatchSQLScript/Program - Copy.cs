using System;
using System.Linq;
using System.Reflection;
using DbUp;

namespace TOM.BatchSQLScript
{
    class Program
    {
        static int Main(string[] args)
        {
            var connectionString =
                args.FirstOrDefault()
            ??
            "Data Source=DESKTOP-O8LRH4J;Initial Catalog=TOM;Persist Security Info=True;User ID=sa;password=p455w0rd.;MultipleActiveResultSets=True;Application Name=EntityFramework";
            //"Data Source=192.168.62.57;Initial Catalog=TOM;Persist Security Info=True;User ID=sa;password=v~:4jsrc;MultipleActiveResultSets=True;Application Name=EntityFramework";
            
            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                Console.ReadKey();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success!");
                Console.ResetColor();
                Console.ReadKey();
            }
            return 0;
        }
    }
}
