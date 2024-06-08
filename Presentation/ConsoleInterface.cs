using TCPA.Models;

namespace TCPA.Presentation
{
    internal class ConsoleInterface
    {
        public void Update(Application.Application application)
        {

        }

        private static void PrintFlags(IControlUnit controlUnit)
        {
            Console.Write("Flags: ");
            Console.ForegroundColor = controlUnit.N ? 
                ConsoleColor.Green : 
                ConsoleColor.Red;
            Console.Write("N");
            Console.ForegroundColor = controlUnit.V ? 
                ConsoleColor.Green : 
                ConsoleColor.Red;
            Console.Write("V");
            Console.ForegroundColor = controlUnit.C ? 
                ConsoleColor.Green : 
                ConsoleColor.Red;
            Console.Write("C");
            Console.ForegroundColor = controlUnit.Z ? 
                ConsoleColor.Green : 
                ConsoleColor.Red;
            Console.Write("Z");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
        }

        private void PrintMemory(IMemory memory, IControlUnit controlUnit)
        {
            var data = memory.GetData();

            for (int i = 0; i < data.Length; i++)
            {

                Console.BackgroundColor = i == controlUnit.CC ? ConsoleColor.Red : ConsoleColor.Black;
                Console.ForegroundColor = i == memory.LastChanged ? ConsoleColor.Yellow : ConsoleColor.Gray;
                Console.Write($"{data[i]:X2}");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(" ");

                if (i % 16 == 15)
                {
                    Console.WriteLine();
                }
            }
        }

        private void PrintRegisters(IRegisterBlock registers)
        {
            var data = registers.GetData();

            for (int i = 0; i < data.Length; i++)
            {
                Console.ForegroundColor = i == registers.LastChanged ? ConsoleColor.Yellow : ConsoleColor.Gray;
                Console.Write($"{data[i]:X2} ");
            }
            Console.WriteLine();
        }
    }
}
