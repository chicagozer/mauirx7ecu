using System;
using System.Diagnostics;
using System.Threading;
using System.IO.Ports;
using System.Text;

namespace RX7Interface
{
    public class Rx7DataStream : IDataStream
    {
        // Read timeout in ms.
        private const int ReadTimeout = 500;

        private const byte ReadCommand = 0x40;
 private const byte DumpCommand = 0x50;

        private const byte ECUIDCommand = 0x15;
        private const byte ChecksumError = 0x14;
        private const int DefaultRetryCount = 3;

        private Rx7SerialPort serialPort;

        public Rx7DataStream(string port)
        {
            serialPort = new Rx7SerialPort(port);
        }

        public void Open()
        {
            serialPort.Open();
        }

        public bool IsOpen
        {
            get
            {
                return serialPort.IsOpen;
            }
        }

        public void Close()
        {
            serialPort.Close();
        }
        public string ReadECUId()
        {
            Stopwatch timer = Stopwatch.StartNew();
            List<byte> buffer = new List<byte>();
            bool foundStart = false;

            while (timer.ElapsedMilliseconds < ReadTimeout)
            {
                if (serialPort.BytesAvailable == 0)
                {
                    Thread.Sleep(0);
                    continue;
                }

                byte[] received = serialPort.Read();

                if (!foundStart)
                {
                    int startIdx = Array.IndexOf(received, ECUIDCommand);
                    if (startIdx >= 0)
                    {
                        buffer.AddRange(received[startIdx..]);
                        foundStart = true;
                    }
                }
                else
                {
                    buffer.AddRange(received);
                    if (buffer.Count >= 9 && IsChecksumOk(buffer.ToArray()))
                    {
                        // Skip the command byte, return ASCII string of the rest except checksum
                        return Encoding.ASCII.GetString(buffer.Skip(1).Take(buffer.Count - 2).ToArray());
                    }
                }
            }

            return "TIMEOUT";
        }
        public byte[] DumpBytes(uint address, byte length)
        {
            return DumpBytes(address, length, DefaultRetryCount);
        }


        public byte[] DumpBytes(uint address, byte length, int retryCount)
        {
            if (retryCount < 0)
            {
                throw new System.IO.IOException("Dump failed.");
            }

            byte addressHigher = (byte)(address >> 8);
            byte addressLower = (byte)(address % 256);

            byte[] packet = new byte[] { DumpCommand, addressHigher, addressLower, length, 0 };

            CalculateChecksum(packet);

            serialPort.Write(packet);

            // Loop until data becomes available. 3 bytes are expected in reply.
            Stopwatch readTimer = Stopwatch.StartNew();
            while (serialPort.BytesAvailable < length + 2 && readTimer.ElapsedMilliseconds < ReadTimeout)
            {
                Thread.Sleep(0);
            }

            byte[] received = serialPort.Read();

            if (received.Length == length + 2 && IsChecksumOk(received) && received[0] == DumpCommand)
            {

                byte[] destination = new byte[length];

                Array.Copy(received, 1, destination, 0, length);
                //Console.WriteLine(string.Format("0x{0:X4}: {1}", address, BitConverter.ToString(destination)));
                return destination;
            }
            else
            {
                if (received.Length == 2 && received[0] == 0x14)
                {
                    // Checksum error on sent packet.
                    Console.WriteLine("Checksum error reported by ECU.");
                }
                else
                {
                    // Unexpected reply.
                    Console.WriteLine(string.Format("Unexpected reply. Length {0}. Data: {1}", received.Length, BitConverter.ToString(received)));
                }

                return DumpBytes(address, length, --retryCount);
            }
        }

        public byte ReadByte(uint address)
        {
            return ReadByte(address, DefaultRetryCount);
        }

        public byte ReadByte(uint address, int retryCount)
        {
            if (retryCount < 0)
            {
                throw new System.IO.IOException("Read failed.");
            }

            byte addressHigher = (byte)(address >> 8);
            byte addressLower = (byte)(address % 256);

            byte[] packet = new byte[] { ReadCommand, addressHigher, addressLower, 0 };

            CalculateChecksum(packet);

            serialPort.Write(packet);

            // Loop until data becomes available. 3 bytes are expected in reply.
            Stopwatch readTimer = Stopwatch.StartNew();
            while (serialPort.BytesAvailable < 3 && readTimer.ElapsedMilliseconds < ReadTimeout)
            {
                Thread.Sleep(0);
            }

            byte[] received = serialPort.Read();

            if (received.Length == 3&& IsChecksumOk(received) && received[0] == ReadCommand)
            {
                // Second byte is the actual data.
                return received[1];
            }
            else
            {
                if (received.Length == 2 && received[0] == 0x14)
                {
                    // Checksum error on sent packet.
                    Console.WriteLine("Checksum error reported by ECU.");
                }
                else
                {
                    // Unexpected reply.
                    Console.WriteLine(string.Format("Unexpected reply. Length {0}. Data: {1}", received.Length, BitConverter.ToString(received)));
                }

                return ReadByte(address, --retryCount);
            }
        }

        private bool IsChecksumOk(byte[] data)
        {
            int total = 0;

            for (int i = 0; i < data.Length - 1; i++)
            {
                total += data[i];
            }

            return data[data.Length - 1] == total % 256;
        }

        private void CalculateChecksum(byte[] data)
        {
            int total = 0;

            for (int i = 0; i < data.Length - 1; i++)
            {
                total += data[i];
            }

            data[data.Length - 1] = (byte)(total % 256);
        }
    }
}
