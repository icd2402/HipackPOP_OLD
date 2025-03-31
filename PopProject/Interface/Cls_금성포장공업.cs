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
            try
            {
                using (var writer = new StreamWriter(filePath, false, Encoding.Default))
                {
                    // 데이터 작성
                    foreach (var row in data)
                    {
                        // 필요한 속성만 선택하여 CSV 형식으로 작성
                        string line = string.Join(",", row);
                        writer.WriteLine(line);
                    }
                }
                Console.WriteLine("CSV 파일이 성공적으로 생성되었습니다.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"파일 저장 중 오류 발생: {ex.Message}");
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
                sb.Append(ProgramNo).Append(",");
                sb.Append(SheetMode).Append(",");
                sb.Append(CalculationMode).Append(",");
                sb.Append(SashMode).Append(",");
                sb.Append(FaceMode).Append(",");
                sb.Append(ReferencePosition).Append(",");
                sb.Append(MovableDieCutPressure).Append(",");
                sb.Append(FixedDieCutPressure).Append(",");
                sb.Append(LCut).Append(",");
                sb.Append(BasicPass).Append(",");
                sb.Append(BindingCount).Append(",");
                sb.Append(ReferenceSize).Append(",");
                sb.Append(PlannedCount).Append(",");
                sb.Append(PadMark(CustomerName)).Append(",");
                sb.Append(PadMark(ProductName)).Append(",");
                sb.Append(RegisteredDataUsage.PadLeft(1, '0')).Append(",");
                sb.Append(Removal.PadLeft(1, '1')).Append(",");
                sb.Append(Blank.PadLeft(0, ' ')).Append(",");

                foreach (var dimension in DimensionUsed)
                {
                    sb.Append(dimension).Append(",");
                }

                foreach (var dimension in DimensionNotUsed)
                {
                    sb.Append(dimension).Append(",");
                }

                foreach (var dimension in PrintPresence)
                {
                    sb.Append(dimension).Append(",");
                }

                foreach (var dimension in PrintColor)
                {
                    sb.Append(dimension).Append(",");
                }

                sb.Append(ExtraBlank46).Append(",");
                sb.Append(ExtraBlank47).Append(",");
                sb.Append(ExtraBlank48).Append(",");
                sb.Append(ExtraBlank49).Append(",");
                sb.Append(ExtraBlank50).Append(",");
                sb.Append(ExtraBlank51).Append(",");
                sb.Append(ExtraBlank52).Append(",");
                sb.Append(ExtraBlank53).Append(",");
                sb.Append(ExtraBlank54).Append(",");
                sb.Append(ExtraBlank55).Append(",");
                sb.Append(ExtraBlank56).Append(",");
                sb.Append(ExtraBlank57).Append(",");

                foreach (var dimension in RGB_Blank)
                {
                    sb.Append(dimension).Append(",");
                }
                sb.Append(End_Blank);

                return sb.ToString();
            }
        }
        public static string PadMark(string input)
        {
            input = $"\"{input}\"";
                return input;
        }

        public static List<ResultData> ReadCsvFile(string csvFilePath)
        {
            using (StreamReader reader = new StreamReader(csvFilePath))
            {
                var records = new List<ResultData>();
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();  // 한 줄씩 읽기
                    if (!line.Equals(""))
                    {
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
                    };
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
