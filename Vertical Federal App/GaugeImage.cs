using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertical_Federal_App
{
    
    class GaugeImage
    {
        private string filename;
        private string directory;
        private string report_description;
        private string caption;
        public GaugeImage()
        {

        }
        public string Directory
        {
            set { directory = value; }
            get { return directory; }
        }
        public string Filename
        {
            set { filename = value; }
            get { return filename; }
        }

        public string Description
        {
            set { report_description = value; }
            get { return report_description; }
        }
        public string Caption
        {
            set { caption = value; }
            get { return caption; }
        }
    }
}
