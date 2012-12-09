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
            TextWriter newOut = new StreamWriter(@"d:\record.txt");
            System.Console.SetOut(newOut);
            BankerControl bCon = new BankerControl(20);
            bCon.ShowConsoleNeed();
            DateTime start = DateTime.Now;
            TimeSpan tsStart = new TimeSpan(start.Ticks);
            while (bCon.UnDoneCount != 0)
            {

                bCon.ShowConsoleAvaliable();
                if (bCon.Request())
                {
                    Console.WriteLine("Request is Found!!");      
                }
                else
                {
                    Console.WriteLine("Request is not Found!!");
                }
                bCon.ShowConsoleRequest();
                bCon.ShowConsoleNeed();
                while (bCon.ExcuteQueue())
                {
                    Console.WriteLine("Queue:  ----------------------------------------------");
                    bCon.ShowConsoleAvaliable();
                    bCon.ShowConsoleRequest();
                    bCon.ShowConsoleNeed();
                    
                }
            }
            DateTime end = DateTime.Now;
            TimeSpan tsEnd = new TimeSpan(end.Ticks);
            TimeSpan final = tsEnd.Subtract(tsStart).Duration();
            StreamWriter sw = new StreamWriter(Console.OpenStandardOutput());
            TextWriter consoleOut = Console.Out;
            System.Console.SetOut(sw);
            Console.WriteLine(final.TotalSeconds);
            newOut.Close();
            sw.Close();
            Console.ReadKey(true);
        }
    }
}
