using System.IO.Ports;
using System.ComponentModel;
using RX7Interface;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Maui.Controls; // Add this for IValueConverter

namespace MauiRX7
{
    public class ParameterDisplayValueConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Parameter? p = value as Parameter;
            if (p == null || p.DataValue == null )
            {
                return "";
            }
           
            switch (p.Units)
            {
                case "Hex":
                    return "0x" + ((int)(p.DataValue.Value)).ToString("X4");
               
                default:
                    return p.DataValue.Value.ToString("F2");
            }
        }



        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public partial class MainPage : ContentPage
    {
        private ParameterRepository viewModel = new ParameterRepository();

        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        private IDataStream? dataStream;
        public List<string> ports = new List<string>();

        void OnCheckBoxCheckedChanged(object? sender, CheckedChangedEventArgs e)
        {
            //Preferences.Default.Set(nameof(simulatorCheckBox), e.Value);
            if (e.Value)
            {
                PortPicker.IsEnabled = false;
                backgroundWorker.RunWorkerAsync();
            }
            else
            {
                if (backgroundWorker.IsBusy)
                {
                    backgroundWorker.CancelAsync();
                }
                PortPicker.IsEnabled = true;
            }

        }
        private void PopulatePortPicker()
        {
            foreach (var port in ports)
            {
                PortPicker.Items.Add(port);
            }
        }

        void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1 && (dataStream == null || !dataStream.IsOpen))
            {
                simulatorCheckBox.IsEnabled = false;





                backgroundWorker.RunWorkerAsync();
            }


        }

        private void LoadPorts()
        {

            ports = new List<string>(SerialPort.GetPortNames());


        }
        private void backgroundWorker_RunWorkerCompleted(object? sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            dataStream?.Close();
        }
        private void backgroundWorker_DoWork(object? sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                if (simulatorCheckBox.IsEnabled)
                {
                    dataStream = new DummyDataStream();
                }
                else
                {

                    dataStream = new Rx7DataStream(PortPicker?.SelectedItem?.ToString() ?? "NONE");


                }
                if (dataStream == null)
                {
                    return;
                }
                dataStream.Open();
                viewModel.ECUId = dataStream!.ReadECUId();
                while (!e.Cancel)
                {
                    try
                    {


                        foreach (Parameter p in viewModel.ParameterCollection)
                        {
                            if (p.Enabled)
                            {
                                foreach (uint address in p.DataValue.Addresses)
                                {
                                    byte received = dataStream.ReadByte(address);
                                    p.DataValue.SetRawValue(address, received);
                                    p.DataValue.UpdateTime = DateTime.Now;
                                }


                            }
                        }

                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in background worker: {ex.Message}");
                dataStream?.Close();
            }
        }

        public MainPage()
        {

            InitializeComponent();
            LoadPorts();
            PopulatePortPicker();


            //simulatorCheckBox.IsChecked = Preferences.Default.Get(nameof(simulatorCheckBox), false);

            BindingContext = viewModel;
            backgroundWorker.WorkerReportsProgress = false;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            //backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
        }


    }

}
