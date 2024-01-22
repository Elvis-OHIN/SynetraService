﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SynetraService.Models
{
    public class SystemInfo
    {
        public static string GetComputerName()
        {
            return Environment.MachineName;
        }

        public static string GetOSVersion()
        {
            return Environment.OSVersion.ToString();
        }
        public static string GetProcessorName()
        {
            string processorName = "";
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (var item in searcher.Get())
            {
                processorName = item["Name"].ToString();
                break; // Généralement, il n'y a qu'un seul processeur
            }
            return processorName;
        }
        public static string GetWindowsProductId()
        {
            string productId = "";
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

            foreach (ManagementObject obj in searcher.Get())
            {
                productId = obj["SerialNumber"].ToString();
                break;
            }

            return productId;
        }
        public static string GetSystemArchitecture()
        {
            return Environment.Is64BitOperatingSystem ? "64 bits" : "32 bits";
        }
        public static string GetOperatingSystemInfo()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT Caption, Version FROM Win32_OperatingSystem"))
            {
                foreach (var os in searcher.Get())
                {
                    return $"{os["Caption"]} - Version {os["Version"]}";
                }
            }
            return "Non trouvé";
        }

        public static string GetGPUName()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController"))
            {
                foreach (var obj in searcher.Get())
                {
                    return obj["Name"].ToString();
                }
            }
            return "Non trouvé";
        }

        public static string GetMotherboardInfo()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard"))
            {
                foreach (var obj in searcher.Get())
                {
                    return $"Fabricant: {obj["Manufacturer"]}, Produit: {obj["Product"]}";
                }
            }
            return "Non trouvé";
        }

        public static List<KeyValuePair<string, KeyValuePair<long, long>>> GetStorageInfo()
        {
            var storageInfo = new List<KeyValuePair<string, KeyValuePair<long, long>>>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    long totalSpaceGB = drive.TotalSize / (1024 * 1024 * 1024); // Convertit en GB
                    long freeSpaceGB = drive.TotalFreeSpace / (1024 * 1024 * 1024); // Convertit en GB
                    long usedSpaceGB = totalSpaceGB - freeSpaceGB;
                    storageInfo.Add(new KeyValuePair<string, KeyValuePair<long, long>>(drive.Name, new KeyValuePair<long, long>(usedSpaceGB, freeSpaceGB)));
                }
            }
            return storageInfo;
        }
        public static ulong GetTotalMemoryInBytes()
        {
            var searcher = new ManagementObjectSearcher("select * from Win32_ComputerSystem");

            foreach (var item in searcher.Get())
            {
                return (ulong)item["TotalPhysicalMemory"];
            }

            return 0;
        }

        public static (long TotalMemory, long FreeMemory) GetRAMInfo()
        {

            var totalMemoryInBytes = (long)GetTotalMemoryInBytes();
            var freeMemoryInBytes = new PerformanceCounter("Memory", "Available Bytes").RawValue;

            long totalMemory = totalMemoryInBytes / (1024 * 1024); // Conversion en MB
            long freeMemory = freeMemoryInBytes / (1024 * 1024); // Conversion en MB

            return (totalMemory, freeMemory);
        }
    }
}
