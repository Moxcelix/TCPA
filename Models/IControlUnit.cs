namespace TCPA.Models
{
    internal enum DataMode
    {
        NONE,
        MEMORY,
        REGISTER,
        ALU
    }

    internal interface IControlUnit : IUpdatable
    {
        public byte DataBus { get; set; }

        public bool Reset { get; set; }

        public bool Enabled { get; set; }

        public bool ALUReady { get; set; }

        public bool MemoryReady { get; set; }

        public bool Z { get; set; }

        public bool C { get; set; }

        public bool V { get; set; }

        public bool N { get; set; }

        public DataMode DataMode { get; }

        public bool RW { get; }

        public byte AddressBus { get; }

        public byte RegisterCodeBus { get; }

        public byte ALUCodeBus { get; }

        public byte MemoryCodeBus { get; }
    }
}
