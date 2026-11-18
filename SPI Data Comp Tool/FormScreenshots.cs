using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Test-only mode used by CI to take a picture of every window, so layout changes can be checked
    /// without opening the app by hand. It never connects to a database or opens a file.
    ///   "SPI Data Comp Tool.exe" --list-forms OUT_FILE         writes every form type to OUT_FILE, one per line
    ///   "SPI Data Comp Tool.exe" --screenshot TYPE OUT_FOLDER  saves TYPE.png (designed size) and
    ///                                                          TYPE_large.png (stretched to 1920 x 1040)
    /// </summary>
    internal static class FormScreenshots
    {
        public static bool TryRun(string[] args)
        {
            if (args == null || args.Length == 0) return false;
            if (args[0] == "--list-forms" && args.Length >= 2)
            {
                File.WriteAllLines(args[1], FormTypes().Select(t => t.FullName));
                return true;
            }
            if (args[0] == "--screenshot" && args.Length >= 3)
            {
                Environment.ExitCode = Capture(args[1], args[2]);
                return true;
            }
            return false;
        }

        private static Type[] FormTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Form)) && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(t => t.Name)
                .ToArray();
        }

        private static int Capture(string typeName, string outFolder)
        {
            var type = FormTypes().FirstOrDefault(t => t.FullName == typeName || t.Name == typeName);
            if (type == null) return 2;
            Directory.CreateDirectory(outFolder);
            using (var form = (Form)Activator.CreateInstance(type))
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(0, 0);
                form.ShowInTaskbar = false;
                form.WindowState = FormWindowState.Normal;
                form.Show();
                Settle();
                Save(form, Path.Combine(outFolder, type.Name + ".png"));

                // what a maximised window on a 1080p screen would show
                form.Size = new Size(1920, 1040);
                Settle();
                Save(form, Path.Combine(outFolder, type.Name + "_large.png"));
                form.Close();
            }
            return 0;
        }

        private static void Settle()
        {
            for (int i = 0; i < 5; i++) { Application.DoEvents(); System.Threading.Thread.Sleep(80); }
        }

        private static void Save(Form form, string path)
        {
            using (var bmp = new Bitmap(Math.Max(1, form.Width), Math.Max(1, form.Height)))
            {
                form.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                bmp.Save(path, ImageFormat.Png);
            }
        }
    }
}
