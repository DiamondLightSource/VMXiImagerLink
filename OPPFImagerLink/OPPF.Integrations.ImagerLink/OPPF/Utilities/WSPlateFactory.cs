using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using OPPF.Proxies2;

namespace OPPF.Utilities
{
    public class WSPlateFactory
    {
        private static WSPlate wsPlate2;

        public static WSPlate getWSPlate2()
        {
            if (null == wsPlate2)
            {
                ServicePointManager.Expect100Continue = false;
                wsPlate2 = new WSPlate();
                wsPlate2.Url = OPPFConfigXML.GetWsPlateEndpoint();
                wsPlate2.Credentials = new NetworkCredential(OPPFConfigXML.GetUsername(), OPPFConfigXML.GetPassword());
                wsPlate2.PreAuthenticate = true;
            }
            return wsPlate2;
        }

        /// <summary>
        /// Extract a string describing a SoapException from the specified Exception
        /// </summary>
        /// <param name="e">The Exception to process</param>
        /// <returns>Details.InnerXml or Details.InnerText or an empty string</returns>
        public static string SoapExceptionToString(Exception e)
        {
            string msg = "";
            if (e is System.Web.Services.Protocols.SoapException)
            {
                System.Web.Services.Protocols.SoapException ee = (System.Web.Services.Protocols.SoapException)e;
                if (null != ee.Detail)
                {
                    if (null != ee.Detail.InnerXml)
                    {
                        msg = "\n\n" + ee.Detail.InnerXml;
                    }
                    else if (null != ee.Detail.InnerText)
                    {
                        msg = "\n\n" + ee.Detail.InnerText;
                    }
                }
            }
            return msg;
        }
    }
}
