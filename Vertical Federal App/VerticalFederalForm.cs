using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
//using Microsoft.Office.Interop.Excel;



namespace Vertical_Federal_App
{
    public delegate void SerialDataReveived(string data);
    public delegate void TemperatureRetrieved(double temperature, DateTime datetime_retreived);
    public delegate void PrintGaugeResultsToRichTextbox(string data);

    public partial class VerticalFedForm : Form
    {
        private SerialDataReveived sdr;
        private TemperatureRetrieved tdr;
        private PrintGaugeResultsToRichTextbox pgr;
        private VerticalFederal federal;
        private Stack ref_g;
        private List<Report> reports = new List<Report>();
        private List<Stack> suitable_gauges;
        private int rb_position;
        private bool radiobuttionclearcalled;
    
        enum RadioButtonPos
        {
            R1, C1, A, B, C2, D, E, C3, R2
        }

        public VerticalFedForm()
        {
            InitializeComponent();
            sdr = new SerialDataReveived(DataReceived);
            tdr = new TemperatureRetrieved(TemperatureReceived);
            pgr = new PrintGaugeResultsToRichTextbox(PrintGaugeResults);
            
            Measurement.working_gauge = new GaugeBlock(true);
            INI2XML.DoIni2XmlConversion(ref messagesRichTextBox, @"G:\Shared drives\MSL - Length\Length\EQUIPREG\XML Files\cal_data_federal_measurement"+System.DateTime.Now.Ticks.ToString()+".xml", @"G:\Shared drives\MSL - Length\Length\EQUIPREG\Length_Stds_Calibration_Data\cal_data.ini", false);
            INI2XML.DoIni2XmlConversion(ref messagesRichTextBox, @"G:\Shared drives\MSL - Length\Length\Technical Procedures\XML Files\config_uncertainty_federal_measurement"+System.DateTime.Now.Ticks.ToString()+".xml", @"G:\Shared drives\MSL - Length\Length\Technical Procedures\Uncertainty Config\config_uncertainty.ini", true);
            if (INI2XML.Converted) INI2XML.PopulateReferenceGaugeComboBox(ref referenceSetComboBox, true);  //initially assume metric reference gauges (second argument true).
            Measurement.calibration_gauge_sets = new List<GaugeBlockSet>();  //make a new list for calibration gauge sets
            Measurement.reference_gauge_sets = new List<GaugeBlockSet>();
            radiobuttionclearcalled = false;
            Measurement.StartRetreivingTemperatures(ref tdr);
            Measurement.LogTemperatures = true;
            Measurement.working_gauge.Metric = true;
        }

        private void Comopenbutton_Click(object sender, EventArgs e)
        {
            federal = new VerticalFederal(ref sdr);
            INI2XML.LoadUncertaintyMetaData(ref federal);
            INI2XML.LoadVerticalFederalMetaData(ref federal);
            federal.COMInit(COMComboBox.Text.ToString());

            imperialCheckBox.Enabled = true;
            metricCheckBox.Enabled = true;
            setSerialTextBox.Enabled = true;
            gaugeSerialTextBox.Enabled = true;
            referenceSetComboBox.Enabled = true;
            calGaugeNominalTextBox.Enabled = true;
            gaugeParametersGroupBox.Enabled = true;
            suitableReferenceGaugesComboBox.Enabled = true;
            expNumericUpDown.Enabled = true;
            materialComboBox.Enabled = true;
            youngModulusTextBox.Enabled = true;
            poissonsRatioTextBox.Enabled = true;
            clientNameTextBox.Enabled = true;
            Comopenbutton.Enabled = false;
            ComplianceComboBox.Enabled = true;

        }


        public void DataReceived(string data)
        {
            if (this.InvokeRequired == false)
            {
                switch (rb_position)
                {
                    case (int)RadioButtonPos.R1:
                        rb_position = (int)RadioButtonPos.C1; //this is the next position we need to go to
                        R1TextBox.Text = data;
                        radiobuttionclearcalled = true;
                        R1RadioButton.Checked = false;
                        centreRadioButton.Checked = true;
                        radiobuttionclearcalled = false;
                        C1label.ForeColor = System.Drawing.Color.Green;
                        break;
                    case (int)RadioButtonPos.C1:
                        rb_position = (int)RadioButtonPos.A; //this is the next position we need to go to
                        C1TextBox.Text = data;
                        radiobuttionclearcalled = true;
                        centreRadioButton.Checked = false;
                        topRightRadioButton.Checked = true;
                        radiobuttionclearcalled = false;
                        C1label.ForeColor = System.Drawing.Color.Black;
                        break;
                    case (int)RadioButtonPos.A:
                        rb_position = (int)RadioButtonPos.B; //this is the next position we need to go to
                        ATextBox.Text = data;
                        radiobuttionclearcalled = true;
                        topRightRadioButton.Checked = false;
                        topLeftRadioButton.Checked = true;
                        radiobuttionclearcalled = false;
                        break;
                    case (int)RadioButtonPos.B:
                        rb_position = (int)RadioButtonPos.C2; //this is the next position we need to go to
                        BTextBox.Text = data;
                        radiobuttionclearcalled = true;
                        topLeftRadioButton.Checked = false;
                        centreRadioButton.Checked = true;
                        radiobuttionclearcalled = false;
                        C2label.ForeColor = System.Drawing.Color.Green;
                        break;
                    case (int)RadioButtonPos.C2:
                        rb_position = (int)RadioButtonPos.D; //this is the next position we need to go to
                        C2TextBox.Text = data;
                        radiobuttionclearcalled = true;
                        centreRadioButton.Checked = false;
                        bottomRightRadioButton.Checked = true;
                        radiobuttionclearcalled = false;
                        C2label.ForeColor = System.Drawing.Color.Black;
                        break;
                    case (int)RadioButtonPos.D:
                        rb_position = (int)RadioButtonPos.E; //this is the next position we need to go to
                        DTextBox.Text = data;
                        radiobuttionclearcalled = true;
                        bottomRightRadioButton.Checked = false;
                        bottomLeftRadioButton.Checked = true;
                        radiobuttionclearcalled = false;
                        break;
                    case (int)RadioButtonPos.E:
                        rb_position = (int)RadioButtonPos.C3; //this is the next position we need to go to
                        ETextBox.Text = data;
                        radiobuttionclearcalled = true;
                        bottomLeftRadioButton.Checked = false;
                        centreRadioButton.Checked = true;
                        radiobuttionclearcalled = false;
                        C3label.ForeColor = System.Drawing.Color.Green;
                        break;
                    case (int)RadioButtonPos.C3:
                        rb_position = (int)RadioButtonPos.R2; //this is the next position we need to go to
                        C3TextBox.Text = data;
                        radiobuttionclearcalled = true;
                        centreRadioButton.Checked = false;
                        R2RadioButton.Checked = true;
                        radiobuttionclearcalled = false;
                        C3label.ForeColor = System.Drawing.Color.Black;
                        break;
                    case (int)RadioButtonPos.R2:
                        rb_position = (int)RadioButtonPos.R1; //this is the next position we need to go to
                        R2TextBox.Text = data;
                        SaveGaugeData();
                        radiobuttionclearcalled = true;
                        R2RadioButton.Checked = false;
                        R1RadioButton.Checked = true;
                        radiobuttionclearcalled = false;
                        break;
                }

                //put data fetched from the federal into the appropriate box.


            }
            else
            {
                object[] textobj = { data };
                this.BeginInvoke(sdr, textobj);
            }
        }
        public void TemperatureReceived(double temperature, DateTime datetime)
        {
            if (this.InvokeRequired == false)
            {
                TemperatureTextBox.Text = Convert.ToString(temperature);
                DateTimeTextBox.Text = Convert.ToString(datetime);
            }
            else
            {
                object[] textobj = { temperature,datetime };
                this.BeginInvoke(tdr, textobj);
            }
        }

