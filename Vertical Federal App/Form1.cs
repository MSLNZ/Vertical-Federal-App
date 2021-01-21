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
        private Material steel;
        private Material tungsten_carbide;
        private Material ceramic;
        private int rb_position;
        private bool rb_C1;
        private bool rb_C2;
        private bool rb_C3;
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
            //if this condition occurs the user has checked this button when it was already checked
            if (!imperialCheckBox.Checked && !metricCheckBox.Checked)
            {
                working_gauge.Metric = false;
                imperialCheckBox.Checked = true;
                referenceSetComboBox.Items.Clear();
                INI2XML.PopulateReferenceGaugeComboBox(ref referenceSetComboBox, false);
            }
            else if (imperialCheckBox.Checked && metricCheckBox.Checked)
            {
                working_gauge.Metric = true;
                metricCheckBox.Checked = false;
                referenceSetComboBox.Items.Clear();
                INI2XML.PopulateReferenceGaugeComboBox(ref referenceSetComboBox, false);
            }
        }

        private void metricCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //if this condition occurs the user has checked this button when it was already checked
            if (!metricCheckBox.Checked && !imperialCheckBox.Checked)
            {
                working_gauge.Metric = true;
                metricCheckBox.Checked = true;
                referenceSetComboBox.Items.Clear();
                INI2XML.PopulateReferenceGaugeComboBox(ref referenceSetComboBox, true);
            }
            else if (metricCheckBox.Checked && imperialCheckBox.Checked)
            {
                working_gauge.Metric = false;
                imperialCheckBox.Checked = false;
                referenceSetComboBox.Items.Clear();
                INI2XML.PopulateReferenceGaugeComboBox(ref referenceSetComboBox, true);
            }
        }

        private void setSerialTextBox_TextChanged(object sender, EventArgs e)
        {
            //check if we have any calibration gauge sets in the list, if we don't then add a new set
            if (calibration_gauge_sets.Count == 0)
            {
                GaugeBlockSet gauge_set = new GaugeBlockSet();
                gauge_set.GaugeSetName = setSerialTextBox.Text.ToString();
                calibration_gauge_sets.Add(gauge_set);
            }
            else
            {
                bool set_exists = false;
                //if we have calibration gauge sets already in the list then see if the set has previously been added (i.e is this a unique serial number)

                foreach (GaugeBlockSet cal_set in calibration_gauge_sets)
                {
                    if (cal_set.GaugeSetName.Equals(setSerialTextBox.Text.ToString()))
                    {
                        set_exists = true;
                    }
                }

                if (!set_exists)
                {
                    GaugeBlockSet gauge_set = new GaugeBlockSet();
                    gauge_set.GaugeSetName = setSerialTextBox.Text.ToString();
                    calibration_gauge_sets.Add(gauge_set);
                }
            }

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
                return;
            }
            if (!working_gauge.Metric && (working_gauge.Nominal < 0.01 || working_gauge.Nominal > 4))   //imperial units
            {
                MessageBox.Show("Gauge block size is not in the measuring range of the comparator");
                return;
            }

            //construct a list of all available reference gauges to use
            List<GaugeBlock> all_ref_gauges = new List<GaugeBlock>();

            foreach (GaugeBlockSet g in reference_gauge_sets)
            {
                foreach (GaugeBlock gb in g.GaugeList)
                {
                    all_ref_gauges.Add(gb);
                }
            }

            //create a list of possible reference stacks.
            suitable_gauges = new List<Stack>();
            int gauge_count1 = 0;
            int gauge_count2 = 0;
            int gauge_count3 = 0;

            foreach (GaugeBlock gb1 in all_ref_gauges)
            {
                //case for a singleton gauge reference
                if (working_gauge.Nominal == gb1.Nominal)
                {
                    Stack gauge_stack = new Stack();
                    gauge_stack.Gauge1 = gb1;

                    //if we have found a singleton gauge then it should be the gauge we use
                    suitable_gauges.Clear();
                    suitable_gauges.Add(gauge_stack);

                    suitableReferenceGaugesComboBox.Items.Clear();
                    suitableReferenceGaugesComboBox.Items.Add(gb1.Nominal.ToString());
                    suitableReferenceGaugesComboBox.SelectedItem = gb1.Nominal.ToString();
                    return;

                }
                foreach (GaugeBlock gb2 in all_ref_gauges)
                {

                    //case for a two gauge stacked reference (don't compare the same gauge with itself)
                    if ((working_gauge.Nominal == (gb1.Nominal + gb2.Nominal)) && (gauge_count1 != gauge_count2))
                    {
                        Stack gauge_stack = new Stack();
                        gauge_stack.Gauge1 = gb1;
                        gauge_stack.Gauge2 = gb2;
                        suitable_gauges.Add(gauge_stack);
                        suitableReferenceGaugesComboBox.Items.Add(gb1.Nominal.ToString() + ",  " + gb2.Nominal.ToString());
                    }
                    foreach (GaugeBlock gb3 in all_ref_gauges)
                    {
                        //case for a three gauge stacked reference (don't compare the same gauge with itself)
                        if ((working_gauge.Nominal == (gb1.Nominal + gb2.Nominal + gb3.Nominal)) && (gauge_count1 != gauge_count2) && (gauge_count2 != gauge_count3) && (gauge_count1 != gauge_count3))
                        {
                            Stack gauge_stack = new Stack();
                            gauge_stack.Gauge1 = gb1;
                            gauge_stack.Gauge2 = gb2;
                            gauge_stack.Gauge3 = gb3;
                            suitable_gauges.Add(gauge_stack);
                            suitableReferenceGaugesComboBox.Items.Add(gb1.Nominal.ToString() + ",  " + gb2.Nominal.ToString() + ",  " + gb3.Nominal.ToString());
                        }

                        gauge_count3++;
                    }
                    gauge_count2++;
                }
                gauge_count1++;
            }


        }

        private void suitableReferenceGaugesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = suitableReferenceGaugesComboBox.SelectedIndex;
            ref_g = suitable_gauges.ElementAt(index);
            double refdev = 0.0;

            GaugeBlock g1 = ref_g.Gauge1;
            GaugeBlock g2 = ref_g.Gauge2;
            GaugeBlock g3 = ref_g.Gauge3;

            if (g1 != null) refdev = g1.Deviation;
            if (g2 != null) refdev += g2.Deviation;
            if (g3 != null) refdev += g3.Deviation;

            //update the deviation field of the measurement group box
            try
            {
                referenceDeviationTextBox.Text = refdev.ToString();
                EnableAllMeasurementRadioButtions();
                //EnableAllMeasurementTextBoxes();
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

            working_gauge.FromSet = setSerialTextBox.Text;
            int index = 0;
            bool set_found;
            foreach(GaugeBlockSet s in calibration_gauge_sets)
            {
                if (s.GaugeSetName.Equals(setSerialTextBox.Text))
                {
                    set_found = true;
                }
                else index++;
            }

            
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
            double corr_centre_dev = 0.0;
            double variation = 0.0;
            double corr_extreme_dev = 0.0;
            double corr_length = 0.0;
            Measurement current_measurement = new Measurement();
            if (!metricCheckBox.Checked) current_measurement.Metric = false;
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
            


            if (!header_written)
            {
                //write the header string of the output rich text box
                gaugeResultsRichTextBox.Text = "Measurement No.  Units  Nominal  Centre Dev  Extreme Dev  Variation\n";
                header_written = true;
            }
            string units = "mm";
            if (!current_measurement.Metric)
            {
                units = "inch";
            }
            else {

                gaugeResultsRichTextBox.Text = "      " + Measurement.Measurements.Count.ToString() + "         " +
                    "  mm   "+ No.Units Nominal  Centre Dev  Extreme Dev  Variation\n";
            }  










        }
    }
}
