using System;
using System.IO;
using System.Security.Permissions;
using System.Xml;
using log4net;
using System.Xml.Serialization;

namespace OPPF.Utilities
{
	/// <summary>
	/// Summary description for OPPFConfigXML.
	/// </summary>
	public class OPPFConfigXML : IDisposable
	{

        private static readonly string CONFIG_FILE_PATH = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //private static readonly string CONFIG_FILE_NAME = "OPPFImagingConfig.xml";
        private static readonly string CONFIG_FILE_NAME = "OPPFImagerLinkConfig.xml";
        private static readonly string CONFIG_FILE_PATH_NAME = Path.Combine(CONFIG_FILE_PATH, CONFIG_FILE_NAME);

        private static readonly string DEFAULT_WEB_DIRECTORY = "C:\\images\\%date%\\%plateid%\\";
        private static readonly string DEFAULT_ARCHIVE_DIRECTORY = "C:\\OPPFImagerLink\\Archive\\%date%\\%plateid%\\";
        private static readonly string DEFAULT_XML_DIRECTORY = "C:\\OPPFImagerLink\\XML\\%date%\\%plateid%\\";
        private static readonly string DEFAULT_DONE_DIRECTORY = "C:\\Temp\\RockImager\\";
        private static readonly string DEFAULT_WSPLATE_ENDPOINT = "http://localhost:8080/xtalpims-ws/services/WSPlate.WSPlateSOAP11port_http/";
        private static readonly string DEFAULT_IMAGE_FORMAT = "jpg";
        private static readonly string DEFAULT_LOGGER_CONFIG = Path.Combine(CONFIG_FILE_PATH, "OPPFlogger.xml");
        private static readonly string DEFAULT_IMAGE_BASE_URL = "http://localhost/images/";
        private static readonly string DEFAULT_DB_CONNECTION_STRING = "";
        private static readonly string DEFAULT_DB_QUERY_STRING = "select s.SCHEDULEDTIME AT TIME ZONE 'UTC' as dttoimage, s.COMPLETIONTIME AT TIME ZONE 'UTC' as dtimaged, s.PRIORITY as intpriority, s.STATE as intstate, s1.DETAILS as details from SCHE_SCHEDULEDTASK s inner join CORE_LABBOOKENTRY s1 on s.LabBookEntryID=s1.DBID inner join HOLD_ABSTRACTHOLDER ah on s.HOLDERID=ah.LABBOOKENTRYID where ah.NAME=? order by s.SCHEDULEDTIME";
        private static readonly string DEFAULT_LIGHTPATH_ENDPOINT = "http://cs04r-sc-vserv-49/imaging/inspection?iid=";
        
        /// <summary>
        /// Singleton instance of OPPFConfigXML
        /// </summary>
        private static OPPFConfigXML instance = new OPPFConfigXML();

        /// <summary>
        /// Do all the necessary configuration. This method should
        /// be called from all entry-point classes (the *Provider
        /// classes).
        /// </summary>
        public static void Configure()
        {
            // Do nothing - just ensure this class is loaded!
        }

        /// <summary>
        /// Get the image format
        /// </summary>
        /// <returns>The image format</returns>
        public static string GetImageFormat()
        {
            return instance.ImageFormat;
        }

        /// <summary>
        /// Get the directory to which the images will be saved.
        /// </summary>
        /// <returns>The directory</returns>
        public static string GetWebDirectory()
        {
            return instance.WebDirectory;
        }

        /// <summary>
        /// Get the directory to which the images will be saved.
        /// </summary>
        /// <returns>The directory</returns>
        public static string GetArchiveDirectory()
        {
            return instance.ArchiveDirectory;
        }

        /// <summary>
        /// Get the directory to which the images will be saved.
        /// </summary>
        /// <returns>The directory</returns>
        public static string GetXmlDirectory()
        {
            return instance.XmlDirectory;
        }

        /// <summary>
        /// Get the directory to which RockImager is writing the done files.
        /// </summary>
        /// <returns>The directory</returns>
        public static string GetDoneDirectory()
        {
            return instance.DoneDirectory;
        }

        /// <summary>
        /// Get the WSPlate Endpoint
        /// </summary>
        /// <returns>The endpoint</returns>
        public static string GetWsPlateEndpoint()
        {
            return instance.WsPlateEndpoint;
        }

        /// <summary>
        /// Get the WSPlate Endpoint
        /// </summary>
        /// <returns>The endpoint</returns>
        public static string GetUsername()
        {
            return instance.Username;
        }

        /// <summary>
        /// Get the WSPlate Endpoint
        /// </summary>
        /// <returns>The endpoint</returns>
        public static string GetPassword()
        {
            return instance.Password;
        }

        /// <summary>
        /// Get the base url for the images
        /// </summary>
        /// <returns>The base url for the images</returns>
        public static string GetImageBaseUrl()
        {
            return instance.ImageBaseUrl;
        }

