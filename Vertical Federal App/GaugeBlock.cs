using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Vertical_Federal_App
{
    public class Stack
    {
        private GaugeBlock gauge1;
        private GaugeBlock gauge2;
        private GaugeBlock gauge3;

        public Stack(short num_gauges_in_stack)
        {
            switch (num_gauges_in_stack) {
                case 1:
                    gauge1 = new GaugeBlock(true);
                    gauge2 = null;
                    gauge3 = null;
                    break;
                case 2:
                    gauge1 = new GaugeBlock(true);
                    gauge2 = new GaugeBlock(false); 
                    gauge3 = null;
                    break;
                case 3:
                    gauge1 = new GaugeBlock(true);
                    gauge2 = new GaugeBlock(false); 
                    gauge3 = new GaugeBlock(false);
                    break;
            }
            
        }
        public Stack Clone()
        {
            Stack stk = new Stack(1);
            stk.Gauge1 = Gauge1.Clone();
            if (Gauge2 == null) stk.Gauge2 = null;
            else stk.Gauge2 = Gauge2.Clone();
            if (Gauge3 == null) stk.Gauge3 = null;
            else stk.Gauge3 = Gauge3.Clone();
            return stk;
        }
        public GaugeBlock Gauge1
        {
            set { gauge1 = value; }
            get { return gauge1;  }
        }
        public GaugeBlock Gauge2
        {
            set { gauge2 = value; }
            get { return gauge2; }
        }
        public GaugeBlock Gauge3
        {
            set { gauge3 = value; }
            get { return gauge3; }
        }
    }

    public class Material
    {
        public string material;
        public double poissons_ratio;
        public double youngs_modulus;
        public double exp_coeff;
    }

    public class GaugeBlock
    {
        private double nominal;
        private string serial_number;
        private string client_name;
        private bool metric;
        private double deviation;
        private double extreme_deviation;
        private double min_deviation;
        private double max_deviation;
        private double corrected_length;
        private double variation;
        private string from_set; //the name of the reference set this gauge belongs to
        private Material gauge_material;
        private bool illegal_size;
        public GaugeBlock(bool with_values)
        {

            nominal = 0;
            serial_number = "";
            metric = true;
            gauge_material = new Material();
            if (with_values)
            {
                gauge_material.exp_coeff = 9.5;
                gauge_material.poissons_ratio = 0.290;
                gauge_material.youngs_modulus = 205;
                gauge_material.material = "ceramic";
            }
            else
            {
                gauge_material.exp_coeff = 0;
                gauge_material.poissons_ratio = 0;
                gauge_material.youngs_modulus = 0;
                gauge_material.material = "";
            }
            illegal_size = false;
                
        }

        public GaugeBlock Clone()
        {
            //make a new gauge block
            GaugeBlock gb = new GaugeBlock(false);
            gb.nominal = nominal;
            if (serial_number == null) gb.serial_number = null;
            else gb.serial_number = (string) serial_number.Clone();
            if (client_name == null) gb.client_name = null;
            else gb.client_name = (string) client_name.Clone();
            gb.metric = metric;
            gb.deviation = deviation;
            gb.extreme_deviation = extreme_deviation;
            gb.min_deviation = min_deviation;
            gb.max_deviation = max_deviation;
            gb.corrected_length = corrected_length;
            gb.variation = variation;
            if (from_set == null) gb.from_set = null;
            else gb.from_set = (string)from_set.Clone();
            gb.illegal_size = illegal_size;

            Material mtrl = new Material();
            //does the gauge block have a material allocated? if it doesn't then the gauge is not in use.
            if (gauge_material.material == null) mtrl.material = "";
            else mtrl.material = (string) gauge_material.material.Clone();

            mtrl.exp_coeff = gauge_material.exp_coeff;
            mtrl.poissons_ratio = gauge_material.poissons_ratio;
            mtrl.youngs_modulus = gauge_material.youngs_modulus;
            gb.GaugeBlockMaterial = mtrl;
            return gb;
        }
        public double Nominal
        {
            get { return nominal; }
            set { nominal = value; }
        }
        public string SerialNumber
        {
            get { return serial_number; }
            set { serial_number = value; }
        }
        public bool IllegalSize
        {
            get { return illegal_size; }
            set { illegal_size = value; }
        }
        public string ClientName
        {
            get { return client_name; }
            set { client_name = value; }
        }

        public bool Metric
        {
            get { return metric; }
            set { metric = value; }
        }
        public double CentreDeviation
        {
            get { return deviation; }
            set { deviation = value; }
        }
        public double CorrLength
        {
            get { return corrected_length; }
            set { corrected_length = value; }
        }
        public double Variation
        {
            get { return variation; }
            set { variation = value; }
        }
        public double MinDev
        {
            get { return min_deviation; }
            set { min_deviation = value; }
        }
        public double MaxDev
        {
            get { return max_deviation; }
            set { max_deviation = value; }
        }
        public double ExtremeDeviation
        {
            get { return extreme_deviation; }
            set { extreme_deviation = value; }
        }
        public string FromSet
        {
            get { return from_set; }
            set { from_set = value; }
        }
        public Material GaugeBlockMaterial
        {
            get { return gauge_material; }
            set { gauge_material = value; }
        }
        public static double calculateGaugeStackDeviation(Stack ref_g, bool metric)
        {
            double refdev = 0.0;

            GaugeBlock g1 = ref_g.Gauge1;
            GaugeBlock g2 = ref_g.Gauge2;
            GaugeBlock g3 = ref_g.Gauge3;
            double gauge1_dev = 0.0;
            double gauge2_dev = 0.0;
            double gauge3_dev = 0.0;

            if (g1 != null)
            {
                //If necessary, convert gauge 1 devation to the same unit as the check box.
                if (metric && !g1.Metric) gauge1_dev = Math.Round(g1.CentreDeviation * (25.4 / 1000), 5);
                else if (!metric && g1.Metric) gauge1_dev = Math.Round(g1.CentreDeviation * (1000 / 25.4), 5);
                else gauge1_dev = g1.CentreDeviation;
                refdev = gauge1_dev;
            }
            if (g2 != null)
            {
                //If necessary, convert gauge 2 devation to the same unit as the check box.
                if (metric && !g2.Metric) gauge2_dev = Math.Round(g2.CentreDeviation * (25.4 / 1000), 5);
                else if (!metric && g2.Metric) gauge2_dev = Math.Round(g2.CentreDeviation * (1000 / 25.4), 5);
                else gauge2_dev = g2.CentreDeviation;
                refdev += gauge2_dev;
            }
            if (g3 != null)
            {
                //If necessary, convert gauge 3 devation to the same unit as the check box.
                if (metric && !g3.Metric) gauge3_dev = Math.Round(g3.CentreDeviation * (25.4 / 1000), 5);
                else if (!metric && g3.Metric) gauge3_dev = Math.Round(g3.CentreDeviation * (1000 / 25.4), 5);
                else gauge3_dev = g3.CentreDeviation;
                refdev += gauge3_dev;
            }
            return refdev;
        }
    }
}
