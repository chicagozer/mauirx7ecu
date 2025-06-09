using System.IO.Ports;
using System.ComponentModel;
using RX7Interface;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Maui.Controls;
using System.Text; // Add this for IValueConverter

namespace MauiRX7
{
    public class ParameterDisplayValueConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Parameter? p = value as Parameter;
            if (p == null || p.DataValue == null)
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

        private string decodeDiagCodes(byte[] diagData)
        {
            var codeMap = new (int index, byte mask, string code)[]
            {
        (1, 0x20, "05"),
        (3, 0x10, "09"),
        (5, 0x80, "11"),
        (5, 0x20, "12"),
        (3, 0x80, "13"),
        (3, 0x40, "18"),
        (5, 0x40, "23"),
        (9, 0x80, "25"),
        (9, 0x40, "26"),
       (9, 0x02, "30"),
         (9, 0x08, "31"),
        (9, 0x04, "32"),
         (13, 0x02, "34"),
       (9, 0x01, "38"),
        (11, 0x80, "39"),
        (11, 0x40, "40"),
        (11, 0x20, "42"),
        (11, 0x10, "43"),
        (11, 0x08, "44"),
        (11, 0x04, "45"),
        (11, 0x02, "46"),
        (11, 0x01, "47"),
        (13, 0x40, "51"),
        (13, 0x20, "54"),
        (15, 0x80, "71"),
        (15, 0x40, "73"),
            };

            var unknownMasks = new (int index, byte mask)[]
            {
        (1,  0b11011111),
        (3,  0b00101111),
        (5,  0b00011111),
        (7,  0xFF),
        (9,  0b00110000),
        (13, 0b10011101),
        (15, 0b00111111),
            };

            bool hasUnknownCodes = false;
            var sb = new StringBuilder();

            foreach (var (index, mask, code) in codeMap)
            {
                if ((diagData[index] & mask) != 0)
                    sb.Append(code + " ");
            }

            foreach (var (index, mask) in unknownMasks)
            {
                if ((diagData[index] & mask) != 0)
                    hasUnknownCodes = true;
            }

            if (hasUnknownCodes)
                sb.Append("UNK ");

            return sb.ToString().TrimEnd();
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
                        byte[] diagData = dataStream.DumpBytes(0x0100, 16);
                        if (diagData.Length == 16)
                        {
                            viewModel.DiagCodes = decodeDiagCodes(diagData);
                        }
                        else
                        {
                            Debug.WriteLine("Dump failed, no data received.");
                            viewModel.DiagCodes = "READ FAILED";
                        }


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
                                Thread.Sleep(100);


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
