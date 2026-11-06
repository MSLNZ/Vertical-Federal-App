using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Vertical_Federal_App
{
    public class InventoryItem
    {
        public string? Keywords { get; set; }
        public string? Id { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? Serial { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
        public string? Loggable { get; set; }
        public string? Traceable { get; set; }

    }

    public class CalibrationRecord
    {
        public string? EquipmentId { get; set; }
        public string? Quantity { get; set; }
        public int CalibrationIntervalYears { get; set; }
        public string? ComponentName { get; set; }
        public string? ReportId { get; set; }
        public DateTime? ReportIssueDate { get; set; }
        public DateTime? MeasurementStartDate { get; set; }
        public DateTime? MeasurementStopDate { get; set; }

        public CalibrationType CalibrationType { get; set; }
        
        public string? EquationUnit { get; set; }

        public string? Equation { get; set; }

    }


    public enum CalibrationType
    {
        Equation,
        Table,
        File,
        Unknown   // optional but very useful fallback
    }


    public class TableData
    {
        public string[]? Type { get; set; }
        public string[]? Unit { get; set; }
        public string[]? Header { get; set; }

        public object[,]? Data { get; set; }
    }


    public class GaugeBlockSetSpecification
    {
        public string? Grade { get; set; }

        public bool? Metric { get; set; }
        public string? Material { get; set; }

        public double? YoungsModulus { get; set; }
        public double? PoissonsRatio { get; set; }

        public double? ExpansionCoefficient { get; set; }
        public double? ExpansionCoefficientUncertainty { get; set; }

        public double? WringingUncertainty { get; set; }
        public double? LengthIndependentUncertainty { get; set; }
        public double? LengthDependentUncertainty { get; set; }
    }

    public class GaugeBlockComparatorSpecifications
    {
        public double? ForceTopAnvil { get; set; }
        public string? ForceTopAnvilUnit { get; set; }

        public double? ForceBottomAnvil { get; set; }
        public string? ForceBottomAnvilUnit { get; set; }

        public double? ProbeDiameter { get; set; }
        public string? ProbeDiameterUnit { get; set; }

        public double? ProbeYoungsModulus { get; set; }
        public string? ProbeYoungsModulusUnit { get; set; }

        public double? ProbePoissonsRatio { get; set; }
        public string? ProbePoissonsRatioUnit { get; set; }

        public double? ScaleResolution { get; set; }
        public string? ScaleResolutionUnit { get; set; }

        public double? ScaleCalibration { get; set; }
        public string? ScaleCalibrationUnit { get; set; }

        public double? Reproducibility { get; set; }
        public string? ReproducibilityUnit { get; set; }

        // Uncertainty terms
        public double? StduDelta { get; set; }
        public string? StduDeltaUnit { get; set; }

        public double? StduAlphaG { get; set; }
        public string? StduAlphaGUnit { get; set; }

        public double? StduDeltaAlpha { get; set; }
        public string? StduDeltaAlphaUnit { get; set; }

        public double? StduThetaS { get; set; }
        public string? StduThetaSUnit { get; set; }

        public double? StduDeltaTheta { get; set; }
        public string? StduDeltaThetaUnit { get; set; }

        public double? StduDeltaThetaVariation { get; set; }
        public string? StduDeltaThetaVariationUnit { get; set; }

        public double? ExpandedUncertaintyDeviationDependentCmc { get; set; }
        public string? ExpandedUncertaintyDeviationDependentCmcUnit { get; set; }

        public double? ExpandedUncertaintyDeviationIndependentCmc { get; set; }
        public string? ExpandedUncertaintyDeviationIndependentCmcUnit { get; set; }

        public double? ExpandedUncertaintyVariationDependentCmc { get; set; }
        public string? ExpandedUncertaintyVariationDependentCmcUnit { get; set; }

        public double? ExpandedUncertaintyVariationIndependentCmc { get; set; }
        public string? ExpandedUncertaintyVariationIndependentCmcUnit { get; set; }
    }

    public class EquipmentRegister
    {
        public required XDocument doc;
        public required XNamespace msl;
        public required XElement register;

       
        public EquipmentRegister()
        {
            
        }


        /// <summary>
        /// General Inventory of Specified Equipment
        /// </summary>
        /// <param name="search_key">A keyword to find specific Equipment</param>
        /// <summary>
        /// Performs a keyword-based inventory search.
        ///
        /// The search_key may contain one or more keywords (space-separated).
        /// 
        /// Behaviour:
        ///   - "Gauge" → matches all equipment containing "Gauge"
        ///   - "Gauge Block" → matches only equipment containing BOTH "Gauge" AND "Block"
        ///   - Case-insensitive matching
        /// </summary>
        public List<InventoryItem> WildCardInventory(string search_key)
        {
            XElement register = doc.Root
                ?? throw new InvalidOperationException("Invalid XML document");

            // ✅ Split search string into tokens
            string[] searchTokens = (search_key ?? "")
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();

            var items =
                from e in register.Elements(msl + "equipment")

                // ✅ Split equipment keywords
                let keywords = ((string?)e.Attribute("keywords") ?? "")
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)

                // ✅ Match ALL search tokens against equipment keywords
                where searchTokens.All(token =>
                    keywords.Any(k =>
                        string.Equals(k, token, StringComparison.OrdinalIgnoreCase)))

                select new InventoryItem
                {
                    Keywords = string.Join(" ", keywords),
                    Id = Convert.ToString(e.Element(msl + "id")),
                    Manufacturer = Convert.ToString(e.Element(msl + "manufacturer")),
                    Model = Convert.ToString(e.Element(msl + "model")),
                    Serial = Convert.ToString(e.Element(msl + "serial")),
                    Description = Convert.ToString(e.Element(msl + "description")),
                    Location = Convert.ToString(e.Element(msl + "location")),
                    Status = Convert.ToString(e.Element(msl + "status")),
                    Loggable = Convert.ToString(e.Element(msl + "loggable")),
                    Traceable = Convert.ToString(e.Element(msl + "traceable"))
                };

            return items.ToList();
        }

        /// <summary>
        /// Retrieves the latest calibration record for a given equipment ID
        /// and component name.
        ///
        /// "Latest" is defined as the calibration report (or digital report)
        /// with the most recent measurementStartDate.
        ///
        /// The function determines the calibration type using the following rules:
        ///   - Equation: if an <equation> element exists
        ///   - Table: if a <table> element exists
        ///   - File: if it is a <digitalReport> (or associated file)
        ///   - Unknown: if none of the above applies
        ///
        /// Parameters:
        ///   equipmentId    - The equipment ID (case-insensitive)
        ///   componentName  - The component name (case-insensitive)
        ///
        /// Returns:
        ///   A populated CalibrationRecord, or null if no calibration exists.
        /// </summary>
        public CalibrationRecord GetLatestCalibration(
            string equipmentId,
            string componentName)
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
                throw new ArgumentException("equipmentId must not be empty.");

            if (string.IsNullOrWhiteSpace(componentName))
                throw new ArgumentException("componentName must not be empty.");

            XNamespace msl = "https://measurement.govt.nz/equipment-register";

            // ✅ Locate the equipment element
            XElement equipment =
                doc.Root
                    .Elements(msl + "equipment")
                    .FirstOrDefault(e =>
                        string.Equals(
                            (string)e.Element(msl + "id"),
                            equipmentId,
                            StringComparison.OrdinalIgnoreCase));

            if (equipment == null)
                throw new InvalidOperationException(
                    "Equipment '" + equipmentId + "' not found.");

            // ✅ Find all reports for the specified component and select the latest
            var latest =
                (from measurand in equipment
                     .Element(msl + "calibrations")?
                     .Elements(msl + "measurand")
                 ?? Enumerable.Empty<XElement>()

                 from component in measurand.Elements(msl + "component")

                     // Match component name (case-insensitive)
                 where string.Equals(
                     (string)component.Attribute("name"),
                     componentName,
                     StringComparison.OrdinalIgnoreCase)

                 from report in component.Elements()
                     .Where(r =>
                         r.Name == msl + "report" ||
                         r.Name == msl + "digitalReport")

                 let startDate =
                     (DateTime?)report.Element(msl + "measurementStartDate")

                 // Only consider valid calibrations
                 where startDate.HasValue

                 select new
                 {
                     Measurand = measurand,
                     Component = component,
                     Report = report,
                     StartDate = startDate.Value
                 })
                .OrderByDescending(x => x.StartDate)
                .FirstOrDefault();

            if (latest == null)
                return null;

            XElement reportElement = latest.Report;

            bool isReport = reportElement.Name == msl + "report";
            bool isDigitalReport = reportElement.Name == msl + "digitalReport";

            // ✅ Determine calibration type
            CalibrationType calibrationType = CalibrationType.Unknown;

            if (reportElement.Element(msl + "equation") != null)
            {
                calibrationType = CalibrationType.Equation;
            }
            else if (reportElement.Element(msl + "table") != null)
            {
                calibrationType = CalibrationType.Table;
            }
            else if (isDigitalReport || reportElement.Element(msl + "file") != null)
            {
                calibrationType = CalibrationType.File;
            }

            // ✅ Extract equation if present
            XElement equationElement = reportElement.Element(msl + "equation");

            string equationValue = null;
            string equationUnit = null;

            if (equationElement != null)
            {
                equationValue =
                    (string)equationElement.Element(msl + "value");

                equationUnit =
                    (string)equationElement.Element(msl + "unit");
            }

            // ✅ Build and return the calibration record
            CalibrationRecord result = new CalibrationRecord
            {
                EquipmentId = equipmentId,

                Quantity =
                    (string)latest.Measurand.Attribute("quantity"),

                CalibrationIntervalYears =
                    (int)latest.Measurand.Attribute("calibrationInterval"),

                ComponentName =
                    (string)latest.Component.Attribute("name"),

                ReportId =
                    (string)reportElement.Attribute("id"),

                // Dates only exist for <report>
                ReportIssueDate =
                    isReport
                        ? (DateTime?)reportElement.Element(msl + "reportIssueDate")
                        : null,

                MeasurementStartDate =
                    isReport
                        ? (DateTime?)reportElement.Element(msl + "measurementStartDate")
                        : null,

                MeasurementStopDate =
                    isReport
                        ? (DateTime?)reportElement.Element(msl + "measurementStopDate")
                        : null,

                CalibrationType = calibrationType,

                Equation = equationValue,
                EquationUnit = equationUnit
            };

            return result;
        }

        /// <summary>
        /// Retrieves the table data from the latest calibration report
        /// for a given equipment ID and component name.
        ///
        /// "Latest" is defined as the report with the most recent measurementStartDate.
        ///
        /// Returns null if no table exists.
        /// </summary>

        public TableData GetLatestTypedTable(string equipmentId, string componentName)
        {
            XElement equipment = doc.Root?
                .Elements(msl + "equipment")
                .FirstOrDefault(e => string.Equals(
                    (string)e.Element(msl + "id"),
                    equipmentId,
                    StringComparison.OrdinalIgnoreCase));

            if (equipment == null)
                throw new InvalidOperationException("Equipment not found.");

            // ✅ Find latest report
            var latest =
                (from measurand in equipment
                     .Element(msl + "calibrations")?
                     .Elements(msl + "measurand")
                 ?? Enumerable.Empty<XElement>()

                 from component in measurand.Elements(msl + "component")

                 where string.Equals(
                     (string)component.Attribute("name"),
                     componentName,
                     StringComparison.OrdinalIgnoreCase)

                 from report in component.Elements(msl + "report")

                 let startDate =
                     (DateTime?)report.Element(msl + "measurementStartDate")

                 where startDate.HasValue

                 orderby startDate descending

                 select report)
                .FirstOrDefault();

            if (latest == null)
                return null;

            XElement table = latest.Element(msl + "table");
            if (table == null)
                return null;

            // ✅ Parse metadata
            string[] type = ParseCsvLine((string)table.Element(msl + "type"));
            string[] unit = ParseCsvLine((string)table.Element(msl + "unit"));
            string[] header = ParseCsvLine((string)table.Element(msl + "header"));

            string rawData = (string)table.Element(msl + "data");

            if (string.IsNullOrWhiteSpace(rawData))
                return null;

            string[] rows = rawData
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            int rowCount = rows.Length;
            int colCount = type.Length;

            object[,] data = new object[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
            {
                string[] cols = ParseCsvLine(rows[i]);

                for (int j = 0; j < colCount && j < cols.Length; j++)
                {
                    data[i, j] = ConvertToType(cols[j], type[j]);
                }
            }

            return new TableData
            {
                Type = type,
                Unit = unit,
                Header = header,
                Data = data
            };
        }


        /// <summary>
        /// Converts a string value to the specified type from &lt;type&gt;.
        /// </summary>
        private object ConvertToType(string value, string type)
        {
            type = type.ToLowerInvariant();

            switch (type)
            {
                case "double":
                    double d;
                    if (double.TryParse(value, out d))
                        return d;
                    break;

                case "int":
                    int i;
                    if (int.TryParse(value, out i))
                        return i;
                    break;

                case "bool":
                    bool b;
                    if (bool.TryParse(value, out b))
                        return b;

                    // handle 0/1
                    if (value == "0") return false;
                    if (value == "1") return true;
                    break;

                case "string":
                    return value;
            }

            // ✅ If parsing fails, fall back safely
            return value;
        }


        /// <summary>
        /// Splits a CSV line into trimmed values.
        /// </summary>
        private string[] ParseCsvLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return new string[0];

            return line
                .Split(',')
                .Select(s => s.Trim())
                .ToArray();
        }

        /// <summary>
        /// Retrieves the Gauge Block Set specifications for a given equipment ID.
        ///
        /// According to the schema, the <specifications> element is of type msl:any,
        /// meaning it may contain arbitrary child elements with no enforced structure.
        /// 
        /// This function extracts known Gauge Block Set fields directly from the
        /// specifications element.
        ///
        /// Returns null if no specifications are found.
        /// </summary>
        public GaugeBlockSetSpecification GetGaugeBlockSetSpecifications(string equipmentId)
        {
            // ✅ Locate equipment
            XElement equipment = doc.Root?
                .Elements(msl + "equipment")
                .FirstOrDefault(e => string.Equals(
                    (string)e.Element(msl + "id"),
                    equipmentId,
                    StringComparison.OrdinalIgnoreCase));

            if (equipment == null)
                throw new InvalidOperationException(
                    "Equipment '" + equipmentId + "' not found.");

            // ✅ Access specifications (msl:any)
            XElement specs = equipment.Element(msl + "specifications");

            if (specs == null || !specs.HasElements)
                return null;

            // ✅ Extract Gauge Block fields safely
            GaugeBlockSetSpecification result = new GaugeBlockSetSpecification
            {
                Grade = GetString(specs, "grade"),
                Metric = GetBool(specs, "metric"),
                Material = GetString(specs, "material"),
                YoungsModulus = GetDouble(specs, "youngsmodulus"),
                PoissonsRatio = GetDouble(specs, "poissonsratio"),
                ExpansionCoefficient = GetDouble(specs, "expansioncoefficient"),
                ExpansionCoefficientUncertainty = GetDouble(specs, "expansioncoefficient_uncertainty"),
                WringingUncertainty = GetDouble(specs, "stdu_wringing_uncertainty"),
                LengthDependentUncertainty = GetDouble(specs, "stdu_dependent"),
                LengthIndependentUncertainty = GetDouble(specs, "stdu_independent")
            };

            return result;
        }
        
        /// <summary>
        /// Retrieves the Gauge Block Comparator specifications for a given equipment ID.
        /// </summary>
        public GaugeBlockComparatorSpecifications GetGaugeBlockComparatorSpecifications(string equipmentId)
        {
            XElement equipment = doc.Root?
                .Elements(msl + "equipment")
                .FirstOrDefault(e => string.Equals(
                    (string)e.Element(msl + "id"),
                    equipmentId,
                    StringComparison.OrdinalIgnoreCase));

            if (equipment == null)
                throw new InvalidOperationException(
                    "Equipment '" + equipmentId + "' not found.");

            XElement specs = equipment.Element(msl + "specifications");

            if (specs == null)
                return null;

            return new GaugeBlockComparatorSpecifications
            {
                ForceTopAnvil = GetDouble(specs, "forceTopAnvil"),
                ForceTopAnvilUnit = GetUnit(specs, "forceTopAnvil"),

                ForceBottomAnvil = GetDouble(specs, "forceBottomAnvil"),
                ForceBottomAnvilUnit = GetUnit(specs, "forceBottomAnvil"),

                ProbeDiameter = GetDouble(specs, "probeDiameter"),
                ProbeDiameterUnit = GetUnit(specs, "probeDiameter"),

                ProbeYoungsModulus = GetDouble(specs, "probeYoungsModulus"),
                ProbeYoungsModulusUnit = GetUnit(specs, "probeYoungsModulus"),

                ProbePoissonsRatio = GetDouble(specs, "probePoissonsRatio"),
                ProbePoissonsRatioUnit = GetUnit(specs, "probePoissonsRatio"),

                ScaleResolution = GetDouble(specs, "scaleResolution"),
                ScaleResolutionUnit = GetUnit(specs, "scaleResolution"),

                ScaleCalibration = GetDouble(specs, "scaleCalibration"),
                ScaleCalibrationUnit = GetUnit(specs, "scaleCalibration"),

                Reproducibility = GetDouble(specs, "reproducibility"),
                ReproducibilityUnit = GetUnit(specs, "reproducibility"),

                StduDelta = GetDouble(specs, "stdu_delta"),
                StduDeltaUnit = GetUnit(specs, "stdu_delta"),

                StduAlphaG = GetDouble(specs, "stdu_alpha_g"),
                StduAlphaGUnit = GetUnit(specs, "stdu_alpha_g"),

                StduDeltaAlpha = GetDouble(specs, "stdu_delta_alpha"),
                StduDeltaAlphaUnit = GetUnit(specs, "stdu_delta_alpha"),

                StduThetaS = GetDouble(specs, "stdu_theta_s"),
                StduThetaSUnit = GetUnit(specs, "stdu_theta_s"),

                StduDeltaTheta = GetDouble(specs, "stdu_delta_theta"),
                StduDeltaThetaUnit = GetUnit(specs, "stdu_delta_theta"),

                StduDeltaThetaVariation = GetDouble(specs, "stdu_delta_theta_variation"),
                StduDeltaThetaVariationUnit = GetUnit(specs, "stdu_delta_theta_variation"),

                ExpandedUncertaintyDeviationDependentCmc =
                    GetDouble(specs, "expanded_uncertainty_deviation_dependent_cmc"),
                ExpandedUncertaintyDeviationDependentCmcUnit =
                    GetUnit(specs, "expanded_uncertainty_deviation_dependent_cmc"),

                ExpandedUncertaintyDeviationIndependentCmc =
                    GetDouble(specs, "expanded_uncertainty_deviation_independent_cmc"),
                ExpandedUncertaintyDeviationIndependentCmcUnit =
                    GetUnit(specs, "expanded_uncertainty_deviation_independent_cmc"),

                ExpandedUncertaintyVariationDependentCmc =
                    GetDouble(specs, "expanded_uncertainty_variation_dependent_cmc"),
                ExpandedUncertaintyVariationDependentCmcUnit =
                    GetUnit(specs, "expanded_uncertainty_variation_dependent_cmc"),

                ExpandedUncertaintyVariationIndependentCmc =
                    GetDouble(specs, "expanded_uncertainty_variation_independent_cmc"),
                ExpandedUncertaintyVariationIndependentCmcUnit =
                    GetUnit(specs, "expanded_uncertainty_variation_independent_cmc")
            };
        }

        private bool? GetBool(XElement parent, string name)
        {
            
            XElement? el = parent.Element(msl + name);
            if (el == null)
                return null;

            bool value;
            return bool.TryParse(el.Value, out value) ? value : (bool?)null;
        }

        private string? GetString(XElement parent, string name)
        {
            XElement? el = parent.Element(msl + name);
            return el != null ? el.Value : null;
        }

        private double? GetDouble(XElement parent, string name)
        {
            XElement? el = parent.Element(msl + name);

            if (el == null)
                return null;

            double value;
            return double.TryParse(el.Value, out value) ? value : (double?)null;
        }


        private string GetUnit(XElement parent, string name)
        {
            return (string)parent.Element(msl + name)?.Attribute("unit");
        }


        /// <summary>
		/// Populates the reference gauge block combo box
		/// </summary>
		public void PopulateReferenceGaugeComboBox(ref ComboBox reference_gauges, bool metric)
        {
         
            List<InventoryItem> items = WildCardInventory("Gauge Block Set");

            
            foreach(InventoryItem item in items)
            {
                //get specifications for this set
                GaugeBlockSetSpecification spec = GetGaugeBlockSetSpecifications(item.Id);
                if (spec.Metric == metric)
                {
                    reference_gauges.Items.Add(item.Serial);
                }
                if (!spec.Metric == metric)
                {
                    reference_gauges.Items.Add(item.Serial);
                }
            }

            
        }

        public bool LoadReferenceGauges(ref GaugeBlockSet gauge_set)
        {
          

            //get all the equipment register items that are a gauge block set
            List<InventoryItem> items = WildCardInventory("Gauge Block Set");
            bool set_found = false;

            foreach (InventoryItem item in items)
            {
                //if the gauge block set name matches the name of the reference gauge set we're trying to populate then populate it with the gauges from the xml file.
                if (item.Serial == gauge_set.GaugeSetName)
                {
                    set_found = true;

                    //get the metadata we need from the XML equipment register file
                    CalibrationRecord record = GetLatestCalibration(item.Id, "Gauge Block Set");
                    if (record == null) return false;

                    TableData table = GetLatestTypedTable(item.Id, "Gauge Block Set");

                    //check for non-standard table colomns
                    int num_columns = table.Unit.Length;
                    if (num_columns != 5) return false;

                    GaugeBlockSetSpecification gauge_spec_metadata = GetGaugeBlockSetSpecifications(item.Id);

                    gauge_set.ReportNumber = record.ReportId;
                    gauge_set.ReportDate = record.ReportIssueDate.ToString();
                    gauge_set.EquipID = record.EquipmentId;


                    if (table.Unit[0] == "mm") gauge_set.Unit = Units.Metric;
                    else gauge_set.Unit = Units.Imperial;

                    gauge_set.GaugeSetMaterial.material = gauge_spec_metadata.Material;
                    double? expcoeff = gauge_spec_metadata.ExpansionCoefficient;
                    double? ym = gauge_spec_metadata.YoungsModulus;
                    double? pr = gauge_spec_metadata.PoissonsRatio;
                    double? std_u_wringing_indep = gauge_spec_metadata.WringingUncertainty;
                    double? std_u_dependent = gauge_spec_metadata.LengthDependentUncertainty;
                    double? std_u_independent = gauge_spec_metadata.LengthIndependentUncertainty;

                    int gauge_num = 0;
                    //loop through each row in the table and create a gauge block for each nominal size.
                    for (int row = 0; row < (table.Data.GetLength(0) - 1); row++)
                    {
                        GaugeBlock gauge = new GaugeBlock(true);
                        if (gauge_set.Unit == Units.Metric)
                        {
                            gauge.Metric = true;
                        }
                        else if (gauge_set.Unit == Units.Imperial)
                        {
                            gauge.Metric = false;
                        }
                        gauge.Nominal = (double)table.Data[row, 0];
                        gauge.SerialNumber = (string)table.Data[row, 1];
                        gauge.CentreDeviation = (double)table.Data[row, 2];
                        gauge.Dof = (int)table.Data[row, 4];

                        double std_uncertainty = (double)table.Data[row, 3];
                        gauge.ExpandedUncertaintyDev = std_uncertainty * MathNet.Numerics.ExcelFunctions.TInv(0.05, gauge.Dof);
                        gauge.FromSet = gauge_set.GaugeSetName;


                        if (ym != null) gauge.GaugeBlockMaterial.youngs_modulus = (double)ym;
                        if (pr != null) gauge.GaugeBlockMaterial.poissons_ratio = (double)pr;
                        if (expcoeff != null) gauge.GaugeBlockMaterial.exp_coeff = (double)expcoeff;
                        if (std_u_wringing_indep != null) gauge.WringingFilm = (double)std_u_wringing_indep;
                        if (std_u_independent != null) gauge.GaugeStdU_Indp = (double)std_u_independent;
                        if (std_u_dependent != null) gauge.GaugeStdU_Dep = (double)std_u_dependent;

                        gauge_set.NumGauges++;
                        //add the gauge
                        gauge_set.AddGauge(gauge);

                    }
                }

            }
            return set_found;
        }

        public void LoadVerticalFederalMetaData(ref VerticalFederal vfed)
        {
            
            GaugeBlockComparatorSpecifications s = GetGaugeBlockComparatorSpecifications("MSLE.L.004");

            if (s.ForceTopAnvil != null) vfed.TopProbeForce = (double)s.ForceTopAnvil;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.ForceBottomAnvil != null) vfed.BottomProbeForce = (double) s.ForceBottomAnvil;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if(s.ProbeDiameter != null) vfed.ProbeDiameter = (double)s.ProbeDiameter;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.ProbeYoungsModulus != null) vfed.ProbeYoungsMod = (double)s.ProbeYoungsModulus;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.ProbePoissonsRatio != null) vfed.ProbePoissonsRatio = (double)s.ProbePoissonsRatio;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.ScaleResolution != null) vfed.ScaleResStduIndependent = (double)s.ScaleResolution / Math.Sqrt(12);
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.ScaleCalibration != null) vfed.ScaleCalStduIndependent = (double)s.ScaleCalibration;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.Reproducibility != null) vfed.ReproducibilityStduIndependent = (double)s.Reproducibility;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.StduDelta != null) vfed.u_Delta = (double)s.StduDelta;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.StduAlphaG != null) vfed.u_AlphaG = (double)s.StduAlphaG;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.StduDeltaAlpha != null) vfed.u_DeltaAlpha = (double)s.StduDeltaAlpha;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.StduThetaS != null) vfed.u_ThetaS = (double)s.StduThetaS;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.StduDeltaAlpha != null) vfed.u_DeltaTheta = (double)s.StduDeltaAlpha;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.StduDeltaThetaVariation != null) vfed.u_DeltaThetaVar = (double)s.StduDeltaThetaVariation;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.ExpandedUncertaintyDeviationDependentCmc != null) vfed.ExpanedUncertaintyCMCDevDep = (double)s.ExpandedUncertaintyDeviationDependentCmc;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.ExpandedUncertaintyDeviationIndependentCmc != null) vfed.ExpanedUncertaintyCMCDevIndep = (double)s.ExpandedUncertaintyDeviationIndependentCmc;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.ExpandedUncertaintyVariationDependentCmc != null) vfed.ExpanedUncertaintyCMCVarDep = (double)s.ExpandedUncertaintyVariationDependentCmc;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }
            if (s.ExpandedUncertaintyVariationIndependentCmc != null) vfed.ExpanedUncertaintyCMCVarIndep = (double)s.ExpandedUncertaintyVariationIndependentCmc;
            else { MessageBox.Show("Unable to read Federal MetaData"); return; }

            //TO DO
            //Once we have a scale calibration in the equipment register this should be added to the measurement

        }

    }
}
