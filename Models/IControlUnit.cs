namespace TCPA.Models
{
    internal interface IControlUnit : IUpdatable
    {
        public byte DataBus { get; set; }

        public bool Reset { get; set; }

        public bool Enabled { get; set; }

        public bool ALUReady { get; set; }

        public bool MemoryReady { get; set; }

        public byte AddressBus { get; }

        public byte RegisterCodeBus { get; }

        public byte ALUCodeBus { get; }

        public byte MemoryCodeBus {  get; }
    }
}
