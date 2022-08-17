using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;

namespace Vertical_Federal_App
{
    public partial class Report : Form
    {
        private string title = "Report on the Calibration of a Set of Gauge Blocks";
        private string bussiness_name = "";
        private string physical_address = "";
        private string manufacturer_name = "";
        private bool date_plural = false;
        private string s_num = "";
        private string rep_num = "";
        private string j_l_num = "";
        private bool impl = false;
        private bool met = false;
        private DateTime min_date;
        private DateTime max_date;
        private double min_t = 100;
        private double max_t = 0;
        private GaugeBlockSet cal_set;
        private List<GaugeBlock> gauge_blocks;
        //private List<Measurement> raw_measurements;
        private List<string[]> deviations_table_rows;
        private List<string[]> variation_table_rows;
        private List<string[]> uncertainty_table_rows;
        private List<int> compliance_stds;
        private string description;
        private List<GaugeImage> gauge_images;
        private VerticalFederal federal;

        public Report(GaugeBlockSet gbs,ref VerticalFederal fed)
        { 
            InitializeComponent();
            federal = fed;
            cal_set = gbs;
            
            gauge_blocks = cal_set.GaugeList.OrderBy(x => x.Nominal).ToList();
            deviations_table_rows = new List<string[]>();
            variation_table_rows = new List<string[]>();
            uncertainty_table_rows = new List<string[]>();
            compliance_stds = new List<int>();
            gauge_images = new List<GaugeImage>();
        }
        private void JobNoText_TextChanged(object sender, EventArgs e)
        {
            
            j_l_num = JobNoText.Text;
            Title();
        }
        private void Title()
        {
            title = "Report on the Calibration of a Set of Gauge Blocks";
            TitleSerialNumber();
        }
        private void TitleSerialNumber()
        {
            
            s_num = "Set Serial No: " + cal_set.GaugeSetName;
        }

        private void TitleDate()
        {
            
        }
        /// <summary>
        /// Determine whether we have metric only, imperial only or a mixture of metric and imperial
        /// </summary>
        private void Metric_Imperial()
        {
            //are there any imperial gauges in the set
            foreach (GaugeBlock block in gauge_blocks)
            {
                if (!block.Metric)
                {
                    impl = true;
                }
                else
                {
                    met = true;
                }
            }
        }
        private void ComplianceStandardsUsed()
        {
            //are there any imperial gauges in the set
            foreach (GaugeBlock block in gauge_blocks)
            {
                if (!compliance_stds.Contains(block.ComplianceStandard))
                {
                    compliance_stds.Add(block.ComplianceStandard);
                }
            }
        }
        private string CompStdString(int s)
        {
            switch (s)
            {
                case (int) ComplianceMetric.BS_EN_ISO_3650_1999_Grade_K:
                    return "BS EN ISO 3650:1999 Geometrical product specifications (GPS) - Length standards - Gauge blocks, Grade K classification";
                case (int) ComplianceMetric.BS_EN_ISO_3650_1999_Grade_0:
                    return "BS EN ISO 3650:1999 Geometrical product specifications (GPS) - Length standards - Gauge blocks, Grade 0 classification";
                case (int) ComplianceMetric.BS_EN_ISO_3650_1999_Grade_1:
                    return "BS EN ISO 3650:1999 Geometrical product specifications (GPS) - Length standards - Gauge blocks, Grade 1 classification";
                case (int) ComplianceMetric.BS_EN_ISO_3650_1999_Grade_2:
                    return "BS EN ISO 3650:1999 Geometrical product specifications (GPS) - Length standards - Gauge blocks, Grade 2 classification";
                case (int) ComplianceMetric.JIS_B_7506_2004_Grade_K:
                    return "JIS B 7506:2004: (JMA) Gauge Blocks, Grade K classification";
                case (int) ComplianceMetric.JIS_B_7506_2004_Grade_0:
                    return "JIS B 7506:2004: (JMA) Gauge Blocks, Grade 0 classification";
                case (int) ComplianceMetric.JIS_B_7506_2004_Grade_1:
                    return "JIS B 7506:2004: (JMA) Gauge Blocks, Grade 1 classification";
                case (int) ComplianceMetric.JIS_B_7506_2004_Grade_2:
                    return "JIS B 7506:2004: (JMA) Gauge Blocks, Grade 2 classification";
                case (int) ComplianceMetric.AS_1457_1999_Grade_K:
                    return "AS 1457-1999: Geometrical Product Specifications (GPS) - Length standards - Gauge blocks, Grade K classification";
                case (int) ComplianceMetric.AS_1457_1999_Grade_0:
                    return "AS 1457-1999: Geometrical Product Specifications (GPS) - Length standards - Gauge blocks, Grade 0 classification";
                case (int) ComplianceMetric.AS_1457_1999_Grade_1:
                    return "AS 1457-1999: Geometrical Product Specifications (GPS) - Length standards - Gauge blocks, Grade 1 classification";
                case (int) ComplianceMetric.AS_1457_1999_Grade_2:
                    return "AS 1457-1999: Geometrical Product Specifications (GPS) - Length standards - Gauge blocks, Grade k classification";
                case (int) ComplianceMetric.ASME_B89_1_9_2002_Grade_K:
                    return "ASME B89.1.9-2002: Gauge Blocks, Grade K classification";
                case (int) ComplianceMetric.ASME_B89_1_9_2002_Grade_00:
                    return "ASME B89.1.9-2002: Gauge Blocks, Grade 00 classification";
                case (int) ComplianceMetric.ASME_B89_1_9_2002_Grade_0:
                    return "ASME B89.1.9-2002: Gauge Blocks, Grade 0 classification";
                case (int) ComplianceMetric.ASME_B89_1_9_2002_Grade_AS1:
                    return "ASME B89.1.9-2002: Gauge Blocks, Grade AS1 classification";
                case (int) ComplianceMetric.ASME_B89_1_9_2002_Grade_AS2:
                    return "ASME B89.1.9-2002: Gauge Blocks, Grade AS2 classification";
                case (int) ComplianceImperial.BS_4311_1_2007_Grade_K:
                    return "BS 4311 - 1:2007 Gauge blocks manufactured to imperial specification – Part 1: Specification and validation, Grade K classification";
                case (int) ComplianceImperial.BS_4311_1_2007_Grade_0:
                    return "BS 4311 - 1:2007 Gauge blocks manufactured to imperial specification – Part 1: Specification and validation, Grade 0 classification";
                case (int) ComplianceImperial.BS_4311_1_2007_Grade_1:
                    return "BS 4311 - 1:2007 Gauge blocks manufactured to imperial specification – Part 1: Specification and validation, Grade 1 classification";
                case (int) ComplianceImperial.BS_4311_1_2007_Grade_2:
                    return "BS 4311 - 1:2007 Gauge blocks manufactured to imperial specification – Part 1: Specification and validation, Grade 2 classification";
                default: return "";
            }
        }
        private void DateOfCalibration()
        {
            min_date = DateTime.MaxValue;
            max_date = DateTime.MinValue;

            foreach (GaugeBlock g in gauge_blocks)
            {
                foreach (string k in g.DList)
                {
                    DateTime d = Convert.ToDateTime(k);
                    if (d <= min_date) min_date = d;
                    else if (d > max_date) max_date = d;
                }
            }
            if (min_date.Date != max_date.Date) date_plural = true; 
        }

