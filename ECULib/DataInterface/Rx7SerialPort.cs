
using System.IO.Ports;
using System.Timers;

namespace RX7Interface
{
    class Rx7SerialPort(string port)
    {
        private const int BaudRate = 976; // 968 is good 
        private System.Timers.Timer? aTimer = null;

        private SerialPort serialPort = new SerialPort(port, BaudRate, Parity.None, 8, StopBits.One);

        private byte[]? lastSentPacket;
        private int loopbackPosition;

        private List<byte> receiveBuffer = new List<byte>();


        /// <summary>
        /// Gets the number of available bytes.
        /// </summary>
        public int BytesAvailable
        {
            get
            {
                ProcessReceiveBuffer();
                return receiveBuffer.Count;
            }
        }

        /// <summary>
        /// Transmits data to the ECU.
        /// </summary>
        /// <param name="data"></param>
        public void Write(byte[] data)
        {
            // Clear any old received data, as the loopback will screw the data up.
            serialPort.DiscardInBuffer();

            lastSentPacket = data;
            loopbackPosition = 0;
            serialPort.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Gets all available data that was received, with and loopback
        /// data excluded.
        /// </summary>
        /// <returns></returns>
        public byte[] Read()
        {
            ProcessReceiveBuffer();

            byte[] data = receiveBuffer.ToArray();

            receiveBuffer.Clear();

            return data;
        }

        public void Open()
        {
            serialPort.Open();
            StartMultimediaTimer();
        }

        public void Close()
        {
            StopMultimediaTimer();
            serialPort.Close();
        }

        public bool IsOpen
        {
            get
            {
                return serialPort.IsOpen;
            }
        }

        private void ProcessReceiveBuffer()
        {
            if (serialPort.BytesToRead > 0)
            {
                if (lastSentPacket != null)
                {
                    // Remove the loopback bytes;
                    while (loopbackPosition < lastSentPacket.Length && serialPort.BytesToRead > 0)
                    {
                        int byteRead = serialPort.ReadByte();
                        if (lastSentPacket[loopbackPosition] != byteRead)
                        {
                            // Loopback data does not match.
                            Console.WriteLine("Loopback packet doesn't match.");
                            lastSentPacket = null;
                            break;
                        }

                        loopbackPosition++;
                    }
                }

                while (serialPort.BytesToRead > 0)
                {
                    receiveBuffer.Add((byte)serialPort.ReadByte());
                }
            }
        }
        private void OnTimedEvent(Object? source, ElapsedEventArgs e)
        {
            serialPort.DtrEnable = !serialPort.DtrEnable;
            serialPort.RtsEnable = !serialPort.RtsEnable;
        }

        private void StartMultimediaTimer()

        {

            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(25);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;


        }

        private void StopMultimediaTimer()
        {
            if (aTimer != null)
            {
                aTimer.Stop();
                aTimer.Dispose();
            }


        }
    }

}

