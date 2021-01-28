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
//using Microsoft.Office.Interop.Excel;



namespace Vertical_Federal_App
{
    public delegate void SerialDataReveived(string data);




    public partial class VerticalFedForm : Form
    {
        
        private SerialDataReveived sdr;
        private bool header_written;
        private VerticalFederal federal;
        private GaugeBlock working_gauge;
        private Stack ref_g;
        private List<GaugeBlockSet> reference_gauge_sets;
        private List<GaugeBlockSet> calibration_gauge_sets;
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
            working_gauge = new GaugeBlock();
            INI2XML.DoIni2XmlConversion(ref messagesRichTextBox);
            if(INI2XML.Converted) INI2XML.PopulateReferenceGaugeComboBox(ref referenceSetComboBox, true);  //initially assume metric reference gauges (second argument true).
            calibration_gauge_sets = new List<GaugeBlockSet>();  //make a new list for calibration gauge sets
            reference_gauge_sets = new List<GaugeBlockSet>();
            radiobuttionclearcalled = false;
            header_written = false;
        }

        private void Comopenbutton_Click(object sender, EventArgs e)
        {
            federal = new VerticalFederal(ref sdr);
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
                        topRightRadioButton.Checked = false;
                        topLeftRadioButton.Checked = true;
                        radiobuttionclearcalled = false;
                        break;
                    case (int)RadioButtonPos.E:
                        rb_position = (int)RadioButtonPos.C3; //this is the next position we need to go to
                        ETextBox.Text = data;
                        radiobuttionclearcalled = true;
                        topLeftRadioButton.Checked = false;
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
                this.BeginInvoke(new SerialDataReveived(DataReceived), textobj);
            }
        }

        private void imperialCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //if this condition occurs the user has checked this checkbox when it was already checked
            if (!imperialCheckBox.Checked && !metricCheckBox.Checked)
            {
                working_gauge.Metric = false;
                imperialCheckBox.Checked = true;
                referenceSetComboBox.Items.Clear();
                INI2XML.PopulateReferenceGaugeComboBox(ref referenceSetComboBox, false);
            }
            else if (imperialCheckBox.Checked && metricCheckBox.Checked)
            {
                working_gauge.Metric = false;
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
                working_gauge.Metric = true;
                metricCheckBox.Checked = true;
                referenceSetComboBox.Items.Clear();
                INI2XML.PopulateReferenceGaugeComboBox(ref referenceSetComboBox, true);
            }
            else if (metricCheckBox.Checked && imperialCheckBox.Checked)
            {
                working_gauge.Metric = true;
                imperialCheckBox.Checked = false;
                referenceSetComboBox.Items.Clear();
                INI2XML.PopulateReferenceGaugeComboBox(ref referenceSetComboBox, true);
            }
            ProcessSizeChangeEvent();
        }

        private void setSerialTextBox_TextChanged(object sender, EventArgs e)
        {
            working_gauge.FromSet = setSerialTextBox.Text;
            

        }

        private void gaugeSerialTextBox_TextChanged(object sender, EventArgs e)
        {
            working_gauge.SerialNumber = gaugeSerialTextBox.Text.ToString();
        }

        private void referenceSetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            //check if we have any reference gauge sets in the list, if we don't then add a new set
            if (reference_gauge_sets.Count == 0)
            {
                GaugeBlockSet gauge_set = new GaugeBlockSet();
                gauge_set.GaugeSetName = referenceSetComboBox.SelectedItem.ToString();
                reference_gauge_sets.Add(gauge_set);

                //Get the reference set data from the xml file
                INI2XML.GetReferenceSetMetaData(ref gauge_set);
                //Add all gauges found in the xml file to the reference gauge set.
                INI2XML.LoadReferenceGauges(ref gauge_set);

                foreach (GaugeBlockSet gbs in reference_gauge_sets)
                {
                    gbs.PrintGaugeList(ref messagesRichTextBox);
                }
            }
            else
            {
                bool set_exists = false;

                //if we have reference gauge sets already in the list then see if the set has previously been added (i.e is this a unique serial number)
                foreach (GaugeBlockSet ref_set in reference_gauge_sets)
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
                    reference_gauge_sets.Add(gauge_set);

                    //Get the reference set data from the xml file
                    INI2XML.GetReferenceSetMetaData(ref gauge_set);
                    //Add all gauges found in the xml file to the reference gauge set.
                    INI2XML.LoadReferenceGauges(ref gauge_set);

                    foreach (GaugeBlockSet gbs in reference_gauge_sets)
                    {
                        gbs.PrintGaugeList(ref messagesRichTextBox);
                    }
                }
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
            if (reference_gauge_sets.Count == 0)
            {
                MessageBox.Show("Please Select a reference set");
                return;
            }

            try
            {
                working_gauge.Nominal = System.Convert.ToDouble(calGaugeNominalTextBox.Text);
            }
            catch (FormatException)
            {
                //MessageBox.Show("Invalid gauge size entered");
                return;
            }

            if (working_gauge.Metric && (working_gauge.Nominal < 0.5 || working_gauge.Nominal > 100))  //metric units
            {
                MessageBox.Show("Gauge block size is not in the measuring range of the comparator");
                working_gauge.IllegalSize = true;
                return;
            }
            else if (!working_gauge.Metric && (working_gauge.Nominal < 0.01 || working_gauge.Nominal > 4))   //imperial units
            {
                MessageBox.Show("Gauge block size is not in the measuring range of the comparator");
                working_gauge.IllegalSize = true;
                return;
            }
            else
            {
                working_gauge.IllegalSize = false;
                
            }

            //construct a list of all available reference gauges to use
            //make all gauge block units in millimetres to avoid confusion
            List<GaugeBlock> all_ref_gauges = new List<GaugeBlock>();
            all_ref_gauges.Clear();



            foreach (GaugeBlockSet g in reference_gauge_sets)
            {
                foreach (GaugeBlock gb in g.GaugeList)
                {
                    if (!gb.Metric) gb.Nominal = Math.Round(gb.Nominal * 25.4, 5);
                    all_ref_gauges.Add(gb);

                }
            }

            //temporarily convert the working gauge to metric too.  The algorithm below is far more complicated in mixed units
            if (!working_gauge.Metric) working_gauge.Nominal = Math.Round(working_gauge.Nominal * 25.4, 5);


            //create a list of possible reference stacks.
            suitable_gauges = new List<Stack>();
            int gauge_count1 = 0;



            foreach (GaugeBlock gb1 in all_ref_gauges)
            {
                //case for a singleton gauge reference
                if (working_gauge.Nominal == gb1.Nominal)
                {
                    Stack gauge_stack = new Stack();


                    if (!working_gauge.Metric) working_gauge.Nominal = Math.Round(working_gauge.Nominal / 25.4, 5); //convert the working gauge nominal back to imperial    
                    if (!gb1.Metric) gb1.Nominal = Math.Round(gb1.Nominal / 25.4, 5); //convert back to imperial units

                    gauge_stack.Gauge1 = gb1;


                    //if we have found a singleton gauge then it should be the gauge we use
                    suitable_gauges.Clear();
                    suitable_gauges.Add(gauge_stack);


                    suitableReferenceGaugesComboBox.Items.Clear();
                    if (gb1.Metric) suitableReferenceGaugesComboBox.Items.Add(gb1.Nominal.ToString() + " mm");
                    else suitableReferenceGaugesComboBox.Items.Add(gb1.Nominal.ToString() + " inch");
                    suitableReferenceGaugesComboBox.SelectedIndex = 0;

                    foreach (GaugeBlockSet g in reference_gauge_sets)
                    {
                        foreach (GaugeBlock gb in g.GaugeList)
                        {
                            //remember we already converted the working gauge back to imperial so don't do it again here
                            if (!(working_gauge.Nominal == gb.Nominal))
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
                    if ((working_gauge.Nominal == Math.Round((gb1.Nominal + gb2.Nominal), 5)) && (gauge_count1 != gauge_count2))
                    {
                        Stack gauge_stack = new Stack();
                        gauge_stack.Gauge1 = gb1;
                        gauge_stack.Gauge2 = gb2;
                        if (!gb1.Metric) gauge_stack.Gauge1.Nominal = Math.Round(gb1.Nominal / 25.4, 5); //convert the reference gauge block nominal back to imperial units. 
                        if (!gb2.Metric) gauge_stack.Gauge2.Nominal = Math.Round(gb2.Nominal / 25.4, 5); //convert the reference gauge block nominal back to imperial units

                        suitable_gauges.Add(gauge_stack);


                        if (gb1.Metric && gb2.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " mm,  " + gauge_stack.Gauge2.Nominal.ToString() + " mm");
                        else if (gb1.Metric && !gb2.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " mm,  " + gauge_stack.Gauge2.Nominal.ToString() + " inch");
                        else if (!gb1.Metric && gb2.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " inch,  " + gauge_stack.Gauge2.Nominal.ToString() + " mm");
                        else suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " inch,  " + gauge_stack.Gauge2.Nominal.ToString() + " inch");

                        if (!gb1.Metric) gb1.Nominal = Math.Round(gb1.Nominal * 25.4, 5); //convert the reference gauge block nominal back to metric units. 
                        if (!gb2.Metric) gb2.Nominal = Math.Round(gb2.Nominal * 25.4, 5); //convert the reference gauge block nominal back to metric units

                    }
                    int gauge_count3 = 0;
                    foreach (GaugeBlock gb3 in all_ref_gauges)
                    {
                        //case for a three gauge stacked reference (don't compare the same gauge with itself)
                        if ((working_gauge.Nominal == Math.Round((gb1.Nominal + gb2.Nominal + gb3.Nominal), 5)) && (gauge_count1 != gauge_count2) && (gauge_count2 != gauge_count3) && (gauge_count1 != gauge_count3))
                        {
                            Stack gauge_stack = new Stack();
                            gauge_stack.Gauge1 = gb1;
                            gauge_stack.Gauge2 = gb2;
                            gauge_stack.Gauge3 = gb3;

                            if (!gb1.Metric) gauge_stack.Gauge1.Nominal = Math.Round(gb1.Nominal / 25.4, 5); //convert the reference gauge block nominal back to imperial units. 
                            if (!gb2.Metric) gauge_stack.Gauge2.Nominal = Math.Round(gb2.Nominal / 25.4, 5); //convert the reference gauge block nominal back to imperial units
                            if (!gb3.Metric) gauge_stack.Gauge3.Nominal = Math.Round(gb3.Nominal / 25.4, 5); //convert the reference gauge block nominal back to imperial units



                            suitable_gauges.Add(gauge_stack);

                            if (gb1.Metric && gb2.Metric && gb3.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " mm,  " + gauge_stack.Gauge2.Nominal.ToString() + " mm,  " + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " mm");
                            else if (gb1.Metric && gb2.Metric && !gb3.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " mm,  " + gauge_stack.Gauge2.Nominal.ToString() + " mm,  " + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " inch");
                            else if (gb1.Metric && !gb2.Metric && gb3.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " mm,  " + gauge_stack.Gauge2.Nominal.ToString() + " inch,  " + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " mm");
                            else if (gb1.Metric && !gb2.Metric && !gb3.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " mm,  " + gauge_stack.Gauge2.Nominal.ToString() + " inch,  " + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " inch");
                            else if (!gb1.Metric && gb2.Metric && gb3.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " inch,  " + gauge_stack.Gauge2.Nominal.ToString() + " mm,  " + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " mm");
                            else if (!gb1.Metric && gb2.Metric && !gb3.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " inch,  " + gauge_stack.Gauge2.Nominal.ToString() + " mm,  " + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " inch");
                            else if (!gb1.Metric && !gb2.Metric && gb3.Metric) suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " inch,  " + gauge_stack.Gauge2.Nominal.ToString() + " inch  ," + Math.Round(gauge_stack.Gauge3.Nominal, 5).ToString() + " mm");
                            else suitableReferenceGaugesComboBox.Items.Add(gauge_stack.Gauge1.Nominal.ToString() + " inch,  " + gauge_stack.Gauge2.Nominal.ToString() + " inch,  " + gauge_stack.Gauge3.Nominal.ToString() + " inch");

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
            if (!working_gauge.Metric) working_gauge.Nominal = Math.Round(working_gauge.Nominal / 25.4, 5);

            foreach (GaugeBlockSet g in reference_gauge_sets)
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
                working_gauge.GaugeBlockMaterial.youngs_modulus = System.Convert.ToDouble(youngModulusTextBox.Text);
            }
            catch (FormatException) { }

        }

        private void poissonsRatioTextBox_TextChanged(object sender, EventArgs e)
        {

            try
            {
                working_gauge.GaugeBlockMaterial.poissons_ratio = System.Convert.ToDouble(poissonsRatioTextBox.Text);
            }
            catch (FormatException) { }
        }
        private void clientNameTextBox_TextChanged(object sender, EventArgs e)
        {
            working_gauge.ClientName = clientNameTextBox.Text;
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
                    break;
                case "steel":
                    mtrl.exp_coeff = 11.5; //NIST Gauge Block Handbook
                    mtrl.poissons_ratio = 0.296; //0.75% C hardened - Kaye and Laby "Tables of Physical and Chemical Constants", 16th eddition
                    mtrl.youngs_modulus = 201.4; //0.75% C hardened - Kaye and Laby "Tables of Physical and Chemical Constants", 16th eddition
                    break;
                case "tungsten carbide":
                    mtrl.exp_coeff = 4.5; //NIST Gauge Block Handbook
                    mtrl.poissons_ratio = 0.2; //NIST EMToolBox 10% Cobalt  - this agrees well with the opus website
                    mtrl.youngs_modulus = 599.84; //NIST EMToolBox 10% Cobalt - this agrees well with the opus website
                    break;
            }

            poissonsRatioTextBox.Text = mtrl.poissons_ratio.ToString();
            youngModulusTextBox.Text = mtrl.youngs_modulus.ToString();
            expNumericUpDown.Value = System.Convert.ToDecimal(mtrl.exp_coeff);
            working_gauge.GaugeBlockMaterial = mtrl;
            
            
        }

        private void expNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            working_gauge.GaugeBlockMaterial.exp_coeff = System.Convert.ToDouble(expNumericUpDown.Value);
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

        private void saveGaugeData_Click(object sender, EventArgs e)
        {
            if (working_gauge.IllegalSize)
            {
                MessageBox.Show("Illegal gauge size!  Change size before continuing");
                return;
            }
            int set_index = 0;
            if (!CreateNewCalSet(ref set_index)) return;

            //add the gauge to the calibration gauge set
            calibration_gauge_sets[set_index].GaugeList.Add(working_gauge);
            calibration_gauge_sets[set_index].NumGauges++;
            
            
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
            double variation = 0.0;
            double corr_centre_dev = 0.0;
            double corr_extreme_dev = 0.0;
            double corr_length = 0.0;
            Measurement current_measurement = new Measurement();
            current_measurement.Metric = working_gauge.Metric;
            current_measurement.Nominal = working_gauge.Nominal;
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
            current_measurement.CalSetSerial = setSerialTextBox.Text;
            current_measurement.CalibrationGauge = working_gauge;
            current_measurement.ReferenceStack = ref_g;
            current_measurement.CalculateVariation();
            current_measurement.CalculateMeasuredDiff_um_uinch();
            current_measurement.RefLength();
            current_measurement.calculateElasticDeformations(federal);
            current_measurement.CalculateDeviations(ref corr_centre_dev,ref corr_extreme_dev,ref corr_length);
            current_measurement.CalibrationGauge.CorrLength = Math.Round(corr_length, 7);
            current_measurement.CalibrationGauge.CentreDeviation = Math.Round(corr_centre_dev, 5);
            current_measurement.CalibrationGauge.ExtremeDeviation = Math.Round(corr_extreme_dev, 5);
            current_measurement.CalibrationGauge.Variation = Math.Round(current_measurement.CalculateVariation(), 3);


            if (!header_written)
            {
                //write the header string of the output rich text box
                
                gaugeResultsRichTextBox.Text = "Measurement No.\tUnits\tNominal\tCentre Dev\tExtreme Dev\tVariation\n";
                gaugeResultsRichTextBox.SelectAll();
                gaugeResultsRichTextBox.SelectionTabs = new int[] { 110, 210, 280, 350, 430 };
                gaugeResultsRichTextBox.AcceptsTab = true;
                gaugeResultsRichTextBox.Select(0, 0);
                header_written = true;
            }
            string units = "mm µm";
            if (!current_measurement.Metric)
            {
                units = "inch µinch";
            }
            
            gaugeResultsRichTextBox.AppendText(Measurement.Measurements.Count.ToString() + "\t" +
                units+"\t" + current_measurement.Nominal.ToString() + "\t" + Math.Round(corr_centre_dev,5).ToString()+
                "\t"+Math.Round(corr_extreme_dev,5).ToString()+"\t"+Math.Round(variation,5).ToString()+"\n");
            gaugeResultsRichTextBox.SelectAll();
            gaugeResultsRichTextBox.SelectionTabs = new int[] { 110, 210, 280, 350, 430 };
            gaugeResultsRichTextBox.AcceptsTab = true;
            gaugeResultsRichTextBox.Select(0, 0);

            //add the current measurement to the measurement list.
            Measurement.Measurements.Add(current_measurement);

            StringBuilder line = new StringBuilder();
            // Create a string array that consists of three lines.
            line.Append(units+",");
            line.Append(current_measurement.Nominal.ToString() + ",");
            line.Append(current_measurement.CalibrationGauge.SerialNumber + ",");
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
                line.Append(Math.Round(current_measurement.ReferenceStack.Gauge1.Nominal,5).ToString() + ",");
                line.Append(Math.Round(current_measurement.ReferenceStack.Gauge1.CentreDeviation,5).ToString() + ",");
            }
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
                    line.Append(Math.Round(current_measurement.ReferenceStack.Gauge2.Nominal,5).ToString() + ",");
                    line.Append(Math.Round(current_measurement.ReferenceStack.Gauge2.CentreDeviation,5).ToString() + ",");
                }
            }
            else line.Append(",,");

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
                    line.Append(Math.Round(current_measurement.ReferenceStack.Gauge3.Nominal,5).ToString() + ",");
                    line.Append(Math.Round(current_measurement.ReferenceStack.Gauge3.CentreDeviation,5).ToString() + ",");
                }
            }
            else line.Append(",,");

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
            variation = Math.Round(current_measurement.CalibrationGauge.Variation, 5);
            corr_centre_dev = Math.Round(current_measurement.CalibrationGauge.CentreDeviation, 5);
            corr_extreme_dev = Math.Round(current_measurement.CalibrationGauge.ExtremeDeviation, 7);
            corr_length = Math.Round(current_measurement.CalibrationGauge.CorrLength, 5);
            line.Append(corr_length.ToString() + ",");
            line.Append(corr_centre_dev.ToString() + ",");
            line.Append(corr_extreme_dev.ToString() + ",");
            line.Append(variation.ToString() + ",");

            //directory to write all measured a calculated data to.  Does not include averages of results.  I have put that in a seperate file below.
            if (!System.IO.Directory.Exists(@"G:\Shared drives\MSL - Shared\Length\Data\FederalData"))
            {
                System.IO.Directory.CreateDirectory(@"G:\Shared drives\MSL - Shared\Length\Data\FederalData");
            }
            //Determine a suitable file name from metadata
            string filename = "";
            if(working_gauge.ClientName != null)  //base file name on the client nname
            {
                filename = @"G:\Shared drives\MSL - Shared\Length\Data\FederalData" + Measurement.Measurements.Last().CalibrationGauge.ClientName + "_" + System.DateTime.Now.Year.ToString();
            }
            else filename = @"G:\Shared drives\MSL - Shared\Length\Data\FederalData" + Measurement.Measurements.Last().CalibrationGauge.FromSet + "_" + System.DateTime.Now.Year.ToString();

            System.IO.StreamWriter writer;
            if (!System.IO.File.Exists(filename))
            {
                writer = System.IO.File.CreateText(filename);
                writer.WriteLine("Units,Nominal,Gauge Serial No.,Ref Gauge 1 Nominal,Ref Gauge 1 Deviation,Ref Gauge 1 exp coeeff,Ref Gauge 1 Young's Modulus,Ref Gauge 1 Poisson Ratio," +
                    "Ref Gauge 2 Nominal,Ref Gauge 2 Deviation,Ref Gauge 2 exp coeeff,Ref Gauge 2 Young's Modulus,Ref Gauge 2 Poisson Ratio," +
                    "Ref Gauge 3 Nominal,Ref Gauge 3 Deviation,Ref Gauge 3 exp coeeff,Ref Gauge 3 Young's Modulus,Ref Gauge 3 Poisson Ratio," +
                    "Reference Stack Deviation,R1,C1,A,B,C2,D,E,C3,R2,Measured Length,Centre Deviation,Extreme Deviation,Variation");
            }
            else writer = System.IO.File.AppendText(filename);

            writer.WriteLine(line);


            writer.Close();

          
            //Determine a suitable file name from metadata
            filename = "";
            if (working_gauge.ClientName != null)  //base file name on the client nname
            {
                filename = @"G:\Shared drives\MSL - Shared\Length\Data\FederalData" + Measurement.Measurements.Last().CalibrationGauge.ClientName + "_summary" + System.DateTime.Now.Year.ToString();
            }
            else filename = @"G:\Shared drives\MSL - Shared\Length\Data\FederalData" + Measurement.Measurements.Last().CalibrationGauge.FromSet + "_summary" + System.DateTime.Now.Year.ToString();

            System.IO.StreamWriter writer2;
            writer2 = System.IO.File.CreateText(filename);
            writer2.WriteLine("Nominal,Serial Number,Measured Length,Centre Deviation,Extreme Deviation,Variation");
           
            

            string unique_id = "";  //a unit id is a concatination of Nominal, setid, serial no
            List<string> unique_ids_used = new List<string>(); //a list of the ids we have used in the loop below

            int count1 = 0;
            foreach (Measurement m in Measurement.Measurements)
            {
                unique_id = m.Nominal + m.CalSetSerial + m.CalibrationGauge.SerialNumber;
                if (!unique_ids_used.Contains(unique_id))
                {
                    //with the unique id loop through each measurement
                    int count2 = 0;
                    int num_id_matches = 0;
                    double nominal = m.CalibrationGauge.Nominal;
                    string ser_no = m.CalibrationGauge.SerialNumber;
                    double sum_centre_dev = m.CalibrationGauge.CentreDeviation;
                    double sum_extreme_dev = m.CalibrationGauge.ExtremeDeviation;
                    double sum_variation = m.CalibrationGauge.Variation;

                    foreach (Measurement k in Measurement.Measurements)
                    {
                        string unique_id_to_compare = k.Nominal + k.CalSetSerial + k.CalibrationGauge.SerialNumber;
                        if (count2 > count1)
                        {
                            if (unique_id_to_compare.Equals(unique_id))
                            {
                                //we have a match
                                num_id_matches++;
                                sum_centre_dev += k.CalibrationGauge.CentreDeviation;
                                sum_extreme_dev += k.CalibrationGauge.ExtremeDeviation;
                                sum_variation += k.CalibrationGauge.Variation;
                            }
                        }
                        count2++;
                    }
                    //compute the means
                    sum_centre_dev /= (num_id_matches+1); 
                    sum_extreme_dev /= (num_id_matches + 1);
                    sum_variation /= (num_id_matches + 1);
                    writer2.WriteLine(nominal + "," + ser_no + "," + sum_centre_dev + "," + sum_extreme_dev + "," + sum_variation);
                    unique_ids_used.Add(unique_id);
                }
                count1++;
            }
            writer2.Close();
        }

        private bool CreateNewCalSet(ref int set_index)
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
