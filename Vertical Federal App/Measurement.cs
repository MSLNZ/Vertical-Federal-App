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
        private bool metric;
        
        
        public Measurement()
        {
            metric = true;
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

        public double RefDeviation_um_uinch
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

        public double ExtremeDeviation_um_uinch
        {
            get { return extreme_deviation; }
            set { extreme_deviation = value; }
        }

        public double limitDeviation_um_uinch
        {
            get { return limit_deviation; }
            set { limit_deviation = value; }
        }
        
        public double CentreDeviation_um_uinch
        {
            get { return centre_deviation; }
            set { centre_deviation = value; }
        }

        public Stack ReferenceStack
        {
            get { return gauge_stack; }
            set { gauge_stack = value; }
        }
        public bool Metric
        {
            get { return metric; }
            set { metric = value; }
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

        public double CalculateVariation()
        {
            double mean_of_centre_value =  (C1+C2+C3)/3;

            //put the results in an array so they are easier to do math on.
            double[] result_array = new double[] {A, B, mean_of_centre_value, D, E };
            variation = result_array.Max() - result_array.Min();
            return variation;
        }

        public double CalculateMeasuredDiff_um_uinch()
        {
            double mean_of_centre_value = (C1 + C2 + C3) / 3;
            double average_ref = (R1 + R2) / 2;
            diff = mean_of_centre_value - average_ref;
            return diff;
        }
        public double RefLength()
        {
            if (metric) return ( CalibrationGauge.Nominal + RefDeviation_um_uinch / 1000);
            else return (CalibrationGauge.Nominal + RefDeviation_um_uinch / 1000000);
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
            
            //convert force units to metric (newtons) 1 oz force 0.27801385n
            double top_probe_force = fed.TopProbeForce * oz_f_to_n_f;
            double bottom_probe_force = fed.BottomProbeForce * oz_f_to_n_f;

            //get federal probe detail
            double probe_d = fed.ProbeDiameter*25.4/1000; //m
            double probe_youngs_mod = fed.ProbeYoungsMod*1000000000; //Pa
            double probe_poissons_ratio = fed.ProbePoissonsRatio;

            double v_ball = (1.0 - Math.Pow(probe_poissons_ratio, 2.0)) / (Math.PI * probe_youngs_mod);
            double v_plate_ref_gauge1 = (1.0 - Math.Pow(gauge_stack.Gauge1.GaugeBlockMaterial.poissons_ratio, 2.0)) / (Math.PI * gauge_stack.Gauge1.GaugeBlockMaterial.youngs_modulus * 1000000000);

            double v_plate_ref_gauge2 = 0.0;
            if (gauge_stack.Gauge2 != null)
            {
                v_plate_ref_gauge2 = (1.0 - Math.Pow(gauge_stack.Gauge2.GaugeBlockMaterial.poissons_ratio, 2.0)) / (Math.PI * gauge_stack.Gauge2.GaugeBlockMaterial.youngs_modulus * 1000000000);
            }

            double v_plate_ref_gauge3 = 0.0;
            if (gauge_stack.Gauge3 != null)
            {
                v_plate_ref_gauge3 = (1.0 - Math.Pow(gauge_stack.Gauge3.GaugeBlockMaterial.poissons_ratio, 2.0)) / (Math.PI * gauge_stack.Gauge3.GaugeBlockMaterial.youngs_modulus * 1000000000);
            }

            
            double v_plate_cal_gauge = (1.0 - Math.Pow(calibration_gauge.GaugeBlockMaterial.poissons_ratio, 2.0)) / (Math.PI * calibration_gauge.GaugeBlockMaterial.youngs_modulus * 1000000000);
            double tt =  2.0 / 3.0;
            double tt2 = 1.0 / 3.0;
            double term1 =  (Math.Pow(3 * Math.PI, tt) / 2.0);
            double term2_top = Math.Pow(top_probe_force, tt);
            double term2_bottom =  Math.Pow(bottom_probe_force, tt);
            double term3_ref_top =  Math.Pow(v_ball + v_plate_ref_gauge1, tt);
            double term3_ref_bot1 =  Math.Pow(v_ball + v_plate_ref_gauge1, tt);
            double term3_ref_bot2 =  Math.Pow(v_ball + v_plate_ref_gauge2, tt);
            double term3_ref_bot3 =  Math.Pow(v_ball + v_plate_ref_gauge3, tt);
            double term3_cal =  Math.Pow(v_ball + v_plate_cal_gauge, tt);
            double term4 = Math.Pow(1.0 / probe_d, tt2);



            //The top gauge should alway be assigned to Gauge1 in the case where a stack is made from gauges with difference mechanical properties.
            reference_deformation_top_probe = term1 * term2_top * term3_ref_top * term4;

            if (singleton)
            {
                reference_deformation_bottom_probe = term1 * term2_bottom * term3_ref_bot1 * term4;
            }
            else if (two_gauge_stack)
            {
                
                reference_deformation_bottom_probe = term1 * term2_bottom * term3_ref_bot2 * term4;
            }
            else
            {
              
                reference_deformation_bottom_probe = term1 * term2_bottom * term3_ref_bot3 * term4;
            }

            top_probe_deformation_cal_gauge = term1 * term2_top * term3_cal * term4;

            bottom_probe_deformation_cal_gauge = term1 * term2_bottom * term3_cal * term4;

        }
        public void CalculateDeviations(ref double corrected_centre_dev, ref double corrected_extreme_dev, ref double corrected_length_mm_inch)
        {
            if (metric)
            {
                corrected_length_mm_inch = (bottom_probe_deformation_cal_gauge * 1000) - (reference_deformation_bottom_probe * 1000) + RefLength() - (reference_deformation_top_probe * 1000) + CalculateMeasuredDiff_um_uinch() / 1000 + (top_probe_deformation_cal_gauge * 1000);
                corrected_length = corrected_length_mm_inch;
                centre_deviation = (corrected_length - CalibrationGauge.Nominal) *1000; //centre deviation is in um
                corrected_centre_dev = centre_deviation;
            }
            else
            {
                corrected_length_mm_inch = (bottom_probe_deformation_cal_gauge * 1000/25.4) - (reference_deformation_bottom_probe * 1000 / 25.4) + RefLength() - (reference_deformation_top_probe * 1000 / 25.4) + CalculateMeasuredDiff_um_uinch() / 1000000 + (top_probe_deformation_cal_gauge * 1000 / 25.4);
                corrected_length = corrected_length_mm_inch;
                centre_deviation = (corrected_length - CalibrationGauge.Nominal) * 1000000; //centre deviation is in uinch
                corrected_centre_dev = centre_deviation;
            }

                //calculate the deviation of each point.
                double A_dev = A - ((R1 + R2) / 2) + RefDeviation_um_uinch + getCorrection_um_uinch();
                double B_dev = B - ((R1 + R2) / 2) + RefDeviation_um_uinch + getCorrection_um_uinch();
                double D_dev = D - ((R1 + R2) / 2) + RefDeviation_um_uinch + getCorrection_um_uinch();
                double E_dev = E - ((R1 + R2) / 2) + RefDeviation_um_uinch + getCorrection_um_uinch();
                double[] values_array = new double[] { A_dev, B_dev, D_dev, E_dev, centre_deviation };
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

       
        public double getCorrection_um_uinch()
        {
            
            
             elastic_correction = centre_deviation - (CalculateMeasuredDiff_um_uinch() + RefDeviation_um_uinch);
             return elastic_correction;
            
          
        }
    }
}
