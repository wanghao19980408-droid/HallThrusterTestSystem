using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NationalInstruments.DAQmx;

namespace HallThrusterTestSystem
{
    public partial class MainForm : Form
    {
        private ReadExcel readExcel;
        private DigitalControl digitalControl;

        private List<AnalogInputReader> deviceReaders;

        private string ChannelConfigPath = "ChannelConfig.xlsx";

        public MainForm()
        {
            InitializeComponent();

            readExcel = new ReadExcel();
            digitalControl = new DigitalControl();
            deviceReaders = new List<AnalogInputReader>();

            try
            {
                readExcel.ReadDeviceExcel(ChannelConfigPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取 Excel 失败: {ex.Message}");
                return;
            }

            foreach (var sheet in readExcel.SheetData)
            {
                string deviceName = sheet.Key;
                List<Dictionary<string, object>> rows = sheet.Value;

                if (rows == null || rows.Count == 0) continue;

                List<ChannelParam> currentDeviceChannels = new List<ChannelParam>();
                double deviceRate = 1000.0;
                foreach (var row in rows)
                {
                    try
                    {
                        if (!row.ContainsKey("Channel") || row["Channel"] == null) continue;

                        string channel = row["Channel"].ToString();
                        string typeStr = row.ContainsKey("Type") && row["Type"] != null ? row["Type"].ToString() : "";
                        AITerminalConfiguration atc = typeStr.Contains("单端") ?
                            AITerminalConfiguration.Rse : AITerminalConfiguration.Differential;

                        if (row.ContainsKey("rate") && row["rate"] != null)
                        {
                            double.TryParse(row["rate"].ToString(), out deviceRate);
                        }

                        double rawMin = row.ContainsKey("Min") ? Convert.ToDouble(row["Min"]) : 0;
                        double rawMax = row.ContainsKey("Max") ? Convert.ToDouble(row["Max"]) : 5.0;

                        double safeMin = Math.Min(rawMin, rawMax);
                        double safeMax = Math.Max(rawMin, rawMax);

                        if (safeMin == safeMax) safeMax = safeMin + 1.0;

                        currentDeviceChannels.Add(new ChannelParam
                        {
                            PhysicalName = $"{deviceName}/{channel}",
                            MinVolts = safeMin,
                            MaxVolts = safeMax,
                            TerminalConfig = atc
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"解析 {deviceName} 通道参数时出错: {ex.Message}");
                    }
                }

                if (currentDeviceChannels.Count > 0)
                {
                    AnalogInputReader reader = new AnalogInputReader();

                    reader.DataAcquired += Reader_DataAcquired;
                    reader.HardwareError += (ex) =>
                    {
                        this.BeginInvoke(new Action(() => {
                            MessageBox.Show($"设备 [{deviceName}] 发生异常: {ex.Message}");
                        }));
                    };

                    var res = reader.StartContinuousReading(currentDeviceChannels, deviceRate, 100);
                    if (res.IsSuccess)
                    {
                        deviceReaders.Add(reader); 
                    }
                    else
                    {
                        MessageBox.Show($"设备 [{deviceName}] 启动采集失败: {res.Message}");
                    }
                }
            }
        }

        private void Reader_DataAcquired(NationalInstruments.AnalogWaveform<double>[] multiChannelData)
        {
            if (!this.IsHandleCreated) return;
            this.BeginInvoke(new Action(() =>
            {
                for (int i = 0; i < multiChannelData.Length; i++)
                {
                    var waveform = multiChannelData[i];

                    if (waveform.Samples.Count == 0) continue;

                    double averageValue = waveform.Samples.Average(s => s.Value);
                    Label targetLabel = FindLabelByTag(this, waveform.ChannelName);

                    if (targetLabel != null)
                    {
                        targetLabel.Text = averageValue.ToString("F3");
                    }
                }
            }));
        }

        private Label FindLabelByTag(Control parent, string tagValue)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Label lbl && lbl.Tag != null && lbl.Tag.ToString() == tagValue)
                {
                    return lbl;
                }

                if (c.HasChildren)
                {
                    Label found = FindLabelByTag(c, tagValue);
                    if (found != null) return found;
                }
            }
            return null; 
        }
    }
}