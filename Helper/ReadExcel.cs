using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallThrusterTestSystem
{
    public class ReadExcel
    {
        public Dictionary<string, List<Dictionary<string, object>>> SheetData { get; } = new Dictionary<string, List<Dictionary<string, object>>>();

        public void ReadDeviceExcel(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Excel文件不存在: {filePath}");

            // 获取所有工作表名称
            var sheetNames = GetSheetNames(filePath);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                foreach (var sheetName in sheetNames)
                {
                    try
                    {
                        // 读取当前工作表数据
                        var rows = MiniExcel.Query(stream,
                            useHeaderRow: true,
                            sheetName: sheetName,
                            configuration: null)
                            .Cast<IDictionary<string, object>>();

                        // 转换为字典列表并存储
                        var sheetRows = rows.Select(row =>
                            row.ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value
                            )).ToList();

                        SheetData[sheetName] = sheetRows;
                    }
                    catch (Exception ex)
                    {
                        // 存储空列表表示读取失败
                        SheetData[sheetName] = new List<Dictionary<string, object>>();
                        Console.WriteLine($"读取工作表 '{sheetName}' 失败: {ex.Message}");
                    }
                }
            }
        }

        private List<string> GetSheetNames(string filePath)
        {
            try
            {
                // 直接使用 MiniExcel 提供的方法获取工作表名称
                return MiniExcel.GetSheetNames(filePath).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取工作表名称失败: {ex.Message}");
                return new List<string>();
            }
        }

        // 便捷方法获取特定工作表数据
        public List<Dictionary<string, object>> GetSheet(string sheetName)
        {
            if (SheetData.TryGetValue(sheetName, out var data))
            {
                return data;
            }
            return new List<Dictionary<string, object>>();
        }
    }
}
