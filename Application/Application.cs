﻿using TCPA.Infrastructure;
using TCPA.Models;

namespace TCPA.Application
{
    internal class Application
    {
        public delegate void OnUpdateDelegate();
        private event OnUpdateDelegate? OnUpdate;

        public Controller Controller { get; }
        public IMemory Memory { get; }
        public IArithmeticLogicUnit ALU { get; }
        public IRegisterBlock Registers { get; }
        public IControlUnit ControlUnit { get; }

        internal static readonly char[] separator = [' ', '\r', '\n'];

        public Application()
        {
            Memory = new LinearMemory(64);
            ALU = new ArithmeticLogicUnit();
            Registers = new GeneralPurposeRegisters();
            ControlUnit = new ControlUnit();

            Controller = new Controller(
                Memory,
                ALU, 
                Registers, 
                ControlUnit);
        }

        public void LoadData(string path)
        {
            if(File.Exists(path))
            {
                var file = File.ReadAllText(path);

                byte[] byteArray = file
                   .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                   .Select(hex => Convert.ToByte(hex, 16))
                   .ToArray();

                Memory.SetData(byteArray);
            }
        }

        public void AddUpdateListener(OnUpdateDelegate listener)
        {
            OnUpdate += listener;
        }

        public void Run()
        {
            var thread = new Thread(Update);

            thread.Start();
        }

        private void Update()
        {
            Controller.Start();

            while(true)
            {
                OnUpdate?.Invoke();

                Controller.Update();

                Thread.Sleep(10);
            }
        }
    }
}
