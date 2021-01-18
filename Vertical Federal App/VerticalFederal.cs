using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;

namespace Vertical_Federal_App
{
    public class VerticalFederal
    {
        
        private SerialPort s_port;
        private SerialDataReveived sdr_;
        private double top_probe_force;
        private double bottom_probe_force;
        private double probe_diameter;
        private double youngs_mod;
        private double poissons_ratio;
        public VerticalFederal(ref SerialDataReveived x)
        {
            sdr_ = x;
        }

        public void COMInit(string portname)
        {

            s_port = new SerialPort();
            s_port.PortName = portname;
            s_port.BaudRate = 9600;
            s_port.Parity = Parity.None;
            s_port.DataBits = 8;
            s_port.StopBits = StopBits.One;
            s_port.Handshake = Handshake.XOnXOff;
            s_port.ReadTimeout = 1000;
            s_port.WriteTimeout = 1000;
            try {

                s_port.Open();
                s_port.DiscardInBuffer();

                //register and event handler
                s_port.DataReceived += new SerialDataReceivedEventHandler(DataReceviedEventHandler);
            }
            catch(System.IO.IOException e)
            {

            }
        }

        private void DataReceviedEventHandler(object sender,SerialDataReceivedEventArgs e)
        {
            
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();

            //invoke the gui to do something
            sdr_(indata);
        }

        public double ParseForResult(string line)
        {

            char[] delimeter = new char[1];
            delimeter[0] = ',';
            int comma_count = 0;

            foreach (char c in line)
            {
                if (c == ',')
                {
                    comma_count++;
                }
            }

            if (comma_count == 0)
            {
                return 0.0;
            }

            //find the first comma if it exists
            string[] split_up_by_commas = line.Split(delimeter);

            double result1 = 0.0;
            double result = 0.0;
            try
            {
                result1 = Convert.ToDouble(split_up_by_commas[comma_count]);
                result = Math.Round(result1, 4, MidpointRounding.AwayFromZero);
            }
            catch (FormatException)
            {
                return 0.0;
            }

            return result;
        }

        public double TopProbeForce
        {
            get { return top_probe_force; }
            set { top_probe_force = value; }
        }
        public double BottomProbeForce
        {
            get { return bottom_probe_force; }
            set { bottom_probe_force = value; }
        }
        public double ProbeDiameter
        {
            get { return probe_diameter; }
            set { probe_diameter = value; }
        }
        public double ProbeYoungsMod
        {
            get { return youngs_mod; }
            set { youngs_mod = value; }
        }
        public double ProbePoissonsRatio
        {
            get { return poissons_ratio; }
            set { poissons_ratio = value; }
        }
    }
}
