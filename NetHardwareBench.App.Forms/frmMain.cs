using NetHardwareBench.Model.Modules.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetHardwareBench.App.Forms
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            this.lblCPUDescription.Text = Hardware.HardwareInfo.GetCPUDescription();
            this.lblRamDescription.Text = Hardware.HardwareInfo.GetRamDescription();
            this.lblHardDiskDiscription.Text = Hardware.HardwareInfo.GetHDDDescription();

            this.lblGPUDescription.Text = Hardware.HardwareInfo.GetGPUDescription();


            this.lblInternetDescription.Text = Hardware.HardwareInfo.GetNetworDescription();
            this.lblNetworkDescription.Text = Hardware.HardwareInfo.GetNetworDescription();
            this.lblNetworkDescription.SelectionStart = 0;
            this.lblInternetDescription.SelectionLength = 0;
            pnlBenchmarkButton.Focus();
            ShowHidePictureResult(false);

        }

        private void ShowHidePictureResult(bool Show)
        {
            this.pbCPUResult.Visible = Show;
            this.pbRamResult.Visible = Show;
            this.pbHDDResult.Visible = Show;
            this.pbNetworkResult.Visible = Show;
            this.pbInternetResult.Visible = Show;
            this.pbGPUResult.Visible = Show;
        }


        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void lblHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by Rodrigo Ramos. Email: rodrigoramosrs2@hotmail.com");
        }

        private void pnlBenchmarkButton_MouseClick(object sender, MouseEventArgs e)
        {
            StartStopBenchmark(true);

            this.bwBenchmark.RunWorkerAsync();
        }

        private void StartStopBenchmark(bool start)
        {
            if (start)
            {
                pnlBenchmarkButton.Enabled = false;
                ShowHidePictureResult(false);
                this.pnlLoading.Show();
            }
            else
            {
                pnlBenchmarkButton.Enabled = true;
                ShowHidePictureResult(true);
                this.pnlLoading.Hide();
            }
        }

        private Image GetPictureFromBenchmarkStatus(string score)
        {
            if (score == "0")
                return Properties.Resources.icons8_cancel_48;
            if (score == "25")
                return Properties.Resources.icons8_cancel_48;
            if (score == "50")
                return NetHardwareBench.App.Forms.Properties.Resources.icons8_box_important_48;
            if (score == "75")
                return NetHardwareBench.App.Forms.Properties.Resources.icons8_box_important_48;
            if (score == "100")
                return NetHardwareBench.App.Forms.Properties.Resources.icons8_ok_48;

            return Properties.Resources.icons8_minus_48; ;
        }

        private void bwBenchmark_DoWork(object sender, DoWorkEventArgs e)
        {
            
            NetHardwareBench.Benchmark.Main BenchmarkCore = new Benchmark.Main(new Model.Modules.Parameters.ConfigurationParameters()
            {
                NetworkPath = "",
                RemoteEndpoint = "",
                RemoteIP = "",
                WorkDirectory = ""
            });

            BenchmarkCore.OnProgressChanged += (double progress) =>
            {
                lblProgressDescription.Text = $"{Math.Round(progress,0)}%";
                this.pbBenchmarkProgress.Value = Convert.ToInt32(progress);
            };

            try
            {
                var benchmarkResult = BenchmarkCore
                   .DoBenchmark(
                        BenchmarkType.RAM,
                        BenchmarkType.CPU,
                        BenchmarkType.NETWORK,
                        BenchmarkType.INTERNET,
                        //BenchmarkType.GPU,
                        BenchmarkType.LOCAL_STORAGE);

                StringBuilder builder = new StringBuilder();
                

                foreach (var item in benchmarkResult)
                {
                   string calculatedScore =  Utils.ScoreParser.Calculate(item.BenchmarkType, item.Score);

                    Image imageScore = GetPictureFromBenchmarkStatus(calculatedScore);

                    builder.AppendLine($"[{item.BenchmarkType.ToString()}]\r\n|- AVG: " + item.Score + " | Score: " + calculatedScore + " %");

                    switch (item.BenchmarkType)
                    {
                        case BenchmarkType.CPU:
                            pbCPUResult.Image = imageScore;
                            break;
                        case BenchmarkType.GPU:
                            pbGPUResult.Image = imageScore;
                            break;
                        case BenchmarkType.INTERNET:
                            pbInternetResult.Image = imageScore;
                            break;
                        case BenchmarkType.NETWORK:
                            pbNetworkResult.Image = imageScore;
                            break;
                        case BenchmarkType.RAM:
                            pbRamResult.Image = imageScore;
                            break;
                        case BenchmarkType.LOCAL_STORAGE:
                            pbHDDResult.Image = imageScore;
                            break;
                        case BenchmarkType.NETWORK_STORAGE:
                            pbNetworkResult.Image = imageScore;
                            break;
                    };



                   

                    switch (item.BenchmarkType)
                    {

                        case BenchmarkType.INTERNET:
                        case BenchmarkType.LOCAL_STORAGE:

                            foreach (var partialresult in item.PartialResults)
                            {
                                builder.AppendLine($" |- {partialresult.Description}: " + partialresult.Score + " | Score: " +
                                                   Utils.ScoreParser.Calculate(item.BenchmarkType, partialresult.Score) + " %");
                            }
                            break;
                    }
                }

                string JSONResult = JsonConvert.SerializeObject(benchmarkResult, Formatting.Indented);
                File.WriteAllText(Environment.CurrentDirectory + "\\benchmark_result_resumed.json", builder.ToString());
                File.WriteAllText(Environment.CurrentDirectory + "\\benchmark_result_detailed.json", JSONResult);
                
                System.Diagnostics.Process.Start(Environment.CurrentDirectory + "\\benchmark_result_resumed.json");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
           
        }


        private void bwBenchmark_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.StartStopBenchmark(false);
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by Rodrigo Ramos. Email: rodrigoramosrs2@hotmail.com");
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            
            this.lblInternetDescription.SelectionLength = 0;
            this.lblNetworkDescription.SelectionLength = 0;
            this.Focus();
        }

        private void lblFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
