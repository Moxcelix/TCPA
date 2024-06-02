using TCPA.Infrastructure;
using TCPA.Application;

var memory = new LinearMemory(256);
var alu = new ArithmeticLogicUnit();
var registers = new GeneralPurposeRegisters();
var controlUnit = new ControlUnit();

var controller = new Controller(memory, alu, registers, controlUnit);

int tact = 0;

while (true)
{
    controller.Update();

    Console.WriteLine("\nTact: " + tact++);
    Console.WriteLine("CU state: " + controlUnit.CurrentState.ToString());
    Console.WriteLine("CC reg: " + controlUnit.CC.ToString());
    Console.WriteLine("CMD reg: " + controlUnit.CMD.ToString());
    Console.WriteLine("ACC reg: " + controlUnit.ACC.ToString());

    Thread.Sleep(100);
}

