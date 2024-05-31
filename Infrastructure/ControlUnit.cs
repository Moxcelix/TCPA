using TCPA.Models;

namespace TCPA.Infrastructure
{
    internal class ControlUnit : IControlUnit
    {
        public byte DataBus { get; set; }
        public byte AddressBus { get; set; }
        public bool Reset { get; set; }
        public bool Enabled { get; set; }

        public byte RegisterCodeBus => throw new NotImplementedException();

        public byte ALUCodeBus => throw new NotImplementedException();

        public byte MemoryCodeBus => throw new NotImplementedException();

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
