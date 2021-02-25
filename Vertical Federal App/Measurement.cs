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
        private static List<Measurement> measurements = new List<Measurement>();
        private static bool file_header_written = false;
        private const double oz_f_to_n_f = 0.27801385;  //newtons
        private GaugeBlock calibration_gauge;
        private string date_time;
        private string temperature;
        private double reference_deformation_top_probe;
        private double reference_deformation_bottom_probe;
        private double bottom_probe_deformation_cal_gauge;
        private double top_probe_deformation_cal_gauge;
        private double corrected_length;
        private double elastic_correction;
        private Stack gauge_stack;
        private string cal_set_serial;
        private double nominal;
        private double reference_deviation;
        private double variation;
        private double diff; //difference
        private double extreme_deviation;
        private double limit_deviation;
        private double centre_deviation;
        private double min_deviation;
        private double max_deviation;
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
        private static Thread tempRetriever;
        private static bool log_temperatures = false;
        public static string measurement_file_header = "DateTime,Temperature,Units,Cal Nominal,Gauge Set Serial No,Gauge Serial No,Cal Gauge Material,Cal Gauge Exp Coeff, Cal Gauge Young's Modulus," +
                        "Cal Gauge Poisson Ratio,Ref Gauge 1 Nominal,Ref Gauge 1 Deviation,Ref Gauge 1 Set Serial No, Reg Gauge 1 Serial No, Ref Gauge 1 Material,Ref Gauge 1 exp coeeff,Ref Gauge 1 Young's Modulus,Ref Gauge 1 Poisson Ratio," +
                        "Ref Gauge 2 Nominal,Ref Gauge 2 Deviation,Ref Gauge 1 Set Serial No, Reg Gauge 1 Serial No,Ref Gauge 2 Material,Ref Gauge 2 exp coeeff,Ref Gauge 2 Young's Modulus,Ref Gauge 2 Poisson Ratio," +
                        "Ref Gauge 3 Nominal,Ref Gauge 3 Deviation,Ref Gauge 1 Set Serial No, Reg Gauge 1 Serial No,Ref Gauge 3 Material,Ref Gauge 3 exp coeeff,Ref Gauge 3 Young's Modulus,Ref Gauge 3 Poisson Ratio," +
                        "Reference Stack Deviation,R1,C1,A,B,C2,D,E,C3,R2,Measured Length,Centre Deviation,Extreme Deviation,Variation";
        public static string filename="";
        public static List<GaugeBlockSet> calibration_gauge_sets;
        public static GaugeBlock working_gauge;

        public Measurement()
        {
            metric = true;

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
        public string Temperature
        {
            get { return temperature; }
            set { temperature = value; }
        }
        public double Nominal
        {
            get { return nominal; }
            set { nominal = value; }
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

        public string CalSetSerial
        {
            get { return cal_set_serial; }
            set { cal_set_serial = value; }
        }
      

        public double RefDeviation_um_uinch
        {
            get { return reference_deviation; }
            set { reference_deviation = value; }
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
            bool temperature_found;
            while (LogTemperatures)
            {
                temperature_found = false;
                DateTime now = DateTime.Now;
                DateTime previous_interation_datetime = DateTime.MinValue;

                string path = @"G:\Shared drives\MSL - Length\Length\Temperature Monitoring Data\Hilger Lab\"+ now.Year.ToString() + @"\" + now.Year.ToString() + "-" + now.Month.ToString() + @"\" + "Vertical Federal.txt";
                if (!File.Exists(path))
                {
                    MessageBox.Show("Unable to get temperature.  Temperature data file expected, unable to find file at:\n" +
                        path + "\nYou can enter temperatures into the box below manually each time you make a measurement");
                    return;
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
                double temperature_d = -1.0;

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
                    if (lines.Length == 1 || lines.Length == 2) return "";
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
        public void CalculateDeviations(ref double corrected_centre_dev, ref double corrected_extreme_dev, ref double corrected_length_mm_inch, ref double maxd, ref double mind)
        {
            if (metric)
            {
                corrected_length_mm_inch = (bottom_probe_deformation_cal_gauge * 1000) - (reference_deformation_bottom_probe * 1000) + RefLength() - (reference_deformation_top_probe * 1000) + CalculateMeasuredDiff_um_uinch() / 1000 + (top_probe_deformation_cal_gauge * 1000);
                corrected_length = corrected_length_mm_inch;
                centre_deviation = (corrected_length - CalibrationGauge.Nominal) * 1000; //centre deviation is in um
                corrected_centre_dev = centre_deviation;
            }
            else
            {
                corrected_length_mm_inch = (bottom_probe_deformation_cal_gauge * 1000 / 25.4) - (reference_deformation_bottom_probe * 1000 / 25.4) + RefLength() - (reference_deformation_top_probe * 1000 / 25.4) + CalculateMeasuredDiff_um_uinch() / 1000000 + (top_probe_deformation_cal_gauge * 1000 / 25.4);
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
            max_deviation = values_array.Max();
            double min_deviation = values_array.Min();
            mind = min_deviation;
            maxd = max_deviation;

            corrected_extreme_dev = CalculateExtremeDeviation(min_deviation, max_deviation);
            extreme_deviation = corrected_extreme_dev;
      
        }

        public static double CalculateExtremeDeviation(double min, double max)
        {
            //calculate extreme deviation
            if (Math.Abs(max) > Math.Abs(min)) return max;
            else return min;
        }

        public double getCorrection_um_uinch()
        {
             elastic_correction = centre_deviation - (CalculateMeasuredDiff_um_uinch() + RefDeviation_um_uinch);
             return elastic_correction;
        }

        /// <summary>
        /// Parse the file opened by the user line by line while updating the gui
        /// </summary>
        /// <param name="lines">A string array containing lines of the measurement file</param>
        /// <param name="gaugeResultsRichTextBox">A reference to the gauge results rich text box</param>
        /// <param name="clientNameTextBox">A reference to the client name text box</param>
        /// <param name="gaugeSerialTextBox">A reference to the gauge serial text box</param>
        /// <param name="writer">A reference to the stream writer for the measurement data file</param>
        public static bool ParseFile(string[] lines,ref RichTextBox gaugeResultsRichTextBox, ref TextBox clientNameTextBox, ref TextBox gaugeSerialTextBox ,ref System.IO.StreamWriter writer)
        {
            bool first_line = true;
            string line_read;

            foreach (string line in lines)
            {
                //the first line is a title line and doesn't contain data
                if (first_line)
                {
                    first_line = false;
                    continue;
                }
                line_read = (string)line.Clone();

                Measurement meas = new Measurement();

                ParseLine(ref line_read, ref meas);
                working_gauge = meas.CalibrationGauge;
                int set_index = 0;
                CreateNewCalSet(ref set_index);
                
                //add the working gauge to its calibration gauge set
                GaugeBlock working_gauge_clone = working_gauge.Clone();
                calibration_gauge_sets[set_index].GaugeList.Add(working_gauge_clone);
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
        private static void ParseLine(ref string line, ref Measurement meas)
        {
            //create a new measurement and populate it with data we find in this files current line
            
            Stack ref_stack = new Stack(1);
            GaugeBlock calibration_gauge = new GaugeBlock(false);
            meas.CalibrationGauge = calibration_gauge;
            meas.ReferenceStack = ref_stack;
            
            string[] strings = line.Split(',');
            meas.Datetime = strings[0];
            meas.Temperature = strings[1];
            if (strings[2].Equals("mm µm")) meas.Metric = true;
            else meas.Metric = false;
            double.TryParse(strings[3], out meas.nominal);
            meas.CalibrationGauge.FromSet = strings[4];
            meas.CalibrationGauge.SerialNumber = strings[5];
            meas.CalibrationGauge.GaugeBlockMaterial.material = strings[6];
            meas.CalibrationGauge.GaugeBlockMaterial.exp_coeff = Convert.ToDouble(strings[7]);
            meas.CalibrationGauge.GaugeBlockMaterial.youngs_modulus = Convert.ToDouble(strings[8]);
            meas.CalibrationGauge.GaugeBlockMaterial.poissons_ratio = Convert.ToDouble(strings[9]);

            ref_stack.Gauge1.Nominal = Convert.ToDouble(strings[10]);
            ref_stack.Gauge1.CentreDeviation = Convert.ToDouble(strings[11]);
            ref_stack.Gauge1.FromSet = strings[12];
            ref_stack.Gauge1.SerialNumber = strings[13];
            ref_stack.Gauge1.GaugeBlockMaterial.material = strings[14];
            ref_stack.Gauge1.GaugeBlockMaterial.exp_coeff = Convert.ToDouble(strings[15]);
            ref_stack.Gauge1.GaugeBlockMaterial.youngs_modulus = Convert.ToDouble(strings[16]);
            ref_stack.Gauge1.GaugeBlockMaterial.poissons_ratio = Convert.ToDouble(strings[17]);
            ref_stack.Gauge1 = ref_stack.Gauge1;

            if (!strings[18].Equals(""))
            {
                ref_stack.Gauge2 = new GaugeBlock(false);
                ref_stack.Gauge2.Nominal = Convert.ToDouble(strings[18]);
                ref_stack.Gauge2.CentreDeviation = Convert.ToDouble(strings[19]);
                ref_stack.Gauge2.FromSet = strings[20];
                ref_stack.Gauge2.SerialNumber = strings[21];
                ref_stack.Gauge2.GaugeBlockMaterial.material = strings[22];
                ref_stack.Gauge2.GaugeBlockMaterial.exp_coeff = Convert.ToDouble(strings[23]);
                ref_stack.Gauge2.GaugeBlockMaterial.youngs_modulus = Convert.ToDouble(strings[24]);
                ref_stack.Gauge2.GaugeBlockMaterial.poissons_ratio = Convert.ToDouble(strings[25]);
            }

            if (!strings[26].Equals(""))
            {
                ref_stack.Gauge3 = new GaugeBlock(false);
                ref_stack.Gauge3.Nominal = Convert.ToDouble(strings[26]);
                ref_stack.Gauge3.CentreDeviation = Convert.ToDouble(strings[27]);
                ref_stack.Gauge3.FromSet = strings[28];
                ref_stack.Gauge3.SerialNumber = strings[29];
                ref_stack.Gauge3.GaugeBlockMaterial.material = strings[30];
                ref_stack.Gauge3.GaugeBlockMaterial.exp_coeff = Convert.ToDouble(strings[31]);
                ref_stack.Gauge3.GaugeBlockMaterial.youngs_modulus = Convert.ToDouble(strings[32]);
                ref_stack.Gauge3.GaugeBlockMaterial.poissons_ratio = Convert.ToDouble(strings[33]);
            }

            double.TryParse(strings[34], out meas.reference_deviation);
            double.TryParse(strings[35], out meas.r1);
            double.TryParse(strings[36], out meas.c1);
            double.TryParse(strings[37], out meas.a);
            double.TryParse(strings[38], out meas.b);
            double.TryParse(strings[39], out meas.c2);
            double.TryParse(strings[40], out meas.d);
            double.TryParse(strings[41], out meas.e);
            double.TryParse(strings[42], out meas.c3);
            double.TryParse(strings[43], out meas.r2);

            double.TryParse(strings[44], out meas.corrected_length);
            double.TryParse(strings[45], out meas.centre_deviation);
            double.TryParse(strings[46], out meas.extreme_deviation);
            double.TryParse(strings[47], out meas.variation);
            
            

            //Save a copy of the current measurement to the measurement list.
            Measurement.Measurements.Add(meas);

            

        }
        /// <summary>
        /// Write a line of data for this measurement to the gauge results rich text box.
        /// </summary>
        /// <param name="current_measurement">The current measurement object</param>
        /// <param name="units">the unit string, metric or imperial</param>
        /// <param name="gaugeResultsRichTextBox">a reference to the gauge results rich text box</param>
        /// <param name="measurement_number">the nth measurement number</param> 
        public static void writeRichTBLine(Measurement current_measurement, string units, ref RichTextBox gaugeResultsRichTextBox, int measurement_number)
        {
            if (!Measurement.HeaderWritten)
            {
                //write the header string of the output rich text box

                gaugeResultsRichTextBox.Text = "Measurement No.\tUnits\tNominal\tCentre Dev\tExtreme Dev\tVariation\n";
                gaugeResultsRichTextBox.SelectAll();
                gaugeResultsRichTextBox.SelectionTabs = new int[] { 110, 210, 280, 350, 430 };
                gaugeResultsRichTextBox.AcceptsTab = true;
                gaugeResultsRichTextBox.Select(0, 0);
                Measurement.HeaderWritten = true;
            }

            gaugeResultsRichTextBox.AppendText(measurement_number.ToString() + "\t" +
                units + "\t" + current_measurement.Nominal.ToString() + "\t" + Math.Round(current_measurement.centre_deviation, 5).ToString() +
                "\t" + Math.Round(current_measurement.extreme_deviation, 5).ToString() + "\t" + Math.Round(current_measurement.variation, 5).ToString() + "\n");
            gaugeResultsRichTextBox.SelectAll();
            gaugeResultsRichTextBox.SelectionTabs = new int[] { 110, 210, 280, 350, 430 };
            gaugeResultsRichTextBox.AcceptsTab = true;
            gaugeResultsRichTextBox.Select(0, 0);
        }
        /// <summary>
        /// Make a string which has all measurement data delimited by commas, each measurement is recorded as a line in the file
        /// </summary>
        /// <param name="current_measurement">The current measurement object</param>
        /// <param name="line_to_write">a reference to the line string</param>
        /// <param name="gaugeResultsRichTextBox">a reference to units string, either metric or imperial</param> 
        public static void PrepareLineForWrite(Measurement current_measurement, ref string line_to_write, string units)
        {

            
            //build the measurement data string, each parameter is seperated by commas 
            StringBuilder line = new StringBuilder();
            // Create a string array that consists of three lines.
            line.Append(current_measurement.Datetime + ",");
            line.Append(current_measurement.Temperature + ",");
            line.Append(units + ",");
            line.Append(current_measurement.Nominal.ToString() + ",");
            line.Append(current_measurement.CalibrationGauge.FromSet + ",");
            line.Append(current_measurement.CalibrationGauge.SerialNumber + ",");
            line.Append(current_measurement.CalibrationGauge.GaugeBlockMaterial.material.ToString() + ",");
            line.Append(current_measurement.CalibrationGauge.GaugeBlockMaterial.exp_coeff.ToString() + ",");
            line.Append(current_measurement.CalibrationGauge.GaugeBlockMaterial.youngs_modulus.ToString() + ",");
            line.Append(current_measurement.CalibrationGauge.GaugeBlockMaterial.poissons_ratio.ToString() + ",");
            if (current_measurement.Metric && !current_measurement.ReferenceStack.Gauge1.Metric)
            {
                line.Append(Math.Round((current_measurement.ReferenceStack.Gauge1.Nominal * 25.4), 5).ToString() + ",");
                line.Append(Math.Round((current_measurement.ReferenceStack.Gauge1.CentreDeviation * 25.4 / 1000), 5).ToString() + ",");
                

            }
            else if (!current_measurement.Metric && current_measurement.ReferenceStack.Gauge1.Metric)
            {
                line.Append(Math.Round((current_measurement.ReferenceStack.Gauge1.Nominal / 25.4), 5).ToString() + ",");
                line.Append(Math.Round((current_measurement.ReferenceStack.Gauge1.CentreDeviation * 1000 / 25.4), 5).ToString() + ",");
            }
            else
            {
                line.Append(Math.Round(current_measurement.ReferenceStack.Gauge1.Nominal, 5).ToString() + ",");
                line.Append(Math.Round(current_measurement.ReferenceStack.Gauge1.CentreDeviation, 5).ToString() + ",");
            }
            line.Append(current_measurement.ReferenceStack.Gauge1.SerialNumber+",");
            line.Append(current_measurement.ReferenceStack.Gauge1.FromSet+",");
            line.Append(current_measurement.ReferenceStack.Gauge1.GaugeBlockMaterial.material.ToString()+",");
            line.Append(current_measurement.ReferenceStack.Gauge1.GaugeBlockMaterial.exp_coeff.ToString() + ",");
            line.Append(current_measurement.ReferenceStack.Gauge1.GaugeBlockMaterial.youngs_modulus.ToString() + ",");
            line.Append(current_measurement.ReferenceStack.Gauge1.GaugeBlockMaterial.poissons_ratio.ToString() + ",");

            if (current_measurement.ReferenceStack.Gauge2 != null)
            {
                if (current_measurement.Metric && !current_measurement.ReferenceStack.Gauge2.Metric)
                {
                    line.Append(Math.Round((current_measurement.ReferenceStack.Gauge2.Nominal * 25.4), 5).ToString() + ",");
                    line.Append(Math.Round((current_measurement.ReferenceStack.Gauge2.CentreDeviation * 25.4 / 1000), 5).ToString() + ",");

                }
                else if (!current_measurement.Metric && current_measurement.ReferenceStack.Gauge2.Metric)
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

            if (current_measurement.ReferenceStack.Gauge2 != null) line.Append(current_measurement.ReferenceStack.Gauge2.SerialNumber + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge2 != null) line.Append(current_measurement.ReferenceStack.Gauge2.FromSet + ",");
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
                if (current_measurement.Metric && !current_measurement.ReferenceStack.Gauge3.Metric)
                {
                    line.Append(Math.Round((current_measurement.ReferenceStack.Gauge3.Nominal * 25.4), 5).ToString() + ",");
                    line.Append(Math.Round((current_measurement.ReferenceStack.Gauge3.CentreDeviation * 25.4 / 1000), 5).ToString() + ",");

                }
                else if (!current_measurement.Metric && current_measurement.ReferenceStack.Gauge3.Metric)
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
            if (current_measurement.ReferenceStack.Gauge3 != null) line.Append(current_measurement.ReferenceStack.Gauge3.SerialNumber + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge3 != null) line.Append(current_measurement.ReferenceStack.Gauge3.FromSet + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge3 != null) line.Append(current_measurement.ReferenceStack.Gauge3.GaugeBlockMaterial.material.ToString() + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge3 != null) line.Append(current_measurement.ReferenceStack.Gauge3.GaugeBlockMaterial.exp_coeff.ToString() + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge3 != null) line.Append(current_measurement.ReferenceStack.Gauge3.GaugeBlockMaterial.youngs_modulus.ToString() + ",");
            else line.Append(",");
            if (current_measurement.ReferenceStack.Gauge3 != null) line.Append(current_measurement.ReferenceStack.Gauge3.GaugeBlockMaterial.poissons_ratio.ToString() + ",");
            else line.Append(",");

            line.Append(GaugeBlock.calculateGaugeStackDeviation(current_measurement.ReferenceStack, current_measurement.Metric).ToString() + ",");
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
            line.Append(Math.Round(current_measurement.CalibrationGauge.ExtremeDeviation, 5).ToString() + ",");
            line.Append(Math.Round(current_measurement.CalibrationGauge.Variation, 5).ToString());
            line_to_write = line.ToString();
        }

        /// <summary>
        /// processes all measurements and summarises all data into required parameters for reporting purposes
        /// </summary>
        /// <param name="clientNameTextBox">reference to the client name text box</param>
        /// <param name="gaugeSerialTextBox">a reference to the gauge serial text box</param>
        public static void writeSummaryToFile(ref TextBox clientNameTextBox, ref TextBox gaugeSerialTextBox)
        {
           
            if (clientNameTextBox.Text.Equals(""))
            {
                MessageBox.Show("Cannot determine a filename as the client name text box is empty.  Please enter a client name");
                return;
            } 
            //Determine a suitable file name from metadata
            string summaryfilename = @"G:\Shared drives\MSL - Length\Length\Federal\FederalData\" + clientNameTextBox.Text + "_summary" + System.DateTime.Now.Year.ToString();
            

            if (System.IO.File.Exists(summaryfilename)) System.IO.File.Delete(summaryfilename);
            System.IO.StreamWriter writer2;
            writer2 = System.IO.File.CreateText(summaryfilename);
            writer2.WriteLine("DateTime,Temperature,Nominal,Serial Number,Centre Deviation,Extreme Deviation,Variation");
           
            

            string unique_id = "";  //a unit id is a concatination of Nominal, setid, serial no
            List<string> unique_ids_used = new List<string>(); //a list of the ids we have used in the loop below
            
            
            foreach (Measurement m in Measurement.Measurements)
            {
                
                unique_id = m.Nominal + m.CalSetSerial + m.CalibrationGauge.SerialNumber;
                if (!unique_ids_used.Contains(unique_id))
                {
                    int num_id_matches = 0;
                    bool this_is_first_match = true;
                    double t = Convert.ToDouble(m.Temperature);
                    long date_time = Convert.ToDateTime(m.Datetime).Ticks;
                    double nominal = m.CalibrationGauge.Nominal;
                    string ser_no = m.CalibrationGauge.SerialNumber;
                    double sum_centre_dev = m.CalibrationGauge.CentreDeviation;
                    double sum_min_dev = m.CalibrationGauge.MinDev;
                    double sum_max_dev = m.CalibrationGauge.MaxDev;
                    double sum_variation = m.CalibrationGauge.Variation;

                    //with the unique id loop through each measurement
                    foreach (Measurement k in Measurement.Measurements)
                    {
                        string unique_id_to_compare = k.Nominal + k.CalSetSerial + k.CalibrationGauge.SerialNumber;
                        if (unique_id_to_compare.Equals(unique_id))
                        {
                            num_id_matches++;
                            //we have a match but if it's the first match then ignore doing the sums
                            if (!this_is_first_match)
                            {
                                t += Convert.ToDouble(k.Temperature);
                                //we need the date time to be represented as a double to do averaging math on
                                date_time += Convert.ToDateTime(k.Datetime).Ticks;
                                sum_centre_dev += k.CalibrationGauge.CentreDeviation;
                                sum_min_dev += k.CalibrationGauge.MinDev;
                                sum_max_dev += k.CalibrationGauge.MaxDev;
                                sum_variation += k.CalibrationGauge.Variation;
                            }
                            this_is_first_match = false;
                        }
                    }
                    //compute the means
                    sum_centre_dev /= num_id_matches; 
                    sum_min_dev /= num_id_matches;
                    sum_max_dev /= num_id_matches;
                    t /= num_id_matches;
                    date_time /= num_id_matches;
                    var a_dt = new DateTime(date_time);
                    double corr_extreme_dev = Measurement.CalculateExtremeDeviation(sum_min_dev, sum_max_dev);
                    sum_variation /= (num_id_matches);
                    writer2.WriteLine(a_dt.ToString()+"," + t.ToString() + "," + nominal + "," + ser_no + "," + sum_centre_dev + "," + corr_extreme_dev + "," + sum_variation);
                    unique_ids_used.Add(unique_id);
                }
                
            }
            writer2.Close();
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
                    set_index++;
                    return true;
                }
            }

        }
    }
}
