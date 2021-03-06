//// Copyright 2014 ESRI
//// 
//// All rights reserved under the copyright laws of the United States
//// and applicable international laws, treaties, and conventions.
//// 
//// You may freely redistribute and use this sample code, with or
//// without modification, provided you include the original copyright
//// notice and use restrictions.
//// 
//// See the use restrictions at <your ArcGIS install location>/DeveloperKit10.3/userestrictions.txt.
//// 

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using System.Collections.Specialized;

//using System.Runtime.InteropServices;

//using ESRI.ArcGIS.esriSystem;
//using ESRI.ArcGIS.Server;
//using ESRI.ArcGIS.Geometry;
//using ESRI.ArcGIS.Geodatabase;
//using ESRI.ArcGIS.Carto;
//using ESRI.ArcGIS.SOESupport;
//using System.IO;
//using ESRI.ArcGIS.DataSourcesFile;

//using ESRI.ArcGIS.AnalysisTools;
//using ESRI.ArcGIS.Geoprocessor;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace NetSimpleRESTSOE
//{
//    [ComVisible(true)]
//    [Guid("592d9b60-bb6f-49f9-9429-e9c720bca615")]
//    [ClassInterface(ClassInterfaceType.None)]
//    [ServerObjectExtension("MapServer",
//        AllCapabilities = "",
//        DefaultCapabilities = "",
//        Description = "Convert Cad to GIS Data .Net",
//        DisplayName = "Convert Cad to GIS Data SOE",
//        Properties = "",
//        SupportsREST = true,
//        SupportsSOAP = false)]
//    public class CadToGIS : IServerObjectExtension, IRESTRequestHandler
//    {
//        private string soeName;
//        private IServerObjectHelper soHelper;
//        private ServerLogger serverLog;
//        private IRESTRequestHandler _reqHandler;
//        private IMapServerDataAccess mapServerDataAccess;
//        private IMapLayerInfos layerInfos;

//        private string localFilePath = string.Empty;
//        private string virtualFilePath = string.Empty;
//        private string environmentUrl = @"F:\apps\CadFile\Data\Madinah_27-12-2020";

//        public CadToGIS()
//        {
//            soeName = this.GetType().Name;
//            _reqHandler = new SoeRestImpl(soeName, CreateRestSchema()) as IRESTRequestHandler;
//        }

//        public void Init(IServerObjectHelper pSOH)
//        {
//            this.soHelper = pSOH;
//            //string _outputDirectory = "C:\\arcgisserver\\directories\\arcgisoutput"; 
//            this.serverLog = new ServerLogger();
//            this.mapServerDataAccess = (IMapServerDataAccess)this.soHelper.ServerObject;
//            IMapServer3 ms = (IMapServer3)this.mapServerDataAccess;
//            IMapServerInfo mapServerInfo = ms.GetServerInfo(ms.DefaultMapName);
//            this.layerInfos = mapServerInfo.MapLayerInfos;

//            serverLog.LogMessage(ServerLogger.msgType.infoStandard, this.soeName + ".init()", 200, "Initialized " + this.soeName + " SOE.");

//            localFilePath = @"C:\arcgisserver\directories\arcgissystem\arcgisuploads\services\cadToGIsFinal.GPServer\";
//            virtualFilePath = pSOH.ServerObject.ConfigurationName + "_" + pSOH.ServerObject.TypeName;
//        }

//        public void Shutdown()
//        {
//            serverLog.LogMessage(ServerLogger.msgType.infoStandard, this.soeName + ".init()", 200, "Shutting down " + this.soeName + " SOE.");
//            this.soHelper = null;
//            this.serverLog = null;
//            this.mapServerDataAccess = null;
//            this.layerInfos = null;
//        }

//        private RestResource CreateRestSchema()
//        {
//            RestResource soeResource = new RestResource(soeName, false, RootResHandler);

//            RestOperation ConvertCadToGISOp = new RestOperation("ConvertCadToGIS",
//                                                     new string[] { "inputFile" },
//                                                      new string[] { "json" },
//                                                      ConvertCadToGIS);

//            soeResource.operations.Add(ConvertCadToGISOp);

//            return soeResource;
//        }

//        public string GetSchema()
//        {
//            return _reqHandler.GetSchema();
//        }
//        byte[] IRESTRequestHandler.HandleRESTRequest(string Capabilities,
//            string resourceName,
//            string operationName,
//            string operationInput,
//            string outputFormat,
//            string requestProperties,
//            out string responseProperties)
//        {
//            return _reqHandler.HandleRESTRequest(Capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out responseProperties);
//        }

