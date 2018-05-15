using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace Llama.Library
{
    public class Troubleshooting
    {
        public async Task<string> CollectCCMLogs()
        {
            try
            {
                string sourceDir = "C:\\Windows\\CCM\\Logs\\";
                string tempDir = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string llamaDir = "Llama\\CCMLogs\\";
                string outputDir = Path.Combine(tempDir, llamaDir);
                if (Directory.Exists(outputDir))
                {
                    Directory.Delete(outputDir, true);
                    while (Directory.Exists(outputDir))
                    {
                        await Task.Delay(1000);
                    }
                }

                if (!(Directory.Exists(outputDir))) { Directory.CreateDirectory(outputDir); }


                foreach (string subDirs in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(subDirs.Replace(sourceDir, outputDir));
                }

                foreach (string copiedFiles in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
                {
                    File.Copy(copiedFiles, copiedFiles.Replace(sourceDir, outputDir), true);

                }
                
                return "good";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return "good";
        }

        public async Task<string> CompressCCMLogs()
        {
            try
            {
                string tempDir = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string llamaDir = "Llama\\";
                string llamaLogsDir = "Llama\\CCMLogs\\";
                string sourceDir = Path.Combine(tempDir, llamaLogsDir);
                string zipName = "CCMLOGS-" + DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToShortTimeString().Replace(":", "") + ".zip";
                string zipDir = Path.Combine(tempDir, llamaDir, zipName);
                string dstZip = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), zipName);

                await Task.Run(() => ZipFile.CreateFromDirectory(sourceDir, zipDir));
                await Task.Run(() => File.Copy(zipDir, dstZip));

                if (Directory.Exists(llamaLogsDir))
                {
                    Directory.Delete(llamaLogsDir, true);
                    while (Directory.Exists(llamaLogsDir))
                    {
                        await Task.Delay(1000);
                    }
                }

                if (File.Exists(zipDir))
                {
                    File.Delete(zipDir);
                    while (File.Exists(zipDir))
                    {
                        await Task.Delay(1000);
                    }
                }

                return "good";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return "bad";
        }
    }
}