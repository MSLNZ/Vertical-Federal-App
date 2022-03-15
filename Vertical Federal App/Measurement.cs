using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Vertical_Federal_App
{
    
    public class Measurement
    {


        private enum nom { category0 = 0, category1, category2, category3, category4, category5 }
        private static List<Measurement> measurements = new List<Measurement>();
        public static string Version_number = "Rev 3.0";

        private static bool file_header_written = false;
        private const double oz_f_to_n_f = 0.27801385;  //newtons
        private GaugeBlock calibration_gauge;
        private string date_time;
        private double reference_deformation_top_probe;
        private double reference_deformation_bottom_probe;
        private double bottom_probe_deformation_cal_gauge;
        private double top_probe_deformation_cal_gauge;
        private double elastic_correction;
        private Stack gauge_stack;
        private double reference_deviation;
        private double diff; //difference
        private double r1;
        private double r2;
        private double c1;
        private double c2;
        private double c3;
        private double a;
        private double b;
        private double d;
        private double e;
        private static Thread tempRetriever;
        private static bool log_temperatures = false;
        private static double rep_d;
        private static double rep_v;
        private static double rep_A;
        private static double rep_B;
        private static double rep_D;
        private static double rep_E;
        private static double rep_ABDE;
        private static double rep_ext_dev;
        private static double rms_theta_s;

        public static string measurement_file_header = "DateTime,Temperature,Units,Cal Nominal,Gauge Set Serial No,Gauge Serial No,Cal Gauge Material,Cal Gauge Exp Coeff,Cal Gauge Young's Modulus," +
                        "Cal Gauge Poisson Ratio,Ref Gauge 1 Nominal,Ref Gauge 1 Deviation,Ref Gauge 1 Set Serial No,Ref Gauge 1 Metric/Imperial,Ref Gauge 1 Serial No,Ref Gauge 1 Material,Ref Gauge 1 exp coeeff,Ref Gauge 1 Young's Modulus,Ref Gauge 1 Poisson Ratio," +
                        "Ref Gauge 2 Nominal,Ref Gauge 2 Deviation,Ref Gauge 2 Set Serial No,Ref Gauge 2 Metric/Imperial,Ref Gauge 2 Serial No,Ref Gauge 2 Material,Ref Gauge 2 exp coeeff,Ref Gauge 2 Young's Modulus,Ref Gauge 2 Poisson Ratio," +
                        "Ref Gauge 3 Nominal,Ref Gauge 3 Deviation,Ref Gauge 3 Set Serial No,Ref Gauge 3 Metric/Imperial,Ref Gauge 3 Serial No,Ref Gauge 3 Material,Ref Gauge 3 exp coeeff,Ref Gauge 3 Young's Modulus,Ref Gauge 3 Poisson Ratio," +
                        "Reference Stack Deviation,R1,C1,A,B,C2,D,E,C3,R2,Measured Length,Centre Deviation,Min Deviation,Max Deviation,Extreme Deviation,Variation,Compliance Standard ID,Compliance Standard String";
        public static string filename ="";
        public static string filename_sum = "";
        public static string filename_U95_sum = "";
        public static List<GaugeBlockSet> calibration_gauge_sets;
        public static List<GaugeBlockSet> reference_gauge_sets;
        public static GaugeBlock working_gauge;
        
        
        public Measurement()
        {
            //metric = true;

        }
        
        public object Clone(){
            return MemberwiseClone();
        }
        public static bool HeaderWritten
        {
            get { return file_header_written; }
            set { file_header_written = value; }
        }
        public string Datetime
        {
            get { return date_time; }
            set { date_time = value; }
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

        public double RefDeviation_um_uinch
        {
            get { return reference_deviation; }
            set { reference_deviation = value; }
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
        public static bool LogTemperatures
        {
            set { log_temperatures = value; }
            get { return log_temperatures; }
        }

        public static void StartRetreivingTemperatures(ref TemperatureRetrieved tdr)
        {
            tempRetriever = new Thread(new ParameterizedThreadStart(LookUpTemperatures));
            tempRetriever.Start(tdr);
        }

        private static void LookUpTemperatures(object tdr)
        {
            TemperatureRetrieved printData = (TemperatureRetrieved) tdr;
            while (LogTemperatures)
            {
                DateTime now = DateTime.Now;
                DateTime previous_interation_datetime = DateTime.MinValue;

                string path = @"G:\Shared drives\MSL - Length\Length\Temperature Monitoring Data\Hilger Lab\"+ now.Year.ToString() + @"\" + now.Year.ToString() + "-" + now.Month.ToString() + @"\" + "Vertical Federal.txt";
                if (!File.Exists(path))
                {
                    MessageBox.Show("Unable to get temperature.  Temperature data file expected, unable to find file at:\n" +
                        path + "\nYou can enter temperatures into the box below manually each time you make a measurement");
                    continue;
                }
                
                //get the latest temperature from the temperature file.  This should be the last line of the file.
                string line = GetLastLine(path);

                if (line.Equals("")) continue;

                //find the first comma
                int indexofcomma1 = line.IndexOf(",");
                string temperature = line.Remove(indexofcomma1);
                string remainder = line.Substring(indexofcomma1 + 1);
                int indexofcomma2 = remainder.IndexOf(",");
                remainder = remainder.Substring(indexofcomma2 + 1);
                int indexofcomma3 = remainder.IndexOf(",");
                string datetime = remainder.Remove(indexofcomma3);
                DateTime found = DateTime.MinValue;
                double temperature_d = -273.14;

                try
                {
                    found = Convert.ToDateTime(datetime);
                    temperature_d = Convert.ToDouble(temperature);
                }
                catch (FormatException)
                {
                        continue;
                }
                printData(temperature_d, found);
              
                    
               
            }
        }

        public static String GetLastLine(String fileName)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    string[] lines = File.ReadAllLines(fileName);
                    if (lines.Length == 0 || lines.Length == 1 || lines.Length == 2) return "";
                    else return lines[lines.Length - 1];
                }
            }
            catch (OperationCanceledException)
            {
                //access to the file was interupted return an empty string
                return "";
            }
            catch (System.IO.IOException)
            {
                return "";
            }

                
        }


        public double CalculateVariation()
        {
            double mean_of_centre_value =  (C1+C2+C3)/3.0;
            double variation = 0.0;
            //put the results in an array so they are easier to do math on.
            double[] result_array = new double[] {A, B, mean_of_centre_value, D, E };
            variation = result_array.Max() - result_array.Min();
            return variation;
        }

        public double CalculateMeasuredDiff_um_uinch()
        {
            double mean_of_centre_value = (C1 + C2 + C3) / 3.0;
            double average_ref = (R1 + R2) / 2.0;
            diff = mean_of_centre_value - average_ref;
            return diff;
        }
        public double RefLength()
        {
            if (CalibrationGauge.Metric) return ( CalibrationGauge.Nominal + RefDeviation_um_uinch / 1000.0);
            else return (CalibrationGauge.Nominal + RefDeviation_um_uinch / 1000000.0);
        }
        public void calculateElasticDeformations(VerticalFederal fed)
        {
            if(ReferenceStack == null)
            {
                MessageBox.Show("please choose a reference gauge before saving");
            }
            bool singleton = false;
            bool two_gauge_stack = false;
            bool three_gauge_stack = false;
            if (ReferenceStack.Gauge1== null) return ;  //this shouldn't happen
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
            double term1 =  (Math.Pow(3.0 * Math.PI, tt) / 2.0);
            double term2_top = Math.Pow(top_probe_force, tt);
            double term2_bottom =  Math.Pow(bottom_probe_force, tt);
            double term3_ref_top =  Math.Pow(v_ball + v_plate_ref_gauge1, tt);
            double term3_ref_bot1 =  Math.Pow(v_ball + v_plate_ref_gauge1, tt);
            double term3_ref_bot2 =  Math.Pow(v_ball + v_plate_ref_gauge2, tt);
            double term3_ref_bot3 =  Math.Pow(v_ball + v_plate_ref_gauge3, tt);
            double term3_cal =  Math.Pow(v_ball + v_plate_cal_gauge, tt);
            double term4 = Math.Pow(1.0 / probe_d, tt2);



            //The top gauge should alway be assigned to Gauge1 in the case where a stack is made from gauges with different mechanical properties.
            reference_deformation_top_probe = term1 * term2_top * term3_ref_top * term4; //metres

            if (singleton)
            {
                reference_deformation_bottom_probe = term1 * term2_bottom * term3_ref_bot1 * term4; //metres
            }
            else if (two_gauge_stack)
            {
                
                reference_deformation_bottom_probe = term1 * term2_bottom * term3_ref_bot2 * term4; //metres
            }
            else
            {
              
                reference_deformation_bottom_probe = term1 * term2_bottom * term3_ref_bot3 * term4; //metres
            }

            top_probe_deformation_cal_gauge = term1 * term2_top * term3_cal * term4; //metres

            bottom_probe_deformation_cal_gauge = term1 * term2_bottom * term3_cal * term4; //metres

        }
        public void CalculateDeviations()
        {
            double corr_centre_dev = 0.0;
            double corr_extreme_dev = 0.0;
            double corr_length = 0.0;

            if (CalibrationGauge.Metric)
            {
                corr_length = (bottom_probe_deformation_cal_gauge * 1000) - (reference_deformation_bottom_probe * 1000) + RefLength() - (reference_deformation_top_probe * 1000) + CalculateMeasuredDiff_um_uinch() / 1000 + (top_probe_deformation_cal_gauge * 1000);
                corr_centre_dev = (corr_length - CalibrationGauge.Nominal) * 1000; //centre deviation is in um
            }
            else
            {
                corr_length = (bottom_probe_deformation_cal_gauge * 1000 / 25.4) - (reference_deformation_bottom_probe * 1000 / 25.4) + RefLength() - (reference_deformation_top_probe * 1000 / 25.4) + CalculateMeasuredDiff_um_uinch() / 1000000 + (top_probe_deformation_cal_gauge * 1000 / 25.4);
                corr_centre_dev = (corr_length - CalibrationGauge.Nominal) * 1000000; //centre deviation is in uinch
            }
            CalibrationGauge.CentreDeviation = Math.Round(corr_centre_dev, 5);
            CalibrationGauge.CorrLength = Math.Round(corr_length, 7);
           
            //calculate the deviation of each point.
            double A_dev = A - ((R1 + R2) / 2.0) + RefDeviation_um_uinch + getCorrection_um_uinch();
            double B_dev = B - ((R1 + R2) / 2.0) + RefDeviation_um_uinch + getCorrection_um_uinch();
            double D_dev = D - ((R1 + R2) / 2.0) + RefDeviation_um_uinch + getCorrection_um_uinch();
            double E_dev = E - ((R1 + R2) / 2.0) + RefDeviation_um_uinch + getCorrection_um_uinch();
            double[] values_array = new double[] { A_dev, B_dev, D_dev, E_dev, corr_centre_dev };
            CalibrationGauge.MaxDev = Math.Round(values_array.Max(),5);
            CalibrationGauge.MinDev = Math.Round(values_array.Min(),5);
            CalibrationGauge.ADev = Math.Round(A_dev, 5);
            CalibrationGauge.BDev = Math.Round(B_dev, 5);
            CalibrationGauge.DDev = Math.Round(D_dev, 5);
            CalibrationGauge.EDev = Math.Round(E_dev, 5);

            corr_extreme_dev = CalculateExtremeDeviation(CalibrationGauge.MinDev, CalibrationGauge.MaxDev);
            CalibrationGauge.ExtremeDeviation = Math.Round(corr_extreme_dev, 5);
        }

        public static double CalculateExtremeDeviation(double min, double max)
        {
            //calculate extreme deviation
            if (Math.Abs(max) > Math.Abs(min)) return max;
            else return min;
        }

        public double getCorrection_um_uinch()
        {
             elastic_correction = CalibrationGauge.CentreDeviation - (CalculateMeasuredDiff_um_uinch() + RefDeviation_um_uinch);
             return elastic_correction;
        }
        /// <summary>
        /// Calculates the reproducability for deviation and variation in length measurement.  
        /// </summary>
        /// 
        public static void CalculateReproducibility()
        {
            List<string> found = new List<string>();

            //These lists hold the variances of each sample
            List<double> vars_dev = new List<double>();
            List<double> vars_A = new List<double>();
            List<double> vars_B = new List<double>();
            List<double> vars_D = new List<double>();
            List<double> vars_E = new List<double>();

            //These lists hold the sample size of each sample
            List<double> counts_dev = new List<double>();
            List<double> counts_A = new List<double>();
            List<double> counts_B = new List<double>();
            List<double> counts_D = new List<double>();
            List<double> counts_E = new List<double>();

            //These lists hold the mean of each sample
            List<double> means_dev = new List<double>();
            List<double> means_A = new List<double>();
            List<double> means_B = new List<double>();
            List<double> means_D = new List<double>();
            List<double> means_E = new List<double>();


            //determine the standard deviations of all repeat measurements for deviation and variation
            foreach (Measurement m in measurements)
            {
                string find = "";
                if (found.Contains(m.calibration_gauge.Nominal.ToString() + m.calibration_gauge.SerialNumber)) continue;
                else
                {
                    find = m.calibration_gauge.Nominal.ToString() + m.calibration_gauge.SerialNumber;
                }

                found.Add(m.CalibrationGauge.Nominal.ToString()+ m.calibration_gauge.SerialNumber);
                
                List<double> occurences_dev = new List<double>();
                List<double> occurences_Adev = new List<double>();
                List<double> occurences_Bdev = new List<double>();
                List<double> occurences_Ddev = new List<double>();
                List<double> occurences_Edev = new List<double>();
                foreach (Measurement n in measurements)
                {
                    if (n.CalibrationGauge.Nominal.ToString()+n.calibration_gauge.SerialNumber == find) //we have a match
                    {
                        if (n.CalibrationGauge.Metric)
                        {
                            occurences_dev.Add(n.CalibrationGauge.CentreDeviation);
                            occurences_Adev.Add(n.CalibrationGauge.ADev);
                            occurences_Bdev.Add(n.CalibrationGauge.BDev);
                            occurences_Ddev.Add(n.CalibrationGauge.DDev);
                            occurences_Edev.Add(n.CalibrationGauge.EDev);
                          
                        }
                        else //it's imperial so the deviations and variations will be in micro inches - convert them to micrometres for the u95 calculations
                        {
                            occurences_dev.Add(n.CalibrationGauge.CentreDeviation * 0.0254);
                            occurences_Adev.Add(n.CalibrationGauge.ADev * 0.0254);
                            occurences_Bdev.Add(n.CalibrationGauge.BDev * 0.0254);
                            occurences_Ddev.Add(n.CalibrationGauge.DDev * 0.0254);
                            occurences_Edev.Add(n.CalibrationGauge.EDev * 0.0254);
                        }
                    }
                }
                
                vars_dev.Add(getVariance(occurences_dev));
                vars_A.Add(getVariance(occurences_Adev));
                vars_B.Add(getVariance(occurences_Bdev));
                vars_D.Add(getVariance(occurences_Ddev));
                vars_E.Add(getVariance(occurences_Edev));

                counts_dev.Add(occurences_dev.Count);
                counts_A.Add(occurences_Adev.Count);
                counts_B.Add(occurences_Bdev.Count);
                counts_D.Add(occurences_Ddev.Count);
                counts_E.Add(occurences_Edev.Count);
            }

            //now find the sum of the squares
            double weighted_sum_d = 0.0;
            double weighted_sum_A = 0.0;
            double weighted_sum_B = 0.0;
            double weighted_sum_D = 0.0;
            double weighted_sum_E = 0.0;
            double weighted_sum_ABDE = 0.0;
            int count_index = 0;
            double total_samples_centre_dev = 0;
            foreach (double var_d in vars_dev)
            {
                double sample_size = counts_dev.ElementAt(count_index);
                total_samples_centre_dev += sample_size;
                weighted_sum_d += (var_d)*(sample_size-1);
                count_index++;
            }
            double pooled_variance_centre_dev = weighted_sum_d / (total_samples_centre_dev - count_index);

            double total_samples_ABDE_dev = 0;
            count_index = 0;
            foreach (double var_A in vars_A)
            {
                double sample_size = counts_dev.ElementAt(count_index);
                total_samples_ABDE_dev += sample_size;
                weighted_sum_A += (var_A) * (sample_size - 1);
                count_index++;
            }
            count_index = 0;
            foreach (double var_B in vars_B)
            {
                double sample_size = counts_dev.ElementAt(count_index);
                total_samples_ABDE_dev += sample_size;
                weighted_sum_B += (var_B) * (sample_size - 1);
                count_index++;
            }
            count_index = 0;
            foreach (double var_D in vars_D)
            {
                double sample_size = counts_dev.ElementAt(count_index);
                total_samples_ABDE_dev += sample_size;
                weighted_sum_D += (var_D) * (sample_size - 1);
                count_index++;
            }
            count_index = 0;
            foreach (double var_E in vars_E)
            {
                double sample_size = counts_dev.ElementAt(count_index);
                total_samples_ABDE_dev += sample_size;
                weighted_sum_E += (var_E) * (sample_size - 1);
                count_index++;
            }
            weighted_sum_ABDE = weighted_sum_A + weighted_sum_B + weighted_sum_D + weighted_sum_E;
            double pooled_variance_A = weighted_sum_A / (total_samples_ABDE_dev / 4 - count_index);
            double pooled_variance_B = weighted_sum_B / (total_samples_ABDE_dev / 4 - count_index);
            double pooled_variance_D = weighted_sum_D / (total_samples_ABDE_dev / 4 - count_index);
            double pooled_variance_E = weighted_sum_E / (total_samples_ABDE_dev / 4 - count_index);
            double pooled_variance_ABDE = weighted_sum_ABDE / (total_samples_ABDE_dev - count_index*4);
            
            //and the reproducability (pooled standard deviation)
            rep_d = Math.Sqrt(pooled_variance_centre_dev);
            rep_A = Math.Sqrt(pooled_variance_A);
            rep_B = Math.Sqrt(pooled_variance_B);
            rep_D = Math.Sqrt(pooled_variance_D);
            rep_E = Math.Sqrt(pooled_variance_E);
            rep_ABDE = Math.Sqrt(pooled_variance_ABDE);
            
        }
        private static double getStandardDeviation(List<double> doubleList)
        {
            if (doubleList.Count <= 1) return 0.0;

            double average = doubleList.Average();
            double sumOfDeviation = 0;
            foreach (double value in doubleList)
            {
                sumOfDeviation += Math.Pow((average - value), 2);
            }
            sumOfDeviation /= (doubleList.Count-1);
            return Math.Sqrt(sumOfDeviation);
        }
        private static double getVariance(List<double> doubleList)
        {
            if (doubleList.Count <= 1) return 0.0;

            double sample_mean = doubleList.Average();
            double sum = 0;
            foreach (double sample in doubleList)
            {
                sum += Math.Pow(sample - sample_mean, 2);
            }
            double variance = sum / (doubleList.Count - 1);
            
            return variance;
        }

        /// <summary>
        /// Calculates the expanded uncertainty for this measurement for deviation measurements  
        /// </summary>
        /// <param name="vfed">A reference to the vertical federal object</param>
        public void CalculateExpandedUncertaintyDeviation(ref VerticalFederal vfed)
        {
            double s_d = 0.0;
            double s_inp = 0.0; //nm
            u_of_g_s(this, ref s_d, ref s_inp);

            //independent terms
            CalculateReproducibility();
            double stdu_reproducability = rep_d*1000; //in nanometres
            double stdu_scale_resolution = vfed.ScaleResStduIndependent; //in nanometers
            double stdu_scale_calibration = vfed.ScaleCalStduIndependent; //in nanometers
            double stdu_length_of_standard_i = s_inp;

            double delta = 0.0;
            u_of_g_ForDeltaIndependent(ref delta,ref vfed);

            double combined_independent = Math.Sqrt(Math.Pow(stdu_reproducability, 2) + Math.Pow(stdu_scale_resolution, 2) + Math.Pow(stdu_scale_calibration, 2) + Math.Pow(stdu_length_of_standard_i, 2) + Math.Pow(delta, 2));

            //dependent terms
            double stdu_length_of_standard_d = s_d;
            double stdu_alpha_g = 0.0;
            double stdu_delta_theta = 0.0;
            double stdu_delta_alpha = 0.0;
            double stdu_theta_s = 0.0;

            u_of_g_ForAlphag(this, ref stdu_alpha_g, ref vfed);
            u_of_g_ForDeltaAlpha(this, ref stdu_delta_alpha, ref vfed);
            u_of_g_ForThetaS(this, ref stdu_theta_s, ref vfed);
            u_of_g_ForDeltaTheta(this, ref stdu_delta_theta, ref vfed);

            double combined_dependent = Math.Sqrt(Math.Pow(stdu_length_of_standard_d, 2) + Math.Pow(stdu_alpha_g, 2) + Math.Pow(stdu_delta_theta, 2) + Math.Pow(stdu_delta_alpha, 2) + Math.Pow(stdu_theta_s, 2));

            double combined_standard_uncertainty = 0.0;
            if (CalibrationGauge.Metric) combined_standard_uncertainty = Math.Sqrt(Math.Pow(combined_independent, 2) + Math.Pow(combined_dependent * CalibrationGauge.Nominal, 2));
            else combined_standard_uncertainty = Math.Sqrt(Math.Pow(combined_independent, 2) + Math.Pow(combined_dependent * CalibrationGauge.Nominal*25.4, 2));

            CalibrationGauge.ExpandedUncertaintyDev = combined_standard_uncertainty * 2;

        }

        /// <summary>
        /// Calculates the expanded uncertainty for this measurement for Extreme deviation measurements  
        /// </summary>
        /// <param name="vfed">A reference to the vertical federal object</param>
        /// <param name="extrema_pos">integer representing where on the gauge the extrema occurs</param>
        public void CalculateExpandedUncertaintyExtremeDeviation(ref VerticalFederal vfed,int extrema_pos)
        {
            double s_d = 0.0;
            double s_inp = 0.0;
            u_of_g_s(this, ref s_d, ref s_inp);

            //independent terms
            CalculateReproducibility();

            //The extrema can occur at any point on the gauge, it is assumed the centrepoint measurements are a different population to corner points.  Therefore the reproducability is calculated as either a pooled
            //standard deviation of repeat measurements on the centre or a pooled standard deviation of repeat measurements on corners A,B,D and E.

            double stdu_reproducability = 0.0;
            if (extrema_pos == 0) stdu_reproducability = rep_d*1000;
            else stdu_reproducability = rep_ABDE*1000;

            double stdu_scale_resolution = vfed.ScaleResStduIndependent; //in micrometers
            double stdu_scale_calibration = vfed.ScaleCalStduIndependent; //in micrometers
            double stdu_length_of_standard_i = s_inp;

            double delta = 0.0;
            u_of_g_ForDeltaIndependent(ref delta, ref vfed);

            double combined_independent = Math.Sqrt(Math.Pow(stdu_reproducability, 2) + Math.Pow(stdu_scale_resolution, 2) + Math.Pow(stdu_scale_calibration, 2) + Math.Pow(stdu_length_of_standard_i, 2) + Math.Pow(delta, 2));

            //dependent terms
            double stdu_length_of_standard_d = s_d;
            double stdu_alpha_g = 0.0;
            double stdu_delta_theta = 0.0;
            double stdu_delta_alpha = 0.0;
            double stdu_theta_s = 0.0;

            u_of_g_ForAlphag(this, ref stdu_alpha_g, ref vfed);
            u_of_g_ForDeltaAlpha(this, ref stdu_delta_alpha, ref vfed);
            u_of_g_ForThetaS(this, ref stdu_theta_s, ref vfed);
            u_of_g_ForDeltaTheta(this, ref stdu_delta_theta, ref vfed);

            double combined_dependent = Math.Sqrt(Math.Pow(stdu_length_of_standard_d, 2) + Math.Pow(stdu_alpha_g, 2) + Math.Pow(stdu_delta_theta, 2) + Math.Pow(stdu_delta_alpha, 2) + Math.Pow(stdu_theta_s, 2));

            double combined_standard_uncertainty = 0.0;
            if (CalibrationGauge.Metric) combined_standard_uncertainty = Math.Sqrt(Math.Pow(combined_independent, 2) + Math.Pow(combined_dependent * CalibrationGauge.Nominal, 2));
            else combined_standard_uncertainty = Math.Sqrt(Math.Pow(combined_independent, 2) + Math.Pow(combined_dependent * CalibrationGauge.Nominal * 25.4, 2));

            CalibrationGauge.ExpandedUncertaintyExtDev = combined_standard_uncertainty * 2;

        }
        /// <summary>
        /// Calculates the expanded uncertainty for this measurement for Variation in Length measurements  
        /// </summary>
        /// <param name="vfed">A reference to the vertical federal object</param>
        /// <param name="max_pos">integer representing where on the gauge the max occurs, 0=the centre, 1 = a corner</param>
        /// <param name="min_pos">integer representing where on the gauge the min occurs</param>
        public void CalculateExpandedUncertaintyVariation(ref VerticalFederal vfed,int max_pos,int min_pos)
        {
            //independent terms
            CalculateReproducibility();

            
            
            if ((max_pos + min_pos) == 0) rep_v = Math.Sqrt(2) * rep_d;
            else if ((max_pos == 0)||(min_pos == 0)) rep_v = Math.Sqrt(Math.Pow(rep_d, 2) + Math.Pow(rep_ABDE, 2));
            else rep_v = Math.Sqrt(2)*rep_ABDE;
            
            
            double stdu_reproducability = rep_v * 1000;
            double stdu_scale_resolution = vfed.ScaleResStduIndependent;
            double stdu_scale_calibration = vfed.ScaleCalStduIndependent;
     

            double combined_independent = Math.Sqrt(Math.Pow(stdu_reproducability, 2) + Math.Pow(stdu_scale_resolution, 2) + Math.Pow(stdu_scale_calibration, 2));

            //dependent terms
            
            double delta_theta_v = 0.0;
            u_of_g_ForDeltaThetaVar(this, ref delta_theta_v, ref vfed);

            double combined_dependent = Math.Sqrt(Math.Pow(delta_theta_v, 2));

            double combined_standard_uncertainty = 0.0;
            if (CalibrationGauge.Metric)
            {
                combined_standard_uncertainty = Math.Sqrt(Math.Pow(combined_independent, 2) + Math.Pow(combined_dependent * CalibrationGauge.Nominal, 2));
            }
            else
            {
                combined_standard_uncertainty = Math.Sqrt(Math.Pow(combined_independent, 2) + Math.Pow(combined_dependent * CalibrationGauge.Nominal*25.4, 2));
            }
            CalibrationGauge.ExpandedUncertaintyVar = combined_standard_uncertainty * 2;

        }

        public void CalculateCMCs(VerticalFederal vfed)
        {
            double dev_dep = vfed.ExpanedUncertaintyCMCDevDep;
            double dev_indep = vfed.ExpanedUncertaintyCMCDevIndep;
            double var_dep = vfed.ExpanedUncertaintyCMCVarDep;
            double var_indep = vfed.ExpanedUncertaintyCMCVarIndep;
            double cmc_dev = 0.0;
            double cmc_var = 0.0;
            if (CalibrationGauge.Metric)
            {
                CalibrationGauge.DeviationCMC = Math.Sqrt(Math.Pow(dev_dep * CalibrationGauge.Nominal, 2) + Math.Pow(dev_indep, 2));
                CalibrationGauge.VariationCMC = Math.Sqrt(Math.Pow(var_dep * CalibrationGauge.Nominal, 2) + Math.Pow(var_indep, 2));
            }
            else
            {
                double cmc_dev_ = Math.Sqrt(Math.Pow(dev_dep * CalibrationGauge.Nominal*25.4, 2) + Math.Pow(dev_indep, 2));
                double cmc_var_ = Math.Sqrt(Math.Pow(var_dep * CalibrationGauge.Nominal * 25.4, 2) + Math.Pow(var_indep, 2));
                CalibrationGauge.DeviationCMC = cmc_dev_;
                CalibrationGauge.VariationCMC = cmc_var_;
            }
        }

        public void CalculateComplianceLimits()
        {
            
            //determine the category row
            nom category = 0;
            
            double limit_deviation = 0;
            double tolerance_variation = 0;
            //Determine what standard we are using to assess compliance (this is chosen by the user)
            if (CalibrationGauge.Metric)
            {
                if (CalibrationGauge.Nominal <= 0.5) category = nom.category0;
                else if (CalibrationGauge.Nominal > 0.5 && CalibrationGauge.Nominal <= 10.0) category = nom.category1;
                else if (CalibrationGauge.Nominal > 10.0 && CalibrationGauge.Nominal <= 25.0) category = nom.category2;
                else if (CalibrationGauge.Nominal > 25.0 && CalibrationGauge.Nominal <= 50.0) category = nom.category3;
                else if (CalibrationGauge.Nominal > 50.0 && CalibrationGauge.Nominal <= 75.0) category = nom.category4;
                else if (CalibrationGauge.Nominal > 75.0 && CalibrationGauge.Nominal <= 100.0) category = nom.category5;

                switch (CalibrationGauge.ComplianceStandard)
                {
                    case (int)ComplianceMetric.BS_EN_ISO_3650_1999_Grade_K:
                        FetchDevVarMetric(category, ComplianceMetric.BS_EN_ISO_3650_1999_Grade_K, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.BS_EN_ISO_3650_1999_Grade_0:
                        FetchDevVarMetric(category, ComplianceMetric.BS_EN_ISO_3650_1999_Grade_0, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.BS_EN_ISO_3650_1999_Grade_1:
                        FetchDevVarMetric(category, ComplianceMetric.BS_EN_ISO_3650_1999_Grade_1, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.BS_EN_ISO_3650_1999_Grade_2:
                        FetchDevVarMetric(category, ComplianceMetric.BS_EN_ISO_3650_1999_Grade_2, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.JIS_B_7506_2004_Grade_K:
                        FetchDevVarMetric(category, ComplianceMetric.JIS_B_7506_2004_Grade_K, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.JIS_B_7506_2004_Grade_0:
                        FetchDevVarMetric(category, ComplianceMetric.JIS_B_7506_2004_Grade_0, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.JIS_B_7506_2004_Grade_1:
                        FetchDevVarMetric(category, ComplianceMetric.JIS_B_7506_2004_Grade_1, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.JIS_B_7506_2004_Grade_2:
                        FetchDevVarMetric(category, ComplianceMetric.JIS_B_7506_2004_Grade_2, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.AS_1457_1999_Grade_K:
                        FetchDevVarMetric(category, ComplianceMetric.AS_1457_1999_Grade_K, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.AS_1457_1999_Grade_0:
                        FetchDevVarMetric(category, ComplianceMetric.AS_1457_1999_Grade_0, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.AS_1457_1999_Grade_1:
                        FetchDevVarMetric(category, ComplianceMetric.AS_1457_1999_Grade_1, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.AS_1457_1999_Grade_2:
                        FetchDevVarMetric(category, ComplianceMetric.AS_1457_1999_Grade_2, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.ASME_B89_1_9_2002_Grade_K:
                        FetchDevVarMetric(category, ComplianceMetric.ASME_B89_1_9_2002_Grade_K, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.ASME_B89_1_9_2002_Grade_0:
                        FetchDevVarMetric(category, ComplianceMetric.ASME_B89_1_9_2002_Grade_00, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.ASME_B89_1_9_2002_Grade_00:
                        FetchDevVarMetric(category, ComplianceMetric.ASME_B89_1_9_2002_Grade_0, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.ASME_B89_1_9_2002_Grade_AS1:
                        FetchDevVarMetric(category, ComplianceMetric.ASME_B89_1_9_2002_Grade_AS1, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceMetric.ASME_B89_1_9_2002_Grade_AS2:
                        FetchDevVarMetric(category, ComplianceMetric.ASME_B89_1_9_2002_Grade_AS2, ref limit_deviation, ref tolerance_variation);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (CalibrationGauge.Nominal <= 0.4) category = nom.category1;
                else if (CalibrationGauge.Nominal > 0.4 && CalibrationGauge.Nominal <= 1.0) category = nom.category2;
                else if (CalibrationGauge.Nominal > 1.0 && CalibrationGauge.Nominal <= 2.0) category = nom.category3;
                else if (CalibrationGauge.Nominal > 2.0 && CalibrationGauge.Nominal <= 3.0) category = nom.category4;
                else if (CalibrationGauge.Nominal > 3.0 && CalibrationGauge.Nominal <= 4.0) category = nom.category5;
                switch (CalibrationGauge.ComplianceStandard)
                {
                    case (int)ComplianceImperial.BS_4311_1_2007_Grade_K:
                        FetchDevVarImperial(category, ComplianceImperial.BS_4311_1_2007_Grade_K, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceImperial.BS_4311_1_2007_Grade_0:
                        FetchDevVarImperial(category, ComplianceImperial.BS_4311_1_2007_Grade_0, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceImperial.BS_4311_1_2007_Grade_1:
                        FetchDevVarImperial(category, ComplianceImperial.BS_4311_1_2007_Grade_1, ref limit_deviation, ref tolerance_variation);
                        break;
                    case (int)ComplianceImperial.BS_4311_1_2007_Grade_2:
                        FetchDevVarImperial(category, ComplianceImperial.BS_4311_1_2007_Grade_2, ref limit_deviation, ref tolerance_variation);
                        break;
                }
            }
            CalibrationGauge.LimitDeviation = limit_deviation;
            CalibrationGauge.ToleranceVariation = tolerance_variation;
        }

        /// <summary>
        /// fetches the allowable deviation and variation for imperial gauges
        /// </summary>
        /// <param name="l">enum representing the length range of the calibration gauge block</param>
        /// <param name="st">enum representing the chosen documentary standard and grade</param>
        /// <param name="ld">the limit deviation fetched</param>
        /// <param name="vr">the variation fetched</param>
        private static void FetchDevVarImperial(nom l, ComplianceImperial st, ref double ld, ref double vr)
        {
            switch (st)
            {
                case ComplianceImperial.BS_4311_1_2007_Grade_K:
                    switch (l)
                    {
                        case nom.category1:
                            ld = 8.0;
                            vr = 2.0;
                            break;
                        case nom.category2:
                            ld = 12.0;
                            vr = 2.0;
                            break;
                        case nom.category3:
                            ld = 16.0;
                            vr = 3.0;
                            break;
                        case nom.category4:
                            ld = 20.0;
                            vr = 3.0;
                            break;
                        case nom.category5:
                            ld = 24.0;
                            vr = 3.0;
                            break;
                    }
                    break;
                case ComplianceImperial.BS_4311_1_2007_Grade_0:
                    switch (l)
                    {
                        case nom.category1:
                            ld = 5.0;
                            vr = 4.0;
                            break;
                        case nom.category2:
                            ld = 6.0;
                            vr = 4.0;
                            break;
                        case nom.category3:
                            ld = 8.0;
                            vr = 4.0;
                            break;
                        case nom.category4:
                            ld = 10.0;
                            vr = 5.0;
                            break;
                        case nom.category5:
                            ld = 12.0;
                            vr = 5.0;
                            break;
                    }
                    break;
                case ComplianceImperial.BS_4311_1_2007_Grade_1:
                    switch (l)
                    {
                        case nom.category1:
                            ld = 8.0;
                            vr = 6.0;
                            break;
                        case nom.category2:
                            ld = 12.0;
                            vr = 6.0;
                            break;
                        case nom.category3:
                            ld = 16.0;
                            vr = 7.0;
                            break;
                        case nom.category4:
                            ld = 20.0;
                            vr = 7.0;
                            break;
                        case nom.category5:
                            ld = 24.0;
                            vr = 8.0;
                            break;
                    }
                    break;
                case ComplianceImperial.BS_4311_1_2007_Grade_2:
                    switch (l)
                    {
                        case nom.category1:
                            ld = 18.0;
                            vr = 12.0;
                            break;
                        case nom.category2:
                            ld = 24.0;
                            vr = 12.0;
                            break;
                        case nom.category3:
                            ld = 32.0;
                            vr = 12.0;
                            break;
                        case nom.category4:
                            ld = 40.0;
                            vr = 14.0;
                            break;
                        case nom.category5:
                            ld = 48.0;
                            vr = 14.0;
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// fetches the allowable deviation and variation for metric gauges
        /// </summary>
        /// <param name="l">enum representing the length range of the calibration gauge block</param>
        /// <param name="st">enum representing the chosen documentary standard and grade</param>
        /// <param name="ld">the limit deviation fetched</param>
        /// <param name="vr">the variation fetched</param>
        private static void FetchDevVarMetric(nom l, ComplianceMetric st, ref double ld, ref double vr)
        {
            switch (st)
            {
                case ComplianceMetric.ASME_B89_1_9_2002_Grade_K:
                    switch(l){

                        case nom.category0:
                            ld = 0.3;
                            vr = 0.05;
                            break;
                        case nom.category1:
                            ld = 0.2;
                            vr = 0.05;
                            break;
                        case nom.category2:
                            ld = 0.3;
                            vr = 0.05;
                            break;
                        case nom.category3:
                            ld = 0.4;
                            vr = 0.06;
                            break;
                        case nom.category4:
                            ld = 0.5;
                            vr = 0.06;
                            break;
                        case nom.category5:
                            ld = 0.6;
                            vr = 0.07;
                            break;
                    }
                    break;
                case ComplianceMetric.ASME_B89_1_9_2002_Grade_00:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.1;
                            vr = 0.05;
                            break;
                        case nom.category1:
                            ld = 0.07;
                            vr = 0.05;
                            break;
                        case nom.category2:
                            ld = 0.07;
                            vr = 0.05;
                            break;
                        case nom.category3:
                            ld = 0.1;
                            vr = 0.06;
                            break;
                        case nom.category4:
                            ld = 0.12;
                            vr = 0.07;
                            break;
                        case nom.category5:
                            ld = 0.15;
                            vr = 0.07;
                            break;
                    }
                    break;
                case ComplianceMetric.ASME_B89_1_9_2002_Grade_0:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.14;
                            vr = 0.1;
                            break;
                        case nom.category1:
                            ld = 0.12;
                            vr = 0.1;
                            break;
                        case nom.category2:
                            ld = 0.14;
                            vr = 0.1;
                            break;
                        case nom.category3:
                            ld = 0.20;
                            vr = 0.1;
                            break;
                        case nom.category4:
                            ld = 0.25;
                            vr = 0.12;
                            break;
                        case nom.category5:
                            ld = 0.30;
                            vr = 0.12;
                            break;
                    }
                    break;
                case ComplianceMetric.ASME_B89_1_9_2002_Grade_AS1:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.3;
                            vr = 0.16;
                            break;
                        case nom.category1:
                            ld = 0.2;
                            vr = 0.16;
                            break;
                        case nom.category2:
                            ld = 0.3;
                            vr = 0.16;
                            break;
                        case nom.category3:
                            ld = 0.4;
                            vr = 0.18;
                            break;
                        case nom.category4:
                            ld = 0.5;
                            vr = 0.18;
                            break;
                        case nom.category5:
                            ld = 0.6;
                            vr = 0.20;
                            break;
                    }
                    break;
                case ComplianceMetric.ASME_B89_1_9_2002_Grade_AS2:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.6;
                            vr = 0.3;
                            break;
                        case nom.category1:
                            ld = 0.45;
                            vr = 0.3;
                            break;
                        case nom.category2:
                            ld = 0.60;
                            vr = 0.3;
                            break;
                        case nom.category3:
                            ld = 0.80;
                            vr = 0.3;
                            break;
                        case nom.category4:
                            ld = 1.0;
                            vr = 0.35;
                            break;
                        case nom.category5:
                            ld = 1.2;
                            vr = 0.35;
                            break;
                    }
                    break;
                case ComplianceMetric.AS_1457_1999_Grade_K:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.2;
                            vr = 0.05;
                            break;
                        case nom.category1:
                            ld = 0.2;
                            vr = 0.05;
                            break;
                        case nom.category2:
                            ld = 0.3;
                            vr = 0.05;
                            break;
                        case nom.category3:
                            ld = 0.4;
                            vr = 0.06;
                            break;
                        case nom.category4:
                            ld = 0.5;
                            vr = 0.06;
                            break;
                        case nom.category5:
                            ld = 0.6;
                            vr = 0.07;
                            break;
                    }
                    break;
                case ComplianceMetric.AS_1457_1999_Grade_0:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.12;
                            vr = 0.1;
                            break;
                        case nom.category1:
                            ld = 0.12;
                            vr = 0.1;
                            break;
                        case nom.category2:
                            ld = 0.14;
                            vr = 0.1;
                            break;
                        case nom.category3:
                            ld = 0.2;
                            vr = 0.1;
                            break;
                        case nom.category4:
                            ld = 0.25;
                            vr = 0.12;
                            break;
                        case nom.category5:
                            ld = 0.30;
                            vr = 0.12;
                            break;
                    }
                    break;
                case ComplianceMetric.AS_1457_1999_Grade_1:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.2;
                            vr = 0.16;
                            break;
                        case nom.category1:
                            ld = 0.2;
                            vr = 0.16;
                            break;
                        case nom.category2:
                            ld = 0.3;
                            vr = 0.16;
                            break;
                        case nom.category3:
                            ld = 0.4;
                            vr = 0.18;
                            break;
                        case nom.category4:
                            ld = 0.5;
                            vr = 0.18;
                            break;
                        case nom.category5:
                            ld = 0.6;
                            vr = 0.2;
                            break;
                    }
                    break;
                case ComplianceMetric.AS_1457_1999_Grade_2:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.45;
                            vr = 0.3;
                            break;
                        case nom.category1:
                            ld = 0.45;
                            vr = 0.3;
                            break;
                        case nom.category2:
                            ld = 0.6;
                            vr = 0.3;
                            break;
                        case nom.category3:
                            ld = 0.8;
                            vr = 0.3;
                            break;
                        case nom.category4:
                            ld = 1.0;
                            vr = 0.35;
                            break;
                        case nom.category5:
                            ld = 1.2;
                            vr = 0.35;
                            break;
                    }
                    break;
                case ComplianceMetric.BS_EN_ISO_3650_1999_Grade_K:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.2;
                            vr = 0.05;
                            break;
                        case nom.category1:
                            ld = 0.2;
                            vr = 0.05;
                            break;
                        case nom.category2:
                            ld = 0.3;
                            vr = 0.05;
                            break;
                        case nom.category3:
                            ld = 0.4;
                            vr = 0.06;
                            break;
                        case nom.category4:
                            ld = 0.5;
                            vr = 0.06;
                            break;
                        case nom.category5:
                            ld = 0.6;
                            vr = 0.07;
                            break;
                    }
                    break;
                case ComplianceMetric.BS_EN_ISO_3650_1999_Grade_0:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.12;
                            vr = 0.1;
                            break;
                        case nom.category1:
                            ld = 0.12;
                            vr = 0.1;
                            break;
                        case nom.category2:
                            ld = 0.14;
                            vr = 0.1;
                            break;
                        case nom.category3:
                            ld = 0.2;
                            vr = 0.1;
                            break;
                        case nom.category4:
                            ld = 0.25;
                            vr = 0.12;
                            break;
                        case nom.category5:
                            ld = 0.30;
                            vr = 0.12;
                            break;
                    }
                    break;
                case ComplianceMetric.BS_EN_ISO_3650_1999_Grade_1:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.2;
                            vr = 0.16;
                            break;
                        case nom.category1:
                            ld = 0.2;
                            vr = 0.16;
                            break;
                        case nom.category2:
                            ld = 0.3;
                            vr = 0.16;
                            break;
                        case nom.category3:
                            ld = 0.4;
                            vr = 0.18;
                            break;
                        case nom.category4:
                            ld = 0.5;
                            vr = 0.18;
                            break;
                        case nom.category5:
                            ld = 0.6;
                            vr = 0.2;
                            break;
                    }
                    break;
                case ComplianceMetric.BS_EN_ISO_3650_1999_Grade_2:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.45;
                            vr = 0.3;
                            break;
                        case nom.category1:
                            ld = 0.45;
                            vr = 0.3;
                            break;
                        case nom.category2:
                            ld = 0.6;
                            vr = 0.3;
                            break;
                        case nom.category3:
                            ld = 0.8;
                            vr = 0.3;
                            break;
                        case nom.category4:
                            ld = 1.0;
                            vr = 0.35;
                            break;
                        case nom.category5:
                            ld = 1.2;
                            vr = 0.35;
                            break;
                    }
                    break;
                case ComplianceMetric.JIS_B_7506_2004_Grade_K:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.2;
                            vr = 0.05;
                            break;
                        case nom.category1:
                            ld = 0.2;
                            vr = 0.05;
                            break;
                        case nom.category2:
                            ld = 0.3;
                            vr = 0.05;
                            break;
                        case nom.category3:
                            ld = 0.4;
                            vr = 0.06;
                            break;
                        case nom.category4:
                            ld = 0.5;
                            vr = 0.06;
                            break;
                        case nom.category5:
                            ld = 0.6;
                            vr = 0.07;
                            break;
                    }
                    break;
                case ComplianceMetric.JIS_B_7506_2004_Grade_0:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.12;
                            vr = 0.1;
                            break;
                        case nom.category1:
                            ld = 0.12;
                            vr = 0.1;
                            break;
                        case nom.category2:
                            ld = 0.14;
                            vr = 0.1;
                            break;
                        case nom.category3:
                            ld = 0.20;
                            vr = 0.1;
                            break;
                        case nom.category4:
                            ld = 0.25;
                            vr = 0.12;
                            break;
                        case nom.category5:
                            ld = 0.3;
                            vr = 0.12;
                            break;
                    }
                    break;
                case ComplianceMetric.JIS_B_7506_2004_Grade_1:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.2;
                            vr = 0.16;
                            break;
                        case nom.category1:
                            ld = 0.2;
                            vr = 0.16;
                            break;
                        case nom.category2:
                            ld = 0.3;
                            vr = 0.16;
                            break;
                        case nom.category3:
                            ld = 0.4;
                            vr = 0.18;
                            break;
                        case nom.category4:
                            ld = 0.5;
                            vr = 0.18;
                            break;
                        case nom.category5:
                            ld = 0.6;
                            vr = 0.2;
                            break;
                    }
                    break;
                case ComplianceMetric.JIS_B_7506_2004_Grade_2:
                    switch (l)
                    {
                        case nom.category0:
                            ld = 0.45;
                            vr = 0.3;
                            break;
                        case nom.category1:
                            ld = 0.45;
                            vr = 0.3;
                            break;
                        case nom.category2:
                            ld = 0.6;
                            vr = 0.3;
                            break;
                        case nom.category3:
                            ld = 0.8;
                            vr = 0.3;
                            break;
                        case nom.category4:
                            ld = 1.0;
                            vr = 0.35;
                            break;
                        case nom.category5:
                            ld = 1.2;
                            vr = 0.35;
                            break;
                    }
                    break;
            }
        }

        

        /// <summary>
        /// Determines the standard uncertainty component u(g) for s
        /// </summary>
        /// <param name="m">The measurement for which the standard uncertainty component is determined</param>
        /// <param name="u_g_s_dependent">The component to be calculated</param>
        /// <param name="u_g_s_independent">The component to be calculated</param>
        private void u_of_g_s(Measurement m, ref double u_g_s_dependent, ref double u_g_s_independent)
        {

            //case1 (singleton ref gauge) 
            if (m.ReferenceStack.Count == 1)
            {
                u_g_s_dependent = ReferenceStack.Gauge1.GaugeStdU_Dep;
                u_g_s_independent = ReferenceStack.Gauge1.GaugeStdU_Indp;
            }
            //case2 (two wrung ref gauges) 
            else if (m.ReferenceStack.Count == 2)
            {
                double max_wringing_film = 0.0;
                //determine which gauge in the stack has the largest uncertainty for the wringing film.
                if (m.ReferenceStack.Gauge1.WringingFilm >= m.ReferenceStack.Gauge2.WringingFilm) max_wringing_film = m.ReferenceStack.Gauge1.WringingFilm;
                else max_wringing_film = m.ReferenceStack.Gauge1.WringingFilm;
                

                //compute the  weighted mean of the standard uncertainties that make up the gauge block stack.  The weighting is done via nominal length
                u_g_s_dependent = ((m.ReferenceStack.Gauge1.Nominal * m.ReferenceStack.Gauge1.GaugeStdU_Dep) + (m.ReferenceStack.Gauge2.Nominal * m.ReferenceStack.Gauge2.GaugeStdU_Dep)) / (m.ReferenceStack.Gauge1.Nominal + m.ReferenceStack.Gauge2.Nominal);
                double standard_gauge_calibration_indp = Math.Sqrt(2) * (((m.ReferenceStack.Gauge1.Nominal * m.ReferenceStack.Gauge1.GaugeStdU_Indp) + (m.ReferenceStack.Gauge2.Nominal * m.ReferenceStack.Gauge2.GaugeStdU_Indp)) / (m.ReferenceStack.Gauge1.Nominal + m.ReferenceStack.Gauge2.Nominal));
                u_g_s_independent = Math.Sqrt(Math.Pow(standard_gauge_calibration_indp, 2) + Math.Pow(max_wringing_film, 2));
            }
            //case3 (three wrung ref gauges) 
            else if (m.ReferenceStack.Count == 3)
            {
                double max_wringing_film = 0.0;
                //determine which gauge in the stack has the largest uncertainty for the wringing film.
                if (m.ReferenceStack.Gauge1.WringingFilm >= m.ReferenceStack.Gauge2.WringingFilm)
                {
                    if (m.ReferenceStack.Gauge1.WringingFilm >= m.ReferenceStack.Gauge3.WringingFilm) max_wringing_film = m.ReferenceStack.Gauge1.WringingFilm;
                    else max_wringing_film = m.ReferenceStack.Gauge3.WringingFilm;
                }
                else
                {
                    if (m.ReferenceStack.Gauge2.WringingFilm >= m.ReferenceStack.Gauge3.WringingFilm) max_wringing_film = m.ReferenceStack.Gauge2.WringingFilm;
                    else max_wringing_film = m.ReferenceStack.Gauge3.WringingFilm;
                }

                //compute the  weighted mean of the standard uncertainties that make up the gauge block stack.  The weighting is done via nominal length
                u_g_s_dependent = ((m.ReferenceStack.Gauge1.Nominal * m.ReferenceStack.Gauge1.GaugeStdU_Dep) + (m.ReferenceStack.Gauge2.Nominal * m.ReferenceStack.Gauge2.GaugeStdU_Dep) + (m.ReferenceStack.Gauge3.Nominal * m.ReferenceStack.Gauge3.GaugeStdU_Dep)) / (m.ReferenceStack.Gauge1.Nominal + m.ReferenceStack.Gauge2.Nominal + m.ReferenceStack.Gauge3.Nominal);
                double standard_gauge_calibration_indp = Math.Sqrt(3) * (((m.ReferenceStack.Gauge1.Nominal * m.ReferenceStack.Gauge1.GaugeStdU_Indp) + (m.ReferenceStack.Gauge2.Nominal * m.ReferenceStack.Gauge2.GaugeStdU_Indp) + (m.ReferenceStack.Gauge3.Nominal * m.ReferenceStack.Gauge3.GaugeStdU_Indp)) / (m.ReferenceStack.Gauge1.Nominal + m.ReferenceStack.Gauge2.Nominal + m.ReferenceStack.Gauge3.Nominal));
                max_wringing_film = max_wringing_film * Math.Sqrt(2);
                u_g_s_independent = Math.Sqrt(Math.Pow(standard_gauge_calibration_indp, 2) + Math.Pow(max_wringing_film, 2));
            }
        }

        /// <summary>
        /// Determines the standard uncertainty component u(g) for Delta
        /// </summary>
        /// <param name="u_g_delta_independent">The component to be calculated</param>
        /// <param name="vfederal">The vertical federal object which contains the uncertainty in alpha g</param> 
        private double u_of_g_ForDeltaIndependent(ref double u_g_delta_independent, ref VerticalFederal vfederal)
        {
            u_g_delta_independent = vfederal.u_Delta;
            return u_g_delta_independent;
        }
        /// <summary>
        /// Determines the standard uncertainty component u(g) for alpha g
        /// </summary>
        /// <param name="m">The measurement for which the standard uncertainty component is determined</param>
        /// <param name="u_g_alpha_g">The component to be calculated</param>
        /// <param name="vfederal">The vertical federal object which contains the uncertainty in alpha g</param> 
        private double u_of_g_ForAlphag(Measurement m, ref double u_g_alpha_g, ref VerticalFederal vfederal)
        {
            u_g_alpha_g = vfederal.u_AlphaG * vfederal.u_DeltaTheta;
            return u_g_alpha_g;
        }

        /// <summary>
        /// Determines the standard uncertainty component u(g) for delta alpha
        /// </summary>
        /// <param name="m">The measurement for which the standard uncertainty component is determined</param>
        /// <param name="u_g_delta_alpha">The component to be calculated</param>
        /// <param name="vfederal">The vertical federal object which contains the uncertainty in delta alpha</param> 
        private double u_of_g_ForDeltaAlpha(Measurement m, ref double u_g_delta_alpha, ref VerticalFederal vfederal)
        {
            double utheta_s = RMS_Theta_S();
            u_g_delta_alpha = vfederal.u_DeltaAlpha * utheta_s;

            return u_g_delta_alpha;
        }

        /// <summary>
        /// Determines the standard uncertainty component u(g) for theta_s
        /// </summary>
        /// <param name="m">The measurement for which the standard uncertainty component is determined</param>
        /// <param name="u_g_theta_s">The component to be calculated</param>
        /// <param name="vfederal">The vertical federal object which contains the uncertainty in delta alpha</param> 
        private double u_of_g_ForThetaS(Measurement m, ref double u_g_theta_s, ref VerticalFederal vfederal)
        {
            double delta_alpha = 0.0;
            //case1 (singleton ref gauge) 
            if (m.ReferenceStack.Count == 1)
            {
                delta_alpha = Math.Abs(Math.Round(m.CalibrationGauge.GaugeBlockMaterial.exp_coeff, 2) - Math.Round(m.ReferenceStack.Gauge1.GaugeBlockMaterial.exp_coeff, 2));
            }
            //case2 (two wrung ref gauges) 
            else if (m.ReferenceStack.Count == 2)
            {
                //compute the  weighted mean of the expansion coefficient for gauges that make up the stack.  The weighting is done via nominal length
                double ref_g_exp_c = 0.0;
                delta_alpha = 0.0;
                ref_g_exp_c = ((m.ReferenceStack.Gauge1.Nominal * m.ReferenceStack.Gauge1.GaugeBlockMaterial.exp_coeff) + (m.ReferenceStack.Gauge2.Nominal * m.ReferenceStack.Gauge2.GaugeBlockMaterial.exp_coeff)) / (m.ReferenceStack.Gauge1.Nominal + m.ReferenceStack.Gauge2.Nominal);

                delta_alpha = Math.Abs(Math.Round(m.CalibrationGauge.GaugeBlockMaterial.exp_coeff, 2) - Math.Round(ref_g_exp_c, 2));

            }
            //case3 (three wrung ref gauges) 
            else if (m.ReferenceStack.Count == 3)
            {
                //compute the  weighted mean of the expansion coefficient for gauges that make up the stack.  The weighting is done via nominal length
                double ref_g_exp_c = 0.0;
                ref_g_exp_c = ((m.ReferenceStack.Gauge1.Nominal * m.ReferenceStack.Gauge1.GaugeBlockMaterial.exp_coeff) + (m.ReferenceStack.Gauge2.Nominal * m.ReferenceStack.Gauge2.GaugeBlockMaterial.exp_coeff) + (m.ReferenceStack.Gauge3.Nominal * m.ReferenceStack.Gauge3.GaugeBlockMaterial.exp_coeff)) / (m.ReferenceStack.Gauge1.Nominal + m.ReferenceStack.Gauge2.Nominal + m.ReferenceStack.Gauge3.Nominal);
                delta_alpha = Math.Abs(Math.Round(m.CalibrationGauge.GaugeBlockMaterial.exp_coeff, 2) - Math.Round(ref_g_exp_c, 2));
            }

            if (delta_alpha == 0.0) u_g_theta_s = RMS_Theta_S()*vfederal.u_DeltaAlpha;
            else u_g_theta_s = RMS_Theta_S() * delta_alpha;

            return u_g_theta_s;
        }

        /// <summary>
        /// Determines the standard uncertainty component u(g) for delta theta
        /// </summary>
        /// <param name="m">The measurement for which the standard uncertainty component is determined</param>
        /// <param name="u_g_delta_theta">The component to be calculated</param>
        /// <param name="vfederal">The vertical federal object which contains the uncertainty in delta theta</param> 
        private double u_of_g_ForDeltaTheta(Measurement m, ref double u_g_delta_theta, ref VerticalFederal vfederal)
        {
            u_g_delta_theta = vfederal.u_DeltaTheta * m.CalibrationGauge.GaugeBlockMaterial.exp_coeff;
            return u_g_delta_theta;
        }

        /// <summary>
        /// Determines the standard uncertainty component u(g) for delta theta
        /// </summary>
        /// <param name="m">The measurement for which the standard uncertainty component is determined</param>
        /// <param name="u_g_delta_theta">The component to be calculated</param>
        /// <param name="vfederal">The vertical federal object which contains the uncertainty in delta theta</param> 
        private double u_of_g_ForDeltaThetaVar(Measurement m, ref double u_g_delta_theta_var, ref VerticalFederal vfederal)
        {
            u_g_delta_theta_var = vfederal.u_DeltaThetaVar * m.CalibrationGauge.GaugeBlockMaterial.exp_coeff;
            return u_g_delta_theta_var;
        }

        /// <summary>
        /// Determines the root mean square of the departure from 20 degrees for all measurements
        /// </summary>
        private static double RMS_Theta_S()
        {
            double sumsq_theta_s = 0.0;
            int count_theta_s = 0;

            foreach(Measurement m in measurements)
            {  
                sumsq_theta_s += Math.Pow(m.CalibrationGauge.Temperature - 20.000,2);
                count_theta_s++; 
            }
            return Math.Sqrt(sumsq_theta_s / count_theta_s);
        }

        /// <summary>
        /// Parse the file opened by the user line by line while updating the gui
        /// </summary>
        /// <param name="lines">A string array containing lines of the measurement file</param>
        /// <param name="verticalFederal">A reference to the vertical federal object</param>
        public static bool ParseFile(string[] lines, ref VerticalFederal verticalFederal)
        {
            bool first_line = true;
            bool second_line = true;
            string line_read;

            foreach (string line in lines)
            {
                //the first line is a title line and doesn't contain data
                if (first_line)
                {
                    first_line = false;
                    continue;
                }
                //the second line doesn't contain data either, it just shows the revision number
                if (second_line)
                {
                    second_line = false;
                    continue;
                }
                line_read = (string)line.Clone();

                Measurement meas = new Measurement();

                ParseLine(ref line_read, ref meas, ref verticalFederal);


                int set_index = 0;

                working_gauge.Nominal = meas.CalibrationGauge.Nominal;
                working_gauge.CentreDeviation = meas.CalibrationGauge.CentreDeviation;
                working_gauge.ClientName = meas.CalibrationGauge.ClientName;
                working_gauge.CorrLength = meas.CalibrationGauge.CorrLength;
                working_gauge.ExtremeDeviation = meas.CalibrationGauge.ExtremeDeviation;
                working_gauge.FromSet = meas.CalibrationGauge.FromSet;
                working_gauge.GaugeBlockMaterial.exp_coeff = meas.CalibrationGauge.GaugeBlockMaterial.exp_coeff;
                working_gauge.GaugeBlockMaterial.poissons_ratio = meas.CalibrationGauge.GaugeBlockMaterial.poissons_ratio;
                working_gauge.GaugeBlockMaterial.youngs_modulus = meas.CalibrationGauge.GaugeBlockMaterial.youngs_modulus;
                working_gauge.GaugeBlockMaterial.material = (string)meas.CalibrationGauge.GaugeBlockMaterial.material.Clone();
                working_gauge.IllegalSize = false;
                working_gauge.MaxDev = meas.CalibrationGauge.MaxDev;
                working_gauge.Metric = meas.CalibrationGauge.Metric;
                working_gauge.MinDev = meas.CalibrationGauge.MinDev;
                working_gauge.SerialNumber = meas.CalibrationGauge.SerialNumber;
                working_gauge.Variation = meas.CalibrationGauge.Variation;
                working_gauge.Temperature = meas.CalibrationGauge.Temperature;
                working_gauge.DeviationCMC = meas.CalibrationGauge.DeviationCMC;
                working_gauge.VariationCMC = meas.CalibrationGauge.VariationCMC;
                CreateNewCalSet(ref set_index);

                //add the working gauge to its calibration gauge set
                calibration_gauge_sets[set_index].GaugeList.Add(meas.CalibrationGauge);
                calibration_gauge_sets[set_index].NumGauges++;
            }
            return true;
        }
        /// <summary>
        /// Parse a single line of the file opened by the user. 
        /// Metadata read from the file will be used to construct a new measurement object
        /// </summary>
        /// <param name="line">A string containing meta data for a single measurement object</param>
        /// <param name="meas">A to the measurement object to be updated with meta data</param>
        /// <param name="verticalFederal">A reference to the vertical federal object</param>
        private static void ParseLine(ref string line, ref Measurement meas, ref VerticalFederal verticalFederal)
        {
            //Populate the measurement with data we find in this files current line
            Stack ref_stack = new Stack(1);
            GaugeBlock calibration_gauge = new GaugeBlock(false);
            meas.CalibrationGauge = calibration_gauge;
            
            meas.ReferenceStack = ref_stack;
            bool metric = true;


            string[] strings = line.Split(',');
            meas.Datetime = strings[0];

            double m = 0.0;
            double.TryParse(strings[1], out m);
            meas.CalibrationGauge.Temperature = m;

            if (strings[2].Contains("mm")) meas.CalibrationGauge.Metric = true;
            else meas.CalibrationGauge.Metric = false;

            double n = 0.0;
            double.TryParse(strings[3], out n);
            meas.CalibrationGauge.Nominal = n;

            meas.CalibrationGauge.FromSet = strings[4];
            meas.CalibrationGauge.SerialNumber = strings[5];
            meas.CalibrationGauge.GaugeBlockMaterial.material = strings[6];
            meas.CalibrationGauge.GaugeBlockMaterial.exp_coeff = Convert.ToDouble(strings[7]);
            meas.CalibrationGauge.GaugeBlockMaterial.youngs_modulus = Convert.ToDouble(strings[8]);
            meas.CalibrationGauge.GaugeBlockMaterial.poissons_ratio = Convert.ToDouble(strings[9]);

            ref_stack.Gauge1.Nominal = Convert.ToDouble(strings[10]);
            ref_stack.Gauge1.CentreDeviation = Convert.ToDouble(strings[11]);
            ref_stack.Gauge1.FromSet = strings[12];


            bool.TryParse(strings[13], out metric);
            ref_stack.Gauge1.Metric = metric;
            GaugeBlockSet gb_set = GetGaugeSet(ref_stack.Gauge1.FromSet);
            foreach(GaugeBlock gb in gb_set.GaugeList)
            {
                if(gb.Nominal == ref_stack.Gauge1.Nominal)
                {
                    ref_stack.Gauge1.GaugeStdU_Dep = gb.GaugeStdU_Dep;
                    ref_stack.Gauge1.GaugeStdU_Indp = gb.GaugeStdU_Indp;
                    ref_stack.Gauge1.WringingFilm = gb.WringingFilm;
                }
            }
            ref_stack.Gauge1.SerialNumber = strings[14];
            ref_stack.Gauge1.GaugeBlockMaterial.material = strings[15];
            ref_stack.Gauge1.GaugeBlockMaterial.exp_coeff = Convert.ToDouble(strings[16]);
            ref_stack.Gauge1.GaugeBlockMaterial.youngs_modulus = Convert.ToDouble(strings[17]);
            ref_stack.Gauge1.GaugeBlockMaterial.poissons_ratio = Convert.ToDouble(strings[18]);
            ref_stack.Gauge1 = ref_stack.Gauge1;

            if (!strings[19].Equals(""))
            {
                ref_stack.Gauge2 = new GaugeBlock(false);
                ref_stack.Gauge2.Nominal = Convert.ToDouble(strings[19]);
                ref_stack.Gauge2.CentreDeviation = Convert.ToDouble(strings[20]);
                ref_stack.Gauge2.FromSet = strings[21];

                bool.TryParse(strings[22], out metric);
                ref_stack.Gauge2.Metric = metric;
                ref_stack.Gauge2.SerialNumber = strings[23];
                ref_stack.Gauge2.GaugeBlockMaterial.material = strings[24];
                ref_stack.Gauge2.GaugeBlockMaterial.exp_coeff = Convert.ToDouble(strings[25]);
                ref_stack.Gauge2.GaugeBlockMaterial.youngs_modulus = Convert.ToDouble(strings[26]);
                ref_stack.Gauge2.GaugeBlockMaterial.poissons_ratio = Convert.ToDouble(strings[27]);
                GaugeBlockSet gb_set_2 = GetGaugeSet(ref_stack.Gauge2.FromSet);
                foreach (GaugeBlock gb in gb_set_2.GaugeList)
                {
                    if (gb.Nominal == ref_stack.Gauge2.Nominal)
                    {
                        ref_stack.Gauge2.GaugeStdU_Dep = gb.GaugeStdU_Dep;
                        ref_stack.Gauge2.GaugeStdU_Indp = gb.GaugeStdU_Indp;
                        ref_stack.Gauge2.WringingFilm = gb.WringingFilm;
                    }
                }
            }

            if (!strings[28].Equals(""))
            {
                ref_stack.Gauge3 = new GaugeBlock(false);
                ref_stack.Gauge3.Nominal = Convert.ToDouble(strings[28]);
                ref_stack.Gauge3.CentreDeviation = Convert.ToDouble(strings[29]);
                ref_stack.Gauge3.FromSet = strings[30];

                bool.TryParse(strings[31], out metric);
                ref_stack.Gauge3.Metric = metric;
                ref_stack.Gauge3.SerialNumber = strings[32];
                ref_stack.Gauge3.GaugeBlockMaterial.material = strings[33];
                ref_stack.Gauge3.GaugeBlockMaterial.exp_coeff = Convert.ToDouble(strings[34]);
                ref_stack.Gauge3.GaugeBlockMaterial.youngs_modulus = Convert.ToDouble(strings[35]);
                ref_stack.Gauge3.GaugeBlockMaterial.poissons_ratio = Convert.ToDouble(strings[36]);
                GaugeBlockSet gb_set_3 = GetGaugeSet(ref_stack.Gauge3.FromSet);
                foreach (GaugeBlock gb in gb_set_3.GaugeList)
                {
                    if (gb.Nominal == ref_stack.Gauge3.Nominal)
                    {
                        ref_stack.Gauge3.GaugeStdU_Dep = gb.GaugeStdU_Dep;
                        ref_stack.Gauge3.GaugeStdU_Indp = gb.GaugeStdU_Indp;
                        ref_stack.Gauge3.WringingFilm = gb.WringingFilm;
                    }
                }
            }

            double.TryParse(strings[37], out meas.reference_deviation);
            double.TryParse(strings[38], out meas.r1);
            double.TryParse(strings[39], out meas.c1);
            double.TryParse(strings[40], out meas.a);
            double.TryParse(strings[41], out meas.b);
            double.TryParse(strings[42], out meas.c2);
            double.TryParse(strings[43], out meas.d);
            double.TryParse(strings[44], out meas.e);
            double.TryParse(strings[45], out meas.c3);
            double.TryParse(strings[46], out meas.r2);

            double c_length = 0.0;
            double centre_d = 0.0;
            double min_d = 0.0;
            double max_d = 0.0;
            double ext_d = 0.0;
            double v = 0.0;

            double.TryParse(strings[47], out c_length);
            double.TryParse(strings[48], out centre_d);
            double.TryParse(strings[49], out min_d);
            double.TryParse(strings[50], out max_d);
            double.TryParse(strings[51], out ext_d);
            double.TryParse(strings[52], out v);

            meas.CalibrationGauge.CorrLength = c_length;
            meas.CalibrationGauge.CentreDeviation = centre_d;
            meas.CalibrationGauge.MinDev = min_d;
            meas.CalibrationGauge.MaxDev = max_d;
            meas.CalibrationGauge.ExtremeDeviation = ext_d;
            meas.CalibrationGauge.Variation = v;
        

          
            int comp_std = 0;
            int.TryParse(strings[53], out comp_std);
            meas.CalibrationGauge.ComplianceStandard = comp_std;
            meas.CalculateCMCs(verticalFederal);
            meas.CalculateComplianceLimits();

            //Save a copy of the current measurement to the measurement list.
            Measurement.Measurements.Add(meas);
        }

        public static GaugeBlockSet GetGaugeSet(string set_name)
        {
            //if we have reference gauge sets already in the list then see if the set has previously been added (i.e is this a unique serial number)
            foreach (GaugeBlockSet ref_set in reference_gauge_sets)
            {
                if (ref_set.GaugeSetName.Equals(set_name))
                {
                    return ref_set;
                }
            }
            //we haven't added the set to the set list so add it now.
            GaugeBlockSet gauge_set = new GaugeBlockSet();
            gauge_set.GaugeSetName = set_name;
            reference_gauge_sets.Add(gauge_set);

            //Add all gauges found in the xml file to the reference gauge set.
            INI2XML.LoadReferenceGauges(ref gauge_set);

            return gauge_set;   
        }

        /// <summary>
        /// Write a line of data for this measurement to the gauge results rich text box.
        /// </summary>
        /// <param name="current_measurement">The current measurement object</param>
        /// <param name="units">the unit string, metric or imperial</param>
        /// <param name="gaugeResultsRichTextBox">a reference to the gauge results rich text box</param>
        /// <param name="measurement_number">the nth measurement number</param> 
        public static string writeRichTBLine(Measurement current_measurement, string units, int measurement_number)
        {
            string rtb_line = "";
            if (!Measurement.HeaderWritten)
            {
                //write the header string of the output rich text box
                rtb_line = "Measurement No.\tUnits\tNominal\tCentre Dev\tExtreme Dev\tVariation\n";
                Measurement.HeaderWritten = true;
            }

            rtb_line = String.Concat(rtb_line, measurement_number.ToString(), "\t",
                units, "\t", current_measurement.CalibrationGauge.Nominal.ToString(), "\t", Math.Round(current_measurement.CalibrationGauge.CentreDeviation, 5).ToString(),
                "\t", Math.Round(current_measurement.CalibrationGauge.ExtremeDeviation, 5).ToString(), "\t" + Math.Round(current_measurement.CalibrationGauge.Variation, 5).ToString(), "\n");
            return rtb_line;
        }


        /// <summary>
        /// Make a string which has all measurement data delimited by commas, each measurement is recorded as a line in the file
        /// </summary>
        /// <param name="current_measurement">The current measurement object</param>
        /// <param name="line_to_write">a reference to the line string</param>
        /// <param name="units">a reference to units string, either metric or imperial</param> 
        public static void PrepareLineForWrite(Measurement current_measurement, ref string line_to_write, string units)
        {

            //build the measurement data string, each parameter is seperated by commas 
            StringBuilder line = new StringBuilder();
            // Create a string array that consists of three lines.
            line.Append(current_measurement.Datetime + ",");
            line.Append(current_measurement.CalibrationGauge.Temperature + ",");
            line.Append(units + ",");
            line.Append(current_measurement.CalibrationGauge.Nominal.ToString() + ",");
            line.Append(current_measurement.CalibrationGauge.FromSet + ",");
            line.Append(current_measurement.CalibrationGauge.SerialNumber + ",");
            line.Append(current_measurement.CalibrationGauge.GaugeBlockMaterial.material.ToString() + ",");
            line.Append(current_measurement.CalibrationGauge.GaugeBlockMaterial.exp_coeff.ToString() + ",");
            line.Append(current_measurement.CalibrationGauge.GaugeBlockMaterial.youngs_modulus.ToString() + ",");
            line.Append(current_measurement.CalibrationGauge.GaugeBlockMaterial.poissons_ratio.ToString() + ",");
            if (current_measurement.CalibrationGauge.Metric && !current_measurement.ReferenceStack.Gauge1.Metric)
            {
                line.Append(Math.Round((current_measurement.ReferenceStack.Gauge1.Nominal * 25.4), 5).ToString() + ",");
                line.Append(Math.Round((current_measurement.ReferenceStack.Gauge1.CentreDeviation * 25.4 / 1000), 5).ToString() + ",");


            }
            else if (!current_measurement.CalibrationGauge.Metric && current_measurement.ReferenceStack.Gauge1.Metric)
            {
                line.Append(Math.Round((current_measurement.ReferenceStack.Gauge1.Nominal / 25.4), 5).ToString() + ",");
                line.Append(Math.Round((current_measurement.ReferenceStack.Gauge1.CentreDeviation * 1000 / 25.4), 5).ToString() + ",");
            }
            else
            {
                line.Append(Math.Round(current_measurement.ReferenceStack.Gauge1.Nominal, 5).ToString() + ",");
                line.Append(Math.Round(current_measurement.ReferenceStack.Gauge1.CentreDeviation, 5).ToString() + ",");
            }
            line.Append(current_measurement.ReferenceStack.Gauge1.FromSet + ",");
            line.Append(current_measurement.ReferenceStack.Gauge1.Metric.ToString() + ",");
            line.Append(current_measurement.ReferenceStack.Gauge1.SerialNumber + ",");
            line.Append(current_measurement.ReferenceStack.Gauge1.GaugeBlockMaterial.material.ToString() + ",");
            line.Append(current_measurement.ReferenceStack.Gauge1.GaugeBlockMaterial.exp_coeff.ToString() + ",");
            line.Append(current_measurement.ReferenceStack.Gauge1.GaugeBlockMaterial.youngs_modulus.ToString() + ",");
            line.Append(current_measurement.ReferenceStack.Gauge1.GaugeBlockMaterial.poissons_ratio.ToString() + ",");

            if (current_measurement.ReferenceStack.Gauge2 != null)
            {
                if (current_measurement.CalibrationGauge.Metric && !current_measurement.ReferenceStack.Gauge2.Metric)
                {
                    line.Append(Math.Round((current_measurement.ReferenceStack.Gauge2.Nominal * 25.4), 5).ToString() + ",");
                    line.Append(Math.Round((current_measurement.ReferenceStack.Gauge2.CentreDeviation * 25.4 / 1000), 5).ToString() + ",");

                }
                else if (!current_measurement.CalibrationGauge.Metric && current_measurement.ReferenceStack.Gauge2.Metric)
                {
                    line.Append(Math.Round((current_measurement.ReferenceStack.Gauge2.Nominal / 25.4), 5).ToString() + ",");
                    line.Append(Math.Round((current_measurement.ReferenceStack.Gauge2.CentreDeviation * 1000 / 25.4), 5).ToString() + ",");
                }
                else
                {
                    line.Append(Math.Round(current_measurement.ReferenceStack.Gauge2.Nominal, 5).ToString() + ",");
                    line.Append(Math.Round(current_measurement.ReferenceStack.Gauge2.CentreDeviation, 5).ToString() + ",");
                }
            }
            else line.Append(",,");
            
            
            if (current_measurement.ReferenceStack.Gauge2 != null) line.Append(current_measurement.ReferenceStack.Gauge2.FromSet + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge2 != null) line.Append(current_measurement.ReferenceStack.Gauge2.Metric.ToString() + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge2 != null) line.Append(current_measurement.ReferenceStack.Gauge2.SerialNumber + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge2 != null) line.Append(current_measurement.ReferenceStack.Gauge2.GaugeBlockMaterial.material.ToString() + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge2 != null) line.Append(current_measurement.ReferenceStack.Gauge2.GaugeBlockMaterial.exp_coeff.ToString() + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge2 != null) line.Append(current_measurement.ReferenceStack.Gauge2.GaugeBlockMaterial.youngs_modulus.ToString() + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge2 != null) line.Append(current_measurement.ReferenceStack.Gauge2.GaugeBlockMaterial.poissons_ratio.ToString() + ",");
            else line.Append(",");

            if (current_measurement.ReferenceStack.Gauge3 != null)
            {
                if (current_measurement.CalibrationGauge.Metric && !current_measurement.ReferenceStack.Gauge3.Metric)
                {
                    line.Append(Math.Round((current_measurement.ReferenceStack.Gauge3.Nominal * 25.4), 5).ToString() + ",");
                    line.Append(Math.Round((current_measurement.ReferenceStack.Gauge3.CentreDeviation * 25.4 / 1000), 5).ToString() + ",");

                }
                else if (!current_measurement.CalibrationGauge.Metric && current_measurement.ReferenceStack.Gauge3.Metric)
                {
                    line.Append(Math.Round((current_measurement.ReferenceStack.Gauge3.Nominal / 25.4), 5).ToString() + ",");
                    line.Append(Math.Round((current_measurement.ReferenceStack.Gauge3.CentreDeviation * 1000 / 25.4), 5).ToString() + ",");
                }
                else
                {
                    line.Append(Math.Round(current_measurement.ReferenceStack.Gauge3.Nominal, 5).ToString() + ",");
                    line.Append(Math.Round(current_measurement.ReferenceStack.Gauge3.CentreDeviation, 5).ToString() + ",");
                }
            }

            else line.Append(",,");
            if (current_measurement.ReferenceStack.Gauge3 != null) line.Append(current_measurement.ReferenceStack.Gauge3.FromSet + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge3 != null) line.Append(current_measurement.ReferenceStack.Gauge3.Metric.ToString() + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge3 != null) line.Append(current_measurement.ReferenceStack.Gauge3.SerialNumber + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge3 != null) line.Append(current_measurement.ReferenceStack.Gauge3.GaugeBlockMaterial.material.ToString() + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge3 != null) line.Append(current_measurement.ReferenceStack.Gauge3.GaugeBlockMaterial.exp_coeff.ToString() + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge3 != null) line.Append(current_measurement.ReferenceStack.Gauge3.GaugeBlockMaterial.youngs_modulus.ToString() + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge3 != null) line.Append(current_measurement.ReferenceStack.Gauge3.GaugeBlockMaterial.poissons_ratio.ToString() + ",");
            else line.Append(",");

            line.Append(GaugeBlock.calculateGaugeStackDeviation(current_measurement.ReferenceStack, current_measurement.CalibrationGauge.Metric).ToString() + ",");
            line.Append(current_measurement.R1.ToString() + ",");
            line.Append(current_measurement.C1.ToString() + ",");
            line.Append(current_measurement.A.ToString() + ",");
            line.Append(current_measurement.B.ToString() + ",");
            line.Append(current_measurement.C2.ToString() + ",");
            line.Append(current_measurement.D.ToString() + ",");
            line.Append(current_measurement.E.ToString() + ",");
            line.Append(current_measurement.C3.ToString() + ",");
            line.Append(current_measurement.R2.ToString() + ",");
            line.Append(Math.Round(current_measurement.CalibrationGauge.CorrLength, 7).ToString() + ",");
            line.Append(Math.Round(current_measurement.CalibrationGauge.CentreDeviation, 5).ToString() + ",");
            line.Append(Math.Round(current_measurement.CalibrationGauge.MinDev, 5).ToString() + ",");
            line.Append(Math.Round(current_measurement.CalibrationGauge.MaxDev, 5).ToString() + ",");
            line.Append(Math.Round(current_measurement.CalibrationGauge.ExtremeDeviation, 5).ToString() + ",");
            line.Append(Math.Round(current_measurement.CalibrationGauge.Variation, 5).ToString()+ "," );
            line.Append(current_measurement.CalibrationGauge.ComplianceStandard.ToString() + ",");
            string compliance_std = FetchComplianceStandardString(current_measurement.CalibrationGauge.ComplianceStandard,ref current_measurement);

            line.Append(compliance_std);
            line_to_write = line.ToString();
        }

        /// <summary>
        /// processes all measurements and summarises all data into required parameters for reporting purposes
        /// </summary>
        public static void writeSummaryToFile()
        {
            if (System.IO.File.Exists(filename_sum)) System.IO.File.Delete(filename_sum);
            System.IO.StreamWriter writer2;
            writer2 = System.IO.File.CreateText(filename_sum);
            writer2.WriteLine("DateTime,Temperature,Nominal,Serial Number,Centre Deviation,Extreme Deviation,Variation,Count");
            writer2.WriteLine(Version_number);

            string unique_id = "";  //a unit id is a concatination of Nominal, setid, serial no
            List<string> unique_ids_used = new List<string>(); //a list of the ids we have used in the loop below

            foreach (Measurement m in Measurement.Measurements)
            {
                unique_id = m.CalibrationGauge.Nominal + m.CalibrationGauge.FromSet + m.CalibrationGauge.SerialNumber;
                if (!unique_ids_used.Contains(unique_id))
                {
                    int num_id_matches = 0;
                    bool this_is_first_match = true;
                    double t = 0.0;
                    long date_time = 0;
                    double nominal = m.CalibrationGauge.Nominal;
                    string ser_no = m.CalibrationGauge.SerialNumber;
                    double sum_centre_dev = 0.0;
                    double sum_A_dev = 0.0;
                    double sum_B_dev = 0.0;
                    double sum_D_dev = 0.0;
                    double sum_E_dev = 0.0;
                    double sum_variation = 0.0;

                    //with the unique id loop through each measurement
                    foreach (Measurement k in Measurement.Measurements)
                    {
                        string unique_id_to_compare = k.CalibrationGauge.Nominal + k.CalibrationGauge.FromSet + k.CalibrationGauge.SerialNumber;
                        if (unique_id_to_compare.Equals(unique_id))
                        {
                            num_id_matches++;
                            //we have a match but if it's the first match then ignore doing the sums
                         
                            t += Convert.ToDouble(k.CalibrationGauge.Temperature);
                            //we need the date time to be represented as a double to do averaging math on
                            date_time += Convert.ToDateTime(k.Datetime).Ticks;
                            sum_centre_dev += k.CalibrationGauge.CentreDeviation;
                            sum_A_dev += k.CalibrationGauge.ADev;
                            sum_B_dev += k.CalibrationGauge.BDev;
                            sum_D_dev += k.CalibrationGauge.DDev;
                            sum_E_dev += k.CalibrationGauge.EDev;
                            sum_variation += k.CalibrationGauge.Variation;
                            
                        }
                    }
                    //compute the means
                    sum_centre_dev /= num_id_matches;
                    sum_A_dev /= num_id_matches;
                    sum_B_dev /= num_id_matches;
                    sum_D_dev /= num_id_matches;
                    sum_E_dev /= num_id_matches;

                    //put the average gauge points into a list for processing.
                    List<double> devs = new List<double>();
                    devs.Add(sum_centre_dev);
                    devs.Add(sum_A_dev);
                    devs.Add(sum_B_dev);
                    devs.Add(sum_D_dev);
                    devs.Add(sum_E_dev);

                    double corr_min_dev = devs.Min();
                    double corr_max_dev = devs.Max();

                    double corr_extreme_dev = 0.0;
                    if (Math.Abs(corr_max_dev) >= Math.Abs(corr_min_dev)) corr_extreme_dev = corr_max_dev;
                    else corr_extreme_dev = corr_min_dev;

                    double vari = corr_max_dev - corr_min_dev;

                    
                    t /= num_id_matches;
                    date_time /= num_id_matches;
                    var a_dt = new DateTime(date_time);
                    sum_variation /= (num_id_matches);
                    writer2.WriteLine(a_dt.ToString() + "," + t.ToString() + "," + nominal + "," + ser_no + "," + sum_centre_dev + "," + corr_extreme_dev + "," + vari + "," + num_id_matches.ToString());
                    unique_ids_used.Add(unique_id);
                }

            }
            writer2.Close();
        }

        /// <summary>
        /// processes all measurements and summarises all data into required parameters for reporting purposes
        /// </summary>
        public static void WriteUncertaintyAndComplianceToFile(ref VerticalFederal vfederal)
        {
            try
            {
                if (System.IO.File.Exists(filename_U95_sum)) System.IO.File.Delete(filename_U95_sum);
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show("Cannot write to the U95 uncertainty and compliance file, file may be in use. If it's in use please close it now before pushing OK");
                try
                {
                    if (System.IO.File.Exists(filename_U95_sum)) System.IO.File.Delete(filename_U95_sum);
                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show("Cannot write to the U95 uncertainty and compliance file, file may be in use. Uncertainties calculations won't be attempted until the next measurement is made");
                    return;
                }
            }
            System.IO.StreamWriter writer3;
            writer3 = System.IO.File.CreateText(filename_U95_sum);
            if (measurements[0].CalibrationGauge.Metric)
            {
                writer3.WriteLine("DateTime,Nominal (inch),Serial Number,Centre Deviation (um),U95 Centre Deviation As Calculated (um),U95 Centre Deviation CMC (um),CMC U95 Centre Deviation As Reported (um),Extreme Deviation (um),U95 Extreme Deviation As Calculated (um),U95 Extreme Deviation CMC (um),U95 Extreme Deviation As Reported (um),Variation,U95 Variation As Calculated (um),U95 Variation CMC (um),U95 Variation As Reported (um),Compliance Deviation, Compliance Variation");
            }
            else writer3.WriteLine("DateTime,Nominal (inch),Serial Number,Centre Deviation (uinch),U95 Centre Deviation As Calculated (uinch),U95 Centre Deviation CMC (uinch) ,CMC U95 Centre Deviation As Reported (uinch),Extreme Deviation (uinch),U95 Extreme Deviation As Calculated (uinch),U95 Extreme Deviation CMC (uinch),U95 Extreme Deviation As Reported (uinch),Variation (uinch),U95 Variation As Calculated (uinch),U95 Variation CMC (uinch),U95 Variation As Reported (uinch),Compliance Deviation, Compliance Variation");

            writer3.WriteLine(Version_number);

            string unique_id = "";  //a unit id is a concatination of Nominal, setid, serial no
            List<string> unique_ids_used = new List<string>(); //a list of the ids we have used in the loop below
            

            foreach (Measurement m in Measurement.Measurements)
            {
                unique_id = m.CalibrationGauge.Nominal + m.CalibrationGauge.FromSet + m.CalibrationGauge.SerialNumber;

                if (!unique_ids_used.Contains(unique_id))
                {
                    int num_id_matches = 0;

                    long date_time = 0;
                    double nominal = m.CalibrationGauge.Nominal;
                    string ser_no = m.CalibrationGauge.SerialNumber;
         
                    //double sum_variation = 0.0;
                    double sum_centre_dev = 0.0;
                    double U95_centre_dev = 0.0;
                    double U95_extreme_dev = 0.0;
                    double U95_variation = 0.0;
                    double sum_A_dev = 0.0;
                    double sum_B_dev = 0.0;
                    double sum_D_dev = 0.0;
                    double sum_E_dev = 0.0;
                    double tolerance_variation = 0.0;
                    double limit_deviation = 0.0;
                    double cmc_d = 0.0;
                    double cmc_v = 0.0;
                    List<Measurement> gauge_measurements = new List<Measurement>();

                    //with the unique id loop through each measurement
                    foreach (Measurement k in Measurement.Measurements)
                    {
                        string unique_id_to_compare = k.CalibrationGauge.Nominal + k.CalibrationGauge.FromSet + k.CalibrationGauge.SerialNumber;

                        if (unique_id_to_compare.Equals(unique_id))
                        {
                            gauge_measurements.Add(k);
                            num_id_matches++;
                            k.CalculateExpandedUncertaintyDeviation(ref vfederal);
                            U95_centre_dev = k.CalibrationGauge.ExpandedUncertaintyDev;
                            limit_deviation = k.CalibrationGauge.LimitDeviation;
                            tolerance_variation = k.CalibrationGauge.ToleranceVariation;
                            cmc_d = k.CalibrationGauge.DeviationCMC;
                            cmc_v = k.CalibrationGauge.VariationCMC;

                            //we need the date time to be represented as a double to do averaging math on
                            date_time += Convert.ToDateTime(k.Datetime).Ticks;
                            
                            //add up all the repeat measurements for each point of this gauge block
                            sum_centre_dev += k.CalibrationGauge.CentreDeviation;
                            sum_A_dev += k.CalibrationGauge.ADev;
                            sum_B_dev += k.CalibrationGauge.BDev;
                            sum_D_dev += k.CalibrationGauge.DDev;
                            sum_E_dev += k.CalibrationGauge.EDev;
                        }
                    }

                    //compute the means
                    date_time /= gauge_measurements.Count;
                    var a_dt = new DateTime(date_time);
                    sum_centre_dev /= gauge_measurements.Count;
                    sum_A_dev /= gauge_measurements.Count;
                    sum_B_dev /= gauge_measurements.Count;
                    sum_D_dev /= gauge_measurements.Count;
                    sum_E_dev /= gauge_measurements.Count;
      

                    
                    //put the average values into a list for processing
                    List<double> devs = new List<double>();
                    devs.Add(sum_centre_dev);
                    devs.Add(sum_A_dev);
                    devs.Add(sum_B_dev);
                    devs.Add(sum_D_dev);
                    devs.Add(sum_E_dev);

                    //determine the extreme deviation and at which position is occurs on the gauge
                    double corr_extreme_dev = 0.0;
                    int index_of_extreme_deviation = 0;

                    if (Math.Abs(devs.Max()) >= Math.Abs(devs.Min()))
                    {
                        corr_extreme_dev = devs.Max();
                        index_of_extreme_deviation = devs.IndexOf(devs.Max());

                    }
                    else
                    {
                        corr_extreme_dev = devs.Min();
                        index_of_extreme_deviation = devs.IndexOf(devs.Min());
                    }

                    //determine the variation
                    double var_ = devs.Max() - devs.Min();

                    int max_index = devs.IndexOf(devs.Max());
                    int min_index = devs.IndexOf(devs.Min());

                    //Now that we have the extrema calculated we can determine the uncertainty for the extreme deviation and variation in length
                    foreach (Measurement k in gauge_measurements)
                    {
                        k.CalculateExpandedUncertaintyExtremeDeviation(ref vfederal,index_of_extreme_deviation);
                        U95_extreme_dev = k.CalibrationGauge.ExpandedUncertaintyExtDev;
                        k.CalculateExpandedUncertaintyVariation(ref vfederal,max_index,min_index);
                        U95_variation = k.CalibrationGauge.ExpandedUncertaintyVar;
                    }

                    if (!m.CalibrationGauge.Metric) CalculateUncertaintiesMicroInch(ref U95_centre_dev, ref U95_extreme_dev, ref U95_variation);
                    if (!m.CalibrationGauge.Metric) CalculateCMCMicroInch(ref cmc_d, ref cmc_v);


                    string compliance_d = "";
                    if (m.CalibrationGauge.Metric)
                    {
                        if ((Math.Abs(corr_extreme_dev) + U95_extreme_dev / 1000) < limit_deviation) compliance_d = "P";
                        else if (Math.Abs(corr_extreme_dev) > (limit_deviation + U95_extreme_dev / 1000)) compliance_d = "F";
                        else compliance_d = "U";
                    }
                    else
                    {
                        if ((Math.Abs(corr_extreme_dev) + U95_extreme_dev) < limit_deviation) compliance_d = "P";
                        else if (Math.Abs(corr_extreme_dev) > (limit_deviation + U95_extreme_dev)) compliance_d = "F";
                        else compliance_d = "U";
                    }

                    string compliance_v ="";
                    if (m.CalibrationGauge.Metric)
                    {
                        if ((Math.Abs(var_) + U95_variation / 1000) < tolerance_variation) compliance_v = "P";
                        else if (Math.Abs(var_) > (tolerance_variation + U95_variation / 1000)) compliance_v = "F";
                        else compliance_v = "U";
                    }
                    else
                    {
                        if ((Math.Abs(var_) + U95_variation) < tolerance_variation) compliance_v = "P";
                        else if (Math.Abs(var_) > (tolerance_variation + U95_variation)) compliance_v = "F";
                        else compliance_v = "U";
                    }

                    double reported_U95_dev = 0.0;
                    double reported_U95_ext_dev = 0.0;
                    double reported_U95_var = 0.0;

                    if (U95_centre_dev <= cmc_d) reported_U95_dev = cmc_d;
                    else reported_U95_dev = U95_centre_dev;
                    if (U95_extreme_dev <= cmc_d) reported_U95_ext_dev = cmc_d;
                    else reported_U95_ext_dev = U95_extreme_dev;
                    if (U95_variation < cmc_v) reported_U95_var = cmc_v;
                    else reported_U95_var = U95_variation;

                    if(m.CalibrationGauge.Metric) writer3.WriteLine(a_dt.ToString() + "," + nominal + "," + ser_no + "," + sum_centre_dev + "," + U95_centre_dev/1000 + "," + cmc_d/1000 + "," + reported_U95_dev/1000 + "," + corr_extreme_dev + "," + U95_extreme_dev/1000 + "," + cmc_d/1000 + "," + reported_U95_ext_dev/1000 + "," + var_ + "," + U95_variation/1000 + "," + cmc_v/1000 + "," + reported_U95_var/1000 + "," + compliance_d + "," + compliance_v);
                    else writer3.WriteLine(a_dt.ToString() + "," + nominal + "," + ser_no + "," + sum_centre_dev + "," + U95_centre_dev + "," + cmc_d + "," + reported_U95_dev + "," + corr_extreme_dev + "," + U95_extreme_dev + "," + cmc_d + "," + reported_U95_ext_dev + "," + var_ + "," + U95_variation + "," + cmc_v + "," + reported_U95_var + "," + compliance_d + "," + compliance_v);
                    unique_ids_used.Add(unique_id);
                }
            }
            writer3.Close();
        }

        public static bool CreateNewCalSet(ref int set_index)
        {
            if (working_gauge.FromSet.Equals(""))
            {
                MessageBox.Show("Set serial number is blank.  This needs to be filled in");
                return false;
            }
            //check if we have any calibration gauge sets in the list, if we don't then add a new set
            if (calibration_gauge_sets.Count == 0)
            {
                GaugeBlockSet gauge_set = new GaugeBlockSet();
                gauge_set.GaugeSetName = working_gauge.FromSet;
                calibration_gauge_sets.Add(gauge_set);
                return true;
            }
            else
            {
                bool set_exists = false;
                set_index = 0;
                //if we have calibration gauge sets already in the list then see if the set has previously been added (i.e is this a unique serial number)
                foreach (GaugeBlockSet cal_set in calibration_gauge_sets)
                {
                    if (cal_set.GaugeSetName.Equals(working_gauge.FromSet))
                    {
                        set_exists = true;
                        break;
                    }
                    set_index++;

                }
                if (set_exists) return true;
                else
                {
                    GaugeBlockSet gauge_set = new GaugeBlockSet();
                    gauge_set.GaugeSetName = working_gauge.FromSet;
                    calibration_gauge_sets.Add(gauge_set);
                    //set_index++;
                    return true;
                }
            }

        }
        /// <summary>
        /// Converts the uncertainty from nanometers to microinches
        /// </summary>
        /// <param name="U_dev">The deviation uncertainty in nm</param>
        /// <param name="U_ext_dev">The extreme deviation uncertainty in nm</param>
        /// <param name="U_var">The variation uncertainty in nm</param> 
        public static void CalculateUncertaintiesMicroInch(ref double U_dev, ref double U_ext_dev, ref double U_var)
        {
            U_dev /= 25.4;
            U_ext_dev /= 25.4;
            U_var /= 25.4;
        }
        /// <summary>
        /// Converts the CMC from nanometers to microinches
        /// </summary>
        /// <param name="CMC_dev">The deviation CMC in nm</param>
        /// <param name="CMC_var">The variation CMC in nm</param> 
        public static void CalculateCMCMicroInch(ref double CMC_dev, ref double CMC_var)
        {
            CMC_dev /= 25.4;
            CMC_var /= 25.4;
        }
        public static string FetchComplianceStandardString(int index,ref Measurement current_measurement)
        {
            string cs = "";
            if (current_measurement.CalibrationGauge.Metric)
            {
                switch (index)
                {
                    case (int)ComplianceMetric.BS_EN_ISO_3650_1999_Grade_K:
                        cs = "BS_EN_ISO_3650_1999_Grade_K";
                        break;
                    case (int)ComplianceMetric.BS_EN_ISO_3650_1999_Grade_0:
                        cs = "BS_EN_ISO_3650_1999_Grade_0";
                        break;
                    case (int)ComplianceMetric.BS_EN_ISO_3650_1999_Grade_1:
                        cs = "BS_EN_ISO_3650_1999_Grade_1";
                        break;
                    case (int)ComplianceMetric.BS_EN_ISO_3650_1999_Grade_2:
                        cs = "BS_EN_ISO_3650_1999_Grade_2";
                        break;
                    case (int)ComplianceMetric.AS_1457_1999_Grade_K:
                        cs = "AS_1457_1999_Grade_K";
                        break;
                    case (int)ComplianceMetric.AS_1457_1999_Grade_0:
                        cs = "AS_1457_1999_Grade_0";
                        break;
                    case (int)ComplianceMetric.AS_1457_1999_Grade_1:
                        cs = "AS_1457_1999_Grade_1";
                        break;
                    case (int)ComplianceMetric.AS_1457_1999_Grade_2:
                        cs = "AS_1457_1999_Grade_2";
                        break;
                    case (int)ComplianceMetric.JIS_B_7506_2004_Grade_K:
                        cs = "JIS_B_7506_2004_Grade_K";
                        break;
                    case (int)ComplianceMetric.JIS_B_7506_2004_Grade_0:
                        cs = "JIS_B_7506_2004_Grade_0";
                        break;
                    case (int)ComplianceMetric.JIS_B_7506_2004_Grade_1:
                        cs = "JIS_B_7506_2004_Grade_1";
                        break;
                    case (int)ComplianceMetric.JIS_B_7506_2004_Grade_2:
                        cs = "JIS_B_7506_2004_Grade_2";
                        break;
                    case (int)ComplianceMetric.ASME_B89_1_9_2002_Grade_K:
                        cs = "ASME_B89_1_9_2002_Grade_K";
                        break;
                    case (int)ComplianceMetric.ASME_B89_1_9_2002_Grade_00:
                        cs = "ASME_B89_1_9_2002_Grade_00";
                        break;
                    case (int)ComplianceMetric.ASME_B89_1_9_2002_Grade_0:
                        cs = "ASME_B89_1_9_2002_Grade_0";
                        break;
                    case (int)ComplianceMetric.ASME_B89_1_9_2002_Grade_AS1:
                        cs = "ASME_B89_1_9_2002_Grade_AS1";
                        break;
                    case (int)ComplianceMetric.ASME_B89_1_9_2002_Grade_AS2:
                        cs = "ASME_B89_1_9_2002_Grade_AS2";
                        break;

                }
            }
            else
            {
                switch (index)
                {
                    case (int)ComplianceImperial.BS_4311_1_2007_Grade_K:
                        cs = "BS_4311_1_2007_Grade_K";
                        break;
                    case (int)ComplianceImperial.BS_4311_1_2007_Grade_0:
                        cs = "BS_4311_1_2007_Grade_0";
                        break;
                    case (int)ComplianceImperial.BS_4311_1_2007_Grade_1:
                        cs = "BS_4311_1_2007_Grade_1";
                        break;
                    case (int)ComplianceImperial.BS_4311_1_2007_Grade_2:
                        cs = "BS_4311_1_2007_Grade_2";
                        break;
                }
            }
            return cs;
        }
    }
}