        /// <summary>
        /// Get the db connection string
        /// </summary>
        /// <returns>The db connection string</returns>
        public static string GetDbConnectionString()
        {
            return instance.DbConnectionString;
        }

        /// <summary>
        /// Get the db query string
        /// </summary>
        /// <returns>The db query string</returns>
        public static string GetDbQueryString()
        {
            return instance.DbQueryString;
        }

        /// <summary>
        /// Get the lightpath endpoint string
        /// </summary>
        /// <returns>The lightpath endpoint</returns>
        public static string GetLightpathEndpoint()
        {
            return instance.LightPathEndpoint;
        }

        /// <summary>
        /// Track whether dispose has been called
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Object to use a lock for thread-safe code
        /// </summary>
        private object lockObject = new object();

        /// <summary>
        /// The image format
        /// </summary>
        private string _imageFormat;

        /// <summary>
        /// The archive image directory
        /// </summary>
        private string _archiveDirectory;

        /// <summary>
        /// The web image directory
        /// </summary>
        private string _webDirectory;

        /// <summary>
        /// The xml directory
        /// </summary>
        private string _xmlDirectory;

        /// <summary>
        /// The done directory
        /// </summary>
        private string _doneDirectory;

        /// <summary>
        /// The WSPlate endpoint
        /// </summary>
        private string _wsPlateEndpoint;

        /// <summary>
        /// The WSPlate username
        /// </summary>
        private string _username;

        /// <summary>
        /// The WSPlate password
        /// </summary>
        private string _password;

        /// <summary>
        /// The logger config
        /// </summary>
        private string _loggerConfig;

        /// <summary>
        /// The image base url
        /// </summary>
        private string _imageBaseUrl;

        /// <summary>
        /// The image base url
        /// </summary>
        private string _dbConnectionString;

        /// <summary>
        /// The image base url
        /// </summary>
        private string _dbQueryString;

        /// <summary>
        /// The lightpath endpoint
        /// </summary>
        private string _lightpathEndpoint;

        /// <summary>
        /// FileSystemWatcher to watch the config file
        /// </summary>
        private FileSystemWatcher watcher = null;

        /// <summary>
        /// Private constructor - nobody else should be able to instantiate this
        /// </summary>
        private OPPFConfigXML()
        {
            this.ImageFormat = DEFAULT_IMAGE_FORMAT;
            this.WebDirectory = DEFAULT_WEB_DIRECTORY;
            this.ArchiveDirectory = DEFAULT_ARCHIVE_DIRECTORY;
            this.XmlDirectory = DEFAULT_XML_DIRECTORY;
            this.DoneDirectory = DEFAULT_DONE_DIRECTORY;
            this.WsPlateEndpoint = DEFAULT_WSPLATE_ENDPOINT;
            this.Username = "";
            this.Password = "";
            this.ImageBaseUrl = DEFAULT_IMAGE_BASE_URL;
            //this.LoggerConfig = DEFAULT_LOGGER_CONFIG;
            this.DbConnectionString = DEFAULT_DB_CONNECTION_STRING;
            this.DbQueryString = DEFAULT_DB_QUERY_STRING;
            this.LightPathEndpoint = DEFAULT_LIGHTPATH_ENDPOINT;

            Environment.SetEnvironmentVariable("__OPPF_IMAGERLINK_EXTDIR", CONFIG_FILE_PATH);
            Environment.SetEnvironmentVariable("__OPPF_IMAGERLINK_LOGDIR", Path.Combine(CONFIG_FILE_PATH, Path.Combine("..", "LogFiles")));

            ReadConfig();
            WatchConfigFile();
        }

        /// <summary>
        /// The format in which the images will be saved. Valid values include
        /// jpg and tif
        /// </summary>
        string ImageFormat
        {
            get
            {
                return _imageFormat;
            }

            set
            {
                if ((null == value) || ("".Equals(value.Trim())))
                {
                    _imageFormat = DEFAULT_IMAGE_FORMAT;
                }
                else
                {
                    _imageFormat = value.Trim();
                }
            }
        }

        /// <summary>
        /// The directory to which the images will be written for archive. Path may include
        /// write-time parameters.
        /// Example: "c:\\images\\%date%\\%plateid%\\"
        /// </summary>
        string ArchiveDirectory
        {
            get
            {
                return _archiveDirectory;
            }

            set
            {
                if ((null == value) || ("".Equals(value.Trim())))
                {
                    _archiveDirectory = DEFAULT_ARCHIVE_DIRECTORY;
                }
                else
                {
                    _archiveDirectory = value.Trim();
                }
            }
        }

