using TCPA.Models;

namespace TCPA.Infrastructure
{
    internal class GeneralPurposeRegisters : IRegisterBlock
    {
        private readonly byte[] _registers = new byte[16];

        public byte DataBus { get; set; }

        public byte CodeBus { get; set; }

        public void Update()
        {
            if (!CheckBit(CodeBus, 7))
            {
                return;
            }

            if (CheckBit(CodeBus, 6))
            {
                _registers[CodeBus & 0b_0000_1111] = DataBus;
            }
            else
            {
                DataBus = _registers[CodeBus & 0b_0000_1111];
            }
        }

        private static bool CheckBit(byte data, byte position)
        {
            return (data & 0x1 << position) != 0;
        }
    }
}
