using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace proxy_shit
{
    class GetProxyList
    {

        public List<Proxy> Start_GetProxyList()
        {
            List<Proxy> Proxyler = new List<Proxy>();
            int sayac = 1;
            while (true)
            {
                List<Proxy> Proxys = GetProxys("http://www.cool-proxy.net/proxies/http_proxy_list/sort:score/direction:desc/page:" + sayac + "");
                if (Proxys.Count == 0)
                    break;
                else
                    Proxyler.AddRange(Proxys);
                sayac++;
            }
            return Proxyler;
        }

        List<Proxy> GetProxys(string Link)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Link);

            request.KeepAlive = true;
            request.Headers.Set(HttpRequestHeader.CacheControl, "max-age=0");
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Upgrade-Insecure-Requests", @"1");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.103 Safari/537.36";
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch");
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "tr-TR,tr;q=0.8,en-US;q=0.6,en;q=0.4");

            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            request.ServicePoint.ConnectionLeaseTimeout = 10000;
            request.ServicePoint.MaxIdleTime = 10000;
            request.ServicePoint.ConnectionLimit = 10000;

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                return new List<Proxy>();
            }
            string gelen = new StreamReader(response.GetResponseStream()).ReadToEnd();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(gelen);

            HtmlNodeCollection nodes = null;
            try
            {
                nodes = doc.DocumentNode.SelectNodes("//table")[0].SelectNodes(".//tr");
            }
            catch (Exception)
            {
            }
            List<Proxy> Proxys = new List<Proxy>();
            if (nodes == null)
                return Proxys;
            foreach (HtmlNode item in nodes)
            {
                HtmlNodeCollection nd = null;
                try
                {
                    nd = item.SelectNodes(".//td");
                }
                catch (Exception)
                {
                }
                if (nd == null)
                    continue;
                if (nd.Count < 8)
                    continue;
                Proxy p = new Proxy();
                if (Convert.ToInt16(nd[8].InnerText) > 200)
                {
                    p.IP = nd[0].InnerText;
                    p.IP = p.IP.Substring(p.IP.IndexOf('\"') + 1, p.IP.LastIndexOf('\"') - p.IP.IndexOf('\"') - 1);
                    p.IP = Rot13.Transform(p.IP);
                    p.IP = Encoding.UTF8.GetString(Convert.FromBase64String(p.IP));
                    p.Port = nd[1].InnerText;
                    p.Ulke = nd[3].InnerText;
                    p.Hiz = nd[8].InnerText;
                    Proxys.Add(p);
                }

            }
            return Proxys;
        }
    }

    static class Rot13
    {
        /// <summary>
        /// Performs the ROT13 character rotation.
        /// </summary>
        public static string Transform(string value)
        {
            char[] array = value.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                int number = (int)array[i];

                if (number >= 'a' && number <= 'z')
                {
                    if (number > 'm')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                else if (number >= 'A' && number <= 'Z')
                {
                    if (number > 'M')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                array[i] = (char)number;
            }
            return new string(array);
        }
    }
}
