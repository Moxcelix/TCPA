using TCPA.Models;

namespace TCPA.Infrastructure
{
    internal class ControlUnit : IControlUnit
    {
        private enum State
        {
            DISABLED,
            REQUEST_READ_BYTE,
            READ_BYTE,
            READ_OP0
        }

        private const byte READ_MEMORY      = 0b_1000_0000;
        private const byte WRITE_MEMORY     = 0b_1000_0001;
        private const byte DISABLE_MEMORY   = 0b_0000_0000;

        private const byte ALU_FIRST = 0b_1001_0000;
        private const byte ALU_SECOND = 0b_1010_0000;
        private const byte ALU_RESULT = 0b_1011_0000;
        private const byte ALU_DISABLE = 0b_0000_0000;

        private State _state = State.DISABLED;
        private byte _cmd = 0;
        private byte _op0 = 0;
        private byte _op1 = 0;
        private byte _acc = 0;
        private byte _buf = 0;
        private byte _cc = 0;

        public byte DataBus { get; set; }

        public bool Reset { get; set; }

        public bool Enabled { get; set; }

        public bool ALUReady { get; set; }

        public bool MemoryReady { get; set; }

        public byte AddressBus { get; private set; }

        public byte RegisterCodeBus { get; private set; }

        public byte ALUCodeBus { get; private set; }

        public byte MemoryCodeBus { get; private set; }

        public void Update()
        {
            if (!Enabled)
            {
                return;
            }

            switch (_state)
            {
                case State.DISABLED:
                    _state = State.REQUEST_READ_BYTE;
                    break;
                case State.REQUEST_READ_BYTE:
                    AddressBus = _cc;
                    MemoryCodeBus = READ_MEMORY;
                    _state = State.READ_BYTE;
                    break;
                case State.READ_BYTE:
                    if (MemoryReady)
                    {
                        _cmd = DataBus;
                        MemoryCodeBus = DISABLE_MEMORY;
                        DataBus = _cc;
                        ALUCodeBus = ALU_FIRST;
                        _state = State.READ_BYTE;
                    }
                    break;
            }
        }
    }
}
