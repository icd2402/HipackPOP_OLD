using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace PopProject
{
    class Cls_Variable
    {
        private static bool _sharedValue;

        // 설비 선택 마우스 클릭시, 화면 move에 대한 방지를 위한 변수.
        public static bool SharedValue
        {
            get { return _sharedValue; }
            set { _sharedValue = value; }
        }

        private static double _Dbl_ReportPageTop;
        public static double Dbl_ReportPageTop
        {
            get { return _Dbl_ReportPageTop; }
            set { _Dbl_ReportPageTop = value; }
        }

        public static void EnsureDirectoryExists(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
        // 설비인라인의 경우, 실패에 따른 재 작업카운터
        private static int _g_checkValue;
        public static int g_checkValue
        {
            get { return _g_checkValue; }
            set { _g_checkValue = value; }
        }

        // 폴더열기
        public static string OpenFolderDialog()
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                // 대화 상자 제목 설정
                folderBrowserDialog.Description = "폴더를 선택하세요";

                // 대화 상자 표시
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    return folderBrowserDialog.SelectedPath;
                }
                else
                {
                    return null;
                }
            }
        }

        //파일열기
        public static void OpenFile(string filePath)
        {
            try
            {
                // Process 클래스를 사용하여 파일 열기
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                Console.WriteLine("파일이 열렸습니다.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"파일을 여는 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        //파일 삭제
        public static void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Console.WriteLine($"파일이 삭제되었습니다: {filePath}");
                }
                else
                {
                    Console.WriteLine("파일이 존재하지 않습니다.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"파일 삭제 중 오류가 발생했습니다: {ex.Message}");
            }
        }
    }
}
