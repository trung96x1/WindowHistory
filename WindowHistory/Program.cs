using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowHistory {
    class Program {
        //Get pointer to current window which is focused
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        //Get title of windown corespond with hWnd (output store in text with lenght count)
        [DllImport("user32.dll")]
        static extern int GetWindowText( IntPtr hWnd, StringBuilder text, int count );

        //Get titlte of current focused window
        private static string GetActiveWindowTitle() {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();
            if(GetWindowText(handle, Buff, nChars) > 0) {
                return Buff.ToString();
            }
            return null;
        }

        //Default dir is C:\WindowHistory\
        //Default interval is 5s
        public static String FOLDER = @"C:\WindowHistory\";
        public static String preTitle;
        public static int interval;
        static void Main( string[] args ) {
            interval = 5000;
            if(args.Length == 2) {
                FOLDER = args[0];
                bool isInt = int.TryParse(args[1], out interval);
                if(!isInt)
                    interval = 5000;
            }

            DirectoryInfo dir = new DirectoryInfo(FOLDER);
            if(!dir.Exists) {
                dir = new DirectoryInfo(@"C:\WindowHistory\");
                if(!dir.Exists)
                    dir.Create();
            }

            //Loop infinity and write to file
            preTitle = "";
            while(true) {
                writeLog();
                Thread.Sleep(interval);
            }
        }

        private static void writeLog() {
            try {
                DateTime today = DateTime.Now;
                String fileName = today.Day + "-" + today.Month + "-" + today.Year + ".log";
                using(StreamWriter writer = new StreamWriter((FOLDER + fileName), true, Encoding.UTF8)) {
                    String title = GetActiveWindowTitle();
                    if(title == null)
                        title = "Desktop";
                    if(title != preTitle) {
                        writer.WriteLine(today + "\t" + title);
                        preTitle = title;
                    }
                }
            } catch(Exception e) {
                Console.WriteLine(e.ToString());
            }    
        }
    }
}
