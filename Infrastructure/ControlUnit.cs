using TCPA.Models;

namespace TCPA.Infrastructure
{
    internal class ControlUnit : IControlUnit
    {
        public enum State
        {
            DISABLED,
            REQUEST_READ_BYTE,
            READ_BYTE,
            CHECK_PART,
            CHECK_DOUBLE_OP,
            CHECK_SINGLE_OP,
            CHECK_MOV,
            NEXT_CC_ALU_FIRST,
            NEXT_CC_ALU_SECOND,
            NEXT_CC_ALU_GET_RESULT,
            NEXT_CC_ALU_WAIT_RESULT,
            JUMP_IF_C,
            JUMP_IF_N,
            JUMP_IF_V,
            JUMP_IF_Z,
            JUMP_IF_NOT_C,
            JUMP_IF_NOT_N,
            JUMP_IF_NOT_V,
            JUMP_IF_NOT_Z,
            PARSE_ADDRESSING,
            CONST_ADDRESSING,
            DIRECT_ADDRESSING,
            INDIRECT_ADDRESSING,
            REGISTER_ADDRESSING,
            INDIRECT_TO_DIRECT,
            RETURN,
            CHECK_COMMAND,
            ALU_DOUBLE_OPERATION,
            ALU_GET_RESULT_NOT,
            ALU_GET_RESULT_AND,
            ALU_GET_RESULT_OR,
            ALU_GET_RESULT_SUM,
            ALU_GET_RESULT_SUB,
            ALU_GET_RESULT_ROL,
            ALU_GET_RESULT_ROR,
            ALU_GET_RESULT_CMP,
            ALU_WAIT_RESULT_CMP,
            ALU_WAIT_RESULT,
        }

        private enum MemoryCode : byte
        {
            READ = 0b_1000_0000,
            WRITE = 0b_1000_0001,
            DISABLE = 0b_0000_0000,
        }

        private enum ALUCode : byte
        {
            FIRST = 0b_1001_0000,
            SECOND = 0b_1010_0000,
            RESULT = 0b_1011_0000,
            RESULT_FLAGS = 0b_1111_0000,
            DISABLE = 0b_0000_0000,
        }

        private enum OperationCode : byte
        {
            NOP = 0b_0000_0000,
            ORG = 0b_0000_0001,
            CMP = 0b_0000_0010,
            JZ = 0b_0000_0011,
            JN = 0b_0000_0100,
            JV = 0b_0000_0101,
            JC = 0b_0000_0110,
            JNZ = 0b_0000_0111,
            JNN = 0b_0000_1000,
            JNV = 0b_0000_1001,
            JNC = 0b_0000_1010,
            SETZ = 0b_0000_1011,
            SETN = 0b_0000_1100,
            SETV = 0b_0000_1101,
            SETC = 0b_0000_1110,
            CLRZ = 0b_0000_1111,
            CLRN = 0b_0001_0000,
            CLRV = 0b_0001_0001,
            CLRC = 0b_0001_0010,
            SUM = 0b_0001_0011,
            SUB = 0b_0001_0100,
            ROL = 0b_0001_0101,
            ROR = 0b_0001_0110,
            OR = 0b_0001_0111,
            AND = 0b_0001_1000,
            NOT = 0b_0001_1001,
            MOV = 0b_0001_1010,
        }


        private State _state = State.DISABLED;
        private byte _cmd = 0;
        private byte _op = 0;
        private byte _op0 = 0;
        private byte _op1 = 0;
        private byte _acc = 0;
        private byte _buf = 0;
        private byte _cc = 0;
        private byte _pc = 0;
        private byte _cs = 1;
        private bool _trw = false;

        public byte DataBus { get; set; }

        public bool Reset { get; set; }

        public bool Enabled { get; set; }

        public bool ALUReady { get; set; }

        public bool MemoryReady { get; set; }

        public bool Z { get; set; }

        public bool C { get; set; }

        public bool V { get; set; }

        public bool N { get; set; }

        public bool RW { get; set; }

        public byte AddressBus { get; private set; }

        public byte RegisterCodeBus { get; private set; }

        public byte ALUCodeBus { get; private set; }

        public byte MemoryCodeBus { get; private set; }

        public string CurrentState => _state.ToString();

        public byte CMD => _cmd;

        public byte ACC => _acc;

        public byte CC => _cc;

        private static bool CheckBit(byte data, byte position)
        {
            return (data & 0x1 << position) != 0;
        }

