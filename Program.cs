using TCPA.Infrastructure;
using TCPA.Application;

var memory = new LinearMemory(256);
var alu = new ArithmeticLogicUnit();
var registers = new GeneralPurposeRegisters();
var controlUnit = new ControlUnit();

var controller = new Controller(memory, alu, registers, controlUnit);

int tact = 0;

byte[] program = {
    0x01,
    0x07,
    0x04,
    0x00,
    0x03,
    0x06,
    0x00,
    0b_0001_1010, // MOV
    0b_0010_0101, // R5
    0b_0100_0010  // [2]
};

memory.SetData(program);
controller.Start();

void PrintMemory()
{
    var data = memory.GetData();

    for(int i = 0; i < data.Length; i++)
    {

        Console.BackgroundColor = i == controlUnit.CC ? ConsoleColor.Red : ConsoleColor.Black;
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
    Console.WriteLine();
    Console.WriteLine("Memory");
    PrintMemory();
    Console.WriteLine();
    Console.WriteLine("Registers");
    PrintRegisters();
    Console.WriteLine();

    Thread.Sleep(500);
}