        public void PrintGaugeResults(string data)
        {
            if (this.InvokeRequired == false)
            {
               
                    gaugeResultsRichTextBox.AppendText(data);
                    gaugeResultsRichTextBox.SelectAll();
                    gaugeResultsRichTextBox.SelectionTabs = new int[] { 110, 210, 280, 350, 430 };
                    gaugeResultsRichTextBox.AcceptsTab = true;
                    gaugeResultsRichTextBox.Select(0, 0);
                    gaugeResultsRichTextBox.ScrollToCaret();
                
                
            }
            else
            {
                object[] textobj = { data };
                this.BeginInvoke(pgr, textobj);
            }
        }

        private void imperialCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //if this condition occurs the user has checked this checkbox when it was already checked
            if (!imperialCheckBox.Checked && !metricCheckBox.Checked)
            {
                Measurement.working_gauge.Metric = false;
                imperialCheckBox.Checked = true;
                referenceSetComboBox.Items.Clear();
                INI2XML.PopulateReferenceGaugeComboBox(ref referenceSetComboBox, false);
            }
            else if (imperialCheckBox.Checked && metricCheckBox.Checked)
            {
                Measurement.working_gauge.Metric = false;
                metricCheckBox.Checked = false;
                referenceSetComboBox.Items.Clear();
                INI2XML.PopulateReferenceGaugeComboBox(ref referenceSetComboBox, false);
            }
            ProcessSizeChangeEvent();
        }

        private void metricCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //if this condition occurs the user has checked this checkbox when it was already checked
            if (!metricCheckBox.Checked && !imperialCheckBox.Checked)
            {
                Measurement.working_gauge.Metric = true;
                metricCheckBox.Checked = true;
                referenceSetComboBox.Items.Clear();
                INI2XML.PopulateReferenceGaugeComboBox(ref referenceSetComboBox, true);
            }
            else if (metricCheckBox.Checked && imperialCheckBox.Checked)
            {
                Measurement.working_gauge.Metric = true;
                imperialCheckBox.Checked = false;
                referenceSetComboBox.Items.Clear();
                INI2XML.PopulateReferenceGaugeComboBox(ref referenceSetComboBox, true);
            }

