using System.Runtime.InteropServices;
using System.Text;

namespace Nanocode.Data.IO
{
    public class IniSettingsManager
    {
        /* 
         * USAGE
         * var ini = new IniSettingsManager(Application.StartupPath + "\\AGuard.ini");
         * var sqlKODB = ini.Read("SQLServer", "Database");
         * ini.Write("SQLServer", "Database", cmbKODB.Text);
         * */

        public string FilePath
        {
            get { return this.filePath; }
            set { this.filePath = value; }
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        private string filePath;

        public IniSettingsManager(string filePath)
        {
            this.filePath = filePath;
        }

        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, this.filePath);
        }

        public string Read(string section, string key)
        {
            var sb = new StringBuilder(255);
            var i = GetPrivateProfileString(section, key, "", sb, 255, this.filePath);
            return sb.ToString();
        }
    }
}
