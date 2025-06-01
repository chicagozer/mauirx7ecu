using System;
using System.Collections.Generic;
using System.Text;

namespace RX7Interface
{
    public interface IDataStream
    {
         byte[] DumpBytes(uint address, byte length);

        byte[] DumpBytes(uint address, byte length, int retryCount);

        public string ReadECUId();
        void Open();

        bool IsOpen
        {
            get;
        }

        void Close();

        byte ReadByte(uint address);

        byte ReadByte(uint address, int retryCount);
    }
}
