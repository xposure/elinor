using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Elinor
{
    class VolumeFetcher
    {
        internal static Dictionary<string, int> GetVolumes(int typeId, int systemId)
        {
            var result = new Dictionary<string, int>();

            string html = GetHtml(typeId, systemId);
            if(html != null)
            {
                /*  <b>Volume for sale now:</b> 46 units
                    <br />
                    <b class="view">Movement <span>(Initial volume - volume remaining)</span></b>: 0 units */   
                var dunits = new Regex(@"[\d,]+ units");
                var matches = dunits.Matches(html);

                int step = 0;
                foreach (Match m in matches)
                {
                    int i;
                    if(int.TryParse(m.Value.Replace("units", "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out i))
                    {
                        switch (step)
                        {
                            case 0:
                                result.Add("sellvol", i);
                                break;
                            case 1:
                                result.Add("sellmov", i);
                                break;
                            case 2:
                                result.Add("buyvol", i);
                                break;
                            case 3:
                                result.Add("buymov", i);
                                break;
                        }
                    }
                    step++;
                }



            }
            return result;
        }

        private static string GetHtml(int typeId, int systemId)
        {
            string result;
            using (var client = new WebClient())
            {
                try
                {
                    result = client.DownloadString(string.Format("http://eve-central.com/home/quicklook.html?typeid={0}&usesystem={1}", typeId, systemId));
                }
                catch (Exception)
                {
                    result = null;
                }
            }
            return result;
        }
    }
}
