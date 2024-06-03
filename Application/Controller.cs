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

        public void Start()
        {
            _controlUnit.Enabled = true;
        }

        public void Update()
        {
            if (!_controlUnit.Enabled)
            {
                _controlUnit.Enabled = true;
            }

            if(_memory.OutOfRange)
            {
                _controlUnit.Reset = true;
            }

            _controlUnit.MemoryReady = _memory.Ready;
            _controlUnit.ALUReady = _arithmeticLogicUnit.Ready;

            if((_controlUnit.ALUCodeBus & 0b_0100_0000) != 0)
            {
                _controlUnit.N = _arithmeticLogicUnit.N;
                _controlUnit.V = _arithmeticLogicUnit.V;
                _controlUnit.Z = _arithmeticLogicUnit.Z;
                _controlUnit.C = _arithmeticLogicUnit.C;
            }

            _dataBus = _controlUnit.DataBus;

            if (!_controlUnit.RW)
            {
                if(_controlUnit.RegisterCodeBus != 0)
                {
                    _dataBus = _registerBlock.DataBus;
                }
                else if (_controlUnit.ALUCodeBus != 0)
                {
                    _dataBus = _arithmeticLogicUnit.DataBus;
                }
                else if(_controlUnit.MemoryCodeBus != 0)
                {
                    _dataBus = _memory.DataBus;
                }
            }

            _addressBus = _controlUnit.AddressBus;

            _controlUnit.DataBus = _dataBus;
            _memory.DataBus = _dataBus;
            _arithmeticLogicUnit.DataBus = _dataBus;
            _registerBlock.DataBus = _dataBus;

            _memory.AddressBus = _addressBus;

            _memory.CodeBus = _controlUnit.MemoryCodeBus;
            _registerBlock.CodeBus = _controlUnit.RegisterCodeBus;
            _arithmeticLogicUnit.CodeBus = _controlUnit.ALUCodeBus;

            _controlUnit.Update();
            _memory.Update();
            _arithmeticLogicUnit.Update();
            _registerBlock.Update();
        }
    }
}
