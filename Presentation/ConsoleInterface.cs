using TCPA.Models;

namespace TCPA.Presentation
{
    internal class ConsoleInterface
    {
        private int _tact = 0;
        private readonly int _initialCursorTop;
        private readonly Application.Application _application;

        public ConsoleInterface(Application.Application application)
        {
            _initialCursorTop = Console.CursorTop;
            _application = application;

            Console.CursorVisible = false;

            byte[] program = [
                0b_0000_0001, // ORG
                0b_0000_1000, // 08h
                0b_0000_0100, // 04h
                0b_0000_1111, // 15h
                0b_0000_0011, // 03h
                0b_0000_0110, // 06h
                0b_0000_0000, // 00h
                0b_1110_1011, // EBh (-21)
                0b_0001_1010, // MOV
                0b_0010_0101, // R5
                0b_0100_0010, // [2]
                0b_0001_1010, // MOV
                0b_0010_0100, // R4
                0b_0000_1111, // #0F
                0b_0000_1011, // SETZ
                0b_0000_1100, // SETN
                0b_0000_1101, // SETV
                0b_0000_1110, // SETC
                0b_0000_1111, // CLRZ
                0b_0001_0000, // CLRN
                0b_0001_0001, // CLRV
                0b_0001_0010, // CLRC
                0b_0001_1001, // NOT
                0b_1000_0011, // 03h
                0b_0001_0011, // SUM
                0b_1000_0110, // 06h
                0b_0100_0010, // [2]
                0b_0000_1110, // SETC
                0b_0000_0110, // JC
                0b_1000_0111, // 07h
            ];

            _application.LoadData("p.dat");
            _application.AddUpdateListener(Update);
        }

        private void Update()
        {
            Console.SetCursorPosition(0, _initialCursorTop);
            Console.WriteLine($"Tact: {_tact++:0000}");
            Console.WriteLine($"CU State: {_application.ControlUnit.CurrentState,-32}");
            Console.WriteLine($"CC reg:  {_application.ControlUnit.CC,2:X2}");
            Console.WriteLine($"CMD reg: {_application.ControlUnit.CMD,2:X2}");
            Console.WriteLine($"ACC reg: {_application.ControlUnit.ACC,2:X2}");
            Console.WriteLine($"DB:      {_application.ControlUnit.DataBus,2:X2}");
            PrintFlags(_application.ControlUnit);
            Console.WriteLine();
            Console.WriteLine("Memory");
            PrintMemory(_application.Memory, _application.ControlUnit);
            Console.WriteLine();
            Console.WriteLine("Registers");
            PrintRegisters(_application.Registers);
            Console.WriteLine();
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

        private static void PrintMemory(IMemory memory, IControlUnit controlUnit)
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

        private static void PrintRegisters(IRegisterBlock registers)
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
