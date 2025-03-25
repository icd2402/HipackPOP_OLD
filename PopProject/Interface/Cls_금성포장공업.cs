using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopProject.Interface
{
    class Cls_금성포장공업
    {
        public static void SaveCsvFile(string filePath, List<Cls_금성포장공업.PlanData> data)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                foreach (var row in data)
                {
                    string line = string.Join(",", row);
                    byte[] byteArray = Encoding.UTF8.GetBytes(line);
                    writer.WriteLine(Encoding.UTF8.GetString(byteArray));
                }
            }
        }

        public static string GetSheetModeResult(string sheetMode)
        {
            sheetMode = sheetMode.Replace(" ", "");
            sheetMode = sheetMode.Replace("골", "");
            switch (sheetMode)
            {
                case "A":
                    return "0";
                case "B":
                    return "1";
                case "AB":
                    return "2";
                case "E":
                    return "3";
                case "BA":
                    return "10";
                case "AA":
                    return "11";
                case "BE":
                    return "14";
                case "BB":
                    return "15";
                // 필요한 경우 다른 SheetMode 값에 대한 처리를 추가합니다.
                default:
                    return " ";
            }
        }

        public static string GetCalculationModeResult(string boxType, string boxSize, string pCnt, string boxScore)
        {
            switch (boxType)
            {
                case "A-1":
                    switch (pCnt)
                    { 
                        case "0.5":
                            return "2";
                        default:
                            return "1";
                    }
                default:
                    return " ";
            }
        }

        public class PlanData
        {
            public string SerialNo { get; set; }
            public string ProgramNo { get; set; }
            public string SheetMode { get; set; }
            public string CalculationMode { get; set; }
            public string SashMode { get; set; }
            public string FaceMode { get; set; }
            public string ReferencePosition { get; set; }
            public string MovableDieCutPressure { get; set; }
            public string FixedDieCutPressure { get; set; }
            public string LCut { get; set; }
            public string BasicPass { get; set; }
            public string BindingCount { get; set; }
            public string ReferenceSize { get; set; }
            public string PlannedCount { get; set; }
            public string CustomerName { get; set; }
            public string ProductName { get; set; }
            public string RegisteredDataUsage { get; set; }
            public string Removal { get; set; }
            public string Blank { get; set; }
            public List<string> DimensionUsed { get; set; }
            public List<string> DimensionNotUsed { get; set; }
            public List<string> PrintPresence { get; set; }
            public List<string> PrintColor { get; set; }
            public string ExtraBlank46 { get; set; }
            public string ExtraBlank47 { get; set; }
            public string ExtraBlank48 { get; set; }
            public string ExtraBlank49 { get; set; }
            public string ExtraBlank50 { get; set; }
            public string ExtraBlank51 { get; set; }
            public string ExtraBlank52 { get; set; }
            public string ExtraBlank53 { get; set; }
            public string ExtraBlank54 { get; set; }
            public string ExtraBlank55 { get; set; }
            public string ExtraBlank56 { get; set; }
            public string ExtraBlank57 { get; set; }
            public List<string> RGB_Blank { get; set; }
            public string End_Blank { get; set; }


            public PlanData()
            {
                DimensionUsed = new List<string>(new string[0]);//10
                DimensionNotUsed = new List<string>(new string[0]);//8
                PrintPresence = new List<string>(new string[0]);//4
                PrintColor = new List<string>(new string[0]);   //4
                RGB_Blank = new List<string>(new string[0]);    //3+1
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append(SerialNo.PadLeft(16, '0')).Append(",");
                sb.Append(ProgramNo.PadLeft(16, ' ')).Append(",");
                sb.Append(SheetMode.PadLeft(2, ' ')).Append(",");
                sb.Append(CalculationMode.PadLeft(2, ' ')).Append(",");
                sb.Append(SashMode.PadLeft(1, ' ')).Append(",");
                sb.Append(FaceMode.PadLeft(1, ' ')).Append(",");
                sb.Append(ReferencePosition.PadLeft(1, ' ')).Append(",");
                sb.Append(MovableDieCutPressure.PadLeft(1, ' ')).Append(",");
                sb.Append(FixedDieCutPressure.PadLeft(1, ' ')).Append(",");
                sb.Append(LCut.PadLeft(1, ' ')).Append(",");
                sb.Append(BasicPass.PadLeft(1, ' ')).Append(",");
                sb.Append(BindingCount.PadLeft(2, ' ')).Append(",");
                sb.Append(ReferenceSize.PadLeft(6, ' ')).Append(",");
                sb.Append(PlannedCount.PadLeft(6, ' ')).Append(",");
                sb.Append(PadRightByte(CustomerName, 30).PadRight(30, ' ')).Append(",");
                sb.Append(PadRightByte(ProductName, 40).PadRight(40, ' ')).Append(",");
                sb.Append(RegisteredDataUsage.PadLeft(1, '0')).Append(",");
                sb.Append(Removal.PadLeft(1, '1')).Append(",");
                sb.Append(Blank.PadLeft(0, ' ')).Append(",");

                foreach (var dimension in DimensionUsed)
                {
                    sb.Append(dimension.PadLeft(6, ' ')).Append(",");
                }

                foreach (var dimension in DimensionNotUsed)
                {
                    sb.Append(dimension.PadLeft(6, ' ')).Append(",");
                }

                foreach (var dimension in PrintPresence)
                {
                    sb.Append(dimension.PadLeft(1, ' ')).Append(",");
                }

                foreach (var dimension in PrintColor)
                {
                    sb.Append(dimension.PadRight(10, ' ')).Append(",");
                }

                sb.Append(ExtraBlank46.PadLeft(5, ' ')).Append(",");
                sb.Append(ExtraBlank47.PadLeft(1, ' ')).Append(",");
                sb.Append(ExtraBlank48.PadLeft(6, ' ')).Append(",");
                sb.Append(ExtraBlank49.PadLeft(6, ' ')).Append(",");
                sb.Append(ExtraBlank50.PadLeft(1, ' ')).Append(",");
                sb.Append(ExtraBlank51.PadLeft(6, ' ')).Append(",");
                sb.Append(ExtraBlank52.PadLeft(6, ' ')).Append(",");
                sb.Append(ExtraBlank53.PadLeft(1, ' ')).Append(",");
                sb.Append(ExtraBlank54.PadLeft(6, ' ')).Append(",");
                sb.Append(ExtraBlank55.PadLeft(6, ' ')).Append(",");
                sb.Append(ExtraBlank56.PadLeft(1, ' ')).Append(",");
                sb.Append(ExtraBlank57.PadLeft(1, ' ')).Append(",");

                foreach (var dimension in RGB_Blank)
                {
                    sb.Append(dimension.PadLeft(9, ' ')).Append(",");
                }
                sb.Append(End_Blank.PadLeft(9, ' '));

                return sb.ToString();
            }

            public static string PadRightByte(string input, int totalBytes)
            {
                Encoding encoding = Encoding.UTF8;
                input = $"\"{input}\"";
                byte[] bytes = encoding.GetBytes(input);

                if (bytes.Length >= totalBytes)
                {
                    return input;
                }

                int paddingBytes = totalBytes - bytes.Length;
                string padding = new string(' ', paddingBytes / encoding.GetByteCount(" "));
                return input + padding;
            }

        }
        public static List<ResultData> ReadCsvFile(string csvFilePath)
        {
            using (StreamReader reader = new StreamReader(csvFilePath))
            {
                var records = new List<ResultData>();
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();  // 한 줄씩 읽기
                    string[] values = line.Split(',');  // 콤마(,)로 분리
                    var record = new ResultData
                    {
                        SerialNo = values[0],
                        ProgramNo = values[1],
                        pworkCount = values[2],
                        sDatetime = values[3],
                        eDatetime = values[4]
                    };
                    records.Add(record);
                    Console.WriteLine(string.Join(" | ", values)); // 데이터 출력
                }
                return records;

            }
        }

        public class ResultData
        {
            public string SerialNo { get; set; }
            public string ProgramNo { get; set; }
            public string pworkCount { get; set; }
            public string sDatetime { get; set; }
            public string eDatetime { get; set; }
        }
    }
}
