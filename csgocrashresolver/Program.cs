using System;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace csgocrashresolver
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Settings> items;
            using (StreamReader r = new StreamReader("C:\\settings.json"))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<Settings>>(json);
            }

            string token = items[0].token;
            string username = "znipedell--";
            username += items[0].port;

            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFF") + " running csgo crash resolver.. \n");
            while (true)
            {
                Thread.Sleep(100);
                Process[] dialogWindow = Process.GetProcessesByName("WerFault");

                // if we have a dialog window close it
                if (dialogWindow.Length > 0)
                {
                    string text = DateTime.Now.ToString("[HH:mm:ss]") + "  crash detected";
                    string url = "https://slack.com/api/chat.postMessage?token=" + token + "&channel=production&text= " + text + "&username=" + username + "&pretty=1";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (!response.StatusCode.ToString().Equals("OK"))
                    {
                        Console.WriteLine("failed to notify on slack!");
                    }

                    // then restart csgo with restartcsgo powershell script
                    ProcessStartInfo processStartInfo = new ProcessStartInfo();
                    processStartInfo.FileName = "powershell.exe";
                    processStartInfo.Arguments = "-ExecutionPolicy ByPass -File C:\\Dropbox\\znipeobserver\\powershell\\obswaitingforgame.ps1";
                    processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    processStartInfo.CreateNoWindow = true;

                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " setting waiting for game..");
                    using (Process obswaitingforgame = Process.Start(processStartInfo))
                    {
                        obswaitingforgame.WaitForExit();
                    }
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " sleeping for 2 seconds to prevent errors..");
                    Thread.Sleep(2000);

                    for (int i = dialogWindow.Length - 1; i >= 0; i--)
                    {
                        while (!dialogWindow[i].HasExited)
                        {
                            dialogWindow[i].CloseMainWindow();
                            Thread.Sleep(100);
                        }
                        Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " closing dialog " + i + "..");
                    }

                    Thread.Sleep(5000);
                    Process[] csgo = Process.GetProcessesByName("csgo");

                    // if the dialog didn't close csgo then close csgo
                    for (int i = csgo.Length - 1; i >= 0; i--)
                    {
                        while (!csgo[i].HasExited)
                        {
                            csgo[i].Kill();
                            Thread.Sleep(100);
                        }
                        Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " killed csgo: " + i);
                    }

                    // restart csgo with restartcsgo powershell script
                    processStartInfo = new ProcessStartInfo();
                    processStartInfo.FileName = "powershell.exe";
                    processStartInfo.Arguments = "-ExecutionPolicy ByPass -File C:\\Dropbox\\znipeobserver\\powershell\\restartcsgo.ps1";
                    processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    processStartInfo.CreateNoWindow = true;

                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " restarting cs go..");
                    using (Process restartcsgo = Process.Start(processStartInfo))
                    {
                        restartcsgo.WaitForExit();
                    }
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " sleeping for 10 seconds to prevent errors..");
                    Thread.Sleep(10000);

                    // set OBS CS scene
                    processStartInfo = new ProcessStartInfo();
                    processStartInfo.FileName = "powershell.exe";
                    processStartInfo.Arguments = "-ExecutionPolicy ByPass -File C:\\Dropbox\\znipeobserver\\powershell\\obscsgo.ps1";
                    processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    processStartInfo.CreateNoWindow = true;

                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " setting obs scene..");
                    using (Process obscsgo = Process.Start(processStartInfo))
                    {
                        obscsgo.WaitForExit();
                    }
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " sleeping for 3 seconds to prevent errors..");
                    Thread.Sleep(3000);
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " csgo restart logic completed. watching for new crash..\n");
                }
            }
        }
    }
    public class Settings
    {
        public string token;
        public string port;
    }
}