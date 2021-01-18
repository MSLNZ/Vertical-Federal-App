using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertical_Federal_App
{
    public class Stack
    {
        GaugeBlock gauge1;
        GaugeBlock gauge2;
        GaugeBlock gauge3;

        public Stack()
        {

        }
        public GaugeBlock Gauge1
        {
            set { gauge1 = value; }
            get { return gauge1;  }
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
    }

    public class Material
    {
        public double poissons_ratio;
        public double youngs_modulus;
        public double exp_coeff;
    }

    public class GaugeBlock
    {
        private double size;
        private string serial_number;
        private bool metric;
        private double deviation;
        private string from_set; //the name of the reference set this gauge belongs to
        private Material gauge_material;
        
        public GaugeBlock()
        {
            size = 0;
            serial_number = "";
            metric = true;
            gauge_material = new Material();
            gauge_material.exp_coeff = 9.5;
            gauge_material.poissons_ratio = 0.290;
            gauge_material.youngs_modulus = 205;
                
        }
        public double Size
        {
            get { return size; }
            set { size = value; }
        }
        public string SerialNumber
        {
            get { return serial_number; }
            set { serial_number = value; }
        }
      
        public bool Metric
        {
            get { return metric; }
            set { metric = value; }
        }
        public double Deviation
        {
            get { return deviation; }
            set { deviation = value; }
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
        
    }
}
