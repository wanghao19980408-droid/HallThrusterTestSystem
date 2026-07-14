using NationalInstruments.DAQmx;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HallThrusterTestSystem
{
    public partial class MainForm : Form
    {
        private ConcurrentDictionary<string, double> latestDataPool = new ConcurrentDictionary<string, double>();


        private Timer uiRefreshTimer = new Timer();
        private ReadExcel readExcel;
        private DigitalControl digitalControl;
        private List<AnalogInputReader> deviceReaders;
        private string ChannelConfigPath = "ChannelConfig.xlsx";

        public MainForm()
        {
            InitializeComponent();
            InitializeHardwareConfig();

            uiRefreshTimer.Interval = 100;
            uiRefreshTimer.Tick += (s, e) => UpdateUI();
            uiRefreshTimer.Start();
        }

        private void InitializeHardwareConfig()
        {
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

                double maxDeviceRate = 0.0;

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
                            if (double.TryParse(row["rate"].ToString(), out double currentRate))
                            {
                                maxDeviceRate = Math.Max(maxDeviceRate, currentRate);
                            }
                        }

                        double rawMin = row.ContainsKey("Min") ? Convert.ToDouble(row["Min"]) : 0;
                        double rawMax = row.ContainsKey("Max") ? Convert.ToDouble(row["Max"]) : 5.0;
                        double safeMin = Math.Min(rawMin, rawMax);
                        double safeMax = Math.Max(rawMin, rawMax);

                        var chParam = new ChannelParam
                        {
                            PhysicalName = $"{deviceName}/{channel}",
                            MinVolts = safeMin,
                            MaxVolts = safeMax,
                            TerminalConfig = atc,
                        };

                        currentDeviceChannels.Add(chParam);
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
                        this.BeginInvoke(new Action(() => MessageBox.Show($"设备 [{deviceName}] 发生异常: {ex.Message}")));
                    };


                    if (maxDeviceRate <= 0) maxDeviceRate = 1000.0;

                    double targetControlHz = 10.0;

                    int dynamicSamples = (int)(maxDeviceRate / targetControlHz);

                    if (dynamicSamples < 1) dynamicSamples = 1;

                    var res = reader.StartContinuousReading(currentDeviceChannels, maxDeviceRate, dynamicSamples);

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
            foreach (var waveform in multiChannelData)
            {
                if (waveform.Samples.Count == 0) continue;

                string name = waveform.ChannelName;

                double avg = waveform.Samples.Last().Value;
                latestDataPool[name] = avg;

            }
        }
        private void UpdateUI()
        {
            foreach (var kvp in latestDataPool)
            {
                Label lbl = FindLabelByTag(this, kvp.Key);
                if (lbl != null)
                {
                    lbl.Text = kvp.Value.ToString("F3");

                }
            }
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