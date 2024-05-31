using TCPA.Models;

namespace TCPA.Infrastructure
{
    internal class LinearMemory(int size) : IMemory
    {
        private enum State
        {
            READY,
            WRITE,
            READ,
            INDIRECT_WRITE,
            INDIRECT_READ,
        }

        private readonly byte[] _data = new byte[size];

        private State _state = State.READY;
        private byte _address = 0;

        public byte DataBus { get; set; }
        public byte AddressBus { get; set; }
        public byte CodeBus { get; set; }

        public bool Ready => _state == State.READY;

        public void Update()
        {
            switch (_state)
            {
                case State.READY:
                    switch (CodeBus)
                    {
                        case 0b_0000_0000:
                            DataBus = 0b_0000_0000;
                            break;
                        case 0b_1000_0000:
                            _state = State.READ;
                            _address = AddressBus;
                            break;
                        case 0b_1000_0001:
                            _state = State.WRITE;
                            _address = AddressBus;
                            break;
                        case 0b_1000_0010:
                            _state = State.INDIRECT_READ;
                            _address = AddressBus;
                            break;
                        case 0b_1000_0011:
                            _state = State.INDIRECT_WRITE;
                            _address = AddressBus;
                            break;
                        default:
                            break;
                    }
                    break;
                case State.WRITE:
                    _data[_address] = DataBus;
                    _state = State.READY;
                    break;
                case State.READ:
                    DataBus = _data[_address];
                    _state = State.READY;
                    break;
                case State.INDIRECT_WRITE:
                    _address = _data[_address];
                    _state = State.WRITE;
                    break;
                case State.INDIRECT_READ:
                    _address = _data[_address];
                    _state = State.READ;
                    break;
            }
        }
    }
}
