namespace TCPA.Models
{
    internal interface IMemory : IUpdatable
    {
        public byte DataBus {  get; set; }

        public byte AddressBus {  get; set; }

        public byte CodeBus { get; set; }

        public bool Ready { get; }
    }
}
