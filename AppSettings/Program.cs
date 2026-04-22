using DbUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.BatchSQLScripts
{
    class Program
    {
        static int Main(string[] args)
        {
			// Bisa di modif untuk keperluan koneksi DB local
            var connectionString =
                args.FirstOrDefault()
                ?? "Data Source=192.168.62.57;Initial Catalog=DFIS;Persist Security Info=True;User ID=sa;Password=v~:4jsrc;MultipleActiveResultSets=True;Application Name=EntityFramework";

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
#if DEBUG
                Console.ReadLine();
#endif
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            Console.ReadLine();
            return 0;
        }
    }
}
