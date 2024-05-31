using TCPA.Models;

namespace TCPA.Application
{
    internal class Controller(
        IMemory memory,
        IArithmeticLogicUnit arithmeticLogicUnit,
        IRegisterBlock registerBlock,
        IControlUnit controlUnit) : IUpdatable
    {
        private readonly IControlUnit _controlUnit = controlUnit;
        private readonly IMemory _memory = memory;
        private readonly IArithmeticLogicUnit _arithmeticLogicUnit = arithmeticLogicUnit;
        private readonly IRegisterBlock _registerBlock = registerBlock;

        private byte _dataBus = 0b_0000_0000;
        private byte _addressBus = 0b_0000_0000;

        public void Update()
        {
            _dataBus = (byte)(_controlUnit.DataBus
                | _memory.DataBus 
                | _arithmeticLogicUnit.DataBus 
                | _registerBlock.DataBus);

            _addressBus = (byte)(_controlUnit.AddressBus);

            _controlUnit.DataBus = _dataBus;
            _memory.DataBus = _dataBus;
            _arithmeticLogicUnit.DataBus = _dataBus;
            _registerBlock.DataBus = _dataBus;

            _memory.AddressBus = _addressBus;

            _memory.CodeBus = _controlUnit.MemoryCodeBus;
            _registerBlock.CodeBus = _controlUnit.RegisterCodeBus;

            _controlUnit.Update();
            _memory.Update();
            _arithmeticLogicUnit.Update();
            _registerBlock.Update();
        }
    }
}
