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
        private string tp;
        private string tp_expiry;
        private double reproducibility_std_u_independent;
        private double scale_resolution_std_u_independent;
        private double scale_calibration_std_u_independent;
        private double delta;
        private double alpha_g;
        private double delta_alpha;
        private double theta_s;
        private double delta_theta;
        private double delta_theta_var;
        private double exp_u_dev_indep = 0.0;
        private double exp_u_dev_dep = 0.0;
        private double exp_u_var_indep = 0.0;
        private double exp_u_var_dep = 0.0;
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
            catch (System.IO.IOException)
            {

            }
        }

        private void DataReceviedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {

            SerialPort sp = (SerialPort)sender;
            System.Threading.Thread.Sleep(20);
            string indata = sp.ReadExisting();
            if (indata.Contains("D")) indata = indata.Remove(0, 1);
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

        public string TechnicalProcedure
        {
            set { tp = value; }
            get { return tp; }
        }

        public string TechnicalProcedureExpiry
        {
            set { tp_expiry = value; }
            get { return tp_expiry; }
        }

        public double ReproducibilityStduIndependent
        {
            set { reproducibility_std_u_independent = value; }
            get { return reproducibility_std_u_independent; }
        }
        public double ScaleResStduIndependent
        {
            set { scale_resolution_std_u_independent = value; }
            get { return scale_resolution_std_u_independent; }
        }
        public double ScaleCalStduIndependent
        {
            set { scale_calibration_std_u_independent = value; }
            get { return scale_calibration_std_u_independent; }
        }

        public double u_Delta
        {
            set { delta = value; }
            get { return delta; }
        }
        public double u_AlphaG
        {
            set { alpha_g = value; }
            get { return alpha_g; }
        }
        public double u_DeltaAlpha
        {
            set { delta_alpha = value; }
            get { return delta_alpha; }
        }
        public double u_ThetaS
        {
            set { theta_s = value; }
            get { return theta_s; }
        }
        public double u_DeltaTheta
        {
            set { delta_theta = value; }
            get { return delta_theta; }
        }
        public double u_DeltaThetaVar
        {
            set { delta_theta_var = value; }
            get { return delta_theta_var; }
        }

        public double ExpanedUncertaintyCMCDevIndep
        {
            set { exp_u_dev_indep = value; }
            get { return exp_u_dev_indep; }
        }
        public double ExpanedUncertaintyCMCDevDep
        {
            set { exp_u_dev_dep = value; }
            get { return exp_u_dev_dep; }
        }
        public double ExpanedUncertaintyCMCVarIndep
        {
            set { exp_u_var_indep = value; }
            get { return exp_u_var_indep; }
        }
        public double ExpanedUncertaintyCMCVarDep
        {
            set { exp_u_var_dep = value; }
            get { return exp_u_var_dep; }
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
