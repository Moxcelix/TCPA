using TCPA.Infrastructure;
using TCPA.Application;

var memory = new LinearMemory(256);
var alu = new ArithmeticLogicUnit();
var registers = new GeneralPurposeRegisters();
var controlUnit = new ControlUnit();

var controller = new Controller(memory, alu, registers, controlUnit); 

while (true)
{
    controller.Update();

   // Console.WriteLine(controlUnit.CurrentState.ToString());

    Thread.Sleep(100);
}

