using TCPA.Infrastructure;

var alu = new ArithmeticLogicUnit();
alu.DataBus = 255;
alu.CodeBus = 0b_1001_0000;
alu.Update();
alu.DataBus = 0;
alu.CodeBus = 0b_1010_0000;
alu.Update();
alu.CodeBus = 0b_1011_0000;
do
{
    alu.Update();
} while (!alu.Ready);

Console.WriteLine(alu.DataBus);
Console.WriteLine(alu.C);