            ComplianceComboBox.Items.Clear();
            //populate the compliance combo box.
            if (metricCheckBox.Checked)
            {
                ComplianceComboBox.Items.Add("BS EN ISO 3650:1999 Grade 00");
                ComplianceComboBox.Items.Add("BS EN ISO 3650:1999 Grade 0");
                ComplianceComboBox.Items.Add("BS EN ISO 3650:1999 Grade 1");
                ComplianceComboBox.Items.Add("BS EN ISO 3650:1999 Grade 2");
                ComplianceComboBox.Items.Add("JIS B 7506 : 2004 Grade K");
                ComplianceComboBox.Items.Add("JIS B 7506 : 2004 Grade 0");
                ComplianceComboBox.Items.Add("JIS B 7506 : 2004 Grade 1");
                ComplianceComboBox.Items.Add("JIS B 7506 : 2004 Grade 2");
                ComplianceComboBox.Items.Add("AS 1457 - 1999 Grade K");
                ComplianceComboBox.Items.Add("AS 1457 - 1999 Grade 0");
                ComplianceComboBox.Items.Add("AS 1457 - 1999 Grade 1");
                ComplianceComboBox.Items.Add("AS 1457 - 1999 Grade 2");
                ComplianceComboBox.Items.Add("ASME B89.1.9 - 2002 Grade K");
                ComplianceComboBox.Items.Add("ASME B89.1.9 - 2002 Grade 00");
                ComplianceComboBox.Items.Add("ASME B89.1.9 - 2002 Grade 0");
                ComplianceComboBox.Items.Add("ASME B89.1.9 - 2002 Grade AS-1");
                ComplianceComboBox.Items.Add("ASME B89.1.9 - 2002 Grade AS-2");
            }
            else
            {
                ComplianceComboBox.Items.Add("BS4311:2007 Grade K");
                ComplianceComboBox.Items.Add("BS4311:2007 Grade 0");
                ComplianceComboBox.Items.Add("BS4311:2007 Grade 1");
                ComplianceComboBox.Items.Add("BS4311:2007 Grade 2");
            }
            ProcessSizeChangeEvent();
        }

        private void setSerialTextBox_TextChanged(object sender, EventArgs e)
        {
            Measurement.working_gauge.FromSet = setSerialTextBox.Text;
            

        }

        private void gaugeSerialTextBox_TextChanged(object sender, EventArgs e)
        {
            Measurement.working_gauge.SerialNumber = gaugeSerialTextBox.Text.ToString();
        }

        private void referenceSetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            //check if we have any reference gauge sets in the list, if we don't then add a new set
            if (Measurement.reference_gauge_sets.Count == 0)
            {
                GaugeBlockSet gauge_set = new GaugeBlockSet();
                gauge_set.GaugeSetName = referenceSetComboBox.SelectedItem.ToString();
                Measurement.reference_gauge_sets.Add(gauge_set);

                //Get the reference set data from the xml file
                INI2XML.GetReferenceSetMetaData(ref gauge_set);
                //Add all gauges found in the xml file to the reference gauge set.
                INI2XML.LoadReferenceGauges(ref gauge_set);

                CheckBBFDate(gauge_set.ReportDate);
                
                foreach (GaugeBlockSet gbs in Measurement.reference_gauge_sets)
                {
                    gbs.PrintGaugeList(ref messagesRichTextBox);
                }
            }
            else
            {
                bool set_exists = false;

                //if we have reference gauge sets already in the list then see if the set has previously been added (i.e is this a unique serial number)
                foreach (GaugeBlockSet ref_set in Measurement.reference_gauge_sets)
                {
                    if (ref_set.GaugeSetName.Equals(referenceSetComboBox.SelectedItem.ToString()))
                    {
                        set_exists = true;
                    }
                }

                if (!set_exists)
                {
                    GaugeBlockSet gauge_set = new GaugeBlockSet();
                    gauge_set.GaugeSetName = referenceSetComboBox.SelectedItem.ToString();
                    Measurement.reference_gauge_sets.Add(gauge_set);

                    //Get the reference set data from the xml file
                    INI2XML.GetReferenceSetMetaData(ref gauge_set);
                    //Add all gauges found in the xml file to the reference gauge set.
                    INI2XML.LoadReferenceGauges(ref gauge_set);
                    CheckBBFDate(gauge_set.ReportDate);

                    foreach (GaugeBlockSet gbs in Measurement.reference_gauge_sets)
                    {
                        gbs.PrintGaugeList(ref messagesRichTextBox);
                    }
                }
            }

        }

        private void CheckBBFDate(string report_date)
        {
            DateTime r_date = Convert.ToDateTime(report_date);
            DateTime now_date = DateTime.Now;
            long ticks = now_date.Ticks - r_date.Ticks ;
            long ticks_five_y = (long)(TimeSpan.TicksPerDay * 365.24219 * 5);
            if (ticks > ticks_five_y)
            {
                MessageBox.Show("Warning - It has been over 5 years since this gauge block set has been calibrated!");
            }

        } 
        private void calGaugeSizeTextBox_TextChanged(object sender, EventArgs e)
        {
            ProcessSizeChangeEvent();
            
        }

        private void ProcessSizeChangeEvent()
        {
            DisableAllMeasurementRadioButton();
            DisableAllMeasurementTextBoxes();

            //we just changed the size so we need to calculate new reference gauges.  We must clear away any gauges stored in the combobox.
            suitableReferenceGaugesComboBox.SelectedItem = "";
            suitableReferenceGaugesComboBox.Items.Clear();

            //has the user selected a reference set?
            if (Measurement.reference_gauge_sets.Count == 0)
            {
                MessageBox.Show("Please Select a reference set");
                return;
            }

            try
            {
                Measurement.working_gauge.Nominal = System.Convert.ToDouble(calGaugeNominalTextBox.Text);
            }
            catch (FormatException)
            {
                //MessageBox.Show("Invalid gauge size entered");
                return;
            }

            if (Measurement.working_gauge.Metric && (Measurement.working_gauge.Nominal < 0.5 || Measurement.working_gauge.Nominal > 102))  //metric units
            {
                MessageBox.Show("Gauge block size is not in the measuring range of the comparator");
                Measurement.working_gauge.IllegalSize = true;
                return;
            }
            else if (!Measurement.working_gauge.Metric && (Measurement.working_gauge.Nominal < 0.01 || Measurement.working_gauge.Nominal > 4))   //imperial units
            {
                MessageBox.Show("Gauge block size is not in the measuring range of the comparator");
                Measurement.working_gauge.IllegalSize = true;
                return;
            }
            else
            {
                Measurement.working_gauge.IllegalSize = false;
                
            }

            //construct a list of all available reference gauges to use
            //make all gauge block units in millimetres to avoid confusion
            List<GaugeBlock> all_ref_gauges = new List<GaugeBlock>();
            all_ref_gauges.Clear();



            foreach (GaugeBlockSet g in Measurement.reference_gauge_sets)
            {
                foreach (GaugeBlock gb in g.GaugeList)
                {
                    if (!gb.Metric) gb.Nominal = Math.Round(gb.Nominal * 25.4, 5);
                    gb.GaugeBlockMaterial = g.GaugeSetMaterial;
                    all_ref_gauges.Add(gb);

                }
            }

            //temporarily convert the working gauge to metric too.  The algorithm below is far more complicated in mixed units
            if (!Measurement.working_gauge.Metric) Measurement.working_gauge.Nominal = Math.Round(Measurement.working_gauge.Nominal * 25.4, 5);


            //create a list for possible reference stacks.
            suitable_gauges = new List<Stack>();
            int gauge_count1 = 0;

            bool message_shown = false;
            

            foreach (GaugeBlock gb1 in all_ref_gauges)
            {
                //case for a singleton gauge reference
                if (Measurement.working_gauge.Nominal == gb1.Nominal)
                {
                    Stack gauge_stack = new Stack(1);


                    if (!Measurement.working_gauge.Metric) Measurement.working_gauge.Nominal = Math.Round(Measurement.working_gauge.Nominal / 25.4, 5); //convert the working gauge nominal back to imperial    
                    if (!gb1.Metric) gb1.Nominal = Math.Round(gb1.Nominal / 25.4, 5); //convert back to imperial units

                    gauge_stack.Gauge1 = gb1;


                    //if we have found a singleton gauge then it should be the gauge we use
                    suitable_gauges.Clear();
                    suitable_gauges.Add(gauge_stack);


                    suitableReferenceGaugesComboBox.Items.Clear();
                    if (gb1.Metric) suitableReferenceGaugesComboBox.Items.Add(gb1.Nominal.ToString() + " mm");
                    else suitableReferenceGaugesComboBox.Items.Add(gb1.Nominal.ToString() + " inch");
                    suitableReferenceGaugesComboBox.SelectedIndex = 0;

                    foreach (GaugeBlockSet g in Measurement.reference_gauge_sets)
                    {
                        foreach (GaugeBlock gb in g.GaugeList)
                        {
                            //remember we already converted the working gauge back to imperial so don't do it again here
                            if (!(Measurement.working_gauge.Nominal == gb.Nominal))
                            {
                                if (!gb.Metric) gb.Nominal = Math.Round(gb.Nominal / 25.4, 5);
                            }
                        }
                    }
                    return;

                }
                int gauge_count2 = 0;
                //case for two gauges
                foreach (GaugeBlock gb2 in all_ref_gauges)
                {
                    

                    //case for a two gauge stacked reference (don't compare the same gauge with itself)
                    if ((Measurement.working_gauge.Nominal == Math.Round((gb1.Nominal + gb2.Nominal), 5)) && (gauge_count1 != gauge_count2))
                    {
                        

                        Stack gauge_stack = new Stack(2);
                        gauge_stack.Gauge1 = gb1;
                        gauge_stack.Gauge2 = gb2;
                        if (!gb1.Metric) gauge_stack.Gauge1.Nominal = Math.Round(gb1.Nominal / 25.4, 5); //convert the reference gauge block nominal back to imperial units. 
                        if (!gb2.Metric) gauge_stack.Gauge2.Nominal = Math.Round(gb2.Nominal / 25.4, 5); //convert the reference gauge block nominal back to imperial units

                        suitable_gauges.Add(gauge_stack);


                        if (gb1.Metric && gb2.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " mm__" + gauge_stack.Gauge1.GaugeBlockMaterial.material + ",  " + gauge_stack.Gauge2.Nominal.ToString() + " mm__" + gauge_stack.Gauge2.GaugeBlockMaterial.material);
                        else if (gb1.Metric && !gb2.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " mm__" + gauge_stack.Gauge1.GaugeBlockMaterial.material + ",  " + gauge_stack.Gauge2.Nominal.ToString() + " inch__" + gauge_stack.Gauge2.GaugeBlockMaterial.material);
                        else if (!gb1.Metric && gb2.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " inch__" + gauge_stack.Gauge1.GaugeBlockMaterial.material + ",  " + gauge_stack.Gauge2.Nominal.ToString() + " mm__" + gauge_stack.Gauge2.GaugeBlockMaterial.material);
                        else suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " inch__" + gauge_stack.Gauge1.GaugeBlockMaterial.material + ",  " + gauge_stack.Gauge2.Nominal.ToString() + " inch__" + gauge_stack.Gauge2.GaugeBlockMaterial.material);

                        if (!gb1.Metric) gb1.Nominal = Math.Round(gb1.Nominal * 25.4, 5); //convert the reference gauge block nominal back to metric units. 
                        if (!gb2.Metric) gb2.Nominal = Math.Round(gb2.Nominal * 25.4, 5); //convert the reference gauge block nominal back to metric units

                    }
                    int gauge_count3 = 0;
                    foreach (GaugeBlock gb3 in all_ref_gauges)
                    {
                        //case for a three gauge stacked reference (don't compare the same gauge with itself)
                        if ((Measurement.working_gauge.Nominal == Math.Round((gb1.Nominal + gb2.Nominal + gb3.Nominal), 5)) && (gauge_count1 != gauge_count2) && (gauge_count2 != gauge_count3) && (gauge_count1 != gauge_count3))
                        {
                            Stack gauge_stack = new Stack(3);
                            gauge_stack.Gauge1 = gb1;
                            gauge_stack.Gauge2 = gb2;
                            gauge_stack.Gauge3 = gb3;

                            if (!gb1.Metric) gauge_stack.Gauge1.Nominal = Math.Round(gb1.Nominal / 25.4, 5); //convert the reference gauge block nominal back to imperial units. 
                            if (!gb2.Metric) gauge_stack.Gauge2.Nominal = Math.Round(gb2.Nominal / 25.4, 5); //convert the reference gauge block nominal back to imperial units
                            if (!gb3.Metric) gauge_stack.Gauge3.Nominal = Math.Round(gb3.Nominal / 25.4, 5); //convert the reference gauge block nominal back to imperial units



                            suitable_gauges.Add(gauge_stack);

                            if (gb1.Metric && gb2.Metric && gb3.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " mm__" + gauge_stack.Gauge1.GaugeBlockMaterial.material + ",  " + gauge_stack.Gauge2.Nominal.ToString() + " mm__" + gauge_stack.Gauge2.GaugeBlockMaterial.material + ", " + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " mm__"+gauge_stack.Gauge3.GaugeBlockMaterial.material);
                            else if (gb1.Metric && gb2.Metric && !gb3.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " mm__" + gauge_stack.Gauge1.GaugeBlockMaterial.material + ",  " + gauge_stack.Gauge2.Nominal.ToString() + " mm__" + gauge_stack.Gauge2.GaugeBlockMaterial.material + ", " + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " inch__" + gauge_stack.Gauge3.GaugeBlockMaterial.material);
                            else if (gb1.Metric && !gb2.Metric && gb3.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " mm__" + gauge_stack.Gauge1.GaugeBlockMaterial.material + ",  " + gauge_stack.Gauge2.Nominal.ToString() + " inch__" + gauge_stack.Gauge2.GaugeBlockMaterial.material + ", " + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " mm__" + gauge_stack.Gauge3.GaugeBlockMaterial.material);
                            else if (gb1.Metric && !gb2.Metric && !gb3.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " mm__" + gauge_stack.Gauge1.GaugeBlockMaterial.material + ",  " + gauge_stack.Gauge2.Nominal.ToString() + " inch__" + gauge_stack.Gauge2.GaugeBlockMaterial.material + ", " + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " inch__" + gauge_stack.Gauge3.GaugeBlockMaterial.material);
                            else if (!gb1.Metric && gb2.Metric && gb3.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " inch__" + gauge_stack.Gauge1.GaugeBlockMaterial.material + ",  " + gauge_stack.Gauge2.Nominal.ToString() + " mm__" + gauge_stack.Gauge2.GaugeBlockMaterial.material + ", " + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " mm__" + gauge_stack.Gauge3.GaugeBlockMaterial.material);
                            else if (!gb1.Metric && gb2.Metric && !gb3.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " inch__" + gauge_stack.Gauge1.GaugeBlockMaterial.material + ",  " + gauge_stack.Gauge2.Nominal.ToString() + " mm__" + gauge_stack.Gauge2.GaugeBlockMaterial.material + ", " + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " inch__" + gauge_stack.Gauge3.GaugeBlockMaterial.material);
                            else if (!gb1.Metric && !gb2.Metric && gb3.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " inch__" + gauge_stack.Gauge1.GaugeBlockMaterial.material + ",  " + gauge_stack.Gauge2.Nominal.ToString() + " inch__" + gauge_stack.Gauge2.GaugeBlockMaterial.material + ", " + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " mm__" + gauge_stack.Gauge3.GaugeBlockMaterial.material);
                            else suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " inch__" + gauge_stack.Gauge1.GaugeBlockMaterial.material + ",  " + gauge_stack.Gauge2.Nominal.ToString() + " inch__" + gauge_stack.Gauge2.GaugeBlockMaterial.material + ", " + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " inch__" + gauge_stack.Gauge3.GaugeBlockMaterial.material);

                            if (!gb1.Metric) gb1.Nominal = Math.Round(gb1.Nominal * 25.4, 5); //convert the reference gauge block nominal back to metric units. 
                            if (!gb2.Metric) gb2.Nominal = Math.Round(gb2.Nominal * 25.4, 5); //convert the reference gauge block nominal back to metric units
                            if (!gb3.Metric) gb3.Nominal = Math.Round(gb3.Nominal * 25.4, 5); //convert the reference gauge block nominal back to metric units
                        }

                        gauge_count3++;
                    }
                    gauge_count2++;
                }
                gauge_count1++;
            }
            if (!message_shown)
            {
                MessageBox.Show("In the case where the reference gauge is formed from wrung " +
                  "gauges made from difference materials the gauge that appears first in " +
                  "the \"Suitable Gauges\" dropdown menu should be placed at the top of the stack. " +
                  "This is important as the vertical federal has different probing forces for the top " +
                  "and bottom anvil");
                suitableReferenceGaugesComboBox.Text = "";
                referenceDeviationTextBox.Text = "";
            }
            message_shown = true;
            if (!Measurement.working_gauge.Metric) Measurement.working_gauge.Nominal = Math.Round(Measurement.working_gauge.Nominal / 25.4, 5);

            foreach (GaugeBlockSet g in Measurement.reference_gauge_sets)
            {
                foreach (GaugeBlock gb in g.GaugeList)
                {
                    if (!gb.Metric) Math.Round(gb.Nominal = gb.Nominal / 25.4, 5);

                }
            }

        }

        private void suitableReferenceGaugesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = suitableReferenceGaugesComboBox.SelectedIndex;
            ref_g = suitable_gauges.ElementAt(index);

            double refdev = GaugeBlock.calculateGaugeStackDeviation(ref_g, metricCheckBox.Checked);


            //update the deviation field of the measurement group box
            try
            {
                referenceDeviationTextBox.Text = refdev.ToString();
                EnableAllMeasurementRadioButtions();
                ClearAllMeasurementRadioButtons();
                rb_position = (int)RadioButtonPos.R1;
                R1RadioButton.Checked = true;
                radiobuttionclearcalled = false;
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid deviation calculated");
            }
        }

        private void youngModulusTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Measurement.working_gauge.GaugeBlockMaterial.youngs_modulus = System.Convert.ToDouble(youngModulusTextBox.Text);
            }
            catch (FormatException) { }

        }

        private void poissonsRatioTextBox_TextChanged(object sender, EventArgs e)
        {

            try
            {
                Measurement.working_gauge.GaugeBlockMaterial.poissons_ratio = System.Convert.ToDouble(poissonsRatioTextBox.Text);
            }
            catch (FormatException) { }
        }
        private void clientNameTextBox_TextChanged(object sender, EventArgs e)
        {
            Measurement.working_gauge.ClientName = clientNameTextBox.Text;
        }

        private void materialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Material mtrl = new Material();


            switch (materialComboBox.SelectedItem.ToString())
            {
                case "ceramic":
                    mtrl.exp_coeff = 9.2;  //NIST Gauge Block Handbook
                    mtrl.poissons_ratio = 0.23; //NIST EMToolBox
                    mtrl.youngs_modulus = 200; //NIST EMToolBox
                    mtrl.material = "ceramic";
                    break;
                case "steel":
                    mtrl.exp_coeff = 11.5; //NIST Gauge Block Handbook
                    mtrl.poissons_ratio = 0.296; //0.75% C hardened - Kaye and Laby "Tables of Physical and Chemical Constants", 16th eddition
                    mtrl.youngs_modulus = 201.4; //0.75% C hardened - Kaye and Laby "Tables of Physical and Chemical Constants", 16th eddition
                    mtrl.material = "steel";
                    break;
                case "tungsten carbide":
                    mtrl.exp_coeff = 4.5; //NIST Gauge Block Handbook
                    mtrl.poissons_ratio = 0.2; //NIST EMToolBox 10% Cobalt  - this agrees well with the opus website
                    mtrl.youngs_modulus = 599.84; //NIST EMToolBox 10% Cobalt - this agrees well with the opus website
                    mtrl.material = "tungsten carbide";
                    break;
                case "fused silica":
                    mtrl.exp_coeff = 0.49; //NIST Gauge Block Handbook
                    mtrl.poissons_ratio = 0.17; //pg 45 elasticities of glass - Kaye and Laby "Tables of Physical and Chemical Constants", 16th eddition
                    mtrl.youngs_modulus = 73.1; //pg 45 elasticities of glass - Kaye and Laby "Tables of Physical and Chemical Constants", 16th eddition
                    mtrl.material = "fused silica";
                    break;
            }

            poissonsRatioTextBox.Text = mtrl.poissons_ratio.ToString();
            youngModulusTextBox.Text = mtrl.youngs_modulus.ToString();
            expNumericUpDown.Value = System.Convert.ToDecimal(mtrl.exp_coeff);
            Measurement.working_gauge.GaugeBlockMaterial = mtrl;
            
            
        }

        private void expNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Measurement.working_gauge.GaugeBlockMaterial.exp_coeff = System.Convert.ToDouble(expNumericUpDown.Value);
        }

        private void R1TextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void C1TextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ATextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void BTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void C2TextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void DTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ETextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void C3TextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void R2TextBox_TextChanged(object sender, EventArgs e)
        {

        }
        private void R1RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!radiobuttionclearcalled)
            {
                ClearAllMeasurementRadioButtons();
                R1RadioButton.Checked = true;
                rb_position = (int)RadioButtonPos.R1;
                radiobuttionclearcalled = false;
            }
        }
        private void topLeftRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!radiobuttionclearcalled)
            {
                ClearAllMeasurementRadioButtons();
                topLeftRadioButton.Checked = true;
                rb_position = (int)RadioButtonPos.B;
                radiobuttionclearcalled = false;
            }
        }

        private void topRightRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!radiobuttionclearcalled)
            {
                ClearAllMeasurementRadioButtons();
                topRightRadioButton.Checked = true;
                rb_position = (int)RadioButtonPos.A;
                radiobuttionclearcalled = false;
            }
        }

        private void centreRadioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void bottomLeftRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!radiobuttionclearcalled)
            {
                ClearAllMeasurementRadioButtons();
                bottomLeftRadioButton.Checked = true;
                rb_position = (int)RadioButtonPos.E;
                radiobuttionclearcalled = false;
            }
        }

        private void bottomRightRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!radiobuttionclearcalled)
            {
                ClearAllMeasurementRadioButtons();
                bottomRightRadioButton.Checked = true;
                rb_position = (int)RadioButtonPos.D;
                radiobuttionclearcalled = false;
            }
        }

        private void R2RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!radiobuttionclearcalled)
            {
                ClearAllMeasurementRadioButtons();
                R2RadioButton.Checked = true;
                rb_position = (int)RadioButtonPos.R2;
                radiobuttionclearcalled = false;
            }
        }

        private void C1label_Click(object sender, EventArgs e)
        {
            if (!radiobuttionclearcalled)
            {
                ClearAllMeasurementRadioButtons();
                centreRadioButton.Checked = true;
                C1label.ForeColor = System.Drawing.Color.Green;
                rb_position = (int)RadioButtonPos.C1;
                radiobuttionclearcalled = false;
            }
        }

        private void C2label_Click(object sender, EventArgs e)
        {
            if (!radiobuttionclearcalled)
            {
                ClearAllMeasurementRadioButtons();
                centreRadioButton.Checked = true;
                C2label.ForeColor = System.Drawing.Color.Green;
                rb_position = (int)RadioButtonPos.C2;
                radiobuttionclearcalled = false;
            }
        }

        private void C3label_Click(object sender, EventArgs e)
        {
            if (!radiobuttionclearcalled)
            {
                ClearAllMeasurementRadioButtons();
                centreRadioButton.Checked = true;
                C3label.ForeColor = System.Drawing.Color.Green;
                rb_position = (int)RadioButtonPos.C3;
                radiobuttionclearcalled = false;
            }
        }


        private void ClearAllMeasurementRadioButtons()
        {
            radiobuttionclearcalled = true;
            topLeftRadioButton.Checked = false;
            topRightRadioButton.Checked = false;
            bottomRightRadioButton.Checked = false;
            bottomLeftRadioButton.Checked = false;
            centreRadioButton.Checked = false;
            R1RadioButton.Checked = false;
            R2RadioButton.Checked = false;
            C1label.ForeColor = System.Drawing.Color.Black;
            C2label.ForeColor = System.Drawing.Color.Black;
            C3label.ForeColor = System.Drawing.Color.Black;

        }
        private void EnableAllMeasurementRadioButtions()
        {
            topLeftRadioButton.Enabled = true;
            topRightRadioButton.Enabled = true;
            bottomRightRadioButton.Enabled = true;
            bottomLeftRadioButton.Enabled = true;
            //centreRadioButton.Enabled = true;
            R1RadioButton.Enabled = true;
            R2RadioButton.Enabled = true;
            C1label.Enabled = true;
            C2label.Enabled = true;
            C3label.Enabled = true;
        }
        private void DisableAllMeasurementRadioButton()
        {
            topLeftRadioButton.Enabled = false;
            topRightRadioButton.Enabled = false;
            bottomRightRadioButton.Enabled = false;
            bottomLeftRadioButton.Enabled = false;
            centreRadioButton.Enabled = false;
            R1RadioButton.Enabled = false;
            R2RadioButton.Enabled = false;
        }
        private void EnableAllMeasurementTextBoxes()
        {
            R1TextBox.Enabled = true;
            R2TextBox.Enabled = true;
            C1TextBox.Enabled = true;
            C2TextBox.Enabled = true;
            C3TextBox.Enabled = true;
            ATextBox.Enabled = true;
            BTextBox.Enabled = true;
            DTextBox.Enabled = true;
            ETextBox.Enabled = true;
        }
        private void DisableAllMeasurementTextBoxes()
        {
            R1TextBox.Enabled = false;
            R2TextBox.Enabled = false;
            C1TextBox.Enabled = false;
            C2TextBox.Enabled = false;
            C3TextBox.Enabled = false;
            ATextBox.Enabled = false;
            BTextBox.Enabled = false;
            DTextBox.Enabled = false;
            ETextBox.Enabled = false;
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void SaveGaugeData()
        {
            if (Measurement.working_gauge.IllegalSize)
            {
                MessageBox.Show("Illegal gauge size!  Change size before continuing");
                return;
            }

            if (clientNameTextBox.Text.Equals(""))
            {
                MessageBox.Show("The filenames are determined from the Client Name field, but nothing has been entered.  " +
                    "Please enter a client name before continuing");
                return;
            }
            else
            {
                Measurement.working_gauge.ClientName = clientNameTextBox.Text;
            }
            int set_index = 0;
            if (!Measurement.CreateNewCalSet(ref set_index,Measurement.working_gauge.FromSet)) return;

            //add the gauge to the calibration gauge set
            GaugeBlock working_gauge_clone = Measurement.working_gauge.Clone();
            Measurement.calibration_gauge_sets[set_index].GaugeList.Add(working_gauge_clone);
            Measurement.calibration_gauge_sets[set_index].NumGauges++;
            
            
            //first see if all measurement text field have been populated.
            double A = 0.0;
            double B = 0.0;
            double C1 = 0.0;
            double C2 = 0.0;
            double C3 = 0.0;
            double D = 0.0;
            double E = 0.0;
            double R1 = 0.0;
            double R2 = 0.0;

            if ((R1TextBox.Text == "") || (!double.TryParse(R1TextBox.Text, out R1)))
            {
                MessageBox.Show("No data or invalid data for R1");
                return;
            }
            if ((R2TextBox.Text == "") || (!double.TryParse(R2TextBox.Text, out R2)))
            {
                MessageBox.Show("No data or invalid data for R2");
                return;
            }
            if ((ATextBox.Text == "") || (!double.TryParse(ATextBox.Text, out A)))
            {
                MessageBox.Show("No data or invalid data for A");
                return;
            }
            if ((BTextBox.Text == "") || (!double.TryParse(BTextBox.Text, out B)))
            {
                MessageBox.Show("No data or invalid data for B");
                return;
            }
            if ((C1TextBox.Text == "") || (!double.TryParse(C1TextBox.Text, out C1)))
            {
                MessageBox.Show("No data or invalid data for C1");
                return;
            }
            if ((C2TextBox.Text == "") || (!double.TryParse(C2TextBox.Text, out C2)))
            {
                MessageBox.Show("No data or invalid data for C2");
                return;
            }
            if ((C3TextBox.Text == "") || (!double.TryParse(C3TextBox.Text, out C3)))
            {
                MessageBox.Show("No data or invalid data for C3");
                return;
            }
            if ((DTextBox.Text == "") || (!double.TryParse(DTextBox.Text, out D)))
            {
                MessageBox.Show("No data or invalid data for D");
                return;
            }
            if ((ETextBox.Text == "") || (!double.TryParse(ETextBox.Text, out E)))
            {
                MessageBox.Show("No data or invalid data for E");
                return;
            }
            double r_dev = 0.0;
            double tv = -273.14;

            Measurement current_measurement = new Measurement();
            current_measurement.CalibrationGauge = working_gauge_clone;
            current_measurement.Datetime = DateTimeTextBox.Text;
            double.TryParse(TemperatureTextBox.Text, out tv);
            current_measurement.CalibrationGauge.Temperature = tv;
            current_measurement.A = A;
            current_measurement.B = B;
            current_measurement.C1 = C1;
            current_measurement.C2 = C2;
            current_measurement.C3 = C3;
            current_measurement.D = D;
            current_measurement.E = E;
            current_measurement.R1 = R1;
            current_measurement.R2 = R2;
            double.TryParse(referenceDeviationTextBox.Text, out r_dev);
            current_measurement.RefDeviation_um_uinch = r_dev;
            Stack ref_s = ref_g.Clone();
            current_measurement.ReferenceStack = ref_s;
            current_measurement.CalculateVariation();
            current_measurement.CalculateMeasuredDiff_um_uinch();
            current_measurement.RefLength();
            current_measurement.calculateElasticDeformations(federal);
            current_measurement.CalculateDeviations();
            current_measurement.CalibrationGauge.FromSet = setSerialTextBox.Text;
            current_measurement.CalibrationGauge.Variation = Math.Round(current_measurement.CalculateVariation(), 5);
            current_measurement.CalculateComplianceLimits();
            current_measurement.CalculateCMCs(ref federal);
            current_measurement.measurement_working_filename = @"G:\Shared drives\MSL - Length\Length\Federal\FederalData\" + current_measurement.CalibrationGauge.FromSet + ".txt";
            current_measurement.measurement_working_filename_sum = @"G:\Shared drives\MSL - Length\Length\Federal\FederalData\"+ current_measurement.CalibrationGauge.FromSet + "_summary.txt";
            current_measurement.measurement_working_filename_U95_sum = @"G:\Shared drives\MSL - Length\Length\Federal\FederalData\" + current_measurement.CalibrationGauge.FromSet + "_U95_Compliance_summary.txt";

            //if needed add the set name to the set name list
            if (!Measurement.SetNames.Contains(current_measurement.CalibrationGauge.FromSet)) //this measurement doesn't have an association with a set yet, so make it now
            {
                List<Measurement> m_list = new List<Measurement>();
                Measurement.SetNames.Add(current_measurement.CalibrationGauge.FromSet);
                Measurement.Measurements.Add(m_list);  //add an empty measurement list to the list of measurement lists.
                
            }
           
            //get the index of the current measurements set name.
            int index_of_set = Measurement.SetNames.IndexOf(current_measurement.CalibrationGauge.FromSet);

            //Save a copy of the current measurement to the measurement list.  It's a 2 dimensional list 
            //the 1st dimension enables sorting measurements by the set they belong to.
            Measurement.Measurements[index_of_set].Add(current_measurement);
            
            gaugeResultsRichTextBox.Clear();
            Thread saveprintThread = new Thread(new ThreadStart(DoSavePrint));
            saveprintThread.Start();
            
        }
        /// <summary>
        /// Saves all measurement data to file and prints measurement to the GUI
        /// </summary>
        private void DoSavePrint()
        {
            foreach (List<Measurement> m_list in Measurement.Measurements)
            {
                

                if (m_list == null) continue; //make sure the list has something in it

                System.IO.File.Delete(m_list[0].measurement_working_filename);

                //Note: every measurement in m_list is from the same gauge block set by implementation
                System.IO.StreamWriter writer = System.IO.File.CreateText(m_list[0].measurement_working_filename);

                Measurement.HeaderWritten = false;
                if (!Measurement.HeaderWritten)
                {
                    writer.WriteLine(Measurement.measurement_file_header);
                    writer.WriteLine(Measurement.Version_number);
                }

                StringBuilder rtb = new StringBuilder();
                int count = 1;



                foreach (Measurement m in m_list)
                {
                    string line_to_write = "";
                    string units = "mm µm";
                    if (!m.CalibrationGauge.Metric) units = "inch µinch";
                    m.calculateElasticDeformations(federal);
                    m.CalculateDeviations();
                    Measurement.PrepareLineForWrite(m, ref line_to_write, units);
                    string l = Measurement.writeRichTBLine(m, units, count);
                    rtb.Append(l);
                    writer.WriteLine(line_to_write);
                    count++;
                }
                writer.Close();
                //invoke the gui to print to the gauge results rich text box
                pgr(rtb.ToString());
            }
            

            

            Measurement.writeSummaryToFile();
            Measurement.WriteUncertaintyAndComplianceToFile(ref federal);
        }
        private void DeleteLastButton_Click(object sender, EventArgs e)
        {
            if (Measurement.Measurements.Count > 0)
            {
                Measurement.Measurements.RemoveAt(Measurement.Measurements.Count - 1);
                int index_of_last_line_feed = gaugeResultsRichTextBox.Text.LastIndexOf('\n');
                gaugeResultsRichTextBox.Text = gaugeResultsRichTextBox.Text.Remove(index_of_last_line_feed);
                index_of_last_line_feed = gaugeResultsRichTextBox.Text.LastIndexOf('\n');

                if (index_of_last_line_feed != 0)
                {
                    gaugeResultsRichTextBox.Text = gaugeResultsRichTextBox.Text.Remove(index_of_last_line_feed + 1);
                    gaugeResultsRichTextBox.SelectAll();
                    gaugeResultsRichTextBox.SelectionTabs = new int[] { 110, 210, 280, 350, 430 };
                    gaugeResultsRichTextBox.AcceptsTab = true;
                    gaugeResultsRichTextBox.Select(0, 0);
                }
                else gaugeResultsRichTextBox.Text.Append('\n');
            }
        }

        //load can only be performed on an active measurement filename
        private void LoadMeasurementButton_Click(object sender, EventArgs e)
        {
           
            string[] lines;
            if (MeasurementOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                //check if this files data already exists i.e does the program already have data loaded relating to the file
                
                Measurement.filen = MeasurementOpenFileDialog.FileName;

                foreach (List<Measurement> m_list in Measurement.Measurements)
                {
                    if (m_list[0].measurement_working_filename.Contains(Measurement.filen))
                    {
                        MessageBox.Show("This file has already been loaded");
                        return; //we've already loaded this file
                    }
                }

                lines = System.IO.File.ReadAllLines(Measurement.filen);
            }
            else return;

            if (lines.Length == 0)
            {
                MessageBox.Show("The file is empty");
                return;
            }
            if (!lines[0].Contains(Measurement.measurement_file_header)) return;

            gaugeResultsRichTextBox.Clear();

            Thread loaderthread = new Thread(new ParameterizedThreadStart(doLoadWork));
            loaderthread.Start(lines);

           
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="string_array"></param>
        private void doLoadWork(object string_array)
        {
           

            string[] lines = (string[])string_array;
            int meas_list_index = 0;

            System.IO.StreamWriter writer = System.IO.File.CreateText(Measurement.filen);

            //read the file and add measurements to the measurement list
            Measurement.ParseFile(lines, ref federal, ref meas_list_index);

            Measurement.HeaderWritten = false;
            if (!Measurement.HeaderWritten)
            {
                writer.WriteLine(Measurement.measurement_file_header);
                writer.WriteLine(Measurement.Version_number);
            }
            int count = 1;
            StringBuilder sb = new StringBuilder();


            //process all measurements
            foreach (List<Measurement> m_list in Measurement.Measurements){
                foreach (Measurement m in m_list)
                {
                    string line_to_write = "";

                    string units = "mm µm";
                    if (!m.CalibrationGauge.Metric) units = "inch µinch";
                    m.calculateElasticDeformations(federal);
                    m.CalculateDeviations();
                    Measurement.PrepareLineForWrite(m, ref line_to_write, units);
                    sb.Append(Measurement.writeRichTBLine(m, units, count));
                    writer.WriteLine(line_to_write);
                    
                    count++;
                }
            }
            

            writer.Close();

            //invoke the gui to print to the gauge results rich textbox
            pgr(sb.ToString());
            Measurement.writeSummaryToFile();
            Measurement.WriteUncertaintyAndComplianceToFile(ref federal);
        }

        private void ComplianceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComplianceComboBox.SelectedItem == null) return;

            Measurement.working_gauge.ComplianceStandard = ComplianceComboBox.SelectedIndex;
        }

        

        private void GenerateReportButton_Click(object sender, EventArgs e)
        {
            int i = 0;
            foreach (GaugeBlockSet gbs in Measurement.calibration_gauge_sets)
            {
                Report report = new Report(gbs, ref federal);
                reports.Add(report);
                i++;
                report.Text = "Report for gauge set " + gbs.GaugeSetName.ToString();
                report.Show();
            }
        }

        private void VerticalFedForm_Load(object sender, EventArgs e)
        {

        }
    }
}
