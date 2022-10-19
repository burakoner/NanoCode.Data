using System;
using System.Management;

namespace NanoCode.Data.Diagnostics
{
    public static class WindowsInformation
    {
        public static string[] BasicFeatures = new string[] { "USERNAME", "COMPUTERNAME", "USERDOMAIN", "NUMBER_OF_PROCESSORS", "OS", "SYSTEMDRIVE", "HOMEDRIVE", "WINDIR", "SYSTEMROOT", "PROGRAMFILES", "COMMONPROGRAMFILES", "PUBLIC", "USERPROFILE", "LOCALAPPDATA", "TEMP", "APPDATA" };
        public static string[] AdvancedFeatures = new string[] { "OS_NAME", "OS_ARCHITECTURE", "PROCESSOR_ID", "PROCESSOR_ARCHITECTURE", "GRAPHICCARD_NAME", "SCREEN_WIDTH", "SCREEN_HEIGHT", "MEMORY_PHYSICAL", "MEMORY_VIRTUAL", "MEMORY_TOTAL" };

        private static bool isInArray(string[] strArray, string key)
        {
            for (int i = 0; i <= strArray.Length - 1; i++)
                if (strArray[i].ToString() == key) return true;
            return false;
        }

        private static string GetOSName()
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else
                            operatingSystem = "7";
                        break;
                    default:
                        break;
                }
            }
            //Make sure we actually got something in our OS check
            //We don't want to just return " Service Pack 2" or " 32-bit"
            //That information is useless without the OS version.
            if (operatingSystem != "")
            {
                //Got something.  Let's prepend "Windows" and get more info.
                operatingSystem = "Windows " + operatingSystem;
                //See if there's a service pack installed.
                if (os.ServicePack != "")
                {
                    //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
                    operatingSystem += " " + os.ServicePack;
                }
                //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
                //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
            }
            //Return the information we've gathered.
            return operatingSystem;
        }

        private static int GetOSArchitecture()
        {
            string pa = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            return ((string.IsNullOrEmpty(pa) || string.Compare(pa, 0, "x86", 0, 3, true) == 0) ? 32 : 64);
        }

        public static string GetSystemInfo(string Feature)
        {
            string returnValue = "";

            if (isInArray(BasicFeatures, Feature))
            {
                try
                {
                    returnValue = Environment.GetEnvironmentVariable(Feature);
                }
                catch
                {
                    returnValue = "";
                }
            }
            else if (isInArray(AdvancedFeatures, Feature))
            {
                if (Feature == "OS_NAME")
                    returnValue = GetOSName();

                if (Feature == "OS_ARCHITECTURE")
                    returnValue = GetOSArchitecture().ToString() + "Bit";

                else if (Feature == "PROCESSOR_ID")
                {
                    ManagementObjectSearcher MOS = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor");
                    foreach (ManagementObject MO in MOS.Get())
                        foreach (PropertyData PC in MO.Properties)
                            if (PC.Name == "ProcessorId") returnValue = PC.Value.ToString();
                }

                else if (Feature == "PROCESSOR_ARCHITECTURE")
                {
                    ManagementObjectSearcher MOS = new ManagementObjectSearcher("SELECT AddressWidth FROM Win32_Processor");
                    foreach (ManagementObject MO in MOS.Get())
                        foreach (PropertyData PD in MO.Properties)
                            if (PD.Name == "AddressWidth") returnValue = PD.Value.ToString() + "Bit";
                }

                else if (Feature == "GRAPHICCARD_NAME")
                {
                    ManagementObjectSearcher MOS = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
                    foreach (ManagementObject MO in MOS.Get())
                        foreach (PropertyData PD in MO.Properties)
                            if (PD.Name == "Name") returnValue = PD.Value.ToString();
                }

                else if (Feature == "SCREEN_WIDTH")
                {
                    ManagementObjectSearcher MOS = new ManagementObjectSearcher("SELECT CurrentHorizontalResolution FROM Win32_VideoController");
                    foreach (ManagementObject MO in MOS.Get())
                        foreach (PropertyData PD in MO.Properties)
                            if (PD.Name == "CurrentHorizontalResolution") returnValue = PD.Value.ToString();
                }

                else if (Feature == "SCREEN_HEIGHT")
                {
                    ManagementObjectSearcher MOS = new ManagementObjectSearcher("SELECT CurrentVerticalResolution FROM Win32_VideoController");
                    foreach (ManagementObject MO in MOS.Get())
                        foreach (PropertyData PD in MO.Properties)
                            if (PD.Name == "CurrentVerticalResolution") returnValue = PD.Value.ToString();
                }

                else if (Feature == "MEMORY_PHYSICAL")
                {
                    long MP = 0;
                    ManagementObjectSearcher MOS = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
                    foreach (ManagementObject MO in MOS.Get())
                        foreach (PropertyData PD in MO.Properties)
                            if (PD.Name == "Capacity") MP += long.Parse(PD.Value.ToString());

                    returnValue = (MP / 1024).ToString();
                }

                else if (Feature == "MEMORY_TOTAL")
                {
                    ManagementObjectSearcher MOS = new ManagementObjectSearcher("SELECT MaxCapacity FROM Win32_PhysicalMemoryArray");
                    foreach (ManagementObject MO in MOS.Get())
                        foreach (PropertyData PD in MO.Properties)
                            if (PD.Name == "MaxCapacity") returnValue = PD.Value.ToString();
                }

                else if (Feature == "MEMORY_VIRTUAL")
                {

                    long MT = 0; long MP = 0;
                    ManagementObjectSearcher MOS;

                    MOS = new ManagementObjectSearcher("SELECT MaxCapacity FROM Win32_PhysicalMemoryArray");
                    foreach (ManagementObject MO in MOS.Get())
                        foreach (PropertyData PD in MO.Properties)
                            if (PD.Name == "MaxCapacity") MT = long.Parse(PD.Value.ToString());


                    MOS = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
                    foreach (ManagementObject MO in MOS.Get())
                        foreach (PropertyData PD in MO.Properties)
                            if (PD.Name == "Capacity") MP += long.Parse(PD.Value.ToString());
                    MP = MP / 1024;

                    returnValue = (MT - MP).ToString();
                }
            }

            return returnValue;
        }
    }

}
