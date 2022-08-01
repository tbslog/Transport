using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Service.Repository.Common
{
    public class CommonService : ICommon
    {
        private IHostingEnvironment _environment;

        public CommonService(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public async Task Log(string FileName, string LogMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
                {
                    _environment.WebRootPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Log");
                }

                string dirPath = _environment.WebRootPath;
                string fileName = Path.Combine(dirPath, FileName + " - " + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

                await WriteToLog(dirPath, fileName, LogMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task WriteToLog(string dir, string file, string content)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(dir, file), true))
            {
                await outputFile.WriteLineAsync(string.Format("Logged on: {1} at: {2}{0}Message: {3} at {4}{0}--------------------{0}",
                          Environment.NewLine, DateTime.Now.ToLongDateString(),
                          DateTime.Now.ToLongTimeString(), content, DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss.fff")));
            }
        }
    }
}
