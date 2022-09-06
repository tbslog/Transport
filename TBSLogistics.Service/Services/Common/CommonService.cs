using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;

namespace TBSLogistics.Service.Repository.Common
{
    public class CommonService : ICommon
    {
        private IHostingEnvironment _environment;
        private TMSContext _context;

        public CommonService(IHostingEnvironment environment, TMSContext context)
        {
            _environment = environment;
            _context = context;
        }

        public async Task Log(string FileName, string LogMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
                {
                    _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "Log");
                }

                string dirPath = _environment.WebRootPath;
                string fileName = Path.Combine(dirPath, FileName + " - " + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

                await LogDB(fileName, LogMessage);
                await WriteToLog(dirPath, fileName, LogMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task LogDB(string Name, string LogMessage)
        {
            await _context.Log.AddAsync(new Log()
            {
                ModuleName = Name,
                Message = LogMessage,
                LogTime = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }

        public async Task WriteToLog(string dir, string file, string content)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(dir, file), true))
            {
                await outputFile.WriteLineAsync(string.Format("Logged on: {1} \r\n at: {2}{0}Message: {3} at {4}{0}--------------------{0}",
                          Environment.NewLine, DateTime.Now.ToLongDateString(),
                          DateTime.Now.ToLongTimeString(), content, DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss.fff")));
            }
        }
    }
}
