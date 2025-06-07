// See https://aka.ms/new-console-template for more information

using System.ComponentModel;  // for BackgroundWorker
using Console = System.Console;
using RX7Interface.Gauges;
using System.Text;
using System.Diagnostics;
namespace RX7Interface
{


    class Program
    {

        public static string AppName { get; set; } = "My .NET Byte Dump Program";
        public static string AppVersion { get; set; } = "1.0.0";


        static string NL = Environment.NewLine; // shortcut
        static string NORMAL = Console.IsOutputRedirected ? "" : "\x1b[39m";
        static string RED = Console.IsOutputRedirected ? "" : "\x1b[91m";
        static string GREEN = Console.IsOutputRedirected ? "" : "\x1b[92m";
        static string YELLOW = Console.IsOutputRedirected ? "" : "\x1b[93m";
        static string BLUE = Console.IsOutputRedirected ? "" : "\x1b[94m";
        static string MAGENTA = Console.IsOutputRedirected ? "" : "\x1b[95m";
        static string CYAN = Console.IsOutputRedirected ? "" : "\x1b[96m";
        static string GREY = Console.IsOutputRedirected ? "" : "\x1b[97m";
        static string BOLD = Console.IsOutputRedirected ? "" : "\x1b[1m";
        static string NOBOLD = Console.IsOutputRedirected ? "" : "\x1b[22m";
        static string UNDERLINE = Console.IsOutputRedirected ? "" : "\x1b[4m";
        static string NOUNDERLINE = Console.IsOutputRedirected ? "" : "\x1b[24m";
        static string REVERSE = Console.IsOutputRedirected ? "" : "\x1b[7m";
        static string NOREVERSE = Console.IsOutputRedirected ? "" : "\x1b[27m";


        static void Main(string[] args)
        {
            IDataStream dataStream;





            if (args.Length > 0)
            {
                dataStream = new Rx7DataStream(args[0]);
            }
            else
            {
                dataStream = new DummyDataStream();
            }
            //

            dataStream.Open();





            BackgroundWorker worker = new();
            worker.DoWork += worker_DoWork!;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted!;
            //worker.ProgressChanged += worker_ProgressChanged!;
            worker.WorkerReportsProgress = false;
            worker.WorkerSupportsCancellation = true;

            Console.WriteLine("Starting worker... (any key to cancel/exit)");

            worker.RunWorkerAsync(argument: dataStream);

            Console.Read();  // event loop

            if (worker.IsBusy)
            {
                Console.WriteLine("Interrupting the worker...");
                worker.CancelAsync();
                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (worker.IsBusy && sw.ElapsedMilliseconds < 5000)
                    Thread.Sleep(1);
            }

            dataStream.Close();

        }








        static void worker_RunWorkerCompleted(object _, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Console.WriteLine("Worker: I was busy!");
                return;
            }

            Console.WriteLine("Worker: I worked {0:D} times.", e.Result);
            Console.WriteLine("Worker: Done now!");
        }

        public static void PrintColoredHexDiff(string hexStr1, string hexStr2, System.ConsoleColor color)
        {
            string[] hexBytes1 = hexStr1.Split(' ');
            string[] hexBytes2 = hexStr2.Split(' ');

            int minLength = Math.Min(hexBytes1.Length, hexBytes2.Length);

            for (int i = 0; i < minLength; i++)
            {
                if (hexBytes1[i] != hexBytes2[i])
                {
                    Console.ForegroundColor = color;
                    Console.Write(hexBytes2[i] + " ");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(hexBytes2[i] + " ");
                }
            }

            // Print remaining hex bytes of the longer string with red color.
            if (hexBytes2.Length > minLength)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                for (int i = minLength; i < hexBytes2.Length; i++)
                {
                    Console.Write(hexBytes2[i] + " ");
                }
                Console.ResetColor();
            }

            Console.WriteLine();
        }
        static void worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker? worker = sender as BackgroundWorker;

            IDataStream dataStream = (IDataStream)e.Argument!;

            Console.WriteLine("ECU ID: {0}", dataStream.ReadECUId());
            String[] lastRead = new String[96];
            //string[] lastSaved = File.ReadAllLines("saved.txt");

            while (!e.Cancel)
            {
                int index = 0;
                for (uint address = 0x0100; address < 0x0200; address += 16)
                {

                    byte[] data = dataStream.DumpBytes(address, 16);
                    if (data.Length == 0)
                    {
                        Console.WriteLine("Dump failed at address {0:X4}", address);
                        break;
                    }
                    Console.Write("Address {0:X4}: ", address);
                    String latest = BitConverter.ToString(data).Replace("-", " ");
                    PrintColoredHexDiff(lastRead[index] ?? "", latest, ConsoleColor.Red);
                    //Console.Write("Address {0:X4}: ", address);
                    //PrintColoredHexDiff(lastSaved[index] ?? "", latest, ConsoleColor.Green);
                    lastRead[index] = latest;
                    index++;
                }
                for (uint address = 0x0800; address < 0x0800; address += 16)
                {
                    byte[] data = dataStream.DumpBytes(address, 16);
                    if (data.Length == 0)
                    {
                        Console.WriteLine("Dump failed at address {0:X4}", address);
                        break;
                    }
                    Console.Write("Address {0:X4}: ", address);
                    String latest = BitConverter.ToString(data).Replace("-", " ");
                    PrintColoredHexDiff(lastRead[index] ?? "", latest, ConsoleColor.Red);
                    //Console.Write("Address {0:X4}: ", address);
                    //PrintColoredHexDiff(lastSaved[index] ?? "", latest, ConsoleColor.Green);
                    lastRead[index] = latest;
                    index++;
                }

      /*          List<uint> singles = [0x0880, 0x09A0, 0x0A30];
                foreach (uint address in singles)
                {
                    byte[] data = dataStream.DumpBytes(address, 16);
                    if (data.Length == 0)
                    {
                        Console.WriteLine("Dump failed at address {0:X4}", address);
                        break;
                    }
                    Console.Write("Address {0:X4}: ", address);
                    String latest = BitConverter.ToString(data).Replace("-", " ");
                    PrintColoredHexDiff(lastRead[index] ?? "", latest);
                    lastRead[index] = latest;
                     index++;
                }*/
                Console.WriteLine("-----------------------");
                //File.WriteAllLines("saved.txt", lastRead);

                Thread.Sleep(1000);


            }
        }
    }

}