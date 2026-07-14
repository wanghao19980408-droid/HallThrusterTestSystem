using NationalInstruments;
using NationalInstruments.DAQmx;
using System;
using System.Collections.Generic;
using DataConvertLib;

namespace HallThrusterTestSystem
{

    public class AnalogInputReader : IDisposable
    {
        private Task AiTask;
        private AnalogMultiChannelReader Reader;
        private AsyncCallback AnalogCallback;
        private Task RunningTask;

        public event Action<AnalogWaveform<double>[]> DataAcquired;
        public event Action<Exception> HardwareError;

        public OperateResult StartContinuousReading(List<ChannelParam> channels, double rate = 1000.0, int samplesPerBuffer = 100)
        {
            if (RunningTask != null) return OperateResult.CreateFailResult("该设备已在进行连续采集");
            if (channels == null || channels.Count == 0) return OperateResult.CreateFailResult("没有提供任何通道配置");

            try
            {
                AiTask = new Task();
                foreach (var ch in channels)
                {
                    AiTask.AIChannels.CreateVoltageChannel(
                        ch.PhysicalName,
                        ch.PhysicalName,
                        ch.TerminalConfig,
                        ch.MinVolts,
                        ch.MaxVolts,
                        AIVoltageUnits.Volts);

                }

                int bufferSize = (int)(rate * 5.0);
                if (bufferSize < samplesPerBuffer * 2) bufferSize = samplesPerBuffer * 2;

                AiTask.Timing.ConfigureSampleClock(
                    "",
                    rate,
                    SampleClockActiveEdge.Rising,
                    SampleQuantityMode.ContinuousSamples,
                    bufferSize);

                AiTask.Control(TaskAction.Verify);
                RunningTask = AiTask;

                Reader = new AnalogMultiChannelReader(AiTask.Stream);
                AnalogCallback = new AsyncCallback(AnalogInCallback);
                Reader.SynchronizeCallbacks = false; 

                Reader.BeginReadWaveform(samplesPerBuffer, AnalogCallback, AiTask);
                return OperateResult.CreateSuccessResult();
            }
            catch (DaqException ex)
            {
                StopReading();
                return OperateResult.CreateFailResult($"启动采集失败，详细信息: {ex.Message}");
            }
        }

        private void AnalogInCallback(IAsyncResult ar)
        {
            try
            {
                if (RunningTask != null && RunningTask == ar.AsyncState)
                {
                    AnalogWaveform<double>[] data = Reader.EndReadWaveform(ar);

                    DataAcquired?.Invoke(data);

                    Reader.BeginMemoryOptimizedReadWaveform(data[0].Samples.Count, AnalogCallback, ar.AsyncState, data);
                }
            }
            catch (DaqException ex)
            {
                StopReading();
                HardwareError?.Invoke(new Exception($"后台采集断线: {ex.Message}"));
            }
        }

        public void StopReading()
        {
            if (RunningTask != null)
            {
                RunningTask = null;
                AiTask?.Dispose();
                AiTask = null;
            }
        }

        public void Dispose()
        {
            StopReading();
        }
    }
}