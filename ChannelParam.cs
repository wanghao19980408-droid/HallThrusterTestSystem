using NationalInstruments.DAQmx;

namespace HallThrusterTestSystem
{
    public class ChannelParam
    {
        public string PhysicalName { get; set; }
        public string CustomName { get; set; }   
        public double MinVolts { get; set; }
        public double MaxVolts { get; set; }
        public AITerminalConfiguration TerminalConfig { get; set; }
    }
}