        private void BussinessName_TextChanged(object sender, EventArgs e)
        {
            bussiness_name = BussinessName.Text;
        }

        private void PhysicalAddress_TextChanged(object sender, EventArgs e)
        {
            physical_address = PhysicalAddress.Text;
        }

        public void UpdatePhysicalConditions()
        {
            foreach(GaugeBlock g in gauge_blocks)
            {
                foreach (double t in g.TList)
                {
                    if (t <= min_t) min_t = t;
                    if (t >= max_t) max_t = t;
                }
            }
        }
        public void PrepareJobNumber()
        {

        }
            
        public void PrepareReportTable()
        {
            foreach (GaugeBlock g in gauge_blocks)
            {
                string[] gauge_dev_res = new string[5];

                gauge_dev_res[0] = g.Nominal.ToString(); //the nominal gauge length is in column 1 (zero based).
                gauge_dev_res[1] = g.SerialNumber; //the serial number is in column 2
                gauge_dev_res[2] = g.CentreDeviation.ToString(); //the deviation is in column 3
                gauge_dev_res[3] = g.ExtremeDeviation.ToString(); //the extreme deviation is in column 7
                gauge_dev_res[4] = g.DeviationOutcome; //the compliance is in column 15

                deviations_table_rows.Add(gauge_dev_res);

                string[] gauge_var_res = new string[4];
                gauge_var_res[0] = g.Nominal.ToString(); //the nominal gauge length is in column 1 (zero based).
                gauge_var_res[1] = g.SerialNumber; //the serial number is in column 2
                gauge_var_res[2] = g.Variation.ToString(); //the variation is in column 11
                gauge_var_res[3] = g.VariationOutcome; //the compliance is in column 16

                variation_table_rows.Add(gauge_var_res);

                string[] gauge_uncert_res = new string[5];

                gauge_uncert_res[0] = g.Nominal.ToString(); //the nominal gauge length is in column 1 (zero based).
                gauge_uncert_res[1] = g.SerialNumber; //the serial number is in column 2
                gauge_uncert_res[2] = g.ExpandedUncertaintyDev.ToString(); ; //the variation is in column 11
                gauge_uncert_res[3] = g.ExpandedUncertaintyExtDev.ToString(); //the compliance is in column 16
                gauge_uncert_res[4] = g.ExpandedUncertaintyVar.ToString(); 

                uncertainty_table_rows.Add(gauge_uncert_res);   
            }


        }

