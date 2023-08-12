using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace SPI_Data_Comp_Tool
{
    public static class Logs
    {
        private static Common_Data common_Data;
        public static string strLogPathFolder, strLogFile, counter, strDataCaptureFile;
        private static int intCounter, intCounterForData;
        //private static Thread thread;
        //private static ParameterizedThreadStart parameterized;
        static Logs()
        {
            try
            {
                //parameterized = new ParameterizedThreadStart(EnterDataInLogs);
                //thread = new Thread(parameterized);
                
                common_Data = new Common_Data();
                strLogPathFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                strLogPathFolder = $@"{strLogPathFolder}\LogFolder";
                strLogFile = $@"{strLogPathFolder}\Log_{common_Data.AppendDateInOutputFileName(DateTime.Today, "dd_MM_yyyy")}.log";
                intCounter = 1;
                #region For Log File
                if (!(Directory.Exists(strLogPathFolder)))
                {
                    Directory.CreateDirectory(strLogPathFolder);
                }

                //while (File.Exists(strLogFile) & IsFileLocked(strLogFile))
                while (File.Exists(strLogFile))
                {
                    if (Debugger.IsAttached)
                    {
                        break;
                    }
                    strLogFile = $@"{strLogPathFolder}\Log_{common_Data.AppendDateInOutputFileName(DateTime.Today, "dd_MM_yyyy")}--{intCounter++}.log";
                }
                if (!(File.Exists(strLogFile)))
                {
                    var f = File.Create(strLogFile);
                    f.Close();
                }
                #endregion

                #region For Data Entry

                strDataCaptureFile = $@"{strLogPathFolder}\DataFile_{common_Data.AppendDateInOutputFileName(DateTime.Today, "dd_MM_yyyy")}.log";

                intCounterForData = 1;
                while (File.Exists(strDataCaptureFile))
                {
                    if (Debugger.IsAttached)
                    {
                        break;
                    }
                    strDataCaptureFile = $@"{strLogPathFolder}\Log_{common_Data.AppendDateInOutputFileName(DateTime.Today, "dd_MM_yyyy")}--{intCounterForData++}.log";
                }
                if (!(File.Exists(strDataCaptureFile)))
                {
                    var f = File.Create(strDataCaptureFile);
                    f.Close();
                }
                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void EnterLogs(string strLog)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(strLogFile))
                {
                    sw.WriteLine(strLog);
                    sw.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void EnterLogsWithTime(string strLog)
        {
            try
            {
                strLog = $"{string.Format("{0:T}", DateTime.Now)} : {strLog}";
                using (StreamWriter sw = File.AppendText(strLogFile))
                {
                    sw.WriteLine(strLog);
                    sw.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void EnterLogsForError(Exception exc)
        {
            try
            {
                string strLog = $"++++++++++++++\n\n{string.Format("{0:T}", DateTime.Now)} : {exc.ToString()}++++++++++++\n\n";
                using (StreamWriter sw = File.AppendText(strLogFile))
                {
                    sw.WriteLine(strLog);
                    sw.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        static bool IsFileLocked(string filename)
        {
            bool Locked = false;
            try
            {
                using (StreamWriter sw = File.AppendText(filename))
                {
                    sw.Dispose();
                }
            }
            catch (IOException)
            {
                Locked = true;
            }
            return Locked;
        }

        //public static void EnterDataInLogs(string strData)
        //{
        //    try
        //    {
        //        //EnterLogsForData(strData.ToString());
        //        //await Task.Factory.StartNew(() =>
        //        // {
        //        //     EnterLogsForData(strData);
        //        // });
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //private static void EnterLogsForData(string strData)
        //{
        //    try
        //    {
        //        strData = $"{string.Format("{0:T}", DateTime.Now)} : {strData}";
        //        using (StreamWriter sw = File.AppendText(strDataCaptureFile))
        //        {
        //            sw.WriteLine(strData);
        //            sw.Flush();
        //            sw.Dispose();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}





//static bool IsFileLocked(string fileName)
//{
//    try
//    {
//        FileInfo file = new FileInfo(fileName);
//        using (FileStream stream = file.OpenWrite())//(FileMode.Append, FileAccess.ReadWrite, FileShare.Write))
//        {
//            stream.Close();
//        }
//    }
//    catch (IOException)
//    {
//        return true;
//    }
//    //file is not locked
//    FileStream fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
//    fs.Close();
//    return false;
//}