using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vertical_Federal_App
{
    public struct Units
    {
        public const short Imperial = 0;
        public const short Metric = 1;
        public const short Mixed = 2;

    }
    public class GaugeBlockSet
    {
        private List<GaugeBlock> gauges;  //gauge in the set
        private string gauge_set_name = "";
        private string report_number = "";
        private string report_date = "";
        private string equipment_id = "";
        private short unit;
        private int num_gauges = 0;
        private string material = "ceramic";
        private Material gauge_set_material;
        
        
        public GaugeBlockSet()
        {
            unit = Units.Metric;
            gauges = new List<GaugeBlock>();
            gauge_set_material = new Material();
            gauge_set_material.exp_coeff = 9.5;
            gauge_set_material.poissons_ratio = 0.290;//dimensionless
            gauge_set_material.youngs_modulus = 205;//Gpa
        }

        public string GaugeSetName
        {
            get { return gauge_set_name; }
            set { gauge_set_name = value; }
        }
     
        public int NumGauges
        {
            get { return num_gauges; }
            set { num_gauges = value; }
        }
        public string ReportNumber
        {
            get { return report_number; }
            set { report_number = value; }
        }
        public string ReportDate
        {
            get { return report_date; }
            set { report_date = value; }
        }
        public short Unit
        {
            get { return unit; }
            set { unit = value; }
        }
        public string EquipID
        {
            get { return equipment_id; }
            set { equipment_id = value; }
        }

        public string Material
        {
            get { return material; }
            set { material = value; }
        }

        public Material GaugeSetMaterial
        {
            get { return gauge_set_material; }
            set { gauge_set_material = value; }
        }
        
        public void PrintGaugeList(ref RichTextBox b)
        {
            foreach(GaugeBlock g in gauges)
            {
                b.AppendText(g.Nominal.ToString() + ", ");
                b.AppendText(g.SerialNumber.ToString() + ", ");
                b.AppendText(g.CentreDeviation.ToString() + "\n");
                b.ScrollToCaret();
            }
        }
        public void AddGauge(GaugeBlock g)
        {
            gauges.Add(g);
        }
        public List<GaugeBlock> GaugeList
        {
            get { return gauges; }
        }
    }
}
