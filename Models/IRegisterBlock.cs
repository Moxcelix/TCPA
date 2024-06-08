namespace TCPA.Models
{
    internal interface IRegisterBlock : IUpdatable
    {
        public byte DataBus { get; set; }

        public byte CodeBus { get; set; }

        public int LastChanged { get; }

        public byte[] GetData();
    }
}
