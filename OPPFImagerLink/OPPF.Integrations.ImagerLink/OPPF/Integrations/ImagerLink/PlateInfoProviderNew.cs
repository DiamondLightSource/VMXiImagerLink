using System;
using System.IO;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Serialization;

using OPPF.Proxies;
using OPPF.Utilities;
using OXML = OPPF.XML;
using Formulatrix.Integrations.ImagerLink;
using log4net;
using log4net.Config;

namespace OPPF.Integrations.ImagerLink
{
    /// <summary>
    /// Provides plate information.
    /// </summary>
    /// <remarks>Both RockImager and RockImagerProcessor instantiate and use
    /// IPlateInfoProvider. The function of the class is to provide information
    /// about a plate for the user.
    /// Khalid's comments:
    /// The methods of IPlateInfoProvider are required to display information to the
    /// user about the plate. You should provide as much detail of the plate as
    /// possible. If you don't have a field, let me know and we may be able to
    /// figure out a value you can substitute for the user instead: for example, our
    /// other customer simply returns the barcode for GetPlateID().
    /// </remarks>
    public class PlateInfoProviderNew : IPlateInfoProvider
    {

        /// <summary>
        /// The folder in which to find the PlateTypes xml file
        /// </summary>
        private static readonly string PLATETYPES_FILE_PATH = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// The name of the PlateTypes xml file
        /// </summary>
        private static readonly string PLATETYPES_FILE_NAME = "PlateTypes.xml";

        /// <summary>
        /// The fully specified path and name of the PlateTypes xml file, built
        /// from PLATETYPES_FILE_PATH and PLATETYPES_FILE_NAME
        /// </summary>
        private static readonly string PLATETYPES_FILE_PATH_NAME = Path.Combine(PLATETYPES_FILE_PATH, PLATETYPES_FILE_NAME);

        /// <summary>
        /// 15 min cache lifetime 
        /// </summary>
        private static readonly long _cacheLifetime = 15 * TimeSpan.TicksPerMinute;

        /// <summary>
        /// Cached list of plate types
        /// </summary>
        private static IPlateType[] _plateTypes = null;

        /// <summary>
        /// Expiry date of the cache in ticks
        /// </summary>
        private static long _plateTypesCacheExpires = 0;

        /// <summary>
        /// Lock object for plate types cache
        /// </summary>
        private static readonly System.Object _plateTypesLock = new System.Object();

        /// <summary>
        /// Searches through the list of known PlateTypes (_plateTypes) and returns the
        /// ID of the PlateType with the specified name, or null if no matching PlateType
        /// is found
        /// </summary>
        /// <param name="name">The name of the PlateType for which the ID is to be found</param>
        /// <returns>The ID of the named PlateType, or null if there is no PlateType with that name in the list</returns>
        public static string getIDForName(string name)
        {
            if (null == _plateTypes)
            {
                // Populate it
                IPlateInfoProvider pipn = new PlateInfoProviderNew();
                pipn.GetPlateTypes(new Robot("x","x"));
            }

            for (int i = 0; i < _plateTypes.Length; i++)
            {
                if (_plateTypes[i].Name.Equals(name)) {
                    return _plateTypes[i].ID;
                }
            }
            throw new Exception("Can't map plate type \"" + name + "\" to id");
        }

        /// <summary>
        ///  Logger
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// PlateInfo cache
        /// </summary>
        private readonly PlateInfoCacheNew _plateInfoCache;

        /// <summary>
        /// Zero-arg constructor
        /// </summary>
        public PlateInfoProviderNew()
        {

            // Load configuration
            OPPFConfigXML.Configure();

            // Get Logger
            _log = LogManager.GetLogger(this.GetType());

            // PlateInfoCache
            // TODO: Allow configuration of initialSize and Capacity
            _plateInfoCache = new PlateInfoCacheNew(1000, 1000);

            // Log the call to the constructor
            if (_log.IsDebugEnabled)
            {
                string msg = "Constructed a new " + this;
                _log.Debug(msg);
            }

        }

        /// <summary>
        /// Load the PlateTypes from file. If you want this to re-read the file you must set
        /// the private static IPlateType[] member _plateTypes back to null before calling
        /// this method again.
        /// </summary>
        private void LoadPlateTypes()
        {

            // Double-check locking for thread safety
            if (null == _plateTypes)
            {
                lock (_plateTypesLock)
                {
                    if (null == _plateTypes)
                    {

                        // Attempt to read the list of plate types
                        try
                        {
                            StreamReader reader = new StreamReader(PLATETYPES_FILE_PATH_NAME);
                            XmlSerializer serializer = new XmlSerializer(typeof(OXML.PlateTypes));
                            OXML.PlateTypes oXmlPlateTypes = (OXML.PlateTypes)serializer.Deserialize(reader);
                            reader.Close();

                            IPlateType[] tmpPlateTypes = new PlateType[oXmlPlateTypes.PlateType.Length];
                            int i = 0;
                            foreach (OXML.PlateType oXmlPlateType in oXmlPlateTypes.PlateType)
                            {
                                tmpPlateTypes[i] = new PlateType(oXmlPlateType.ID, oXmlPlateType.Name, oXmlPlateType.NumColumns, oXmlPlateType.NumRows, oXmlPlateType.NumDrops);
                                i++;
                            }
                            _plateTypes = tmpPlateTypes;

                            // Log the new configuration
                            ILog log = LogManager.GetLogger(this.GetType());
                            log.Info("Read PlateTypes: " + this.ToString());

                        }
                        catch (Exception e)
                        {
                            // System.Diagnostics.Trace.TraceError("Failed to read PlateTypes file: " + e.Message);
                            throw new System.Exception("Failed to read PlateTypes file: " + e.Message, e);
                        }

                    }
                }
            }
            // End thread-safe block

        }