        private void WriteReportButton_Click(object sender, EventArgs e)
        {
            string fileName = @"G:\Shared drives\MSL - Length\Length\Federal\FederalData\latex.tex";
            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file     
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file    
                    Byte[] line = new UTF8Encoding(true).GetBytes("%%=======================================================================\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\documentclass[IANZ]{MSLCalCert}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\usepackage{parskip}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\usepackage{upgreek}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\usepackage{longtable,dcolumn}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\usepackage{ragged2e}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\usepackage{graphics}\n"); fs.Write(line, 0, line.Length);
                    
                    List<string> directories_found = new List<string>();
                    StringBuilder s = new StringBuilder();
                    s.Append("\\graphicspath{ ");
                    foreach(GaugeImage g in gauge_images)
                    {
                        if(!directories_found.Contains(g.Directory)) //only add it once
                        {
                            directories_found.Add(g.Directory);
                            s.Append("{" + g.Directory + "}");
                        }

                    }
                    s.Append("}\n");

                    line = new UTF8Encoding(true).GetBytes(s.ToString()+"\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\begin{document}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\date{" + DateTime.Now.ToString("dd MMMM yyyy") + "}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\reportnumber{"+rep_num+"}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\serial{Set Serial Number:" + cal_set.GaugeSetName+"}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\fileref{"+ j_l_num +"}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\title{" + title + "}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\maketitlepage\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\section{Description}\n"); fs.Write(line, 0, line.Length);
                    DescriptionSection();
                    line = new UTF8Encoding(true).GetBytes(description +"\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\section{Identification}\n"); fs.Write(line, 0, line.Length);
                    

                    foreach (GaugeImage g in gauge_images)
                    {
                        line = new UTF8Encoding(true).GetBytes(g.Description+"\\par\n"); fs.Write(line, 0, line.Length);
                        line = new UTF8Encoding(true).GetBytes("\\includegraphics[width=16cm]{"+g.Filename+"}\n"); fs.Write(line, 0, line.Length);
                        line = new UTF8Encoding(true).GetBytes(g.Caption+".\\par\\pagebreak \n"); fs.Write(line, 0, line.Length);
                    }

                    line = new UTF8Encoding(true).GetBytes("\\section{Client}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes(bussiness_name + ", " +physical_address+".\n"); fs.Write(line, 0, line.Length);

                    DateOfCalibration();
                    string doc = $"{(date_plural ? "Dates of Calibration" : "Date of Calibration")}";
                    line = new UTF8Encoding(true).GetBytes("\\section{"+doc+"}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes(min_date.ToString("dd MMMM yyyy")+" to "+ max_date.ToString("dd MMMM yyyy") +".\n"); fs.Write(line, 0, line.Length);
                    
                    line = new UTF8Encoding(true).GetBytes("\\section{Method}\n"); fs.Write(line, 0, line.Length);

                    

                    

                    if (gauge_blocks.Count > 1)
                    {
                        line = new UTF8Encoding(true).GetBytes("The deviation and variation in length" +
                            " of each gauge block was measured according to technical procedure "+federal.TechnicalProcedure+
                            " \\emph{Gauge Blocks(<= 101.4 mm), Calibration by comparison}.  Measurements were " +
                            "taken in five positions on each gauge(at the centre and toward each corner).");
                        fs.Write(line, 0, line.Length);
                        line = new UTF8Encoding(true).GetBytes("\\emph{Deviation} is defined as the measured length minus the nominal length." +
                            " \\emph{Extreme deviation} is defined as either the maximum or minimum measured deviation, " +
                            "depending on which has the larger magnitude.  \\emph{Variation} in length is defined as the difference " +
                            "between the maximum measured length and the minimum measured length. " +
                            "\\emph{Limit Deviation} is defined as the permissible deviation at any point on the measuring face.\n");
                        fs.Write(line, 0, line.Length);
                        
                    }
                    else
                    {
                        line = new UTF8Encoding(true).GetBytes("The deviation and variation in length" +
                            " of the gauge block was measured according to technical procedure MSLT.L.003.008" +
                            " \\emph{Gauge Blocks(<= 101.4 mm), Calibration by comparison}.  Measurements were " +
                            "taken in five positions on the gauge(at the centre and toward each corner).");
                        fs.Write(line, 0, line.Length);
                        line = new UTF8Encoding(true).GetBytes("\\emph{Deviation} is defined as the measured length minus the nominal length." +
                            " \\emph{Extreme deviation} is defined as either the maximum or minimum measured deviation, " +
                            "depending on which has the larger magnitude.  \\emph{Variation} in length is defined as the difference " +
                            "between the maximum measured length and the minimum measured length. " +
                            "\\emph{Limit Deviation} is defined as the permissible deviation at any point on the measuring face.\n");
                        fs.Write(line, 0, line.Length);
                    }
                    Metric_Imperial();
                    string units = "0.001 $\\mu$m";
                    //are any gauges in the set imperial
                    if (impl && !met)
                    {
                        line = new UTF8Encoding(true).GetBytes("1 inch is defined as 25.4 mm.\n"); fs.Write(line, 0, line.Length);
                        units = "0.1 $\\mu$inch";
                    }
                    else if(impl && met)
                    {
                        units = "0.1 $\\mu$inch and 0.001 $\\mu$m";
                    }

                    ComplianceStandardsUsed();
                    line = new UTF8Encoding(true).GetBytes("\\section{Objectives}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("To measure the centre deviation, extreme deviation and variation " +
                        "in length of each gauge block for compliance with:\\par\n"); fs.Write(line, 0, line.Length);

                    
                    foreach(int st in compliance_stds) {  
                        line = new UTF8Encoding(true).GetBytes(CompStdString(st) + ".\\par\n"); fs.Write(line, 0, line.Length); //the last standard list, so we dont need a new line
                    }

                    line = new UTF8Encoding(true).GetBytes("The standard(s) listed here are referred to as \"documentary standards\" in this report.\n"); fs.Write(line, 0, line.Length);

                    UpdatePhysicalConditions();

                    line = new UTF8Encoding(true).GetBytes("\\section{Conditions}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("Gauge block measurements were made with the comparator " +
                        "platen in the temperature range of $\\SI{"+Math.Round(min_t,2).ToString()+"}{\\celsius} $ to " +
                        "$\\SI{"+Math.Round(max_t,2).ToString()+"}{\\celsius}$.\\pagebreak\n"); fs.Write(line, 0, line.Length);

                    string std = CompStdString(compliance_stds[0]);  

                    //if we have multiple standards being used we will need to modify this file.  Its too hard to program this
                    line = new UTF8Encoding(true).GetBytes("\\section{Results}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("The centre deviation of the gauge blocks is given in Table 1. " +
                        "The extreme deviation in length is also shown for determining compliance " +
                        "with the documentary standard(s) for limit deviation.\\par " +
                        "The variation in length of the gauge blocks is given in Table 2. The tolerance for the variation " +
                        "in length is also shown for determining compliance " +
                        "with the documentary standard(s) for variation in length.\\par " +
                        "The tables also include results for compliance with the requirements of the documentary standard(s) for each gauge block for the limit deviation and variation in length. " +
                        "A ‘P’ in the compliance columns states that the gauge block meets the requirements of documentary standard for the tested condition .Similarly, " +
                        "an ‘F’ in the compliance columns states that the gauge block does not meet the requirements of documentary standard for the tested condition. " +
                        "A ‘U’ in the compliance columns states that compliance to the documentary standard for the tested condition cannot be confirmed or refuted.\\par " +
                        "The expanded measurement uncertainty is considered for all compliance outcomes.\\par " +
                        "The results are rounded to the nearest "+units+" and are valid at a reference temperature of $\\SI{20}{\\celsius}$.\\par \n"); fs.Write(line, 0, line.Length);

                    string k = $"{(met ? "mm" : "inch")}";
                    string l = $"{(met ? "$\\mu$m" : "$\\mu$inch")}";


                    line = new UTF8Encoding(true).GetBytes("\\setlength\\extrarowheight{1pt}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\setlength\\tabcolsep{10pt}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\begin{longtable}{D{.}{.}{4}D{.}{.}{4}D{.}{.}{5}D{.}{.}{5}D{.}{.}{4}D{.}{.}{4}}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 1\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{6}{l}{ \\text{Table 1: Deviations for gauge block set "+ cal_set.GaugeSetName + ".} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 2\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{6}{l}{ \\text{} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 3\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{Nominal} } & \\multicolumn{1}{c}{ \\text{Serial} } & \\multicolumn{1}{c}{ \\text{Deviation} } & \\multicolumn{1}{c}{ \\text{Extreme} } & \\multicolumn{1}{c}{ \\text{Limit} } & \\multicolumn{1}{c}{ \\text{Compliance} } \\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 4\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{Length} } & \\multicolumn{1}{c}{ \\text{Number} } & \\multicolumn{1}{c}{ \\text{} } & \\multicolumn{1}{c}{ \\text{Deviation} } & \\multicolumn{1}{c}{ \\text{Deviation} } & \\multicolumn{1}{c}{ \\text{Outcome}}\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 5\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{("+k+")}} & &\\multicolumn{1}{c}{ \\text{("+l+")}} &\\multicolumn{1}{c}{ \\text{("+l+")}} &\\multicolumn{1}{c}{ \\text{("+l+")}} &\\\\ \\hline\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\endfirsthead \n"); fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes("%Line 1\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{6}{l}{ \\text{Table 1(continued): Deviations for gauge block set " + cal_set.GaugeSetName + ".} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 2\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{6}{l}{ \\text{} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 3\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{Nominal} } & \\multicolumn{1}{c}{\\text{Serial} } & \\multicolumn{1}{c}{ \\text{Deviation} } & \\multicolumn{1}{c}{ \\text{Extreme} } & \\multicolumn{1}{c}{ \\text{Limit} } & \\multicolumn{1}{c}{ \\text{Compliance} } \\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 4\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{Length} } & \\multicolumn{1}{c}{ \\text{Number} } & \\multicolumn{1}{c}{ \\text{} } & \\multicolumn{1}{c}{ \\text{Deviation} } & \\multicolumn{1}{c}{ \\text{Deviation} } & \\multicolumn{1}{c}{ \\text{Outcome}}\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 5\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{(" + k + ")}} & &\\multicolumn{1}{c}{\\text{(" + l + ")}} &\\multicolumn{1}{c}{\\text{(" + l + ")}} &\\multicolumn{1}{c}{ \\text{(" + l + ")}} &\\\\ \\hline\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\endhead \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\endfoot \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\endlastfoot \n"); fs.Write(line, 0, line.Length);

                    string cd = "";
                    string ed = "";
                    string d_tol = "";
                    
                    foreach (GaugeBlock g in gauge_blocks)
                    {
                        cd = $"{(met ? Math.Round(g.CentreDeviation, 3).ToString("N3") : Math.Round(g.CentreDeviation,1).ToString("N1"))}";
                        ed = $"{(met ? Math.Round(g.ExtremeDeviation, 3).ToString("N3") : Math.Round(g.ExtremeDeviation, 1).ToString("N1"))}";
                        d_tol = $"{(met ? Math.Round(g.LimitDeviation, 3).ToString("N2") : Math.Round(g.LimitDeviation, 0).ToString("N0"))}";
                        line = new UTF8Encoding(true).GetBytes(g.Nominal.ToString() + " & " + g.SerialNumber +" & " + cd + 
                            " & " + ed + " & " + d_tol + " & " + g.DeviationOutcome + "\\\\ \n"); fs.Write(line, 0, line.Length);
                    }
                    line = new UTF8Encoding(true).GetBytes("\\end{longtable} \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\pagebreak \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\begin{longtable}{D{.}{.}{4}D{.}{.}{4}D{.}{.}{5}D{.}{.}{8}D{.}{.}{4}} \n"); fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes("%Line 1\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{5}{l}{ \\text{Table 2: Variation in length for gauge block set " + cal_set.GaugeSetName + ".} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 2\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{5}{l}{ \\text{} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 3\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{Nominal} } & \\multicolumn{1}{c}{ \\text{Serial} } & \\multicolumn{1}{c}{ \\text{Variation} } & \\multicolumn{1}{c}{ \\text{Tolerance on} } & \\multicolumn{1}{c}{ \\text{Compliance}}\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 4\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{Length} } & \\multicolumn{1}{c}{ \\text{Number} } & \\multicolumn{1}{c}{ \\text{in Length} } & \\multicolumn{1}{c}{ \\text{Variation in Length} } & \\multicolumn{1}{c}{ \\text{Outcome}} \\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 5\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{(" + k + ")}} & \\multicolumn{1}{c}{ \\text{}} & \\multicolumn{1}{c}{ \\text{(" + l + ")}} & \\multicolumn{1}{c}{\\text{(" + l + ")}} & \\\\ \\hline\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\endfirsthead \n"); fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes("%Line 1\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{5}{l}{ \\text{Table 2(continued): Variation in length for gauge block set " + cal_set.GaugeSetName + ".} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 2\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{5}{l}{ \\text{} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 3\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{Nominal} } & \\multicolumn{1}{c}{ \\text{Serial} } & \\multicolumn{1}{c}{ \\text{Variation} } & \\multicolumn{1}{c}{ \\text{Tolerance on} } & \\multicolumn{1}{c}{ \\text{Compliance}}\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 4\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{Length} } & \\multicolumn{1}{c}{ \\text{Number} } & \\multicolumn{1}{c}{ \\text{in Length} } & \\multicolumn{1}{c}{ \\text{Variation in Length} } & \\multicolumn{1}{c}{ \\text{Outcome}} \\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 5\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{(" + k + ")}} & \\multicolumn{1}{c}{\\text{}} & \\multicolumn{1}{c}{\\text{(" + l + ")}} & \\multicolumn{1}{c}{\\text{(" + l + ")}} & \\\\ \\hline\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\endhead \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\endfoot \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\endlastfoot \n"); fs.Write(line, 0, line.Length);

                    string vr = "";
                    string v_tol = "";
                    foreach (GaugeBlock g in gauge_blocks)
                    {
                        vr = $"{(met ? Math.Round(g.Variation, 3).ToString("N3") : Math.Round(g.Variation, 1).ToString("N1"))}";
                        v_tol = $"{(met ? Math.Round(g.ToleranceVariation, 2).ToString("N2") : Math.Round(g.ToleranceVariation, 0).ToString("N0"))}";

                        line = new UTF8Encoding(true).GetBytes(g.Nominal.ToString() + " & " + g.SerialNumber + " & " + vr +
                            " & " + v_tol + " & " + g.VariationOutcome + "\\\\ \n"); fs.Write(line, 0, line.Length);
                    }
                    line = new UTF8Encoding(true).GetBytes("\\end{longtable} \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\pagebreak \n"); fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes("\\section{Uncertainty}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("The expanded measurement uncertainties for the deviation and variation in length measurements are given in the Table 3.\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\setlength\\tabcolsep{10pt}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\begin{longtable}{D{.}{.}{4}D{.}{.}{4}D{.}{.}{5}D{.}{.}{5}D{.}{.}{11}}\n"); fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes("%Line 1\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{5}{l}{ \\text{Table 3: Expanded uncertainties for gauge block set " + cal_set.GaugeSetName + ".} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 2\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{5}{l}{ \\text{} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 3\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{Nominal} } & \\multicolumn{1}{c}{ \\text{Serial} } & \\multicolumn{1}{c}{ \\text{Centre} } & \\multicolumn{1}{c}{ \\text{Extreme} } &  \\multicolumn{1}{c}{ \\text{Variation} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 4\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{Length} } & \\multicolumn{1}{c}{ \\text{Number} } & \\multicolumn{1}{c}{ \\text{Deviation} } & \\multicolumn{1}{c}{ \\text{Deviation} } & \\multicolumn{1}{c}{ \\text{in Length} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 5\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{(" + k + ")}} & \\multicolumn{1}{c}{ \\text{} } & \\multicolumn{1}{c}{ \\text{(" + l + ")}} & \\multicolumn{1}{c}{ \\text{(" + l + ")}} & \\multicolumn{1}{c}{ \\text{(" + l + ")}} \\\\ \\hline\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\endfirsthead\n"); fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes("%Line 1\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{5}{l}{ \\text{Table 3(continued): Expanded uncertainties for gauge block set " + cal_set.GaugeSetName + ".} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 2\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{5}{l}{ \\text{} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 3\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{Nominal} } & \\multicolumn{1}{c}{ \\text{Serial} } & \\multicolumn{1}{c}{ \\text{Centre} } & \\multicolumn{1}{c}{ \\text{Extreme} } &  \\multicolumn{1}{c}{ \\text{Variation} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 4\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{Length} } & \\multicolumn{1}{c}{ \\text{Number} } & \\multicolumn{1}{c}{ \\text{Deviation} } & \\multicolumn{1}{c}{ \\text{Deviation} } & \\multicolumn{1}{c}{ \\text{in Length} }\\\\ \n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("%Line 5\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\multicolumn{1}{c}{ \\text{(" + k + ")}} & \\multicolumn{1}{c}{ \\text{} } & \\multicolumn{1}{c}{ \\text{(" + l + ")}} & \\multicolumn{1}{c}{ \\text{(" + l + ")}} & \\multicolumn{1}{c}{ \\text{(" + l + ")}} \\\\ \\hline\n"); fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes("\\endhead\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\endfoot\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\endlastfoot\n"); fs.Write(line, 0, line.Length);

                    string ed_u = "";
                    string cd_u = "";
                    string vr_u = "";

                    foreach (GaugeBlock g in gauge_blocks)
                    {

                        cd_u = $"{(met ? Math.Round(g.ExpandedUncertaintyDev/1000, 3).ToString("N3") : Math.Round(g.ExpandedUncertaintyDev, 1).ToString("N1"))}";
                        ed_u = $"{(met ? Math.Round(g.ExpandedUncertaintyExtDev/1000, 3).ToString("N3") : Math.Round(g.ExpandedUncertaintyExtDev, 1).ToString("N1"))}";
                        vr_u = $"{(met ? Math.Round(g.ExpandedUncertaintyVar/1000, 3).ToString("N3") : Math.Round(g.ExpandedUncertaintyVar, 1).ToString("N1"))}";
                        line = new UTF8Encoding(true).GetBytes(g.Nominal.ToString() + " & " + g.SerialNumber + " & " + cd_u +
                            " & " + ed_u + " & " + vr_u + "\\\\ \n"); fs.Write(line, 0, line.Length);
                    }

                    line = new UTF8Encoding(true).GetBytes("\\end{longtable}\n"); fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes("A coverage factor of 2.0 was used to calculate the expanded " +
                        "uncertainties at a level of confidence of approximately $95\\%$. The number of degrees of freedom" +
                        " associated with each measurement result was large enough to justify this coverage factor.\n"); fs.Write(line, 0, line.Length);

                   
                    //line = new UTF8Encoding(true).GetBytes("\\pagebreak\n"); fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes("\\paragraph{Note:} \\referenceGUM\n"); fs.Write(line, 0, line.Length);

                    line = new UTF8Encoding(true).GetBytes("\\sigA{L A Evergreen}{Length Standards}{}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\sigB{C M Young}{Length Standards}{}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\chiefMetrologistDelegate{ T J Stewart}\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\signaturesAB\n"); fs.Write(line, 0, line.Length);
                    line = new UTF8Encoding(true).GetBytes("\\end{document}\n"); fs.Write(line, 0, line.Length);

                    fs.Close();
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        private void ReportNumberTextbox_TextChanged(object sender, EventArgs e)
        {

            rep_num = "Length/" + DateTime.Now.ToString("yyyy") + "/" + ReportNumberTextbox.Text;
        }

        private void DescriptionSection()
        {
            //For this section we need the following metadata.
            //The number of metric gauges in the set and what they are made from and the range of sizes.
            //The number of imperial gauges in the set and what they are made from and the range of sizes.

            //we need to group the gauges into set split them into metric and imperial and material
            List<GaugeBlock> metric_subset_steel = new List<GaugeBlock>();
            double min_metric_steel_size = 1000;
            double max_metric_steel_size = 0;
            List<GaugeBlock> metric_subset_ceramic = new List<GaugeBlock>();
            double min_metric_ceramic_size = 1000;
            double max_metric_ceramic_size = 0;
            List<GaugeBlock> metric_subset_TC = new List<GaugeBlock>();
            double min_metric_tc_size = 1000;
            double max_metric_tc_size = 0;

            List<GaugeBlock> imperial_subset_steel = new List<GaugeBlock>();
            double min_imperial_steel_size = 1000;
            double max_imperial_steel_size = 0;
            List<GaugeBlock> imperial_subset_ceramic = new List<GaugeBlock>();
            double min_imperial_ceramic_size = 1000;
            double max_imperial_ceramic_size = 0;
            List<GaugeBlock> imperial_subset_TC = new List<GaugeBlock>();
            double min_imperial_tc_size = 1000;
            double max_imperial_tc_size = 0;

            double min_metric_size = 1000;
            double min_imperial_size = 1000;
            double max_metric_size = 0;
            double max_imperial_size = 0;

            foreach (GaugeBlock g in gauge_blocks)
            {

                if (g.Metric)
                {
                    if (g.Nominal >= max_metric_size) max_metric_size = g.Nominal;
                    if (g.Nominal <= min_metric_size) min_metric_size = g.Nominal;

                    switch (g.GaugeBlockMaterial.material)
                    {
                        case "steel":
                            metric_subset_steel.Add(g);
                            if(g.Nominal <= min_metric_steel_size) min_metric_steel_size = g.Nominal;
                            if (g.Nominal >= max_metric_steel_size) max_metric_steel_size = g.Nominal;
                            break;
                        case "ceramic":
                            metric_subset_ceramic.Add(g);
                            if (g.Nominal <= min_metric_ceramic_size) min_metric_ceramic_size = g.Nominal;
                            if (g.Nominal >= max_metric_ceramic_size) max_metric_ceramic_size = g.Nominal;
                            break;
                        case "tungsten carbide":
                            metric_subset_TC.Add(g);
                            if (g.Nominal <= min_metric_tc_size) min_metric_tc_size = g.Nominal;
                            if (g.Nominal >= max_metric_tc_size) max_metric_tc_size = g.Nominal;
                            break;
                    }
                }
                else
                {
                    if (g.Nominal >= max_imperial_size) max_imperial_size = g.Nominal;
                    if (g.Nominal <= min_imperial_size) min_imperial_size = g.Nominal;
                    switch (g.GaugeBlockMaterial.material)
                    {
                        case "steel":
                            imperial_subset_steel.Add(g);
                            if (g.Nominal <= min_imperial_steel_size) min_imperial_steel_size = g.Nominal;
                            if (g.Nominal <= max_imperial_steel_size) max_imperial_steel_size = g.Nominal;
                            break;
                        case "ceramic":
                            imperial_subset_ceramic.Add(g);
                            if (g.Nominal <= min_imperial_steel_size) min_imperial_ceramic_size = g.Nominal;
                            if (g.Nominal <= max_imperial_steel_size) max_imperial_ceramic_size = g.Nominal;
                            break;
                        case "tungsten carbide":
                            imperial_subset_TC.Add(g);
                            if (g.Nominal <= min_imperial_steel_size) min_imperial_tc_size = g.Nominal;
                            if (g.Nominal <= max_imperial_steel_size) max_imperial_tc_size = g.Nominal;
                            break;
                    }
                }
            }

            bool metric_only = false;
            bool imperial_only = false;
            bool metric_imperial = false;
            int metric_count = metric_subset_steel.Count + metric_subset_ceramic.Count + metric_subset_TC.Count;
            int imperial_count = imperial_subset_steel.Count + imperial_subset_ceramic.Count + imperial_subset_TC.Count;

            //do we have metric or imperial (or both) gauge block in the set?
            if (metric_count != 0) metric_only = true;

            if(imperial_count != 0)
            {
                if (metric_only)
                {
                    metric_imperial = true;
                    metric_only = false;
                }
                else imperial_only = true;
                
            }

            StringBuilder str = new StringBuilder();
            //special case when there's only one gauge (singular case...no imperial cases)
            if ((metric_only && metric_count == 1) || (imperial_only && imperial_count == 1))
            {
                string m = " metric ";
                string u = " mm.";
                if (gauge_blocks[0].Metric)
                {
                    m = " imperial ";
                    u = " inch.";
                }
                str.Append("An " + m + gauge_blocks[0].GaugeBlockMaterial.material + " gauge block manufactured by " + manufacturer_name
                    + ". The nominal size of the gauge block is "
                        + gauge_blocks[0].Nominal.ToString() + u);
            }

            //Case when the set contains only metric gauges 
            else if (metric_only && metric_count > 1)
            {
                //case when all gauges are made from the same material (no singular cases)
                if (metric_count == metric_subset_steel.Count || metric_count == metric_subset_ceramic.Count || metric_count == metric_subset_TC.Count)
                {
                    double min = 0;
                    double max = 0;
                    if (metric_subset_steel.Count != 0)
                    {
                        min = min_metric_steel_size;
                        max = max_metric_steel_size;
                    }
                    else if (metric_subset_ceramic.Count != 0)
                    {
                        min = min_metric_ceramic_size;
                        max = max_metric_ceramic_size;
                    }
                    else if (metric_subset_TC.Count != 0)
                    {
                        min = min_metric_tc_size;
                        max = max_metric_tc_size;
                    }

                    str.Append("The gauge block set is manufactured by " + manufacturer_name
                        + " and contains " + TextbasedNumeric(metric_count) + " metric " + gauge_blocks[0].GaugeBlockMaterial.material + " gauge blocks ranging in nominal size from "
                            + min.ToString() + " mm to " + max.ToString() + " mm.");
                }

                //case where gauges in the set are made of different materials (this may have singular cases)
                else
                {
                    int c = -1;
                    if (metric_subset_steel.Count != 0 && metric_subset_ceramic.Count != 0 && metric_subset_TC.Count == 0) c = 0;  //steel and ceramic gauges in the set
                    else if (metric_subset_steel.Count != 0 && metric_subset_ceramic.Count == 0 && metric_subset_TC.Count != 0) c = 1;  //steel and tc gauges in the set
                    else if (metric_subset_steel.Count == 0 && metric_subset_ceramic.Count != 0 && metric_subset_TC.Count != 0) c = 2;  //ceramic and tc gauges in the set
                    else if (metric_subset_steel.Count != 0 && metric_subset_ceramic.Count != 0 && metric_subset_TC.Count != 0) c = 3;  //steel, ceramic and tc gauge in the set

                    string sentence_start = "The gauge block set is manufactured by " + manufacturer_name
                            + " and contains a total of " + TextbasedNumeric(metric_count) + " metric gauge blocks ranging in nominal size from "
                            + min_metric_size.ToString() + " mm to " + max_metric_size.ToString() + " mm.  The total comprises of ";

                    string steel_singular = "";
                    string ceramic_singular = "";
                    string tc_singular = "";
                    if (metric_subset_steel.Count != 1) steel_singular = "s";
                    if (metric_subset_ceramic.Count != 1) ceramic_singular = "s";
                    if (metric_subset_TC.Count != 1) tc_singular = "s";

                    switch (c)
                    {
                        
                        case 1:
                            str.Append(sentence_start + TextbasedNumeric(metric_subset_steel.Count)
                            + " steel gauge{steel_singular} and " + TextbasedNumeric(metric_subset_ceramic.Count) + " ceramic gauge{ceramic_singular}.");
                            break;
                        case 2:
                            str.Append(sentence_start + TextbasedNumeric(metric_subset_steel.Count)
                            + " steel gauge{steel_singular} and " + TextbasedNumeric(metric_subset_TC.Count) + " tungsten carbide gauge{tc_singular}.");
                            break;
                        case 3:
                            str.Append(sentence_start + TextbasedNumeric(metric_subset_ceramic.Count)
                            + " ceramic gauge{ceramic_singular} and " + TextbasedNumeric(metric_subset_TC.Count) + " tungsten carbide gauge{tc_singular}.");
                            break;
                        case 4:
                            str.Append(sentence_start + TextbasedNumeric(metric_subset_steel.Count)
                            + " steel gauge{steel_singular}, " + TextbasedNumeric(metric_subset_ceramic.Count) + " ceramic gauge{ceramic_singular} and "
                            + TextbasedNumeric(metric_subset_TC.Count) + " tungsten carbide gauge{tc_singular}.");
                            break;
                    }
                }

            }
            //Case when the set contains only imperial gauges 
            else if (imperial_only && imperial_count > 1)
            {
                //case when all gauges are made from the same material (no singular cases)
                if (imperial_count == imperial_subset_steel.Count || imperial_count == imperial_subset_ceramic.Count || imperial_count == imperial_subset_TC.Count)
                {
                    double min = 0;
                    double max = 0;
                    if (imperial_subset_steel.Count != 0)
                    {
                        min = min_imperial_steel_size;
                        max = max_imperial_steel_size;
                    }
                    else if (imperial_subset_ceramic.Count != 0)
                    {
                        min = min_imperial_ceramic_size;
                        max = max_imperial_ceramic_size;
                    }
                    else if (imperial_subset_TC.Count != 0)
                    {
                        min = min_imperial_tc_size;
                        max = max_imperial_tc_size;
                    }

                    str.Append("The gauge block set is manufactured by " + manufacturer_name
                        + " and contains " + TextbasedNumeric(imperial_count) + " imperial " + gauge_blocks[0].GaugeBlockMaterial.material + " gauge blocks ranging in nominal size from "
                            + min.ToString() + " mm to " + max.ToString() + " mm.");
                }

                //case where gauges in the set are made of different materials (this may have singular cases)
                else
                {
                    int c = -1;
                    if (imperial_subset_steel.Count != 0 && imperial_subset_ceramic.Count != 0 && imperial_subset_TC.Count == 0) c = 0;  //steel and ceramic gauges in the set
                    else if (imperial_subset_steel.Count != 0 && imperial_subset_ceramic.Count == 0 && imperial_subset_TC.Count != 0) c = 1;  //steel and tc gauges in the set
                    else if (imperial_subset_steel.Count == 0 && imperial_subset_ceramic.Count != 0 && imperial_subset_TC.Count != 0) c = 2;  //ceramic and tc gauges in the set
                    else if (imperial_subset_steel.Count != 0 && imperial_subset_ceramic.Count != 0 && imperial_subset_TC.Count != 0) c = 3;  //steel, ceramic and tc gauge in the set

                    string sentence_start = "The gauge block set is manufactured by " + manufacturer_name
                            + " and contains a total of " + TextbasedNumeric(imperial_count) + " imperial gauge blocks ranging in nominal size from "
                            + min_imperial_size.ToString() + " mm to " + max_imperial_size.ToString() + " mm.  The total comprises of ";

                    string steel_singular = "";
                    string ceramic_singular = "";
                    string tc_singular = "";
                    if (imperial_subset_steel.Count != 1) steel_singular = "s";
                    if (imperial_subset_ceramic.Count != 1) ceramic_singular = "s";
                    if (imperial_subset_TC.Count != 1) tc_singular = "s";

                    switch (c)
                    {

                        case 1:
                            str.Append(sentence_start + TextbasedNumeric(imperial_subset_steel.Count)
                            + " steel gauge{steel_singular} and " + TextbasedNumeric(imperial_subset_ceramic.Count) + " ceramic gauge{ceramic_singular}.");
                            break;
                        case 2:
                            str.Append(sentence_start + TextbasedNumeric(imperial_subset_steel.Count)
                            + " steel gauge{steel_singular} and " + TextbasedNumeric(imperial_subset_TC.Count) + " tungsten carbide gauge{tc_singular}.");
                            break;
                        case 3:
                            str.Append(sentence_start + TextbasedNumeric(imperial_subset_ceramic.Count)
                            + " ceramic gauge{ceramic_singular} and " + TextbasedNumeric(imperial_subset_TC.Count) + " tungsten carbide gauge{tc_singular}.");
                            break;
                        case 4:
                            str.Append(sentence_start + TextbasedNumeric(imperial_subset_steel.Count)
                            + " steel gauge{steel_singular}, " + TextbasedNumeric(imperial_subset_ceramic.Count) + " ceramic gauge{ceramic_singular} and "
                            + TextbasedNumeric(imperial_subset_TC.Count) + " tungsten carbide gauge{tc_singular}.");
                            break;
                    }
                }

            }
            else //we must have a mixture of metric and imperial gauge blocks, this will be very rare but we must allow for it
            {
                //case one - all gauge blocks in the set are made from the same material (the could be singular language needed)
                if ((metric_count + imperial_count) == (imperial_subset_steel.Count + metric_subset_steel.Count) ||
                    (metric_count + imperial_count) == (imperial_subset_ceramic.Count + metric_subset_ceramic.Count) ||
                    (metric_count + imperial_count) == (imperial_subset_TC.Count + metric_subset_TC.Count))
                {
                    double min_metric = 0;
                    double max_metric = 0;
                    if (metric_subset_steel.Count != 0)
                    {
                        min_metric = min_metric_steel_size;
                        max_metric = max_metric_steel_size;
                    }
                    else if (metric_subset_ceramic.Count != 0)
                    {
                        min_metric = min_metric_ceramic_size;
                        max_metric = max_metric_ceramic_size;
                    }
                    else if (metric_subset_TC.Count != 0)
                    {
                        min_metric = min_metric_tc_size;
                        max_metric = max_metric_tc_size;
                    }

                    double min_imperial = 0;
                    double max_imperial = 0;
                    if (imperial_subset_steel.Count != 0)
                    {
                        min_imperial = min_imperial_steel_size;
                        max_imperial = max_imperial_steel_size;
                    }
                    else if (imperial_subset_ceramic.Count != 0)
                    {
                        min_imperial = min_imperial_ceramic_size;
                        max_imperial = max_imperial_ceramic_size;
                    }
                    else if (imperial_subset_TC.Count != 0)
                    {
                        min_imperial = min_imperial_tc_size;
                        max_imperial = max_imperial_tc_size;
                    }
                    if (metric_count > 1 && imperial_count > 1)
                    { //both plural
                        str.Append("The gauge block set is manufactured by " + manufacturer_name
                            + " and contains a total of " + TextbasedNumeric(metric_count + imperial_count) + " "
                            + gauge_blocks[0].GaugeBlockMaterial.material + " gauge blocks. The total comprises of "
                            + TextbasedNumeric(metric_count) + " metric gauge blocks ranging in nominal size from "
                            + min_metric.ToString() + " mm to " + max_metric.ToString() + " mm and " + TextbasedNumeric(imperial_count)
                            + " imperial gauge blocks ranging in nominal size from " + min_imperial.ToString() + " inch to "
                            + max_imperial.ToString() + " inch.");
                    }
                    else if (metric_count > 1 && imperial_count == 1) //metric plural, imperial singular
                    {
                        str.Append("The gauge block set is manufactured by " + manufacturer_name
                            + " and contains a total of " + TextbasedNumeric(metric_count + imperial_count) + " "
                            + gauge_blocks[0].GaugeBlockMaterial.material + " gauge blocks. The total comprises of "
                            + TextbasedNumeric(metric_count) + " metric gauge block of nominal size "
                            + min_metric.ToString() + " mm and " + TextbasedNumeric(imperial_count)
                            + " imperial gauge blocks ranging in nominal size from " + min_imperial.ToString() + " inch to "
                            + max_imperial.ToString() + " inch.");
                    }
                    else if (metric_count == 1 && imperial_count > 1) //metric singular, imperial plural
                    {
                        str.Append("The gauge block set is manufactured by " + manufacturer_name
                            + " and contains a total of " + TextbasedNumeric(metric_count + imperial_count) + " "
                            + gauge_blocks[0].GaugeBlockMaterial.material + " gauge blocks. The total comprises of "
                            + TextbasedNumeric(metric_count) + " metric gauge blocks ranging in nominal size from "
                            + min_metric.ToString() + " mm to " + max_metric.ToString() + " mm and " + TextbasedNumeric(imperial_count)
                            + " imperial gauge block of nominal size " + min_imperial.ToString() + " inch.");
                    }
                    else //both singular
                    {
                        str.Append("The gauge block set is manufactured by " + manufacturer_name
                            + " and contains a total of " + TextbasedNumeric(metric_count + imperial_count) + " "
                            + gauge_blocks[0].GaugeBlockMaterial.material + " gauge blocks. The total comprises of "
                            + TextbasedNumeric(metric_count) + " metric gauge block of nominal size "
                            + min_metric.ToString() + " mm and " + TextbasedNumeric(imperial_count)
                            + " imperial gauge of nominal size " + min_imperial.ToString() + " inch.");
                    }
                }

                else
                {
                    //case 2 - gauges in the set are made from differing materials
                    List<string> unit_material = new List<string>();
                    List<int> counts = new List<int>();
                    foreach (GaugeBlock gb in gauge_blocks)
                    {
                        StringBuilder to_add = new StringBuilder();
                        if (gb.Metric) to_add.Append(" metric " + gb.GaugeBlockMaterial.material);
                        else to_add.Append(" imperial " + gb.GaugeBlockMaterial.material);



                        if (!unit_material.Contains(to_add.ToString()))
                        {
                            unit_material.Add(to_add.ToString()); //only add the string it's not already added
                            counts.Add(0);
                        }
                        counts[unit_material.IndexOf(to_add.ToString())]++;

                    }
                    string sentence_start = "The gauge block set is manufactured by " + manufacturer_name
                        + " and contains a total of " + TextbasedNumeric(metric_count + imperial_count)
                        + "gauge blocks. The total is comprised of ";

                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < unit_material.Count; i++)
                    {
                        if (counts[i] == 1) //language singular
                        {
                            if (i == (unit_material.Count - 2)) sb.Append(TextbasedNumeric(counts[i]) + unit_material[i] + " gauge block and ");
                            else if (i == (unit_material.Count - 1)) sb.Append(TextbasedNumeric(counts[i]) + unit_material[i] + " gauge block.");
                            else sb.Append(TextbasedNumeric(counts[i]) + unit_material[i] + " gauge block, ");
                        }
                        else
                        {
                            if (i == (unit_material.Count - 2)) sb.Append(TextbasedNumeric(counts[i]) + unit_material[i] + " gauge blocks and ");
                            else if (i == (unit_material.Count - 1)) sb.Append(TextbasedNumeric(counts[i]) + unit_material[i] + " gauge blocks.");
                            else sb.Append(TextbasedNumeric(counts[i]) + unit_material[i] + " gauge blocks, ");
                        }
                    }
                    str.Append(sentence_start + sb.ToString());
                }
                
            }
            
            description = str.ToString();
        }

        private string TextbasedNumeric(int num)
        {
            switch (num)
            {
                case 1:
                    return "one";
                case 2:
                    return "two"; ;
                case 3:
                    return "three";
                case 4:
                    return "four";
                case 5:
                    return "five";
                case 6:
                    return "six";
                case 7:
                    return "seven";
                case 8:
                    return "eight";
                case 9:
                    return "nine";
            }
            return num.ToString();
        }

        private void ManufacturerTextbox_TextChanged(object sender, EventArgs e)
        {
            manufacturer_name = ManufacturerTextbox.Text;
        }

        private void addImageButton_Click(object sender, EventArgs e)
        {
            GaugeImage gi = new GaugeImage();
            ImageFileDialog.InitialDirectory = @"G:\Shared drives\MSL - Length\Length\Jobs";
            if (ImageFileDialog.ShowDialog() == DialogResult.OK)
            {
                gi.Filename = ImageFileDialog.FileName.Substring(ImageFileDialog.FileName.LastIndexOf("\\")+1);
                gi.Directory = System.IO.Path.GetDirectoryName(ImageFileDialog.FileName);

                gi.Directory = gi.Directory.Replace('\\', '/');
                gi.Directory = gi.Directory + "/";
            }
            else return;

            gauge_images.Add(gi);

            switch (gauge_images.Count)
            {
                case 1:
                    gauge_images[gauge_images.Count - 1].Description = 
                        Interaction.InputBox("Change the sentence (if necessary) which goes in the report to describe the image", 
                        "Report Sentence", 
                        "Figure 1 shows the exterior of the box housing the gauge block set.", 10, 10);
                    if(gauge_images[gauge_images.Count - 1].Description.Contains("exterior")&& gauge_images[gauge_images.Count - 1].Description.Contains("box"))
                    {
                        gauge_images[gauge_images.Count - 1].Caption = "Figure 1: Box Exterior";
                    }
                    else gauge_images[gauge_images.Count - 1].Caption = "Figure 1";

                    break;
                case 2:
                    gauge_images[gauge_images.Count - 1].Description =
                        Interaction.InputBox("Change the sentence (if necessary) which goes in the report to describe the image",
                        "Report Sentence",
                        "Figure 2 shows the interior of the box housing the gauge block set.", 10, 10);
                    if (gauge_images[gauge_images.Count - 1].Description.Contains("interior") && gauge_images[gauge_images.Count - 1].Description.Contains("box"))
                    {
                        gauge_images[gauge_images.Count - 1].Caption = "Figure 2: Box Interior";
                    }
                    else gauge_images[gauge_images.Count - 1].Caption = "Figure 2";
                    break;
                default: //subsequent images will need manual editing
                    gauge_images[gauge_images.Count - 1].Description =
                        Interaction.InputBox("Change the sentence (if necessary) which goes in the report to describe the image",
                        "Report Sentence",
                        "Figure X shows ..........", 10, 10);
                    gauge_images[gauge_images.Count - 1].Caption = "Figure X";
                    break;
            }

            imageFileRichTextBox.Clear();

            foreach (GaugeImage g in gauge_images)
                imageFileRichTextBox.AppendText(g.Filename + Environment.NewLine);

            
            
        }
    }
}