        public void Update()
        {
            if (!Enabled)
            {
                return;
            }

            if (Reset)
            {
                Reset = false;
                Enabled = false;
                _state = State.DISABLED;
                _cmd = 0;
                _op0 = 0;
                _op1 = 0;
                _acc = 0;
                _buf = 0;
                _cc = 0;
                _pc = 0;
            }

            switch (_state)
            {
                case State.DISABLED:
                    _state = State.REQUEST_READ_BYTE;
                    break;

                case State.REQUEST_READ_BYTE:
                    AddressBus = _cc;
                    MemoryCodeBus = (byte)MemoryCode.READ;
                    RW = false;
                    _state = State.READ_BYTE;
                    break;

                case State.READ_BYTE:
                    if (MemoryReady)
                    {
                        MemoryCodeBus = (byte)MemoryCode.DISABLE;
                        _state = State.CHECK_PART;
                    }
                    break;

                case State.CHECK_PART:
                    if (_pc == 0)
                    {
                        _cmd = DataBus;
                        _pc = 1;
                        _state = State.CHECK_SINGLE_OP;
                    }
                    else if (_pc == 1)
                    {
                        _pc = 2;
                        _op0 = DataBus;
                        _state = State.CHECK_DOUBLE_OP;
                    }
                    else if (_pc == 2)
                    {
                        _pc = 3;
                        _acc = _op0;
                        _op1 = DataBus;
                        _op = _op1;
                        _trw = false;
                        _state = State.PARSE_ADDRESSING;
                    }
                    break;

                case State.CHECK_SINGLE_OP:
                    if (_cmd == (byte)OperationCode.NOP)
                    {
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else if (_cmd == (byte)OperationCode.SETC)
                    {
                        C = true;
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else if (_cmd == (byte)OperationCode.SETZ)
                    {
                        Z = true;
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else if (_cmd == (byte)OperationCode.SETV)
                    {
                        V = true;
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else if (_cmd == (byte)OperationCode.SETN)
                    {
                        N = true;
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else if (_cmd == (byte)OperationCode.CLRC)
                    {
                        C = false;
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else if (_cmd == (byte)OperationCode.CLRZ)
                    {
                        Z = false;
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else if (_cmd == (byte)OperationCode.CLRV)
                    {
                        V = false;
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else if (_cmd == (byte)OperationCode.CLRN)
                    {
                        N = false;
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else
                    {
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    break;

                case State.CHECK_DOUBLE_OP:
                    if (_cmd == (byte)OperationCode.ORG)
                    {
                        _pc = 0;
                        _cc = _op0;
                        _state = State.REQUEST_READ_BYTE;
                    }
                    else if (_cmd is ((byte)OperationCode.JC)
                        or ((byte)OperationCode.JN)
                        or ((byte)OperationCode.JZ)
                        or ((byte)OperationCode.JV)
                        or ((byte)OperationCode.JNC)
                        or ((byte)OperationCode.JNN)
                        or ((byte)OperationCode.JNZ)
                        or ((byte)OperationCode.JNV))
                    {
                        _trw = false;
                        _op = _op0;
                        _state = State.PARSE_ADDRESSING;
                    }
                    else if (_cmd == (byte)OperationCode.NOT)
                    {
                        _trw = false;
                        _acc = _op0;
                        _op = _op0;
                        _state = State.PARSE_ADDRESSING;
                    }
                    else
                    {
                        _acc = _op0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    break;

                case State.NEXT_CC_ALU_FIRST:
                    RW = true;
                    DataBus = _cc;
                    ALUCodeBus = (byte)ALUCode.FIRST;
                    _state = State.NEXT_CC_ALU_SECOND;
                    break;

                case State.NEXT_CC_ALU_SECOND:
                    RW = true;
                    DataBus = _cs;
                    ALUCodeBus = (byte)ALUCode.SECOND;
                    _state = State.NEXT_CC_ALU_GET_RESULT;
                    break;

                case State.NEXT_CC_ALU_GET_RESULT:
                    RW = false;
                    ALUCodeBus = (byte)ALUCode.RESULT;
                    _state = State.NEXT_CC_ALU_WAIT_RESULT;
                    break;

                case State.NEXT_CC_ALU_WAIT_RESULT:
                    if (ALUReady)
                    {
                        _cc = DataBus;
                        ALUCodeBus = (byte)ALUCode.DISABLE;
                        _state = State.REQUEST_READ_BYTE;
                    }
                    break;

                case State.PARSE_ADDRESSING:
                    if (CheckBit(_op, 7))
                    {
                        RW = _trw;
                        MemoryCodeBus = (byte)(RW ? MemoryCode.WRITE : MemoryCode.READ);
                        AddressBus = (byte)(_op & 0b_0111_1111);
                        _state = State.DIRECT_ADDRESSING;
                    }
                    else if (CheckBit(_op, 6))
                    {
                        RW = false;
                        _buf = DataBus;
                        MemoryCodeBus = (byte)MemoryCode.READ;
                        AddressBus = (byte)(_op & 0b_0011_1111);
                        _state = State.INDIRECT_ADDRESSING;
                    }
                    else if (CheckBit(_op, 5))
                    {
                        RW = _trw;
                        RegisterCodeBus = (byte)(_op | (RW ? 0b_1100_0000 : 0b_1000_0000));
                        _state = State.REGISTER_ADDRESSING;
                    }
                    else
                    {
                        RW = false;
                        _state = State.CONST_ADDRESSING;
                    }
                    break;

                case State.DIRECT_ADDRESSING:
                    if (MemoryReady)
                    {
                        MemoryCodeBus = (byte)MemoryCode.DISABLE;
                        _state = State.RETURN;
                    }
                    break;

                case State.INDIRECT_ADDRESSING:
                    if (MemoryReady)
                    {
                        MemoryCodeBus = (byte)MemoryCode.DISABLE;
                        AddressBus = DataBus;
                        DataBus = _buf;
                        _state = State.INDIRECT_TO_DIRECT;
                    }
                    break;

                case State.INDIRECT_TO_DIRECT:
                    RW = _trw;
                    MemoryCodeBus = (byte)(RW ? MemoryCode.WRITE : MemoryCode.READ);
                    _state = State.DIRECT_ADDRESSING;
                    break;

                case State.REGISTER_ADDRESSING:
                    RegisterCodeBus = 0;
                    _state = State.RETURN;
                    break;

                case State.CONST_ADDRESSING:
                    DataBus = _op;
                    _state = State.RETURN;
                    break;

                case State.RETURN:
                    if (_pc == 1)
                    {
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else if (_pc == 2)
                    {
                        _pc = 1;
                        _op0 = DataBus;
                        _state = State.CHECK_COMMAND;
                    }
                    else if (_pc == 3)
                    {
                        _pc = 2;
                        _op1 = DataBus;
                        _state = State.CHECK_MOV;
                    }
                    break;

                case State.CHECK_COMMAND:
                    if (_cmd == (byte)OperationCode.JC)
                    {
                        _state = State.JUMP_IF_C;
                    }
                    else if (_cmd == (byte)OperationCode.JN)
                    {
                        _state = State.JUMP_IF_N;
                    }
                    else if (_cmd == (byte)OperationCode.JV)
                    {
                        _state = State.JUMP_IF_V;
                    }
                    else if (_cmd == (byte)OperationCode.JZ)
                    {
                        _state = State.JUMP_IF_Z;
                    }
                    else if (_cmd == (byte)OperationCode.JNC)
                    {
                        _state = State.JUMP_IF_NOT_C;
                    }
                    else if (_cmd == (byte)OperationCode.JNN)
                    {
                        _state = State.JUMP_IF_NOT_N;
                    }
                    else if (_cmd == (byte)OperationCode.JNV)
                    {
                        _state = State.JUMP_IF_NOT_V;
                    }
                    else if (_cmd == (byte)OperationCode.JNZ)
                    {
                        _state = State.JUMP_IF_NOT_Z;
                    }
                    else if (_cmd == (byte)OperationCode.NOT)
                    {
                        RW = true;
                        DataBus = _op0;
                        ALUCodeBus = (byte)ALUCode.FIRST;
                        _state = State.ALU_GET_RESULT_NOT;
                    }
                    else
                    {
                        RW = true;
                        DataBus = _op0;
                        ALUCodeBus = (byte)ALUCode.FIRST;
                        _state = State.ALU_DOUBLE_OPERATION;
                    }
                    break;

                case State.ALU_DOUBLE_OPERATION:
                    RW = true;
                    DataBus = _op1;
                    ALUCodeBus = (byte)ALUCode.SECOND;

                    if (_cmd == (byte)OperationCode.SUM)
                    {
                        _state = State.ALU_GET_RESULT_SUM;
                    }
                    else if (_cmd == (byte)OperationCode.SUB)
                    {
                        _state = State.ALU_GET_RESULT_SUB;
                    }
                    else if (_cmd == (byte)OperationCode.ROL)
                    {
                        _state = State.ALU_GET_RESULT_ROL;
                    }
                    else if (_cmd == (byte)OperationCode.ROR)
                    {
                        _state = State.ALU_GET_RESULT_ROR;
                    }
                    else if (_cmd == (byte)OperationCode.CMP)
                    {
                        _state = State.ALU_GET_RESULT_CMP;
                    }
                    else if (_cmd == (byte)OperationCode.OR)
                    {
                        _state = State.ALU_GET_RESULT_OR;
                    }
                    else if (_cmd == (byte)OperationCode.AND)
                    {
                        _state = State.ALU_GET_RESULT_AND;
                    }
                    break;

                case State.ALU_GET_RESULT_SUM:
                    RW = false;
                    ALUCodeBus = (byte)ALUCode.RESULT_FLAGS | 0b_0000;
                    _state = State.ALU_WAIT_RESULT;
                    break;

                case State.ALU_GET_RESULT_SUB:
                    RW = false;
                    ALUCodeBus = (byte)ALUCode.RESULT_FLAGS | 0b_0001;
                    _state = State.ALU_WAIT_RESULT;
                    break;

                case State.ALU_GET_RESULT_CMP:
                    RW = false;
                    ALUCodeBus = (byte)ALUCode.RESULT_FLAGS | 0b_0010;
                    _state = State.ALU_WAIT_RESULT_CMP;
                    break;

                case State.ALU_GET_RESULT_ROL:
                    RW = false;
                    ALUCodeBus = (byte)ALUCode.RESULT_FLAGS | 0b_0011;
                    _state = State.ALU_WAIT_RESULT;
                    break;

                case State.ALU_GET_RESULT_ROR:
                    RW = false;
                    ALUCodeBus = (byte)ALUCode.RESULT_FLAGS | 0b_0100;
                    _state = State.ALU_WAIT_RESULT;
                    break;

                case State.ALU_GET_RESULT_OR:
                    RW = false;
                    ALUCodeBus = (byte)ALUCode.RESULT_FLAGS | 0b_0101;
                    _state = State.ALU_WAIT_RESULT;
                    break;

                case State.ALU_GET_RESULT_AND:
                    RW = false;
                    ALUCodeBus = (byte)ALUCode.RESULT_FLAGS | 0b_0110;
                    _state = State.ALU_WAIT_RESULT;
                    break;

                case State.ALU_GET_RESULT_NOT:
                    RW = false;
                    ALUCodeBus = (byte)ALUCode.RESULT_FLAGS | 0b_0111;
                    _state = State.ALU_WAIT_RESULT;
                    break;

                case State.ALU_WAIT_RESULT:
                    if (ALUReady)
                    {
                        ALUCodeBus = (byte)ALUCode.DISABLE;
                        _trw = true;
                        _op = _acc;
                        _pc = 1;
                        _state = State.PARSE_ADDRESSING;
                    }
                    break;

                case State.ALU_WAIT_RESULT_CMP:
                    if (ALUReady)
                    {
                        ALUCodeBus = (byte)ALUCode.DISABLE;
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    break;

                case State.CHECK_MOV:

                    if (_cmd == (byte)OperationCode.MOV)
                    {
                        _trw = true;
                        _op = _op0;
                        _pc = 1;
                        _state = State.PARSE_ADDRESSING;
                    }
                    else
                    {
                        _trw = false;
                        _op = _op0;
                        _state = State.PARSE_ADDRESSING;
                    }
                    break;

                case State.JUMP_IF_C:
                    if (C)
                    {
                        _pc = 0;
                        _cs = _op0;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else
                    {
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    break;

                case State.JUMP_IF_N:
                    if (N)
                    {
                        _pc = 0;
                        _cs = _op0;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else
                    {
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    break;

                case State.JUMP_IF_V:
                    if (V)
                    {
                        _pc = 0;
                        _cs = _op0;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else
                    {
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    break;

                case State.JUMP_IF_Z:
                    if (Z)
                    {
                        _pc = 0;
                        _cs = _op0;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else
                    {
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    break;

                case State.JUMP_IF_NOT_C:
                    if (!C)
                    {
                        _pc = 0;
                        _cs = _op0;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else
                    {
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    break;

                case State.JUMP_IF_NOT_N:
                    if (!N)
                    {
                        _pc = 0;
                        _cs = _op0;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else
                    {
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    break;

                case State.JUMP_IF_NOT_V:
                    if (!V)
                    {
                        _pc = 0;
                        _cs = _op0;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else
                    {
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    break;

                case State.JUMP_IF_NOT_Z:
                    if (!Z)
                    {
                        _pc = 0;
                        _cs = _op0;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    else
                    {
                        _pc = 0;
                        _cs = 1;
                        _state = State.NEXT_CC_ALU_FIRST;
                    }
                    break;
            }
        }
    }
}
