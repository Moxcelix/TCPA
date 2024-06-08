using TCPA.Infrastructure;

namespace TCPA.Application
{
    internal class Application
    {
        private readonly Controller _controller;
        private readonly LinearMemory _memory;
        private readonly ArithmeticLogicUnit _alu;
        private readonly GeneralPurposeRegisters _registers;
        private readonly ControlUnit _controlUnit;

        internal static readonly char[] separator = [' ', '\r', '\n'];

        public Application()
        {
            _memory = new LinearMemory(64);
            _alu = new ArithmeticLogicUnit();
            _registers = new GeneralPurposeRegisters();
            _controlUnit = new ControlUnit();

            _controller = new Controller(
                _memory,
                _alu, 
                _registers, 
                _controlUnit);
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

                _memory.SetData(byteArray);
            }
        }
    }
}
