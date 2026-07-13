using System;
using NationalInstruments.DAQmx;
using DataConvertLib; 

namespace HallThrusterTestSystem
{

    public class AnalogOutputRegulator
    {
        public OperateResult WriteVoltage(string physicalChannel, double voltageOut, double minVolts = 0.0, double maxVolts = 10.0)
        {
            if (string.IsNullOrWhiteSpace(physicalChannel))
            {
                return OperateResult.CreateFailResult("电压输出失败：物理通道名称不能为空");
            }


            if (voltageOut < minVolts || voltageOut > maxVolts)
            {
                return OperateResult.CreateFailResult(
                    $"电压输出越界拦截！通道[{physicalChannel}] 请求输出 {voltageOut:F2}V，但安全范围是 {minVolts}V ~ {maxVolts}V。");
            }

            using (Task aoTask = new Task())
            {
                try
                {
                    aoTask.AOChannels.CreateVoltageChannel(
                        physicalChannel,
                        "",
                        minVolts,
                        maxVolts,
                        AOVoltageUnits.Volts);

                    AnalogSingleChannelWriter writer = new AnalogSingleChannelWriter(aoTask.Stream);

                    writer.WriteSingleSample(true, voltageOut);

                    return OperateResult.CreateSuccessResult();
                }
                catch (DaqException ex)
                {
                    return OperateResult.CreateFailResult($"模拟量输出失败！通道[{physicalChannel}]异常，详细信息: {ex.Message}");
                }
            }
        }
    }
}