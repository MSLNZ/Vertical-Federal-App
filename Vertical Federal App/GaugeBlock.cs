﻿using System;
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
        private static double single_gauge_wringing_std_uncertainty;
        private static double ref_gauge_std_uncertainty_dependent;
        private static double ref_gauge_std_uncertainty_independent;
        private double stack_wringing_std_uncertainty;
        private double stack_ref_gauge_std_uncertainty_independent;
        private short count = 1;

        public Stack(short num_gauges_in_stack)
        {
            count = num_gauges_in_stack;
            stack_wringing_std_uncertainty = single_gauge_wringing_std_uncertainty * Math.Sqrt(count);
            stack_ref_gauge_std_uncertainty_independent = ref_gauge_std_uncertainty_independent * Math.Sqrt(count);

            switch (count) {
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
            get { return gauge1; }
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

        public short Count
        {
            get { return count; }
        }
    }

    public class Material
    {
        public string material;
        public double poissons_ratio;
        public double youngs_modulus;
        public double exp_coeff;
    }
    public enum ComplianceMetric
    {
        BS_EN_ISO_3650_1999_Grade_K = 0, BS_EN_ISO_3650_1999_Grade_0, BS_EN_ISO_3650_1999_Grade_1, BS_EN_ISO_3650_1999_Grade_2,
        JIS_B_7506_2004_Grade_K, JIS_B_7506_2004_Grade_0, JIS_B_7506_2004_Grade_1, JIS_B_7506_2004_Grade_2,
        AS_1457_1999_Grade_K, AS_1457_1999_Grade_0, AS_1457_1999_Grade_1, AS_1457_1999_Grade_2,
        ASME_B89_1_9_2002_Grade_K, ASME_B89_1_9_2002_Grade_00, ASME_B89_1_9_2002_Grade_0, ASME_B89_1_9_2002_Grade_AS1, ASME_B89_1_9_2002_Grade_AS2
    }
    public enum ComplianceImperial
    {
        BS_4311_1_2007_Grade_K = 17, BS_4311_1_2007_Grade_0, BS_4311_1_2007_Grade_1, BS_4311_1_2007_Grade_2
    }

    public class GaugeBlock
    {
        private int chosen_compliance = -1;
        private string deviation_compliance = "";
        private string variation_compliance = "";
        private double nominal;
        private string serial_number;
        private string client_name;
        private bool metric;
        private double deviation;
        private double extreme_deviation;
        private double A_dev;
        private double B_dev;
        private double D_dev;
        private double E_dev;
        private double min_deviation;
        private double max_deviation;
        private double corrected_length;
        private double variation;
        private double limit_deviation;
        private double tolerance_on_variation;
        private double expanded_uncertainty_dev;
        private double expanded_uncertainty_var;
        private double expanded_uncertainty_ext_dev;
        private double temperature;
        private string from_set; //the name of the reference set this gauge belongs to
        private Material gauge_material;
        private bool illegal_size;
        private double gauge_std_uncertainty_dependent;
        private double gauge_std_uncertainty_independent;
        private double wringing_film_thickness;
        private double cmc_dev;
        private double cmc_var;
        private List<double> temperatures;
        private List<string> dates;

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
            temperatures = new List<double>();
            dates = new List<string>();
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
            gb.A_dev = A_dev;
            gb.B_dev = B_dev;
            gb.D_dev = D_dev;
            gb.E_dev = E_dev;
            gb.extreme_deviation = extreme_deviation;
            gb.min_deviation = min_deviation;
            gb.max_deviation = max_deviation;
            gb.corrected_length = corrected_length;
            gb.variation = variation;
            gb.ComplianceStandard = chosen_compliance;
            if (from_set == null) gb.from_set = null;
            else gb.from_set = (string)from_set.Clone();
            gb.illegal_size = illegal_size;
            gb.WringingFilm = WringingFilm;
            gb.GaugeStdU_Dep = GaugeStdU_Dep;
            gb.GaugeStdU_Indp = GaugeStdU_Indp;
            gb.DeviationOutcome = DeviationOutcome;
            gb.LimitDeviation = LimitDeviation;
            gb.ExpandedUncertaintyDev = ExpandedUncertaintyDev;
            gb.ExpandedUncertaintyVar = ExpandedUncertaintyVar;
            gb.Temperature = Temperature;
            gb.ToleranceVariation = ToleranceVariation;
            gb.VariationOutcome = VariationOutcome;
            gb.DeviationCMC = DeviationCMC;
            gb.VariationCMC = VariationCMC;

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

        public int ComplianceStandard
        {
            get { return chosen_compliance; }
            set { chosen_compliance = value; }
        }

        public string DeviationOutcome
        {
            get { return deviation_compliance; }
            set { deviation_compliance = value; }
        }

        public string VariationOutcome
        {
            get { return variation_compliance; }
            set { variation_compliance = value; }
        }
        public double LimitDeviation
        {
            get { return limit_deviation; }
            set { limit_deviation = value; }
        }
        public double ToleranceVariation
        {
            get { return tolerance_on_variation; }
            set { tolerance_on_variation = value; }
        }
        public double DeviationCMC
        {
            get { return cmc_dev; }
            set { cmc_dev = value; }
        }
        public double VariationCMC
        {
            get { return cmc_var; }
            set { cmc_var = value; }
        }
        public double ExpandedUncertaintyDev
        {
            get { return expanded_uncertainty_dev; }
            set { expanded_uncertainty_dev = value; }
        }
        public double ExpandedUncertaintyExtDev
        {
            get { return expanded_uncertainty_ext_dev; }
            set { expanded_uncertainty_ext_dev = value; }
        }
        public double ExpandedUncertaintyVar
        {
            get { return expanded_uncertainty_var; }
            set { expanded_uncertainty_var = value; }
        }

        public double GaugeStdU_Indp
        {
            set { gauge_std_uncertainty_independent = value; }
            get { return gauge_std_uncertainty_independent; }
        }
        public double GaugeStdU_Dep
        {
            set { gauge_std_uncertainty_dependent = value; }
            get { return gauge_std_uncertainty_dependent; }
        }

        public double WringingFilm
        {
            get { return wringing_film_thickness; }
            set { wringing_film_thickness = value; }
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
        public double Temperature
        {
            get { return temperature; }
            set { temperature = value; }
        }
        /// <summary>
        /// A list of temperatures for measurements on this gauge block.
        /// </summary>
        public List<double> TList
        {
            set { temperatures = value; }
            get { return temperatures; }
        }

        public List<string> DList
        {
            set { dates = value; }
            get { return dates; }
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

        public double ADev
        {
            get { return A_dev; }
            set { A_dev = value; }
        }
        public double BDev
        {
            get { return B_dev; }
            set { B_dev = value; }
        }
        public double DDev
        {
            get { return D_dev; }
            set { D_dev = value; }
        }
        public double EDev
        {
            get { return E_dev; }
            set { E_dev = value; }
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
