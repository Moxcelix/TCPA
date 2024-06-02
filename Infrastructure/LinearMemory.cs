using TCPA.Models;

namespace TCPA.Infrastructure
{
    internal class LinearMemory(int size) : IMemory
    {
        private enum State
        {
            WRITE,
            READY, 
            OUT_OF_RANGE
        }

        private readonly byte[] _data = new byte[size];

        private State _state = State.WRITE;

        public byte DataBus { get; set; }

        public byte AddressBus { get; set; }

        public byte CodeBus { get; set; }

        public bool OutOfRange => _state == State.OUT_OF_RANGE;

        public bool Ready => _state == State.READY;

        public void SetData(byte[] data)
        {
            if(data.Length <= _data.Length)
            {
                for(int i = 0; i < data.Length; i++)
                {
                    _data[i] = data[i];
                }
            }
        }


        public void Update()
        {
            var enabled = (CodeBus & 0b_1000_0000) != 0;

            if (!enabled)
            {
                _state = State.WRITE;

                return;
            }

            switch (_state)
            {
                case State.WRITE:
                    switch (CodeBus)
                    {
                        case 0b_0000_0000:
                            DataBus = 0b_0000_0000;
                            break;
                        case 0b_1000_0000:
                            if(AddressBus >= _data.Length)
                            {
                                _state = State.OUT_OF_RANGE;
                                break;
                            }
                            DataBus = _data[AddressBus];
                            _state = State.READY;
                            break;
                        case 0b_1000_0001:
                            if (AddressBus >= _data.Length)
                            {
                                _state = State.OUT_OF_RANGE;
                                break;
                            }
                            _data[AddressBus] = DataBus;
                            _state = State.READY;
                            break;
                        default:
                            break;
                    }
                    break;

                case State.OUT_OF_RANGE:
                    _state = State.WRITE;
                    break;
            }
        }
    }
}
