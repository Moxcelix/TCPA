using TCPA.Infrastructure;
using TCPA.Application;

var memory = new LinearMemory(64);
var alu = new ArithmeticLogicUnit();
var registers = new GeneralPurposeRegisters();
var controlUnit = new ControlUnit();

var controller = new Controller(memory, alu, registers, controlUnit);

int tact = 0;

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

memory.SetData(program);
controller.Start();

void PrintFlags()
{
    Console.Write("Flags: ");
    Console.ForegroundColor = controlUnit.N ? ConsoleColor.Green : ConsoleColor.Red;
    Console.Write("N");
    Console.ForegroundColor = controlUnit.V ? ConsoleColor.Green : ConsoleColor.Red;
    Console.Write("V");
    Console.ForegroundColor = controlUnit.C ? ConsoleColor.Green : ConsoleColor.Red;
    Console.Write("C");
    Console.ForegroundColor = controlUnit.Z ? ConsoleColor.Green : ConsoleColor.Red;
    Console.Write("Z");
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine();
}

void PrintMemory()
{
    var data = memory.GetData();

    for(int i = 0; i < data.Length; i++)
    {

        Console.BackgroundColor = i == controlUnit.CC ? ConsoleColor.Red : ConsoleColor.Black;
        Console.ForegroundColor = i == memory.LastChanged ? ConsoleColor.Yellow : ConsoleColor.Gray;
        Console.Write($"{data[i]:X2}");
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Write(" ");

        if (i%16 == 15)
        {
            Console.WriteLine();
        }
    }   
}

void PrintRegisters()
{
    var data = registers.GetData();

    for (int i = 0; i < data.Length; i++)
    {
        Console.ForegroundColor = i == registers.LastChanged ? ConsoleColor.Yellow : ConsoleColor.Gray;
        Console.Write($"{data[i]:X2} ");
    }
    Console.WriteLine();
}

int initialCursorTop = Console.CursorTop;
Console.CursorVisible = false;

while (true)
{
    Console.SetCursorPosition(0, initialCursorTop);

    controller.Update();

    Console.WriteLine($"Tact: {tact++:0000}");
    Console.WriteLine($"CU State: {controlUnit.CurrentState,-32}");
    Console.WriteLine($"CC reg:  {controlUnit.CC,2:X2}");
    Console.WriteLine($"CMD reg: {controlUnit.CMD,2:X2}");
    Console.WriteLine($"ACC reg: {controlUnit.ACC,2:X2}");
    Console.WriteLine($"DB:      {controlUnit.DataBus,2:X2}");
    PrintFlags();
    Console.WriteLine();
    Console.WriteLine("Memory");
    PrintMemory();
    Console.WriteLine();
    Console.WriteLine("Registers");
    PrintRegisters();
    Console.WriteLine();

    Thread.Sleep(10);
}

