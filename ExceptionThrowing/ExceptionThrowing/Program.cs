using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionThrowing
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Exception");

            try
            {
                try
                {
                    var number = int.Parse("Number123");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Number parsing error", e);
                    //throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"ToString: {ex.ToString()}");
            }

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }
    }
}
