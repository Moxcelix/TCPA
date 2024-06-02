using TCPA.Models;

namespace TCPA.Infrastructure
{
    internal class ArithmeticLogicUnit : IArithmeticLogicUnit
    {
        private enum State
        {
            WRITE,
            READY,
        }

        private State _state = State.WRITE;
        private byte _op0 = 0;
        private byte _op1 = 0;

        public byte DataBus { get; set; }

        public byte CodeBus { get; set; }

        public bool Ready => _state == State.READY;

        public bool Z { get; private set; }

        public bool C { get; private set; }

        public bool V { get; private set; }

        public bool N { get; private set; }

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
                    var mode = CodeBus & 0b_1111_0000;
                    switch (mode)
                    {
                        case 0b_1001_0000:
                            _op0 = DataBus;
                            return;
                        case 0b_1010_0000:
                            _op1 = DataBus;
                            return;
                        case 0b_1011_0000:
                            break;
                        default:
                            return;

                    }

                    Z = false;
                    N = false;
                    V = false;
                    C = false;

                    var code = CodeBus & 0b_0000_1111;

                    switch (code)
                    {
                        case 0b_0000:
                            int sum = _op0 + _op1;

                            C = sum > 0b_1111_1111;
                            N = (sbyte)sum < 0;
                            Z = sum == 0;

                            DataBus = (byte)sum;
                            break;
                        case 0b_0001:
                            int sub = _op0 - _op1;

                            C = sub > 0b_1111_1111;
                            N = (sbyte)sub < 0;
                            Z = sub == 0;

                            DataBus = (byte)sub;
                            break;
                        case 0b_0010:
                            Z = _op0 <= _op1;
                            V = _op0 >= _op1;
                            break;
                        case 0b_0011:
                            DataBus = (byte)(_op0 << _op1);
                            C = (_op0 & 0b_1000_0000) == 1;
                            break;
                        case 0b_0100:
                            DataBus = (byte)(_op0 >> _op1);
                            C = (_op0 & 0b_0000_0001) == 1;
                            break;
                        case 0b_0101:
                            DataBus = (byte)(_op0 | _op1);
                            C = (_op0 | _op1) != 0;
                            break;
                        case 0b_0110:
                            DataBus = (byte)(_op0 & _op1);
                            C = (_op0 & _op1) != 0;
                            break;
                        case 0b_0111:
                            DataBus = (byte)(~_op0);
                            C = (~_op0) != 0;
                            break;
                    }
                    _state = State.READY;
                    break;
            }
        }
    }
}
