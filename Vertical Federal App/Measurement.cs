using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertical_Federal_App
{
    public class Measurement
    {
        private static List<Measurement> measurements;  
        private const double oz_f_to_n_f = 0.27801385;  //newtons
        private GaugeBlock calibration_gauge;
        private double reference_deformation_top_probe;
        private double reference_deformation_bottom_probe;
        private double bottom_probe_deformation_cal_gauge;
        private double top_probe_deformation_cal_gauge;
        private double corrected_length;
        private double elastic_correction;
        private Stack gauge_stack;
        private string cal_set_serial;
        private string cal_gauge_serial;
        private double nominal;
        private double cal_gauge_exp_coeff;
        private double cal_gauge_youngs_mod;
        private double cal_gauge_poissons_ratio;
        private double reference_deviation;
        private double variation;
        private double diff; //difference
        private double extreme_deviation;
        private double limit_deviation;
        private double centre_deviation;
        private double r1;
        private double r2;
        private double c1;
        private double c2;
        private double c3;
        private double a;
        private double b;
        private double d;
        private double e;
        
        
        public Measurement()
        {

        }

        public double R1
        {
            get { return r1; }
            set { r1 = value; }
        }
        public double R2
        {
            get { return r2; }
            set { r2 = value; }
        }
        public double A
        {
            get { return a; }
            set { a = value; }
        }
        public double B
        {
            get { return b; }
            set { b = value; }
        }
        public double C1
        {
            get { return c1; }
            set { c1 = value; }
        }
        public double C2
        {
            get { return c2; }
            set { c2 = value; }
        }
        public double C3
        {
            get { return c3; }
            set { c3 = value; }
        }
        public double D
        {
            get { return d; }
            set { d = value; }
        }
        public double E
        {
            get { return e; }
            set { e = value; }
        }

        public string CalSetSerial
        {
            get { return cal_set_serial; }
            set { cal_set_serial = value; }
        }
        public string CalGaugeSerial
        {
            get { return cal_gauge_serial; }
            set { cal_gauge_serial = value; }
        }
        public double Nominal
        {
            get { return nominal; }
            set { nominal = value; }
        }
        public double RefDeviation
        {
            get { return reference_deviation; }
            set { reference_deviation = value; }
        }
       
        public double CalGaugeExpCoeff
        {
            get { return cal_gauge_exp_coeff; }
            set { cal_gauge_exp_coeff = value; }
        }
     
        public double CalGaugeYoungMod
        {
            get { return cal_gauge_youngs_mod; }
            set { cal_gauge_youngs_mod = value; }
        }
      
        public double CalGaugePoissonRatio
        {
            get { return cal_gauge_poissons_ratio;}
            set { cal_gauge_poissons_ratio = value; }
        }

        public double ExtremeDeviation
        {
            get { return extreme_deviation; }
            set { extreme_deviation = value; }
        }

        public double limitDeviation
        {
            get { return limit_deviation; }
            set { limit_deviation = value; }
        }
        
        public double CentreDeviation
        {
            get { return centre_deviation; }
            set { centre_deviation = value; }
        }

        public Stack ReferenceStack
        {
            get { return gauge_stack; }
            set { gauge_stack = value; }
        }

        public GaugeBlock CalibrationGauge
        {
            get { return calibration_gauge; }
            set { calibration_gauge = value; }
        }
        public static List<Measurement> Measurements
        {
            get { return measurements; }
            set { measurements = value; }
        }

        public double calculateVariation()
        {
            double mean_of_centre_value =  (C1+C2+C3)/3;

            //put the results in an array so they are easier to do math on.
            double[] result_array = new double[] {A, B, mean_of_centre_value, D, E };
            variation = result_array.Max() - result_array.Min();
            return variation;
        }

        public double calculateMeasuredDiff()
        {
            double mean_of_centre_value = (C1 + C2 + C3) / 3;
            double average_ref = (R1 + R2) / 2;
            diff = mean_of_centre_value - average_ref;
            return diff;
        }
        public double RefLength()
        {
            return (Nominal + RefDeviation);
        }
        public void calculateElasticDeformations(VerticalFederal fed)
        {
            bool singleton = false;
            bool two_gauge_stack = false;
            bool three_gauge_stack = false;
            if (ReferenceStack.Gauge1 == null) return ;  //this shouldn't happen
            else if (ReferenceStack.Gauge2 == null) singleton = true;
            else if (ReferenceStack.Gauge3 == null) two_gauge_stack = true;
            else three_gauge_stack = true;
            
            //convert force units to metric (newtons) 1Oz force 0.27801385n
            double top_probe_force = fed.TopProbeForce * oz_f_to_n_f;
            double bottom_probe_force = fed.BottomProbeForce * oz_f_to_n_f;

            //get federal probe detail
            double probe_d = fed.ProbeDiameter*25.4/1000; //m
            double probe_youngs_mod = fed.ProbeYoungsMod*1000000000; //Pa
            double probe_poissons_ratio = fed.ProbePoissonsRatio;

            //The top gauge should alway be assigned to Gauge1 in the case where a stack is made from gauges with difference mechanical properties.
            reference_deformation_top_probe = Math.Pow((3 * Math.PI),(2 / 3))/2*Math.Pow(top_probe_force,2/3)*
                Math.Pow(((1-Math.Pow(probe_poissons_ratio, 2))/(Math.PI*probe_youngs_mod)+(1-Math.Pow(gauge_stack.Gauge1.GaugeBlockMaterial.poissons_ratio, 2))/(Math.PI* gauge_stack.Gauge1.GaugeBlockMaterial.poissons_ratio * 1000000000)),2/3)*
                Math.Pow(1/probe_d,1/3);

            if (singleton)
            {
                reference_deformation_bottom_probe = Math.Pow((3 * Math.PI), (2 / 3)) / 2 * Math.Pow(bottom_probe_force, 2 / 3) *
                Math.Pow(((1 - Math.Pow(probe_poissons_ratio, 2)) / (Math.PI * probe_youngs_mod) + (1 - Math.Pow(gauge_stack.Gauge1.GaugeBlockMaterial.poissons_ratio, 2)) / (Math.PI * gauge_stack.Gauge1.GaugeBlockMaterial.poissons_ratio * 1000000000)), 2 / 3) *
                Math.Pow(1 / probe_d, 1 / 3);
            }
            else if (two_gauge_stack)
            {
                
                reference_deformation_bottom_probe = Math.Pow((3 * Math.PI), (2 / 3)) / 2 * Math.Pow(bottom_probe_force, 2 / 3) *
                    Math.Pow(((1 - Math.Pow(probe_poissons_ratio, 2)) / (Math.PI * probe_youngs_mod) + (1 - Math.Pow(gauge_stack.Gauge2.GaugeBlockMaterial.poissons_ratio, 2)) / (Math.PI * gauge_stack.Gauge2.GaugeBlockMaterial.poissons_ratio * 1000000000)), 2 / 3) *
                    Math.Pow(1 / probe_d, 1 / 3);
            }
            else
            {
              
                reference_deformation_bottom_probe = Math.Pow((3 * Math.PI), (2 / 3)) / 2 * Math.Pow(bottom_probe_force, 2 / 3) *
                    Math.Pow(((1 - Math.Pow(probe_poissons_ratio, 2)) / (Math.PI * probe_youngs_mod) + (1 - Math.Pow(gauge_stack.Gauge3.GaugeBlockMaterial.poissons_ratio, 2)) / (Math.PI * gauge_stack.Gauge3.GaugeBlockMaterial.poissons_ratio * 1000000000)), 2 / 3) *
                    Math.Pow(1 / probe_d, 1 / 3);
            }

            top_probe_deformation_cal_gauge = Math.Pow((3 * Math.PI), (2 / 3)) / 2 * Math.Pow(top_probe_force, 2 / 3) *
                    Math.Pow(((1 - Math.Pow(probe_poissons_ratio, 2)) / (Math.PI * probe_youngs_mod) + (1 - Math.Pow(calibration_gauge.GaugeBlockMaterial.poissons_ratio, 2)) / (Math.PI * calibration_gauge.GaugeBlockMaterial.youngs_modulus*1000000000)), 2 / 3) *
                    Math.Pow(1 / probe_d, 1 / 3);

            bottom_probe_deformation_cal_gauge = Math.Pow((3 * Math.PI), (2 / 3)) / 2 * Math.Pow(bottom_probe_force, 2 / 3) *
                    Math.Pow(((1 - Math.Pow(probe_poissons_ratio, 2)) / (Math.PI * probe_youngs_mod) + (1 - Math.Pow(calibration_gauge.GaugeBlockMaterial.poissons_ratio, 2)) / (Math.PI * calibration_gauge.GaugeBlockMaterial.youngs_modulus * 1000000000)), 2 / 3) *
                    Math.Pow(1 / probe_d, 1 / 3); 

        }
        public void CalculateDeviations(ref double corrected_centre_dev,ref double corrected_extreme_dev, ref double corrected_length_mm)
        {
            corrected_length_mm = (bottom_probe_deformation_cal_gauge * 1000) - (reference_deformation_bottom_probe * 1000) + RefLength() - (reference_deformation_top_probe * 1000) + calculateMeasuredDiff() + (top_probe_deformation_cal_gauge * 1000);
            corrected_length = corrected_length_mm;


            centre_deviation = corrected_length-Nominal;
            corrected_centre_dev = centre_deviation;

            //calculate the deviation of each point.
            double A_dev = A - (R1 + R2) / 2 + RefDeviation + getCorrection_um();
            double B_dev = B - (R1 + R2) / 2 + RefDeviation + getCorrection_um();
            double D_dev = D - (R1 + R2) / 2 + RefDeviation + getCorrection_um();
            double E_dev = E - (R1 + R2) / 2 + RefDeviation + getCorrection_um();
            double[] values_array = new double[] { A_dev, B_dev, D_dev, E_dev,centre_deviation };
            double max_dev = values_array.Max();
            double min_dev = values_array.Min();

            //calculate extreme deviation
            if (Math.Abs(max_dev) > Math.Abs(min_dev))
            {
                extreme_deviation = max_dev;
                corrected_extreme_dev = max_dev;
            }
            else
            {
                extreme_deviation = min_dev;
                corrected_extreme_dev = min_dev;
            }
            
            
        }

       
        public double getCorrection_um()
        {
            elastic_correction = centre_deviation - calculateMeasuredDiff() * 1000 + reference_deviation;
            return elastic_correction;
        }
    }
}
