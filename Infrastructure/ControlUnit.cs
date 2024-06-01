using TCPA.Models;

namespace TCPA.Infrastructure
{
    internal class ControlUnit : IControlUnit
    {
        public byte DataBus { get; set; }
        public byte AddressBus { get; set; }
        public bool Reset { get; set; }
        public bool Enabled { get; set; }

        public byte RegisterCodeBus { get; private set; }

        public byte ALUCodeBus { get; private set; }

        public byte MemoryCodeBus { get; private set; }

        public void Update()
        {

        }
    }
}
