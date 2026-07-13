using System;
using NationalInstruments.DAQmx;
using DataConvertLib;

namespace HallThrusterTestSystem
{
    public class DigitalControl
    {
        public OperateResult WriteLine(string physicalChannel, bool state)
        {
            if (string.IsNullOrWhiteSpace(physicalChannel))
            {
                return OperateResult.CreateFailResult("控制失败：物理通道名称不能为空");
            }

            using (Task doTask = new Task())
            {
                try
                {
                    doTask.DOChannels.CreateChannel(
                        physicalChannel,
                        "",
                        ChannelLineGrouping.OneChannelForAllLines);

                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(doTask.Stream);

                    writer.WriteSingleSampleSingleLine(true, state);

                    return OperateResult.CreateSuccessResult();
                }
                catch (DaqException ex)
                {
                    return OperateResult.CreateFailResult($"数字量控制失败！通道[{physicalChannel}]异常，详细信息: {ex.Message}");
                }
            }
        }
        public OperateResult<bool> ReadLine(string physicalChannel)
        {
            if (string.IsNullOrWhiteSpace(physicalChannel))
            {
                return OperateResult.CreateFailResult<bool>("读取失败：物理通道名称不能为空");
            }

            using (Task diTask = new Task())
            {
                try
                {
                    diTask.DIChannels.CreateChannel(
                        physicalChannel,
                        "",
                        ChannelLineGrouping.OneChannelForAllLines);

                    DigitalSingleChannelReader reader = new DigitalSingleChannelReader(diTask.Stream);

                    bool state = reader.ReadSingleSampleSingleLine();

                    return OperateResult<bool>.CreateSuccessResult(state);
                }
                catch (DaqException ex)
                {
                    return OperateResult.CreateFailResult<bool>($"数字量读取失败！通道[{physicalChannel}]异常，详细信息: {ex.Message}");
                }
            }
        }
    }
}