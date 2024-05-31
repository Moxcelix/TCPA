namespace TCPA.Models
{
    internal interface IArithmeticLogicUnit : IUpdatable
    {
        public byte DataBus { get; set; }

        public byte CodeBus { get; set; }

        public bool Ready { get; }

        public bool Z { get; }

        public bool C { get; }

        public bool V { get; }

        public bool N { get; }
    }
}
