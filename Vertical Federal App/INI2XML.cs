using System;
using System.IO;
using System.Text;
using System.Data;
using System.Xml;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Vertical_Federal_App
{
	/// <summary>
	/// WIN32 API Wrapper class
	/// </summary>
	public class WIN32Wrapper
	{
		/// <summary>
		/// Get all the section names from an INI file
		/// </summary>
		[DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileSectionNamesA")]
		public extern static int GetPrivateProfileSectionNames(
			[MarshalAs(UnmanagedType.LPArray)] byte[] lpReturnedString,
			int nSize,
			string lpFileName);

		/// <summary>
		/// Get all the settings from a section in a INI file
		/// </summary>
		[DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileSectionA")]
		public extern static int GetPrivateProfileSection(
			string lpAppName,
			[MarshalAs(UnmanagedType.LPArray)] byte[] lpReturnedString,
			int nSize,
			string lpFileName);
	}

	/// <summary>
	/// Convert an INI file into an XML file
	/// </summary>
	public class INI2XML
	{


		private static XmlTextReader xmlreader;
		private static XmlTextReader xmlreader2;

		private static string xmlfilename;
		private static string xmlfilename_uncertainty;
		private static XmlReaderSettings settings;
		private static bool conversion_succeeded = true;
		/// 
		/// </summary>
		public INI2XML()
		{
		}

		public static bool Converted
		{
			get { return conversion_succeeded; }
		}

		/// <summary>
		/// Initial size of the buffer used when calling the Win32 API functions
		/// </summary>
		const int INITIAL_BUFFER_SIZE = 4096;

	

		/// <summary>
		/// Get a token from a delimited string, eg.
		///   intSection = 0
		///   strSection = GetToken(lpSections, charNull, intSection)
		/// </summary>
		/// <param name="strText">Text that is delimited</param>
		/// <param name="delimiter">The delimiter, eg. ","</param>
		/// <param name="intIndex">The index of the token to return, NB. first token is index 0.</param>
		/// <returns>Returns the nth token from a string.</returns>
		private static string GetToken(string strText, char[] delimiter, int intIndex)
		{
			string strTokenRet = "";

			string[] strTokens = strText.Split(delimiter);

			if (strTokens.GetUpperBound(0) >= intIndex)
				strTokenRet = strTokens[intIndex];

			return strTokenRet;
		} // GetToken

		/// <summary>
		/// Does an INI2XML conversion
		/// Only Call from GUI Thread
		/// <summary>
		public static void DoIni2XmlConversion(ref RichTextBox messages, string xmlfilename, string inifilename, bool U95)
		{
			if (U95 == false)
			{
				if (INI2XML.ConvertA(inifilename, ref xmlfilename))
				{
					TextReader tr = new StreamReader(xmlfilename);
					tr.Close();

					messages.AppendText("Successfully converted ini to XML\n");
				}
				else
				{
					messages.AppendText("Problem converting INI to XML: file in use .... new version created\n...proceeding\n\n");

				}
			}
            else
            {
				if (INI2XML.ConvertB(inifilename, ref xmlfilename))
				{
					TextReader tr = new StreamReader(xmlfilename);
					tr.Close();

					messages.AppendText("Successfully converted U95 ini to XML\n");
				}
				else
				{
					messages.AppendText("Problem converting U95 INI to XML: file in use .... new version created\n...proceeding\n\n");

				}
			}
		}
		/// <summary>
		/// Converts an INI file into an XML file.
		/// Output XML file has the following structure...
		///   <?xml version="1.0"?>
		///   <configuration>
		///       <section name="Main">
		///           <setting name="Timeout" value="90"/>
		///           <setting name="Mode" value="Live"/>
		///      </section>
		///   </configuration>
		/// Example:
		///	if (Loki.INI2XML.Convert( txtIniFileName.Text, txtXMLFileName.Text ))
		///		Console.WriteLine( "Successfully converted \"" + txtIniFileName.Text + "\" to \"" + txtXMLFileName.Text + "\"" );
		///	else
		///		Console.WriteLine( "Problem converting \"" + txtIniFileName.Text + "\" to \"" + txtXMLFileName.Text + "\"" );
		/// If an exception is raised, it is passed on to the caller.
		/// </summary>
		/// <param name="strINIFileName">File name of the INI file to convert</param>
		/// <param name="strXMLFileName">File name of the XML file that is created</param>
		/// <returns>True if successfuly, or False if a problem</returns>
		public static bool ConvertA(string strINIFileName, ref string strXMLFileName)
		{
			char[] charEquals = { '=' };
			string lpSections;
			int nSize;
			int nMaxSize;
			string strSection;
			int intSection;
			int intNameValue;
			string strName;
			string strValue;
			string strNameValue;
			string lpNameValues;
			byte[] str = new byte[1];
			XmlWriter xw = null;

			bool could_not_write = false;
			bool ok = false;

			xmlfilename = strXMLFileName;

			try
			{
				if (strXMLFileName.Length == 0)
				{
					strXMLFileName = Path.Combine(Path.GetDirectoryName(strINIFileName), String.Format("{0}.xml", Path.GetFileNameWithoutExtension(strINIFileName)));
				}

				// Get all sections names
				// Making sure allocate enough space for data returned
				// Thanks to Peter for his fix to this loop.
				// Peter G Jones, Microsoft .Net MVP, University of Canterbury, NZ
				nMaxSize = INITIAL_BUFFER_SIZE;
				while (true)
				{
					str = new byte[nMaxSize];
					nSize = WIN32Wrapper.GetPrivateProfileSectionNames(str, nMaxSize, strINIFileName);
					if ((nSize != 0) && (nSize == (nMaxSize - 2)))
					{
						nMaxSize *= 2;
					}
					else
					{
						break;
					}
				}

				// convert the byte array into a .NET string
				lpSections = Encoding.ASCII.GetString(str);

				// Use this for Unicode
				// lpSections = Encoding.Unicode.GetString( str );

				// Create XML File
				//xw = XmlTextWriter.Create(strXMLFileName);
				try
				{
					xw = new XmlTextWriter(strXMLFileName, Encoding.UTF8);
				}
				catch (System.IO.IOException)
				{

					strXMLFileName = @"G:\Shared drives\MSL - Length\Length\EQUIPREG\XML Files\cal_data_" + "Federal_measurement_"+ System.DateTime.Now.Ticks.ToString() + ".xml";
					xmlfilename = strXMLFileName;
					//if the file is in use write it under another name
					try
					{
						xw = new XmlTextWriter(strXMLFileName, Encoding.UTF8);
                    }
                    catch(System.IO.DirectoryNotFoundException) {
						MessageBox.Show("Cannot Find .ini file");
						Application.Exit();
						conversion_succeeded = false;
						return false;
						
					}
					could_not_write = true;
				}
				// Write the opening xml
				xw.WriteStartDocument();
				xw.WriteStartElement("configuration");   //open1

				bool is_written = false;
				bool to_close = false;

				// Loop through each section in the .ini file
				char[] charNull = { '\0' };
				for (intSection = 0,
					strSection = GetToken(lpSections, charNull, intSection);
					strSection.Length > 0;
					strSection = GetToken(lpSections, charNull, ++intSection))
				{

					// Write a Node for the Section
					if (strSection.Contains("prt"))
					{
						if (is_written == false)
						{
							xw.WriteStartElement("PRT");  //open2
							is_written = true;
						}
						if (GetToken(lpSections, charNull, intSection + 1).Contains("resistancebridge"))
						{
							is_written = false;
							to_close = true;
						}
					}


					if (strSection.Contains("resistancebridge"))
					{

						if (is_written == false)
						{
							xw.WriteStartElement("RESISTANCEBRIDGE");  //open2
							is_written = true;
						}

						if (GetToken(lpSections, charNull, intSection + 1).Contains("resistor"))
						{
							is_written = false;
							to_close = true;
						}
					}
					if (strSection.Contains("resistor"))
					{

						if (is_written == false)
						{
							xw.WriteStartElement("RESISTOR");  //open2
							is_written = true;
						}

						if (GetToken(lpSections, charNull, intSection + 1).Contains("comparator"))
						{
							is_written = false;
							to_close = true;
						}
					}
                    if (strSection.Contains("comparator"))
                    {
						if (is_written == false)
						{
							xw.WriteStartElement("GAUGEBLOCKCOMPARATOR");  //open2
							is_written = true;
						}

						if (GetToken(lpSections, charNull, intSection + 1).Contains("gauge"))
						{
							is_written = false;
							to_close = true;
						}
					}
					if (strSection.Contains("gauge"))
					{

						if (is_written == false)
						{
							xw.WriteStartElement("GAUGE");  //open2
							is_written = true;
						}

						if (GetToken(lpSections, charNull, intSection + 1).Contains("laser"))
						{
							is_written = false;
							to_close = true;
						}
					}
					if (strSection.Contains("laser"))
					{

						if (is_written == false)
						{
							xw.WriteStartElement("LASERMEASUREMENTSYSTEM");  //open2
							is_written = true;
						}
						if (GetToken(lpSections, charNull, intSection + 1).Contains("lamp"))
						{
							is_written = false;
							to_close = true;
						}
					}
					if (strSection.Contains("lamp"))
					{

						if (is_written == false)
						{
							xw.WriteStartElement("LAMP");  //open2
							is_written = true;
						}
						if (GetToken(lpSections, charNull, intSection + 1).Contains("laboratory"))
						{
							is_written = false;
							to_close = true;
						}
					}
					if (strSection.Contains("laboratory"))
					{

						if (is_written == false)
						{
							xw.WriteStartElement("LABORATORY");
							is_written = true;
						}
						if (GetToken(lpSections, charNull, intSection + 1).Contains("barometer"))
						{
							is_written = false;
							to_close = true;
						}

					}
					if (strSection.Contains("barometer"))
					{

						if (is_written == false)
						{
							xw.WriteStartElement("BAROMETER");
							is_written = true;
						}
						if (GetToken(lpSections, charNull, intSection + 1).Contains("humidity"))
						{
							is_written = false;
							to_close = true;
						}
					}

					if (strSection.Contains("humidity"))
					{

						if (is_written == false)
						{
							xw.WriteStartElement("HUMIDITY");
							is_written = true;
						}

					}


					// Get all values in this section, making sure to allocate enough space
					for (nMaxSize = INITIAL_BUFFER_SIZE,
						nSize = nMaxSize;
						nSize != 0 && nSize >= (nMaxSize - 2);
						nMaxSize *= 2)
					{
						str = new Byte[nMaxSize];
						nSize = WIN32Wrapper.GetPrivateProfileSection(strSection, str, nMaxSize, strINIFileName);
					}

					// convert the byte array into a .NET string
					lpNameValues = Encoding.ASCII.GetString(str);


					bool isgaugeblockset = false;
					// Use this for Unicode
					// lpNameValues = Encoding.Unicode.GetString( str );

					// Loop through each Name/Value pair
					xw.WriteStartElement(strSection);              //Open3
					for (intNameValue = 0,
						strNameValue = GetToken(lpNameValues, charNull, intNameValue);
						strNameValue.Length > 0;
						strNameValue = GetToken(lpNameValues, charNull, ++intNameValue))
					{
						// Get the name and value from the entire null separated string of name/value pairs
						// Also escape out the special characters, (ie. &"<> )
						strName = GetToken(strNameValue, charEquals, 0);
						if (strNameValue.Length > (strName.Length + 1))
						{
							strValue = strNameValue.Substring(strName.Length + 1);
						}
						else
						{
							strValue = "";
						}
						if (isgaugeblockset == false)
						{
							xw.WriteStartElement(strName);        //Open4
							xw.WriteValue(strValue);
							xw.WriteEndElement();                 //Close4
						}
						else
						{
							xw.WriteStartElement("Gauge");
							xw.WriteAttributeString("Nominal", strName);
							xw.WriteAttributeString("Deviation", strValue);
							xw.WriteEndElement();
						}
						// Write the XML Name/Value Node to the xml file
						if (strValue.Equals("GAUGE_BLOCK_SET"))
						{    //special case for gauge blocks
							isgaugeblockset = true;
							xw.WriteStartElement("GAUGES");
						}



					}
					if (isgaugeblockset == true)
					{
						xw.WriteEndElement();
					}
					//close the strsection node.
					xw.WriteEndElement();                     //close3
															  // Close the section node
					if (to_close == true)
					{
						xw.WriteEndElement();                     //close2
						to_close = false;
					}
				}


				// Thats it
				xw.WriteEndElement();

				xw.WriteEndElement();                         //close1
				xw.WriteEndDocument();


				ok = true;
			}
			finally
			{
				if (xw != null)
				{
					xw.Close();
				}
			}

			if (could_not_write)
			{
				return !ok;
			}

			return ok;
		} // ConvertA


		/// <summary>
		/// Converts an INI file into an XML file.
		/// Output XML file has the following structure...
		///   <?xml version="1.0"?>
		///   <configuration>
		///       <section name="Main">
		///           <setting name="Timeout" value="90"/>
		///           <setting name="Mode" value="Live"/>
		///      </section>
		///   </configuration>
		/// Example:
		///	if (Loki.INI2XML.Convert( txtIniFileName.Text, txtXMLFileName.Text ))
		///		Console.WriteLine( "Successfully converted \"" + txtIniFileName.Text + "\" to \"" + txtXMLFileName.Text + "\"" );
		///	else
		///		Console.WriteLine( "Problem converting \"" + txtIniFileName.Text + "\" to \"" + txtXMLFileName.Text + "\"" );
		/// If an exception is raised, it is passed on to the caller.
		/// </summary>
		/// <param name="strINIFileName">File name of the INI file to convert</param>
		/// <param name="strXMLFileName">File name of the XML file that is created</param>
		/// <returns>True if successfuly, or False if a problem</returns>
		public static bool ConvertB(string strINIFileName, ref string strXMLFileName)
		{
			char[] charEquals = { '=' };
			string lpSections;
			int nSize;
			int nMaxSize;
			string strSection;
			int intSection;
			int intNameValue;
			string strName;
			string strValue;
			string strNameValue;
			string lpNameValues;
			byte[] str = new byte[1];
			XmlWriter xw = null;

			bool could_not_write = false;
			bool ok = false;
			xmlfilename_uncertainty = strXMLFileName;
			try
			{
				if (strXMLFileName.Length == 0)
				{
					strXMLFileName = Path.Combine(Path.GetDirectoryName(strINIFileName), String.Format("{0}.xml", Path.GetFileNameWithoutExtension(strINIFileName)));
				}

				// Get all sections names
				// Making sure allocate enough space for data returned
				// Thanks to Peter for his fix to this loop.
				// Peter G Jones, Microsoft .Net MVP, University of Canterbury, NZ
				nMaxSize = INITIAL_BUFFER_SIZE;
				while (true)
				{
					str = new byte[nMaxSize];
					nSize = WIN32Wrapper.GetPrivateProfileSectionNames(str, nMaxSize, strINIFileName);
					if ((nSize != 0) && (nSize == (nMaxSize - 2)))
					{
						nMaxSize *= 2;
					}
					else
					{
						break;
					}
				}

				// convert the byte array into a .NET string
				lpSections = Encoding.ASCII.GetString(str);

				// Use this for Unicode
				// lpSections = Encoding.Unicode.GetString( str );

				// Create XML File
				//xw = XmlTextWriter.Create(strXMLFileName);
				try
				{
					xw = new XmlTextWriter(strXMLFileName, Encoding.UTF8);
				}
				catch (System.IO.IOException)
				{

					strXMLFileName = @"G:\Shared drives\MSL - Length\Length\Technical Procedures\XML Files\config_uncertainty_federal" + System.DateTime.Now.Ticks.ToString() + ".xml";
					xmlfilename_uncertainty = strXMLFileName;
					//if the file is in use write it under another name
					try
					{
						xw = new XmlTextWriter(strXMLFileName, Encoding.UTF8);
					}
					catch (System.IO.DirectoryNotFoundException)
					{
						MessageBox.Show("Cannot Find .ini file");
						Application.Exit();
						conversion_succeeded = false;
						return false;

					}
					could_not_write = true;
				}
				// Write the opening xml
				xw.WriteStartDocument();
				xw.WriteStartElement("U95_Configuration");   //open1


				// Loop through each section in the .ini file
				char[] charNull = { '\0' };
				for (intSection = 0,
					strSection = GetToken(lpSections, charNull, intSection);
					strSection.Length > 0;
					strSection = GetToken(lpSections, charNull, ++intSection))
				{

					// Get all values in this section, making sure to allocate enough space
					for (nMaxSize = INITIAL_BUFFER_SIZE,
						nSize = nMaxSize;
						nSize != 0 && nSize >= (nMaxSize - 2);
						nMaxSize *= 2)
					{
						str = new Byte[nMaxSize];
						nSize = WIN32Wrapper.GetPrivateProfileSection(strSection, str, nMaxSize, strINIFileName);
					}

					// convert the byte array into a .NET string
					lpNameValues = Encoding.ASCII.GetString(str);


					// Use this for Unicode
					// lpNameValues = Encoding.Unicode.GetString( str );

					// Loop through each Name/Value pair
					xw.WriteStartElement(strSection);              //Open2
					for (intNameValue = 0,
						strNameValue = GetToken(lpNameValues, charNull, intNameValue);
						strNameValue.Length > 0;
						strNameValue = GetToken(lpNameValues, charNull, ++intNameValue))
					{
						// Get the name and value from the entire null separated string of name/value pairs
						// Also escape out the special characters, (ie. &"<> )
						strName = GetToken(strNameValue, charEquals, 0);
						if (strNameValue.Length > (strName.Length + 1))
						{
							strValue = strNameValue.Substring(strName.Length + 1);
						}
						else
						{
							strValue = "";
						}
				
							xw.WriteStartElement(strName);        //Open3
							xw.WriteValue(strValue);
							xw.WriteEndElement();                 //Close3
					}
					//close the strsection node.
					xw.WriteEndElement();                     //close2
				}


				// That's it
				xw.WriteEndElement();                         //close1
				xw.WriteEndDocument();
				ok = true;
			}
			finally
			{
				if (xw != null)
				{
					xw.Close();
				}
			}

			if (could_not_write)
			{
				return !ok;
			}

			return ok;
		} // ConvertA


		/// <summary>
		/// Loads the xml file associated with equipment in the equipment register
		/// <summary>
		public static void LoadXML_A()
		{
			//create a new xml reader setting object incase we need to change settings on the fly
			settings = new XmlReaderSettings();

			//create a new xml doc
			xmlreader = new XmlTextReader(xmlfilename);

		}

		/// <summary>
		/// Loads the xml file located on the G: drive
		/// <summary>
		public static void LoadXML_B()
		{
			//create a new xml reader setting object incase we need to change settings on the fly
			settings = new XmlReaderSettings();

			//create a new xml doc
			xmlreader = new XmlTextReader(xmlfilename_uncertainty);

		}

		/// <summary>
		/// Populates the reference gauge block combo box
		/// </summary>
		public static void PopulateReferenceGaugeComboBox(ref ComboBox reference_gauges,bool metric)
		{

			//set the reader to point at the start of the file
			LoadXML_A();

			xmlreader.ResetState();
			//read the first node
			xmlreader.ReadStartElement();

			while (!xmlreader.EOF)
			{
				while (xmlreader.Name.Contains("GAUGE"))
				{
					xmlreader.Read();
					while (xmlreader.LocalName.Contains("gauge"))
					{
						
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						string unit = xmlreader.ReadElementString(); //get the units of the reference gauge set 
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.Skip();

						if (unit.Equals("METRIC") && metric)
						{
							string gauge_block_set = xmlreader.LocalName;
							gauge_block_set = gauge_block_set.Remove(0, 5);          //remove the gauge prefix off the start (makes viewing in the menu nicer)
							reference_gauges.Items.Add(gauge_block_set);
						}
						else if (unit.Equals("IMPERIAL") && !metric)
                        {
							string gauge_block_set = xmlreader.LocalName;
							gauge_block_set = gauge_block_set.Remove(0, 5);          //remove the gauge prefix off the start (makes viewing in the menu nicer)
							reference_gauges.Items.Add(gauge_block_set);
						}
						xmlreader.Skip();
					}
				}
				xmlreader.Skip();
			}
		}


		/// <summary>
		/// Get the metadata for a reference gauge set
		/// </summary>
		public static void GetReferenceSetMetaData(ref GaugeBlockSet gauge_set)
		{

			//set the reader to point at the start of the file
			LoadXML_A();

			xmlreader.ResetState();
			//read the first node
			xmlreader.ReadStartElement();

			while (!xmlreader.EOF)
			{
				while (xmlreader.Name.Contains("GAUGE"))
				{
					xmlreader.Read();
					while (xmlreader.LocalName.Contains("gauge"))
					{
						short unit_short = 0;
						string gauge_block_set = xmlreader.LocalName;
						gauge_block_set = gauge_block_set.Remove(0, 5);
                        
						string report_number = xmlreader.ReadElementString();
						string report_date = xmlreader.ReadElementString();
						string equip_reg_id = xmlreader.ReadElementString();
						string unit = xmlreader.ReadElementString(); //get the units of the reference gauge set.
						string material = xmlreader.ReadElementString();
						string expcoeff = xmlreader.ReadElementString();
						string youngs_modulus = xmlreader.ReadElementString();
						string poissons_ratio = xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						bool valid = false;
						double expcoeff_ = 9.5;
						double youngs_modulus_ = 200;
						double poissons_ratio_ = 0.23;
						try
						{
							expcoeff_ = System.Convert.ToDouble(expcoeff);
							youngs_modulus_ = System.Convert.ToDouble(youngs_modulus);
							poissons_ratio_ = System.Convert.ToDouble(poissons_ratio);
							valid = true;
						}
                        catch(FormatException)
                        {
							MessageBox.Show("Invalid expansion coefficient or youngs modulus or poissons ratio.. value should be numberic - check ini file");
                        }

						if (gauge_block_set.Equals(gauge_set.GaugeSetName))
						{
							gauge_set.GaugeSetName = gauge_block_set;
							gauge_set.ReportNumber = report_number;
							gauge_set.ReportDate = report_date;
							gauge_set.EquipID = equip_reg_id;

							if (valid)
							{
								Material mtrl = new Material();
								mtrl.exp_coeff= expcoeff_;
								mtrl.youngs_modulus = youngs_modulus_;
								mtrl.poissons_ratio = poissons_ratio_;
								mtrl.material = material;
								gauge_set.GaugeSetMaterial = mtrl; 
							}
							

							switch (unit)
							{
								case "METRIC":
									unit_short = Units.Metric;
									break;
								case "IMPERIAL":
									unit_short = Units.Imperial;
									break;
							}
							gauge_set.Unit = unit_short;
						}
						xmlreader.ReadElementString();
						xmlreader.Skip();
						xmlreader.Skip();
					}
				}
				xmlreader.Skip();
			}
		}

		/// <summary>
		/// Get the metadata for a reference gauge set
		/// </summary>
		public static bool ReferenceSetIsMetric(string name)
		{

			//set the reader to point at the start of the file
			LoadXML_A();

			xmlreader.ResetState();
			//read the first node
			xmlreader.ReadStartElement();

			while (!xmlreader.EOF)
			{
				while (xmlreader.Name.Contains("GAUGE"))
				{
					xmlreader.Read();
					while (xmlreader.LocalName.Contains("gauge"))
					{
						
						string gauge_block_set = xmlreader.LocalName;
						gauge_block_set = gauge_block_set.Remove(0, 5);

						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						string unit = xmlreader.ReadElementString(); //get the units of the reference gauge set.
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						
						if (gauge_block_set.Equals(name))
						{
							switch (unit)
							{
								case "METRIC":
									return true;

								case "IMPERIAL":
									return false;
									
							}
						}
						xmlreader.ReadElementString();
						xmlreader.Skip();
						xmlreader.Skip();
					}
				}
				xmlreader.Skip();
			}
			return true;
		}

		public static void LoadReferenceGauges(ref GaugeBlockSet gauge_set)
        {
			//set the reader to point at the start of the file
			LoadXML_A();

			xmlreader.ResetState();
			//read the first node
			xmlreader.ReadStartElement();

			while (!xmlreader.EOF)
			{
				while (xmlreader.Name.Contains("GAUGE"))
				{
					xmlreader.Read();
					while (xmlreader.LocalName.Contains("gauge"))
					{ 
						
						string gauge_block_set = xmlreader.LocalName;
						gauge_block_set = gauge_block_set.Remove(0, 5);

						string report_number = xmlreader.ReadElementString();
						string report_date = xmlreader.ReadElementString();
						string equip_reg_id = xmlreader.ReadElementString();
						string unit = xmlreader.ReadElementString(); //get the units of the reference gauge set
						string material = xmlreader.ReadElementString();
						string expcoeff = xmlreader.ReadElementString();
						string ym = xmlreader.ReadElementString();
						string pr = xmlreader.ReadElementString();
						string std_u_dep = xmlreader.ReadElementString();
						string std_u_indep = xmlreader.ReadElementString(); 
						string std_u_wringing_indep = xmlreader.ReadElementString();
						xmlreader.ReadElementString();
						string gauge_data_nom;
						string gauge_data_val;
						xmlreader.Read();
						int i = 0;


						while (xmlreader.LocalName.Contains("Gauge"))
						{
							if (i == 0) xmlreader.MoveToFirstAttribute();
							else xmlreader.MoveToNextAttribute();

							gauge_data_nom = xmlreader.GetAttribute("Nominal");
						    gauge_data_val = xmlreader.GetAttribute("Deviation");

							if (gauge_block_set.Equals(gauge_set.GaugeSetName))
							{
								//gauge_set.GaugeSetName = gauge_block_set;
								//gauge_set.ReportNumber = report_number;
								//gauge_set.ReportDate = report_date;
								//gauge_set.EquipID = equip_reg_id;

								GaugeBlock gauge = new GaugeBlock(true);
								if(gauge_set.Unit== Units.Metric)
                                {
									gauge.Metric = true;
                                }
								else if (gauge_set.Unit == Units.Imperial)
								{
									gauge.Metric = false;
								}

								try
								{
									gauge.Nominal = System.Convert.ToDouble(gauge_data_nom);
							    }
                                catch (FormatException)
                                {
									MessageBox.Show("invalid gauge block nominal, check .ini file");
                                }

								int index_of_comma = 0;
								index_of_comma = gauge_data_val.IndexOf(',');
                                try
                                {
									string deviation = gauge_data_val.Substring(index_of_comma +1, gauge_data_val.Length - (index_of_comma+1));
									gauge.CentreDeviation = System.Convert.ToDouble(deviation);
                                }
								catch(FormatException)
                                {
									MessageBox.Show("invalid gauge block deviation, check .ini file");
								}
								string serial = gauge_data_val.Remove(index_of_comma);
								gauge.SerialNumber = serial;
								gauge.FromSet = gauge_block_set;
								double ym_d = 0.0;
								double pr_d = 0.0;
								double exp_c_d = 0.0;
								if (double.TryParse(ym, out ym_d)) gauge.GaugeBlockMaterial.youngs_modulus = ym_d;
								else MessageBox.Show("Youngs modulus for the reference gauge could not be read. Check cal_data.ini configuration file-the value should be numeric");
								if (double.TryParse(pr, out pr_d)) gauge.GaugeBlockMaterial.poissons_ratio = pr_d;
								else MessageBox.Show("Youngs modulus for the reference gauge could not be read. Check cal_data.ini configuration file-the value should be numeric");
								if (double.TryParse(expcoeff, out exp_c_d)) gauge.GaugeBlockMaterial.exp_coeff = exp_c_d;
								else MessageBox.Show("Youngs modulus for the reference gauge could not be read. Check cal_data.ini configuration file-the value should be numeric");

								gauge.GaugeBlockMaterial.material = material;

								double wrng = 0.0;
								double rgsui = 0.0;
								double rgsud = 0.0;

								if (double.TryParse(std_u_dep, out rgsud)) gauge.GaugeStdU_Dep = rgsud;
								else MessageBox.Show("Could not read the length dependent standard uncertainty component for the for the reference gauge. Check cal_data.ini configuration file-the value should be numeric");

								if (double.TryParse(std_u_indep, out rgsui)) gauge.GaugeStdU_Indp = rgsui;
								else MessageBox.Show("Could not read the length independent standard uncertainty component for the for the reference gauge. Check cal_data.ini configuration file-the value should be numeric");

								if (double.TryParse(std_u_wringing_indep, out wrng)) gauge.WringingFilm = wrng;
								else MessageBox.Show("Could not read the wringing standard uncertainty component for the for the reference gauge. Check cal_data.ini configuration file-the value should be numeric");

								if (unit.Equals("METRIC")) gauge.Metric = true;
								else  gauge.Metric = false; 
								gauge_set.AddGauge(gauge);
							}

							xmlreader.Skip();
							i++;
						}


				        xmlreader.Skip();
						xmlreader.Skip();
					}
				}
				xmlreader.Skip();
			}
		}
		public static void LoadVerticalFederalMetaData(ref VerticalFederal vfed)
		{
			//set the reader to point at the start of the file
			LoadXML_A();

			xmlreader.ResetState();
			//read the first node
			xmlreader.ReadStartElement();

			while (!xmlreader.EOF)
			{
				while (xmlreader.Name.Contains("COMPARATOR"))
				{
					xmlreader.Read();
					while (xmlreader.LocalName.Contains("verticalfederal"))
					{

						//string gauge_block_set = xmlreader.LocalName;
						//gauge_block_set = gauge_block_set.Remove(0, 5);

						string report_number = xmlreader.ReadElementString();
						string report_date = xmlreader.ReadElementString();
						string equip_reg_id = xmlreader.ReadElementString();
						string equip_type = xmlreader.ReadElementString(); //get the units of the reference gauge set
						string top_probe_f = xmlreader.ReadElementString();
						string bottom_probe_f = xmlreader.ReadElementString();
						string probe_dia = xmlreader.ReadElementString();
						string probe_youngs_mod = xmlreader.ReadElementString();
						string probe_poissons_ratio = xmlreader.ReadElementString();

						double tpf = 0.0;
						double bpf = 0.0;
						double dia = 0.0;
						double pym = 0.0;
						double ppr = 0.0;

						if (double.TryParse(top_probe_f, out tpf)) vfed.TopProbeForce = tpf;
						if (double.TryParse(bottom_probe_f, out bpf)) vfed.BottomProbeForce = bpf;
						if (double.TryParse(probe_dia, out dia)) vfed.ProbeDiameter = dia;
						if (double.TryParse(probe_youngs_mod, out pym)) vfed.ProbeYoungsMod = pym;
						if (double.TryParse(probe_poissons_ratio, out ppr)) vfed.ProbePoissonsRatio = ppr;

						xmlreader.Skip();
					}
				}
				xmlreader.Skip();
			}
		}

		public static void LoadUncertaintyMetaData(ref VerticalFederal vfed)
		{
			//set the reader to point at the start of the file
			LoadXML_B();

			xmlreader.ResetState();
			//read the first node
			xmlreader.ReadStartElement();

			while (!xmlreader.EOF)
			{
				while (xmlreader.Name.Contains("verticalFederal"))
				{
				
					while (xmlreader.LocalName.Contains("verticalFederal"))
					{

						//string gauge_block_set = xmlreader.LocalName;
						//gauge_block_set = gauge_block_set.Remove(0, 5);

						string technical_procedure = xmlreader.ReadElementString();
						string procedure_expiry = xmlreader.ReadElementString();
						string reproducibility_std_u_independent = xmlreader.ReadElementString();
						string scale_resolution_std_u_independent = xmlreader.ReadElementString(); //get the units of the reference gauge set
						string scale_calibration_std_u_independent = xmlreader.ReadElementString();
						string delta = xmlreader.ReadElementString();
						string alpha_g = xmlreader.ReadElementString();
						string delta_alpha = xmlreader.ReadElementString();
						string theta_s = xmlreader.ReadElementString();
						string delta_theta = xmlreader.ReadElementString();
						string delta_theta_var_ = xmlreader.ReadElementString();
						string expanded_uncertainty_cmc_deviation_independent = xmlreader.ReadElementString();
						string expanded_uncertainty_cmc_deviation_dependent = xmlreader.ReadElementString();
						string expanded_uncertainty_cmc_variation_independent = xmlreader.ReadElementString();
						string expanded_uncertainty_cmc_variation_dependent = xmlreader.ReadElementString();

						double r = 0.0;
						double sc = 0.0;
						double sr = 0.0;
						vfed.TechnicalProcedure = technical_procedure;
						vfed.TechnicalProcedureExpiry = procedure_expiry;
						if (double.TryParse(reproducibility_std_u_independent, out r)) vfed.ReproducibilityStduIndependent = r;
						if (double.TryParse(scale_resolution_std_u_independent, out sr)) vfed.ScaleResStduIndependent = sr;
						if (double.TryParse(scale_calibration_std_u_independent, out sc)) vfed.ScaleCalStduIndependent = sc;

						double delta_ = 0.0;
						double alpha_g_ = 0.0;
						double delta_alpha_ = 0.0;
						double theta_s_ = 0.0;
						double delta_theta_ = 0.0;
						double delta_theta_var = 0.0;
						double exp_u_dev_indep = 0.0;
						double exp_u_dev_dep = 0.0;
						double exp_u_var_indep = 0.0;
						double exp_u_var_dep = 0.0;

						if (double.TryParse(delta, out delta_)) vfed.u_Delta = delta_;
						if (double.TryParse(alpha_g, out alpha_g_)) vfed.u_AlphaG = alpha_g_;
						if (double.TryParse(delta_alpha, out delta_alpha_)) vfed.u_DeltaAlpha = delta_alpha_;
						if (double.TryParse(theta_s, out theta_s_)) vfed.u_ThetaS = theta_s_;
						if (double.TryParse(delta_theta, out delta_theta_)) vfed.u_DeltaTheta = delta_theta_;
						if (double.TryParse(delta_theta_var_, out delta_theta_var)) vfed.u_DeltaThetaVar = delta_theta_var;
						if (double.TryParse(expanded_uncertainty_cmc_deviation_independent, out exp_u_dev_indep)) vfed.ExpanedUncertaintyCMCDevIndep = exp_u_dev_indep;
						if (double.TryParse(expanded_uncertainty_cmc_deviation_dependent, out exp_u_dev_dep)) vfed.ExpanedUncertaintyCMCDevDep = exp_u_dev_dep;
						if (double.TryParse(expanded_uncertainty_cmc_variation_independent, out exp_u_var_indep)) vfed.ExpanedUncertaintyCMCVarIndep = exp_u_var_indep;
						if (double.TryParse(expanded_uncertainty_cmc_variation_dependent, out exp_u_var_dep)) vfed.ExpanedUncertaintyCMCVarDep = exp_u_var_dep;
						xmlreader.Skip();
					}
				}
				xmlreader.Skip();
			}
		}
	} // class INI2XML
} // namespace Loki
