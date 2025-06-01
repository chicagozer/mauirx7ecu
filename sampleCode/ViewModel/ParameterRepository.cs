using System.Collections.ObjectModel;
using RX7Interface;
using RX7Interface.Gauges;
using System.ComponentModel;

namespace MauiRX7
{
    public class ECUID : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        string value = "UNKNOWN";
        public String Value
        {
            get { return value; }
            set { this.value = value; OnPropertyChanged(nameof(Value)); }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ParameterRepository : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string ecuId = "UNKNOWN";
        public string ECUId
        {
            get { return ecuId; }
            set { this.ecuId = value; OnPropertyChanged(nameof(ECUId)); }
        }

        private ObservableCollection<Parameter> parameter;
        public ObservableCollection<Parameter> ParameterCollection
        {
            get { return parameter; }
            set { this.parameter = value; }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ParameterRepository()
        {
            parameter = new ObservableCollection<Parameter>();
            this.GenerateParameters();
        }

        private void OnEnabledChanged(object? sender, PropertyChangedEventArgs e)
        {
              Preferences.Default.Set(((Parameter)sender!).Name, ((Parameter)sender!).Enabled);
            // This code will be executed when a property changes in _myClass
            //Console.WriteLine($"Property '{e.PropertyName}' changed in {_myClass}");
        }

        public void GenerateParameters()
        {
            parameter.Add(new Parameter("Intake Air Pressure", "mmHg", new DataValue(0x0021, ParameterLength.TwoBytes, 500d / 256, -1000), null, Preferences.Default.Get("Intake Air Pressure", true)));
            parameter.Add(new Parameter("Throttle Angle (Narrow)", "V", new DataValue(0x0023, ParameterLength.OneByte, 5d / 256, 0), new GaugeInfo(0, 5), Preferences.Default.Get("Throttle Angle (Narrow)", true)));
            parameter.Add(new Parameter("Throttle Angle (Wide)", "V", new DataValue(0x0024, ParameterLength.OneByte, 5d / 256, 0), new GaugeInfo(0, 5), Preferences.Default.Get("Throttle Angle (Wide)", true)));
            parameter.Add(new Parameter("Oxygen Sensor Voltage", "V", new DataValue(0x0025, ParameterLength.OneByte, 5d / 256, 0), new GaugeInfo(0, 5), Preferences.Default.Get("Oxygen Sensor Voltage", true)));
            parameter.Add(new Parameter("MOP Position", "V", new DataValue(0x0026, ParameterLength.OneByte, 5d / 256, 0), null, Preferences.Default.Get("MOP Position", true)));
            parameter.Add(new Parameter("Battery Voltage", "V", new DataValue(0x0027, ParameterLength.OneByte, 20d / 256, 0), new GaugeInfo(8, 20, 11, 15), Preferences.Default.Get("Battery Voltage", true)));
            parameter.Add(new Parameter("Water Temperature", "°C", new DataValue(0x0028, ParameterLength.OneByte, 160d / 256, -40), new GaugeInfo(0, 140, 80, 100), Preferences.Default.Get("Water Temperature", true)));
            parameter.Add(new Parameter("Fuel Temperature", "°C", new DataValue(0x0029, ParameterLength.OneByte, 160d / 256, 0), null, Preferences.Default.Get("Fuel Temperature", true)));
            parameter.Add(new Parameter("Intake Air Temperature", "°C", new DataValue(0x002a, ParameterLength.OneByte, 160d / 256, -40), null, Preferences.Default.Get("Intake Air Temperature", true)));
            parameter.Add(new Parameter("Engine Speed", "rpm", new DataValue(0x002c, ParameterLength.TwoBytes, 500d / 256, 0), new GaugeInfo(0, 9000), Preferences.Default.Get("Engine Speed", true)));
            parameter.Add(new Parameter("Vehicle Speed", "km/h", new DataValue(0x002e, ParameterLength.OneByte, 356d / 256, 0), new GaugeInfo(0, 320), Preferences.Default.Get("Vehicle Speed", true)));
            parameter.Add(new Parameter("Injector On Time", "ms", new DataValue(0x0800, ParameterLength.TwoBytes, 1d / 256, 0), null, Preferences.Default.Get("Injector On Time", true)));
            parameter.Add(new Parameter("Injector Period", "ms", new DataValue(0x0802, ParameterLength.TwoBytes, 1d / 256, 0), null, Preferences.Default.Get("Injector Period", true)));
            parameter.Add(new Parameter("Ignition Advance (Leading)", "degrees", new DataValue(0x0804, ParameterLength.OneByte, 90d / 256, -25), null, Preferences.Default.Get("Ignition Advance (Leading)", true)));
            parameter.Add(new Parameter("Ignition Advance (Trailling)", "degrees", new DataValue(0x0805, ParameterLength.OneByte, 90d / 256, -25), null, Preferences.Default.Get("Ignition Advance (Trailling)", true)));
            parameter.Add(new Parameter("Idle Speed Control", "%", new DataValue(0x0806, ParameterLength.TwoBytes, 800d / 256, 0), null, Preferences.Default.Get("Idle Speed Control", true)));
            parameter.Add(new Parameter("Unknown 1", "?", new DataValue(0x0808, ParameterLength.OneByte, 1, 0), null, Preferences.Default.Get("Unknown 1", true)));
            parameter.Add(new Parameter("Turbo Pre-Control", "%", new DataValue(0x0809, ParameterLength.OneByte, 100d / 256, 0), null, Preferences.Default.Get("Turbo Pre-Control", true)));
            parameter.Add(new Parameter("Wastegate Control", "%", new DataValue(0x080a, ParameterLength.OneByte, 100d / 256, 0), null, Preferences.Default.Get("Wastegate Control", true)));


            parameter.Add(new Parameter("Unknown 2", "?", new DataValue(0x080b, ParameterLength.OneByte, 1, 0), null, Preferences.Default.Get("Unknown 2", true)));
            parameter.Add(new Parameter("Unknown 3", "?", new DataValue(0x080c, ParameterLength.OneByte, 1, 0), null, Preferences.Default.Get("Unknown 3", true)));
            parameter.Add(new Parameter("Unknown 4", "?", new DataValue(0x080d, ParameterLength.OneByte, 1, 0), null, Preferences.Default.Get("Unknown 4", true)));
            parameter.Add(new Parameter("Unknown 5", "?", new DataValue(0x080e, ParameterLength.OneByte, 1, 0), null, Preferences.Default.Get("Unknown 5", true)));
            parameter.Add(new Parameter("Unknown 6", "?", new DataValue(0x080f, ParameterLength.OneByte, 1, 0), null, Preferences.Default.Get("Unknown 6", true)));
            parameter.Add(new Parameter("Unknown 7", "?", new DataValue(0x0810, ParameterLength.OneByte, 1, 0), null, Preferences.Default.Get("Unknown 7", true)));
            parameter.Add(new Parameter("Unknown 8", "?", new DataValue(0x0811, ParameterLength.OneByte, 1, 0), null, Preferences.Default.Get("Unknown 8", true)));
            parameter.Add(new Parameter("Unknown 9", "?", new DataValue(0x0812, ParameterLength.OneByte, 1, 0), null, Preferences.Default.Get("Unknown 9", true)));
            parameter.Add(new Parameter("Unknown 10", "?", new DataValue(0x0813, ParameterLength.OneByte, 1, 0), null, Preferences.Default.Get("Unknown 10", true)));

            foreach (Parameter p in parameter)
            {
                p.PropertyChanged += OnEnabledChanged;
            }

        }


    }
}
