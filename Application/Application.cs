using TCPA.Infrastructure;
using TCPA.Models;

namespace TCPA.Application
{
    internal class Application
    {
        public delegate void OnUpdateDelegate();
        private event OnUpdateDelegate? OnUpdate;

        public Controller Controller { get; }
        public IMemory Memory { get; }
        public IArithmeticLogicUnit ALU { get; }
        public IRegisterBlock Registers { get; }
        public IControlUnit ControlUnit { get; }

        internal static readonly char[] separator = [' ', '\r', '\n'];

        public Application()
        {
            Memory = new LinearMemory(64);
            ALU = new ArithmeticLogicUnit();
            Registers = new GeneralPurposeRegisters();
            ControlUnit = new ControlUnit();

            Controller = new Controller(
                Memory,
                ALU, 
                Registers, 
                ControlUnit);
        }

        public void LoadData(string path)
        {
            if(File.Exists(path))
            {
                var file = File.ReadAllText(path);

                byte[] byteArray = file
                   .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                   .Select(hex => Convert.ToByte(hex, 16))
                   .ToArray();

                Memory.SetData(byteArray);
            }
            else
            {
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
                Memory.SetData(program);
            }
        }

        public void AddUpdateListener(OnUpdateDelegate listener)
        {
            OnUpdate += listener;
        }

        public void Run()
        {
            var thread = new Thread(Update);

            thread.Start();
        }

        private void Update()
        {
            Controller.Start();

            while(true)
            {
                OnUpdate?.Invoke();

                Controller.Update();

                Thread.Sleep(10);
            }
        }
    }
}
