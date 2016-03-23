using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace proxy_shit
{
    class ChangeProxy
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;

        const string userRoot = "HKEY_CURRENT_USER";
        const string subkey = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";
        const string keyName = userRoot + "\\" + subkey;


        public static void SetProxy(string ProxyAdress)
        {
            Registry.SetValue(keyName, "ProxyServer", ProxyAdress);
            Registry.SetValue(keyName, "ProxyEnable", "1");
            SaveChanges();
        }

        public static void EnableProxy()
        {
            Registry.SetValue(keyName, "ProxyEnable", "1");
            SaveChanges();
        }
        public static void DisableProxy()
        {
            Registry.SetValue(keyName, "ProxyServer", "");
            Registry.SetValue(keyName, "ProxyEnable", 0);
            SaveChanges();
        }
        public static string GetProxyServer()
        {
            return Registry.GetValue(keyName, "ProxyServer", RegistryValueOptions.None).ToString();
        }
        public static string GetProxyStatus()
        {
            return Registry.GetValue(keyName, "ProxyEnable", RegistryValueOptions.None).ToString();
        }
        private static void SaveChanges()
        {
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }
    }
}
