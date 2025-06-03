using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace RX7Interface
{
    public enum ParameterLength
    {
        OneByte,
        TwoBytes
    }

    public class DataValue : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private uint baseAddress;
        private DateTime updateTime;

        public DateTime UpdateTime
        {
            get => updateTime;

            set
            {
                updateTime = value;
                OnPropertyChanged(nameof(LastUpdated));
            }
        }

        public double? LastUpdated
        {
            get
            {
                if (updateTime == DateTime.MinValue)
                {
                    return null;
                }
                TimeSpan timeDifference = DateTime.Now - updateTime;
                return timeDifference.TotalSeconds;
            }

        }

        public uint[] Addresses
        {
            get;
            private set;
        }

        private byte[] rawValues;

        private double offset;
        private double conversion;

        public DataValue(uint address, ParameterLength length, double conversion, double offset)
        {
            UpdateTime = DateTime.MinValue;

            baseAddress = address;

            uint byteCount = 0;
            if (length == ParameterLength.OneByte)
            {
                byteCount = 1;
            }
            else if (length == ParameterLength.TwoBytes)
            {
                byteCount = 2;
            }

            Addresses = new uint[byteCount];
            rawValues = new byte[byteCount];

            for (int i = 0; i < byteCount; i++)
            {
                Addresses[i] = baseAddress + (uint)i;
            }

            this.conversion = conversion;
            this.offset = offset;
        }


        public String Raw
        {
            get
            {
                if (rawValues.Length == 0)
                {
                    return "0X00";
                }
                return "0X" + Convert.ToHexString(rawValues);
            }
        }

        public void SetRawValue(uint address, byte value)
        {
            rawValues[address - baseAddress] = value;
            OnPropertyChanged(nameof(Raw));
            OnPropertyChanged(nameof(Value));
        }
        public Double Value
        {
            get
            {
                ulong rawValue = 0;

                foreach (byte b in rawValues)
                {
                    rawValue <<= 8;
                    rawValue += b;
                }

                return ((Double)rawValue * conversion) + offset;
            }

        }

    }
}
