using TCPA.Models;

namespace TCPA.Infrastructure
{
    internal class ArithmeticLogicUnit : IArithmeticLogicUnit
    {
        private enum State
        {
            READY,
            SUM,
            SUB,
            CMP,
            ROL,
            ROR,
            OR,
            AND,
            NOT
        }

        private State _state = State.READY;
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
            switch (_state)
            {
                case State.READY:
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
                            _state = State.SUM;
                            break;
                        case 0b_0001:
                            _state = State.SUB;
                            break;
                        case 0b_0010:
                            _state = State.CMP;
                            break;
                        case 0b_0011:
                            _state = State.ROL;
                            break;
                        case 0b_0100:
                            _state = State.ROR;
                            break;
                        case 0b_0101:
                            _state = State.OR;
                            break;
                        case 0b_0110:
                            _state = State.AND;
                            break;
                        case 0b_0111:
                            _state = State.NOT;
                            break;
                    }
                    break;
                case State.SUM:
                    int sum = _op0 + _op1;
                    if (sum > 0b_1111_1111)
                    {
                        C = true;
                    }
                    DataBus = (byte)sum;
                    _state = State.READY;
                    Z = sum == 0;
                    break;
                case State.SUB:
                    int sub = _op0 - _op1;
                    if (sub < 0)
                    {
                        N = true;
                    }
                    DataBus = (byte)sub;
                    _state = State.READY;
                    Z = sub == 0;
                    break;
                case State.CMP:
                    Z = _op0 <= _op1;
                    V = _op0 >= _op1;
                    _state = State.READY;
                    break;
                case State.ROR:
                    DataBus = (byte)(_op0 >> _op1);
                    C = (_op0 & 0b_0000_0001) == 1;
                    _state = State.READY;
                    break;
                case State.ROL:
                    DataBus = (byte)(_op0 << _op1);
                    C = (_op0 & 0b_1000_0000) == 1;
                    _state = State.READY;
                    break;
                case State.OR:
                    DataBus = (byte)(_op0 | _op1);
                    C = (_op0 | _op1) != 0;
                    _state = State.READY;
                    break;
                case State.AND:
                    DataBus = (byte)(_op0 & _op1);
                    C = (_op0 & _op1) != 0;
                    _state = State.READY;
                    break;
                case State.NOT:
                    DataBus = (byte)(~_op0);
                    C = (~_op0) != 0;
                    _state = State.READY;
                    break;
            }
        }
    }
}
