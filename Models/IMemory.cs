namespace TCPA.Models
{
    internal interface IMemory : IUpdatable
    {
        public byte DataBus {  get; set; }

        public byte AddressBus {  get; set; }

        public bool Enabled {  get; set; }

        public bool RWMode { get; set; }

        public bool Ready { get; }
    }
}