//        private byte[] RootResHandler(System.Collections.Specialized.NameValueCollection boundVariables,
//            string outputFormat,
//            string requestProperties,
//            out string responseProperties)
//        {
//            responseProperties = null;

//            JSONObject json = new JSONObject();
//            json.AddString("name", "Convert Cad to GIS Data ");
//            json.AddString("description", "Convert Cad to GIS Data with arcobject for .Net");
//            return Encoding.UTF8.GetBytes(json.ToJSONString(null));
//        }

//        private byte[] ConvertCadToGIS(NameValueCollection boundVariables,
//                                                  JsonObject operationInput,
//                                                      string outputFormat,
//                                                      string requestProperties,
//                                                  out string responseProperties)
//        {
//            responseProperties = "";

//            System.Diagnostics.Debugger.Launch();   //for dubuging

//            string inputFile = string.Empty;

//            bool found = operationInput.TryGetString("inputFile", out inputFile);

//            string file = localFilePath + inputFile + "\\" + "parcle.dwg";

//            long fileSize = new System.IO.FileInfo(file).Length;

//            // Initialize the Geoprocessor
//            Geoprocessor GP = new Geoprocessor();

//            // Set workspace environment
//            GP.SetEnvironmentValue("workspace", environmentUrl);
//            // Initialize the Conversion Tool
//            ESRI.ArcGIS.ConversionTools.FeaturesToJSON FJson = new ESRI.ArcGIS.ConversionTools.FeaturesToJSON();
//            //FJson.in_features = @"F:\apps\CadFile\Data\Madinah_27-12-2020\parcle.dwg\Polygon";
//            FJson.in_features = file + "\\Polygon";

//            //FJson.out_json_file = @"F:\apps\CadFile\Data\Madinah_27-12-2020\myjson.json";
//            Guid g = Guid.NewGuid();
//            FJson.out_json_file = environmentUrl + "\\" + g + ".json";
//            string jdonFileName = environmentUrl + "\\" + g + ".json";

//            FJson.format_json = "FORMATTED";

//            GP.Execute(FJson, null);

//            //read file
//            //JObject o1 = JObject.Parse(File.ReadAllText(@"F:\apps\CadFile\Data\Madinah_27-12-2020\myjson.json"));

//            JObject o1 = JObject.Parse(File.ReadAllText(jdonFileName));

//            // read JSON directly from a file
//            JObject o2;
//            //using (StreamReader jsonFile = File.OpenText(@"F:\apps\CadFile\Data\Madinah_27-12-2020\myjson.json"))
//            using (StreamReader jsonFile = File.OpenText(jdonFileName))
//            using (JsonTextReader reader = new JsonTextReader(jsonFile))
//            {
//                o2 = (JObject)JToken.ReadFrom(reader);
//            }
//            ///
//            if (outputFormat == "json")
//            {
//                responseProperties = "{\"Content-Type\" : \"application/json\"}";

//                JsonObject jsonResult = new JsonObject();
//                jsonResult.AddString("jsonContent", o2.ToString());
//                jsonResult.AddString("fileName", inputFile);
//                jsonResult.AddString("fileSizeBytes", Convert.ToString(fileSize));
//                return Encoding.UTF8.GetBytes(jsonResult.ToJson());

//            }
//            else if (outputFormat == "file")
//            {
//                responseProperties = "{\"Content-Type\" : \"application/octet-stream\",\"Content-Disposition\": \"attachment; filename=" + inputFile + "\"}";
//                return System.IO.File.ReadAllBytes(file);
//            }
//            return Encoding.UTF8.GetBytes("");
//        }

//        private byte[] createErrorObject(int codeNumber, String errorMessageSummary, String[] errorMessageDetails)
//        {
//            if (errorMessageSummary.Length == 0 || errorMessageSummary == null)
//            {
//                throw new Exception("Invalid error message specified.");
//            }

//            JSONObject errorJSON = new JSONObject();
//            errorJSON.AddLong("code", codeNumber);
//            errorJSON.AddString("message", errorMessageSummary);

//            if (errorMessageDetails == null)
//            {
//                errorJSON.AddString("details", "No error details specified.");
//            }
//            else
//            {
//                String errorMessages = "";
//                for (int i = 0; i < errorMessageDetails.Length; i++)
//                {
//                    errorMessages = errorMessages + errorMessageDetails[i] + "\n";
//                }

//                errorJSON.AddString("details", errorMessages);
//            }

//            JSONObject error = new JSONObject();
//            errorJSON.AddJSONObject("error", errorJSON);

//            return Encoding.UTF8.GetBytes(errorJSON.ToJSONString(null));
//        }

//    }
//}