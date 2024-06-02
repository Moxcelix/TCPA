﻿using TCPA.Models;

namespace TCPA.Infrastructure
{
    internal class ControlUnit : IControlUnit
    {
        private enum State
        {
            DISABLED,
            REQUEST_READ_BYTE,
            READ_BYTE,
            CHECK_PART,
            CHECK_DOUBLE_OP,
            CHECK_MOV,
            NEXT_CC_ALU_FIRST,
            NEXT_CC_ALU_SECOND,
            NEXT_CC_ALU_RESULT,
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
            DISABLE = 0b_0000_0000,
        }


        private State _state = State.DISABLED;
        private byte _cmd = 0;
        private byte _op0 = 0;
        private byte _op1 = 0;
        private byte _acc = 0;
        private byte _buf = 0;
        private byte _cc = 0;
        private byte _pc = 0;

        public byte DataBus { get; set; }

        public bool Reset { get; set; }

        public bool Enabled { get; set; }

        public bool ALUReady { get; set; }

        public bool MemoryReady { get; set; }

        public byte AddressBus { get; private set; }

        public byte RegisterCodeBus { get; private set; }

        public byte ALUCodeBus { get; private set; }

        public byte MemoryCodeBus { get; private set; }

        public void Update()
        {
            if (!Enabled)
            {
                return;
            }

            switch (_state)
            {
                case State.DISABLED:
                    _state = State.REQUEST_READ_BYTE;
                    break;
                case State.REQUEST_READ_BYTE:
                    AddressBus = _cc;
                    MemoryCodeBus = (byte)MemoryCode.READ;
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
                        _state = State.NEXT_CC_ALU_FIRST;
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
                        _op1 = DataBus;
                        _state = State.CHECK_MOV;
                    }
                    break;
                case State.NEXT_CC_ALU_FIRST:
                    DataBus = _cc;
                    ALUCodeBus = (byte)ALUCode.FIRST;
                    _state = State.NEXT_CC_ALU_SECOND;
                    break;
                case State.NEXT_CC_ALU_SECOND:
                    DataBus = 1;
                    ALUCodeBus = (byte)ALUCode.SECOND;
                    _state = State.NEXT_CC_ALU_RESULT;
                    break;
                case State.NEXT_CC_ALU_RESULT:
                    if (ALUReady)
                    {
                        _cc = DataBus;
                        ALUCodeBus = (byte)ALUCode.DISABLE;
                        _state = State.REQUEST_READ_BYTE;
                    }
                    break;
            }
        }
    }
}
