using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProcessSyHallThrusterTestSystemstemLib
{
    /// <summary>
    /// IniConfigHelper
    /// </summary>
    public class IniConfigHelper
    {
        private static string filePath = "";

        #region API函数声明

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
        private static extern uint GetPrivateProfileStringA(string section, string key,
            string def, Byte[] retVal, int size, string filePath);

        #endregion

        #region 读取Sections
        /// <summary>
        ///  ReadSections
        /// </summary>
        /// <param name="iniFilename">文件路径</param>
        /// <returns>集合</returns>
        public static List<string> ReadSections(string iniFilename)
        {
            List<string> result = new List<string>();
            Byte[] buf = new Byte[65536];
            uint len = GetPrivateProfileStringA(null, null, null, buf, buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < len; i++)
            {
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            }
            return result;
        }

        #endregion

        #region 读Keys
        /// <summary>
        /// ReadKeys
        /// </summary>
        /// <param name="SectionName">区域名称</param>
        /// <param name="iniFilename">路径</param>
        /// <returns>集合</returns>
        public static List<string> ReadKeys(string SectionName, string iniFilename)
        {
            List<string> result = new List<string>();
            Byte[] buf = new Byte[65536];
            uint len = GetPrivateProfileStringA(SectionName, null, null, buf, buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < len; i++)
            {
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            }
            return result;
        }
        #endregion

        #region 读Ini文件

        /// <summary>
        ///  ReadIniData
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="Key">键</param>
        /// <param name="NoText">默认值</param>
        /// <returns>返回值</returns>
        public static string ReadIniData(string Section, string Key, string Defaut)
        {
            return ReadIniData(Section, Key, Defaut, filePath);
        }

        /// <summary>
        ///  ReadIniData
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="Key">键</param>
        /// <param name="NoText">默认值</param>
        /// <param name="iniFilePath">路径</param>
        /// <returns>返回值</returns>
        public static string ReadIniData(string Section, string Key, string Default, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(102400);
                GetPrivateProfileString(Section, Key, Default, temp, 102400, iniFilePath);
                return temp.ToString();
            }
            else return string.Empty;
        }

        #endregion

        #region 写Ini文件

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        /// <returns>是否成功</returns>
        public static bool WriteIniData(string Section, string Key, string Value)
        {
            return WriteIniData(Section, Key, Value, filePath);
        }

        /// <summary>
        ///  WriteIniData
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        /// <param name="iniFilePath">路径</param>
        /// <returns>是否成功</returns>
        public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
        {
            long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
            return OpStation != 0;
        }
        #endregion
        #region UTF-8 读取

        public static string ReadIniDataUtf8(
            string Section,
            string Key,
            string Default,
            string iniFilePath)
        {
            if (!File.Exists(iniFilePath))
                return Default;

            string currentSection = null;

            foreach (var line in File.ReadAllLines(iniFilePath, Encoding.UTF8))
            {
                var txt = line.Trim();

                if (string.IsNullOrEmpty(txt) || txt.StartsWith(";"))
                    continue;

                if (txt.StartsWith("[") && txt.EndsWith("]"))
                {
                    currentSection = txt.Substring(1, txt.Length - 2);
                    continue;
                }

                if (currentSection == Section)
                {
                    int index = txt.IndexOf('=');
                    if (index > 0 && txt.Substring(0, index) == Key)
                    {
                        return txt.Substring(index + 1);
                    }
                }
            }

            return Default;
        }

        #endregion

        #region 自动识别编码读取

        public static string ReadIniDataAuto(
            string Section,
            string Key,
            string Default,
            string iniFilePath)
        {
            if (!File.Exists(iniFilePath))
                return Default;

            if (IsUtf8File(iniFilePath))
            {
                return ReadIniDataUtf8(Section, Key, Default, iniFilePath);
            }

            return ReadIniData(Section, Key, Default, iniFilePath);
        }


        #endregion

        private static bool IsUtf8File(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (fs.Length >= 3)
                {
                    byte[] bom = new byte[3];
                    fs.Read(bom, 0, 3);

                    if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                        return true;
                }
            }

            byte[] bytes = File.ReadAllBytes(filePath);
            try
            {
                Encoding.UTF8.GetString(bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region 新增：Unicode版本API及读写方法
        /// <summary>
        /// Unicode版本写入API（支持中文）
        /// </summary>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileStringW(
            string lpAppName,
            string lpKeyName,
            string lpString,
            string lpFileName);

        /// <summary>
        /// Unicode版本读取API（支持中文）
        /// </summary>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint GetPrivateProfileStringW(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            uint nSize,
            string lpFileName);

        /// <summary>
        /// 读取INI数据（Unicode版本，解决中文乱码）
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="Key">键</param>
        /// <param name="Default">默认值</param>
        /// <param name="iniFilePath">文件路径</param>
        /// <returns>读取结果</returns>
        public static string ReadIniDataUnicode(string Section, string Key, string Default, string iniFilePath)
        {
            if (!File.Exists(iniFilePath))
                return Default;

            const int bufferSize = 102400;
            StringBuilder temp = new StringBuilder(bufferSize);
            GetPrivateProfileStringW(Section, Key, Default, temp, (uint)temp.Capacity, iniFilePath);
            return temp.ToString();
        }

        /// <summary>
        /// 写入INI数据（Unicode版本，解决中文乱码）
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        /// <param name="iniFilePath">文件路径</param>
        /// <returns>是否写入成功</returns>
        public static bool WriteIniDataUnicode(string Section, string Key, string Value, string iniFilePath)
        {
            // 去除值前后空格，解决多余空格问题
            string cleanValue = Value?.Trim() ?? string.Empty;
            bool result = WritePrivateProfileStringW(Section, Key, cleanValue, iniFilePath);
            return result;
        }
        #endregion

        #region 新增：UTF-8版本写入方法
        /// <summary>
        /// 写入INI数据（UTF-8编码，适配ReadIniDataUtf8）
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        /// <param name="iniFilePath">文件路径</param>
        public static void WriteIniDataUtf8(string Section, string Key, string Value, string iniFilePath)
        {
            lock (typeof(IniConfigHelper)) 
            {
                Dictionary<string, Dictionary<string, string>> iniData = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
                string currentSection = null;
                Encoding utf8 = Encoding.UTF8;

                if (File.Exists(iniFilePath))
                {
                    foreach (var line in File.ReadAllLines(iniFilePath, utf8))
                    {
                        var trimLine = line.Trim();
                        if (string.IsNullOrEmpty(trimLine) || trimLine.StartsWith(";"))
                            continue;

                        if (trimLine.StartsWith("[") && trimLine.EndsWith("]"))
                        {
                            currentSection = trimLine.Trim('[', ']');
                            if (!iniData.ContainsKey(currentSection))
                            {
                                iniData[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            }
                            continue;
                        }

                        if (currentSection != null && trimLine.Contains("="))
                        {
                            var parts = trimLine.Split(new[] { '=' }, 2);
                            var lineKey = parts[0].Trim();
                            var lineValue = parts[1].Trim();
                            iniData[currentSection][lineKey] = lineValue;
                        }
                    }
                }

                if (!iniData.ContainsKey(Section))
                {
                    iniData[Section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                iniData[Section][Key] = Value?.Trim() ?? string.Empty; // 去除空格

                StringBuilder sb = new StringBuilder();
                foreach (var section in iniData)
                {
                    sb.AppendLine($"[{section.Key}]");
                    foreach (var kv in section.Value)
                    {
                        sb.AppendLine($"{kv.Key}={kv.Value}");
                    }
                    sb.AppendLine();
                }
                File.WriteAllText(iniFilePath, sb.ToString(), utf8);
            }
        }
        #endregion
    }
}