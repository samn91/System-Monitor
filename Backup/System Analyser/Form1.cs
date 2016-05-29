using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Management;
namespace System_Analyser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;
        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 10; i <= 100; i += 10)
                comboBox1.Items.Add(i);
            comboBox1.SelectedIndex = 7;
            cpuCounter = new PerformanceCounter();

            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            Thread tcm = new Thread(GetAll);
            tcm.IsBackground = true;
            tcm.Start();

            progressBar1.Maximum = 100;
            progressBar2.Maximum = 6000;


        }
        public void GetAll()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            int cpu;
            int memory;
            while (true)
            {
                cpu = (int)getCurrentCpuUsage();
                memory = (int)getAvailableRAM();
                progressBar1.Value = cpu;
                progressBar2.Value = memory;
                label1.Text = cpu + "%";
                label2.Text = memory + "MB";
                if (checkBox1.Checked)
                    label5.Text = DateTime.Now.TimeOfDay.ToString();
                else
                    label5.Text = DateTime.Now.ToString();
                getBattreylevel();
                Thread.Sleep(1000);

            }
        }
        public float getCurrentCpuUsage()
        {
            return cpuCounter.NextValue();
        }

        public float getAvailableRAM()
        {
            return ramCounter.NextValue();
        }


        public void getBattreylevel()
        {
            PowerStatus power = SystemInformation.PowerStatus;
            float battreylevel = power.BatteryLifePercent * 100;
            TimeSpan ts = TimeSpan.FromSeconds(power.BatteryLifeRemaining);
            progressBar3.Maximum = 100;
            progressBar3.Value = (int)battreylevel;

            if (battreylevel >= 0)
            {
                label3.Text = battreylevel.ToString() + "%";
            }
            else
            {
                label3.Text = string.Empty;
            }
            if (power.PowerLineStatus == PowerLineStatus.Offline)
                progressBar3.ForeColor = Color.Red;
            else
                progressBar3.ForeColor = Color.Green;
            if (ts.Seconds > 0)
                label4.Text = ts.Hours.ToString() + "h:" + ts.Seconds + "m";
            else
                label4.Text = string.Empty;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                Size = new Size(60, Size.Height);
            else
                Size = new Size(280, Size.Height);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Opacity = (double)((int)comboBox1.SelectedItem) / 100;
        }
    }

}