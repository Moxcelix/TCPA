using TCPA.Infrastructure;
using TCPA.Application;

var memory = new LinearMemory(16);
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
        Console.Write(data[i] + " ");
    }
    Console.WriteLine();    
}

void PrintRegisters()
{
    var data = registers.GetData();

    for (int i = 0; i < data.Length; i++)
    {
        Console.Write(data[i] + " ");
    }
    Console.WriteLine();
}

while (true)
{
    controller.Update();

    Console.WriteLine("Tact: " + tact++);
    Console.WriteLine("CU state: " + controlUnit.CurrentState.ToString());
    Console.WriteLine("CC reg: " + controlUnit.CC.ToString());
    Console.WriteLine("CMD reg: " + controlUnit.CMD.ToString());
    Console.WriteLine("ACC reg: " + controlUnit.ACC.ToString());
    Console.WriteLine();
    Console.WriteLine("Memory");
    PrintMemory();
    Console.WriteLine();
    Console.WriteLine("Registers");
    PrintRegisters();
    Console.WriteLine();

    Thread.Sleep(200);
}

