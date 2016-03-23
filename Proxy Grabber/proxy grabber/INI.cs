using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
namespace Proxy_Grabber_V1
{
    class INI
    {
        public string path;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public INI(string IniPath)
        {
            path = IniPath;
        }
        public void Write(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }
        public void Write(string Key, string Value)
        {
            WritePrivateProfileString(Assembly.GetExecutingAssembly().GetName().Name, Key, Value, this.path);
        }
        public string Read(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }
        public string Read(string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Assembly.GetExecutingAssembly().GetName().Name, Key, "", temp, 255, this.path);
            return temp.ToString();
        }
    }
}
