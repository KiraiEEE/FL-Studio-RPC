using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DiscordRPC;
using DiscordRPC.Logging;

namespace MyDiscordRPCApp
{
    class DiscordRPCApplication
    {
        private DiscordRpcClient client;

        public void Initialize()
        {
            client = new DiscordRpcClient("1065812333680676906");
            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
            client.Initialize();

            var presence = new RichPresence()
            {
                Details = "Idle",
                Assets = new Assets()
                {
                    LargeImageKey = "icon",
                    LargeImageText = "FL Studio"
                }
            };
            client.SetPresence(presence);
        }

        public void UpdatePresence(string elapsedTime)
        {
            var presence = new RichPresence()
            {
                Details = elapsedTime,
                Assets = new Assets()
                {
                    LargeImageKey = "icon",
                    LargeImageText = "FL Studio"
                }
            };
            client.SetPresence(presence);
        }

        public void Dispose()
        {
            client.ClearPresence();
            client.Dispose();
        }
    }

    class Program : Form
    {
        static DiscordRPCApplication rpcApplication;
        static string currentElapsedTime = "Idle";
        static Process flStudioProcess; // Added variable to store FL Studio process

        // Configuration file path
        static string configFilePath = "config.txt";

        [STAThread] // Add STAThread attribute here
        static void Main(string[] args)
        {
            Application.Run(new Program());
        }

        public Program()
        {
            this.Load += Program_Load;
            this.FormClosed += Program_FormClosed; // Add FormClosed event handler
        }

        private void Program_Load(object sender, EventArgs e)
        {
            this.Visible = false;  // Hide the form
            this.ShowInTaskbar = false;

            // Read FL Studio path from the configuration file
            var flStudioPath = ReadFLStudioPath();
            if (string.IsNullOrEmpty(flStudioPath))
            {
                // If the FL Studio path is not present in the configuration file, ask the user for it
                flStudioPath = AskForFLStudioPath();

                if (!string.IsNullOrEmpty(flStudioPath))
                {
                    // Save the FL Studio path to the configuration file
                    SaveFLStudioPath(flStudioPath);

                    // Initialize Discord RPC and start FL Studio process
                    InitializeDiscordRPC(flStudioPath);
                }
                else
                {
                    MessageBox.Show("FL Studio path not selected. Application will now exit.");
                    this.Close();
                    return;
                }
            }
            else
            {
                // Initialize Discord RPC and start FL Studio process
                InitializeDiscordRPC(flStudioPath);
            }
        }

        private void Program_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Dispose the DiscordRPCApplication
            rpcApplication.Dispose();
        }

        static void InitializeDiscordRPC(string flStudioPath)
        {
            rpcApplication = new DiscordRPCApplication();
            rpcApplication.Initialize();

            flStudioProcess = StartFLStudio(flStudioPath);
            if (flStudioProcess != null)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                while (!flStudioProcess.HasExited)
                {
                    currentElapsedTime = stopwatch.Elapsed.ToString("hh\\:mm\\:ss");
                    rpcApplication.UpdatePresence(currentElapsedTime);

                    // Adjust the sleep interval as per your requirement
                    Thread.Sleep(1000);
                }

                stopwatch.Stop();

                // Close the application after FL Studio is closed
                Application.Exit();
            }
        }

        static Process StartFLStudio(string flStudioPath)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = flStudioPath,
                WorkingDirectory = Path.GetDirectoryName(flStudioPath),
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var flStudioProcess = Process.Start(processStartInfo);

            // Update presence when FL Studio opens
            rpcApplication.UpdatePresence(currentElapsedTime);

            return flStudioProcess;
        }

        static string AskForFLStudioPath()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select FL Studio Executable";
                openFileDialog.Filter = "FL Studio Executable|FL64.exe;FL.exe";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
            }

            return null;
        }

        static string ReadFLStudioPath()
        {
            if (File.Exists(configFilePath))
            {
                try
                {
                    return File.ReadAllText(configFilePath);
                }
                catch (Exception)
                {
                    // Handle file read error
                }
            }

            return null;
        }

        static void SaveFLStudioPath(string flStudioPath)
        {
            try
            {
                File.WriteAllText(configFilePath, flStudioPath);
            }
            catch (Exception)
            {
                // Handle file write error
            }
        }
    }
}
