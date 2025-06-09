using System;
using System.Diagnostics;
using System.Threading;
using System.IO.Ports;

namespace RX7Interface
{
    public class DummyDataStream : IDataStream
    {
        private Random random;

        // Read time in ms.
        private const int ReadTime = 100;

        public DummyDataStream()
        {
            random = new Random();
        }

        public void Open()
        {
            IsOpen = true;
        }

        public bool IsOpen
        {
            get;
            private set;
        }

        public void Close()
        {
            IsOpen = false;
        }

        public byte ReadByte(uint address)
        {
            return ReadByte(address, 1);
        }

        public byte[] DumpBytes(uint address, byte length)
        {
            return DumpBytes(address, length, 1);
        }

        public byte[] DumpBytes(uint address, byte length, int retryCount)
        {
            if (address == 0x0100 && length == 16)
            {
                // Simulate a diagnostic data response.
                //  5A 20 DF F0 0F C0 3F 10 EF FF 00 FE 01 E2 1D C0
                return new byte[]
                {
                    0x5A, 0x20, 0xDF, 0xF0, 0x0F, 0xC0, 0x3F, 0x10,
                    0xEF, 0xFF, 0x00, 0xFE, 0x01, 0xE2, 0x1D, 0xC0
                };
            }



            byte[] randomBytes = new byte[length];
            Random random = new Random();
            random.NextBytes(randomBytes);
            //Console.WriteLine(string.Format("0x{0:X4}: {1}", address, BitConverter.ToString(randomBytes)));
            return randomBytes;

        }


        public string ReadECUId()
        {
            return "N3A1-MPA";
        }

        public byte ReadByte(uint address, int retryCount)
        {
            /*   new Parameter("Intake Air Pressure", "mmHg", 0x0021, ParameterLength.TwoBytes, 500d/256, -1000),
                 new Parameter("Throttle Angle (Narrow)", "V", 0x0023, ParameterLength.OneByte, 5d/256),
                 new Parameter("Throttle Angle (Wide)", "V", 0x0024, ParameterLength.OneByte, 5d/256),
                 new Parameter("Oxygen Sensor Voltage", "V", 0x0025, ParameterLength.OneByte, 5d/256),
                 new Parameter("MOP Position", "V", 0x0026, ParameterLength.OneByte, 5d/256),
                 new Parameter("Battery Voltage", "V", 0x0027, ParameterLength.OneByte, 20d/256),
                 new Parameter("Water Temperature", "ºC", 0x0028, ParameterLength.OneByte, 160d/256, -40),
                 new Parameter("Fuel Temperature", "ºC", 0x0029, ParameterLength.OneByte, 160d/256),
                 new Parameter("Intake Air Temperature", "ºC", 0x002a, ParameterLength.OneByte, 160d/256, -40),
                 new Parameter("Engine Speed", "rpm", 0x002c, ParameterLength.TwoBytes, 500d/256),
                 new Parameter("Vehicle Speed", "km/h", 0x002e, ParameterLength.OneByte, 356d/256),
                 new Parameter("Injector On Time", "ms", 0x0800, ParameterLength.TwoBytes, 1d/256),
                 new Parameter("Injector Period", "ms", 0x0802, ParameterLength.TwoBytes, 1d/256),
                 new Parameter("Ignition Advance (Leading)", "degrees", 0x0804, ParameterLength.OneByte, 90d/256, -25),
                 new Parameter("Ignition Advance (Trailling)", "degrees", 0x0805, ParameterLength.OneByte, 90d/256, -25),
                 new Parameter("Idle Control", "%", 0x0806, ParameterLength.TwoBytes, 800d/256),
                 new Parameter("Unknown", "?", 0x0808, ParameterLength.OneByte, 1),
                 new Parameter("Turbo Pre-Control", "%", 0x0809, ParameterLength.OneByte, 100d/256),
                 new Parameter("Wastegate Control", "%", 0x080a, ParameterLength.OneByte, 100d/256),*/

            Thread.Sleep(ReadTime);
            switch (address)
            {
                default:
                    return (byte)random.Next(256);
            }
        }
    }
}