        /// <summary>
        /// Save the PlateTypes to file. Likely to have no practical use other than the initial
        /// population of the xml file!
        /// </summary>
        private void SavePlateTypes(string fileName)
        {

            // Double-check locking for thread safety
            if (null != _plateTypes)
            {
                lock (_plateTypesLock)
                {
                    if (null != _plateTypes)
                    {

                        // Attempt to write the list of plate types
                        try
                        {
                            OXML.PlateTypes oXmlPlateTypes = new OXML.PlateTypes();
                            OXML.PlateType[] oXmlPlateType = new OXML.PlateType[_plateTypes.Length];
                            oXmlPlateTypes.PlateType = oXmlPlateType;
                            int i = 0;
                            foreach (PlateType plateType in _plateTypes)
                            {
                                oXmlPlateType[i] = new OXML.PlateType();
                                oXmlPlateType[i].ID = plateType.ID;
                                oXmlPlateType[i].Name = plateType.Name;
                                oXmlPlateType[i].NumColumns = plateType.NumColumns;
                                oXmlPlateType[i].NumRows = plateType.NumRows;
                                oXmlPlateType[i].NumDrops = plateType.NumDrops;
                                i++;
                            }

                            StreamWriter writer = new StreamWriter(fileName);
                            XmlSerializer serializer = new XmlSerializer(typeof(OXML.PlateTypes));
                            serializer.Serialize(writer, oXmlPlateTypes);
                            writer.Close();

                            // Log the action
                            ILog log = LogManager.GetLogger(this.GetType());
                            log.Info("Saved PlateTypes: " + this.ToString());


                        }
                        catch (Exception e)
                        {
                            // System.Diagnostics.Trace.TraceError("Failed to save PlateTypes file: " + e.Message);
                            throw new System.Exception("Failed to save PlateTypes file: " + e.Message, e);
                        }

                    }
                }
            }
            // End thread-safe block

        }



        #region IPlateInfoProvider Members

        /// <summary>
        /// Retrieves a plate id.
        /// </summary>
        /// <param name="robot">The robot to find the plate type for.</param>
        /// <param name="barcode">The <c>barcode</c> label of the plate.</param>
        /// <returns>The unique identifier describing the plate. Actually, the barcode is the unique identifier, so it just returns the barcode</returns>
        public string GetPlateID(IRobot robot, string barcode)
        {

            // OPPF PERFORMANCE BODGE - The barcode is the plateID
            return barcode;

        }

        /// <summary>
        /// Retrieves a plate description.
        /// </summary>
        /// <param name="robot">The robot to find the plate type for.</param>
        /// <param name="plateID">The <c>plateID</c> of the plate.</param>
        /// <returns>The <c>IPlateInfo</c> describing the plate.</returns>
        public IPlateInfo GetPlateInfo(IRobot robot, string plateID)
        {
            return _plateInfoCache.GetPlateInfo(robot, plateID);

            /*
             * Replaced by cache
             * 
            // Check arguments - do it up front to avoid possible inconsistencies later
            if (robot == null) throw new System.NullReferenceException("robot must not be null");
            if (plateID == null) throw new System.NullReferenceException("plateID must not be null");

            // Log the call
            if (_log.IsDebugEnabled)
            {
                string msg = "Called " + this + ".GetPlateInfo(robot=" + robot.ToString() + ", plateID=\"" + plateID + "\")";
                _log.Debug(msg);
            }

            // Special case for ReliabilityTestPlate
            if ("ReliabilityTestPlate".Equals(plateID))
            {
                OPPF.Integrations.ImagerLink.PlateInfo dummy = new OPPF.Integrations.ImagerLink.PlateInfo();
                dummy.DateDispensed = DateTime.Now;
                dummy.ExperimentName = "Dummy Expt Name";
                dummy.PlateNumber = 1;
                dummy.PlateTypeID = "1";
                dummy.ProjectName = "Dummy Project Name";
                dummy.UserEmail = "DummyEmailAddress";
                dummy.UserName = "Dummy UserName";

                return dummy;
            }

            // Declare the return variable
            OPPF.Integrations.ImagerLink.PlateInfo pi = null;

            try
            {
                // Create and populate the request object
                getPlateInfo request = new getPlateInfo();
                request.robot = OPPF.Utilities.RobotUtils.createProxy(robot);
                request.plateID = plateID;

                // Make the web service call
                WSPlate wsPlate = new WSPlate();
                getPlateInfoResponse response = wsPlate.getPlateInfo(request);

                // Get the webservice proxy PlateInfo
                OPPF.Proxies.PlateInfo ppi = response.getPlateInfoReturn;

                // Map it into an IPlateInfo
                pi = new OPPF.Integrations.ImagerLink.PlateInfo();
                pi.DateDispensed = ppi.dateDispensed;
                pi.ExperimentName = ppi.experimentName;
                pi.PlateNumber = ppi.plateNumber;
                pi.PlateTypeID = ppi.plateTypeID;
                pi.ProjectName = ppi.projectName;
                pi.UserEmail = ppi.userEmail;
                pi.UserName = ppi.userName;

            }
            catch (Exception e)
            {
                string msg = "WSPlate.getPlateInfo threw " + e.GetType() + ":\n" + e.Message + "\nfor plate \"" + plateID + "\" in robot \"" + robot.Name + "\"\n - probably not in LIMS - not fatal.";
                msg = msg + WSPlateFactory.SoapExceptionToString(e);

                // Log it
                _log.Error(msg, e);

                // Don't rethrow - return null - don't want to stop imaging
            }

            // Return the IPlateInfo
            return pi;
             */
        }