        /// <summary>
        /// The directory to which the images will be written for the web site. Path may include
        /// write-time parameters.
        /// Example: "c:\\images\\%date%\\%plateid%\\"
        /// </summary>
        string WebDirectory
        {
            get
            {
                return _webDirectory;
            }

            set
            {
                if ((null == value) || ("".Equals(value.Trim())))
                {
                    _webDirectory = DEFAULT_WEB_DIRECTORY;
                }
                else
                {
                    _webDirectory = value.Trim();
                }
            }
        }

        /// <summary>
        /// The directory to which the xml files for the asynchronous update of the db
        /// will be written. Path may include write-time parameters.
        /// Example: "c:\\images\\%date%\\%plateid%\\"
        /// </summary>
        string XmlDirectory
        {
            get
            {
                return _xmlDirectory;
            }

            set
            {
                if ((null == value) || ("".Equals(value.Trim())))
                {
                    _xmlDirectory = DEFAULT_XML_DIRECTORY;
                }
                else
                {
                    _xmlDirectory = value.Trim();
                }
            }
        }

        /// <summary>
        /// The directory to which the done files are written by RockImager.
        /// </summary>
        string DoneDirectory
        {
            get
            {
                return _doneDirectory;
            }

            set
            {
                if ((null == value) || ("".Equals(value.Trim())))
                {
                    _doneDirectory = DEFAULT_DONE_DIRECTORY;
                }
                else
                {
                    _doneDirectory = value.Trim();
                }
            }
        }

        /// <summary>
        /// The directory to which the done files are written by RockImager.
        /// </summary>
        string WsPlateEndpoint
        {
            get
            {
                return _wsPlateEndpoint;
            }

            set
            {
                if ((null == value) || ("".Equals(value.Trim())))
                {
                    _wsPlateEndpoint = DEFAULT_DONE_DIRECTORY;
                }
                else
                {
                    _wsPlateEndpoint = value.Trim();
                }
            }
        }

        /// <summary>
        /// The WSPlate username
        /// </summary>
        string Username
        {
            get
            {
                return _username;
            }

            set
            {
                if ((null == value) || ("".Equals(value.Trim())))
                {
                    _username = "";
                }
                else
                {
                    _username = value.Trim();
                }
            }
        }

        /// <summary>
        /// The WSPlate password
        /// </summary>
        string Password
        {
            get
            {
                return _password;
            }

            set
            {
                if ((null == value) || ("".Equals(value.Trim())))
                {
                    _password = "";
                }
                else
                {
                    _password = value.Trim();
                }
            }
        }

