using NationalInstruments;
using NationalInstruments.DAQmx;
using System;
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

        //连续采集内部时钟模式
        public OperateResult StartContinuousReading(string physicalChannel, double rate = 1000.0, int samplesPerBuffer = 100, AITerminalConfiguration atc = AITerminalConfiguration.Rse, double minVolts = -10.0, double maxVolts = 10.0)
        {
            if (RunningTask != null) return OperateResult.CreateFailResult("已在进行连续采集");

            try
            {
                AiTask = new Task();
                //创建模拟输入通道
                AiTask.AIChannels.CreateVoltageChannel(
                    physicalChannel,
                    "",
                    atc,
                    minVolts,
                    maxVolts,
                    AIVoltageUnits.Volts);

                int bufferSize = (int)(rate * 5.0);
                if (bufferSize < samplesPerBuffer * 2) bufferSize = samplesPerBuffer * 2;

                //配置内部采样时钟
                AiTask.Timing.ConfigureSampleClock(
                    "",
                    rate,
                    SampleClockActiveEdge.Rising,
                    SampleQuantityMode.ContinuousSamples,
                    bufferSize);

                //验证任务配置
                AiTask.Control(TaskAction.Verify);
                RunningTask = AiTask;
                //创建模拟输入读取器
                Reader = new AnalogMultiChannelReader(AiTask.Stream);
                AnalogCallback = new AsyncCallback(AnalogInCallback);
                
                Reader.SynchronizeCallbacks = false;
                //开始异步读取数据
                Reader.BeginReadWaveform(samplesPerBuffer, AnalogCallback, AiTask);
                return OperateResult.CreateSuccessResult();
            }
            catch (DaqException ex)
            {
                StopReading();
                return OperateResult.CreateFailResult($"连续采集启动失败: 物理通道[{physicalChannel}]，详细信息: {ex.Message}");
            }
        }

        private void AnalogInCallback(IAsyncResult ar)
        {
            try
            {
                if (RunningTask != null && RunningTask == ar.AsyncState)
                {
                    //结束异步读取并获取数据
                    AnalogWaveform<double>[] data = Reader.EndReadWaveform(ar);
                    //触发数据采集事件
                    DataAcquired?.Invoke(data);
                    //继续异步读取数据
                    Reader.BeginMemoryOptimizedReadWaveform(data[0].Samples.Count, AnalogCallback, ar.AsyncState, data);
                }
            }
            catch (DaqException ex)
            {
                StopReading();
                HardwareError?.Invoke(new Exception($"后台采集异常断线: {ex.Message}"));
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