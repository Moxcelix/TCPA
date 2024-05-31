namespace TCPA.Models
{
    internal interface IControlUnit
    {
        public byte DataBus { get; set; }

        public byte AddressBus { get; set; }
    }
}