        /// <summary>
        /// The log4net config file
        /// </summary>
        string LoggerConfig
        {
            get
            {
                return _loggerConfig;
            }

            set
            {
                string _newLoggerConfig;
                if ((null == value) || ("".Equals(value.Trim())))
                {
                    _newLoggerConfig = DEFAULT_LOGGER_CONFIG;
                }
                else
                {
                    _newLoggerConfig = Environment.ExpandEnvironmentVariables(value.Trim());
                }

                // If the config file has changed and we can find it
                if (!_newLoggerConfig.Equals(_loggerConfig))
                {
                    if (File.Exists(_newLoggerConfig))
                    {
                        log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(_newLoggerConfig));
                        _loggerConfig = _newLoggerConfig;
                    }

                    // Else use our fallback
                    else
                    {
                        Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".OPPF.Utilities.TraceLog4NetConfig.xml");
                        log4net.Config.XmlConfigurator.Configure(stream);
                        stream.Close();
                        ILog log = LogManager.GetLogger(this.GetType());
                        log.Error("log4net config file " + _newLoggerConfig + " not found. Failing down to logging to System.Diagnostics.Trace");
                        _loggerConfig = null;
                    }
                }
            }
        }

        /// <summary>
        /// The base URL for the images
        /// </summary>
        string ImageBaseUrl
        {
            get
            {
                return _imageBaseUrl;
            }

            set
            {
                if ((null == value) || ("".Equals(value.Trim())))
                {
                    _imageBaseUrl = "";
                }
                else
                {
                    _imageBaseUrl = value.Trim();
                    if (!_imageBaseUrl.EndsWith("/"))
                    {
                        _imageBaseUrl = _imageBaseUrl + "/";
                    }
                }
            }
        }

        /// <summary>
        /// The db connection string
        /// </summary>
        string DbConnectionString
        {
            get
            {
                return _dbConnectionString;
            }

            set
            {
                if ((null == value) || ("".Equals(value.Trim())))
                {
                    _dbConnectionString = "";
                }
                else
                {
                    _dbConnectionString = value.Trim();
                }
            }
        }

        /// <summary>
        /// The db query string
        /// </summary>
        string DbQueryString
        {
            get
            {
                return _dbQueryString;
            }

            set
            {
                if ((null == value) || ("".Equals(value.Trim())) || value.ToLower().Contains("delete") || value.ToLower().Contains("drop") || value.ToLower().Contains(";"))
                {
                    _dbQueryString = "";
                }
                else
                {
                    _dbQueryString = value.Trim();
                }
            }
        }

        /// <summary>
        /// The db query string
        /// </summary>
        string LightPathEndpoint
        {
            get
            {
                return _lightpathEndpoint;
            }

            set
            {
                if ((null == value) || ("".Equals(value.Trim())))
                {
                    _lightpathEndpoint = "";
                }
                else
                {
                    _lightpathEndpoint = value.Trim();
                }
            }
        }

        /// <summary>
        /// Read the config file
        /// </summary>
        private void ReadConfig()
        {

            // Thread-safe access to this block
            lock (lockObject)
            {

                try
                {
                    StreamReader reader = new StreamReader(CONFIG_FILE_PATH_NAME);
                    XmlSerializer serializer = new XmlSerializer(typeof(OPPF.XML.Config));
                    OPPF.XML.Config _config = (OPPF.XML.Config)serializer.Deserialize(reader);
                    reader.Close();

                    this.ArchiveDirectory = _config.ArchiveDir;
                    this.WebDirectory = _config.WebDir;
                    this.XmlDirectory = _config.XmlDir;
                    this.DoneDirectory = _config.DoneDir;
                    this.WsPlateEndpoint = _config.WsPlateEndpoint;
                    this.Username = _config.Username;
                    this.Password = _config.Password;
                    this.LoggerConfig = _config.LoggerConfig;
                    this.ImageBaseUrl = _config.ImageBaseUrl;
                    this.DbConnectionString = _config.DbConnectionString;
                    this.DbQueryString = _config.DbQueryString;
                    this.LightPathEndpoint = _config.LightPathEndpoint;

                    // Log the new configuration
                    ILog log = LogManager.GetLogger(this.GetType());
                    log.Info("Read Configuration: " + this.ToString());

                    //throw new System.Exception(CONFIG_FILE_PATH_NAME + "=>" + this.ToString());

                }
                catch (Exception e)
                {
                    // System.Diagnostics.Trace.TraceError("Failed to read config file: " + e.Message);
                    throw new System.Exception("Failed to read config file: " + e.Message, e);
                }

            }
            // End thread-safe block

        }

        /// <summary>
        /// Watch the config file for changes
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void WatchConfigFile()
        {

            // Thread-safe block
            lock (lockObject)
            {

                if (null != watcher)
                {

                    // Create a new FileSystemWatcher
                    watcher = new FileSystemWatcher();

                    // Watch the folder containing the dll
                    watcher.Path = CONFIG_FILE_PATH;

                    // Only watch the config file
                    watcher.Filter = CONFIG_FILE_NAME;

                    // Watch for changes in LastWrite time
                    watcher.NotifyFilter = NotifyFilters.LastWrite;

                    // Add event handlers.
                    watcher.Changed += new FileSystemEventHandler(OnConfigFileChanged);
                    watcher.Created += new FileSystemEventHandler(OnConfigFileChanged);

                    // Begin watching.
                    watcher.EnableRaisingEvents = true;

                }

            }
            // End thread-safe block

        }

        /// <summary>
        /// Handle change notifications for the config file
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnConfigFileChanged(object source, FileSystemEventArgs e)
        {
            this.ReadConfig();
        }

        public string ToString()
        {
            return ("[OPPFConfigXML: ImageFormat=\"" + this.ImageFormat + 
                "\"; ArchiveDirectory=\"" + this.ArchiveDirectory + 
                "\"; WebDirectory=\"" + this.WebDirectory +
                "\"; XmlDirectory=\"" + this.XmlDirectory +
                "\"; DoneDirectory=\"" + this.DoneDirectory +
                "\"; WsPlateEndpoint=\"" + this.WsPlateEndpoint +
                "\"; Username=\"" + this.Username +
                "\"; Password=\"" + "********" +
                "\"; LoggerConfig=\"" + this.LoggerConfig +
                "\"; ImageBaseUrl=\"" + this.ImageBaseUrl +
                "\"; LightPathEndpoint=\"" + this.LightPathEndpoint +
                "\"]");
        }

        # region IDisposable implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // dispose managed resources
                    if (null != watcher)
                    {
                        watcher.Dispose();
                        watcher = null;
                    }
                }

                // free native resources
                lockObject = null;
                _imageFormat = null;
                _archiveDirectory = null;
                _webDirectory = null;
                _xmlDirectory = null;
                _doneDirectory = null;
                _wsPlateEndpoint = null;
                _username = null;
                _password = null;
                _loggerConfig = null;
                _imageBaseUrl = null;
                _lightpathEndpoint = null;

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        # endregion

        # region Finalizer

        /// <summary>
        /// This destructor will run only if the Dispose method does not get called.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~OPPFConfigXML()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }
        
        # endregion
    }

}
