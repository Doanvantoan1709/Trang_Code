using System.Diagnostics;
using System.Reflection;
using Xceed.Words.NET;

namespace CommonHelper.Word
{
    public static class WordHelper
    {
        public static void ConvertDocxToPdf(string inputPath, string outputDir, string? outputFileName = null)
        {
            string libreOfficePath = @"C:\Program Files\LibreOffice\program\soffice.exe";
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = libreOfficePath,
                Arguments = $"--headless --convert-to pdf --outdir \"{outputDir}\" \"{inputPath}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process process = Process.Start(startInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"Không thể tạo file PDF: {error}");
                }
            }

            if (!string.IsNullOrEmpty(outputFileName))
            {
                string generatedPdfPath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(inputPath) + ".pdf");
                string finalPdfPath = Path.Combine(outputDir, outputFileName);

                if (!File.Exists(generatedPdfPath))
                    throw new Exception("Không tìm thấy file PDF sau khi chuyển đổi.");

                File.Move(generatedPdfPath, finalPdfPath, overwrite: true);
            }
        }

        public static void ReplacePlaceholdersAsync(string filePath, object data)
        {
            using (var document = DocX.Load(filePath))
            {
                foreach (var prop in data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var placeholder = "[[" + prop.Name + "]]";
                    var rawValue = prop.GetValue(data)?.ToString();
                    var value = string.IsNullOrEmpty(rawValue) ? "........................." : rawValue;
                    document.ReplaceText(placeholder, value);
                }

                document.Save();
            }
        }
    }
}
