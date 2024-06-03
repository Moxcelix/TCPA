using TCPA.Models;

namespace TCPA.Infrastructure
{
    internal class GeneralPurposeRegisters : IRegisterBlock
    {
        private readonly byte[] _registers = new byte[16];

        public byte DataBus { get; set; }

        public byte CodeBus { get; set; }

        public int LastChanged {  get; private set; }

        public byte[] GetData()
        {
            return _registers;
        }

        public void Update()
        {
            if (!CheckBit(CodeBus, 7))
            {
                LastChanged = -1;

                return;
            }

            var index = CodeBus & 0b_0000_1111;

            if (CheckBit(CodeBus, 6))
            {
                _registers[index] = DataBus;

                LastChanged = index;
            }
            else
            {
                DataBus = _registers[index];
            }
        }

        private static bool CheckBit(byte data, byte position)
        {
            return (data & 0x1 << position) != 0;
        }
    }
}