        /// <summary>
        /// Retrieve a plate type. Rewritten to use the cached list of plate types.
        /// </summary>
        /// <param name="robot">The robot to find the plate type for.</param>
        /// <param name="plateTypeID">The ID of the plate type.</param>
        /// <returns>The plate type with ID of plateTypeID, or null if not found.</returns>
        public IPlateType GetPlateType(IRobot robot, string plateTypeID)
        {

            // Check arguments - do it up front to avoid possible inconsistencies later
            if (robot == null) throw new System.NullReferenceException("robot must not be null");
            if (plateTypeID == null) throw new System.NullReferenceException("plateTypeID must not be null");

            // Log the call
            if (_log.IsDebugEnabled)
            {
                string msg = "Called " + this + ".GetPlateType(robot=" + robot.ToString() + ", plateTypeID=\"" + plateTypeID + "\")";
                _log.Debug(msg);
            }

            IPlateType[] plateTypes = ((IPlateInfoProvider)this).GetPlateTypes(robot);
            if (null != plateTypes)
            {
                for (int i = 0; i < plateTypes.Length; i++)
                {
                    if (plateTypeID.Equals(plateTypes[i].ID))
                    {
                        return plateTypes[i];
                    }
                }

                // Should not get here - log error
                string msg = "Failed to find PlateType with ID: " + plateTypeID + " for robot \"" + robot.Name + "\" from " + plateTypes.Length + " plateTypes - returning null";
                _log.Error(msg);

            }

            else
            {
                // Should not get here - log error
                string msg = "Failed to find PlateType with ID: " + plateTypeID + " for robot \"" + robot.Name + "\" - GetPlateTypes returned null - returning null";
                _log.Error(msg);
            }

            // No better option than to return null
            return null;

        }

        /// <summary>
        /// Retrieve all the plate types. The list of PlateTypes is cached for _cacheLifetime min.
        /// </summary>
        /// <param name="robot">The robot to find the plate types for.</param>
        /// <returns>An array of plate types, or null if there are none.</returns>
        public IPlateType[] GetPlateTypes(IRobot robot)
        {

            // Check arguments - do it up front to avoid possible inconsistencies later
            if (robot == null) throw new System.NullReferenceException("robot must not be null");

            // Log the call
            if (_log.IsDebugEnabled)
            {
                string msg = "Called " + this + ".GetPlateTypes(robot=" + robot.ToString() + ")";
                _log.Debug(msg);
            }

            // Return cached values if appropriate
            if ((_plateTypes != null) && (System.DateTime.Now.Ticks <= _plateTypesCacheExpires))
            {
                _log.Debug("GetPlateTypes() using cached response");
                return _plateTypes;
            }

            _log.Debug("GetPlateTypes() refreshing cache");

            // TODO Use WS for platedb!
            // TODO Figure out how to use WS for pimsdb
            // Was hardcoded list
            // Now list from external xml file

            /*
             * To actually re-read the file
            lock (_plateTypesLock)
            {
                _plateTypes = null;
            }
             */

            LoadPlateTypes();

            // Update cache expiry
            _plateTypesCacheExpires = System.DateTime.Now.Ticks + _cacheLifetime;
            _log.Debug("GetPlateTypes() using fresh response");

            // Return the array of IPlateType[]
            // FIXME Should return a deep copy for safety
            return (IPlateType[])_plateTypes.Clone();

        }

        #endregion

    }

}
