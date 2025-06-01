using RX7Interface.Gauges;
using System.ComponentModel;

namespace RX7Interface
{
   
    public class Parameter : INotifyPropertyChanged
    {
        private bool enabled = true;
        private  DataValue dataValue;
        public event PropertyChangedEventHandler? PropertyChanged;

        public  DataValue DataValue
        {
            get => dataValue;
            set
            {
                dataValue = value;
                OnPropertyChanged(nameof(DataValue));
            }
        }
        protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                 OnPropertyChanged(nameof(Enabled));
                //Preferences.Default.Set(Name, value);
            }
        }

        public GaugeInfo? GaugeInfo
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Units
        {
            get;
            private set;
        }
        /*
                public double Value
                {
                    get
                    {
                        return DataValue.Value;
                    }

                }*/

        public Parameter(string name, string units, DataValue dataValue, GaugeInfo? info, bool? enabled = true)
        {



            this.Name = name;
            this.Units = units;

            this.dataValue = dataValue;
            GaugeInfo = info;
            Enabled = enabled ?? true;
             
        }
    }
}
