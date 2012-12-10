using BankerLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppBanker
{
    class Program
    {
        static void Main(string[] args)
        {
            //TextWriter newOut = new StreamWriter(@"d:\record.txt");
            //System.Console.SetOut(newOut);
            //DateTime start = DateTime.Now;
            //TimeSpan tsStart = new TimeSpan(start.Ticks);

            BankerControl bCon = new BankerControl(10);
            bCon.ShowConsoleAvaliable();
            bCon.ShowConsoleNeed();

            while (bCon.UnDoneCount != 0)
            {
                Console.ReadKey(true);

                if (bCon.Request())
                {
                    Console.WriteLine("Request sucessfully!!");      
                }
                else
                {
                    Console.WriteLine("Request failed!!");
                }

                bCon.ShowConsoleAvaliable();
                bCon.ShowConsoleRequest();
                bCon.ShowConsoleNeed();
                while (bCon.ExcuteQueue())
                {
                    Console.ReadKey(true);
                    Console.WriteLine("---------------------------in waiting queue-----------------------");
                    bCon.ShowConsoleAvaliable();
                    bCon.ShowConsoleRequest();
                    bCon.ShowConsoleNeed();
                }
            }
            bCon.ShowConsoleAvaliable();

            //DateTime end = DateTime.Now;
            //TimeSpan tsEnd = new TimeSpan(end.Ticks);
            //TimeSpan final = tsEnd.Subtract(tsStart).Duration();
            //StreamWriter sw = new StreamWriter(Console.OpenStandardOutput());
            //TextWriter consoleOut = Console.Out;
            //System.Console.SetOut(sw);
            //Console.WriteLine(final.TotalSeconds);
            //newOut.Close();
            //sw.Close();

            Console.ReadKey(true);
        }
    }
}
