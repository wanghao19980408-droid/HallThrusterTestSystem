using System;
using System.Linq;
using NationalInstruments.DAQmx;
using DataConvertLib;

namespace HallThrusterTestSystem
{
    public class HardwareManager
    {

        public string[] GetAllAvailableDevices()
        {
            return DaqSystem.Local.Devices;
        }

        public bool IsDeviceOnline(string deviceName)
        {
            if (string.IsNullOrWhiteSpace(deviceName)) return false;

            string[] devices = GetAllAvailableDevices();

            return devices.Contains(deviceName, StringComparer.OrdinalIgnoreCase);
        }     

        public OperateResult ResetDevice(string deviceName)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
            {
                return OperateResult.CreateFailResult("硬件复位失败：设备名称不能为空！");
            }

            if (!IsDeviceOnline(deviceName))
            {
                return OperateResult.CreateFailResult(
                    $"系统中未检测到主控设备 [{deviceName}]！请检查 USB/PCIe 连线或 NI-MAX 驱动配置。");
            }

            try
            {
                Device device = DaqSystem.Local.LoadDevice(deviceName);

                device.Reset();

                return OperateResult.CreateSuccessResult();
            }
            catch (DaqException ex)
            {
                return OperateResult.CreateFailResult($"设备 [{deviceName}] 硬件复位执行异常，详细信息: {ex.Message}");
            }
        }
    }
}