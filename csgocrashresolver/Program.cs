using System;
using System.Diagnostics;
using System.Threading;

namespace csgocrashresolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFF") + " running cs crash resolver.. \n");
            while (true)
            {
                Thread.Sleep(100);
                Process[] dialogWindow = Process.GetProcessesByName("WerFault");    // if we have a dialog window then close it
                if (dialogWindow.Length > 0)                                            
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo();     // then restart csgo with restartcsgo powershell script
                    processStartInfo.FileName = "powershell.exe";
                    processStartInfo.Arguments = "-ExecutionPolicy ByPass -File C:\\Dropbox\\znipeobserver\\powershell\\obswaitingforgame.ps1";
                    processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    processStartInfo.CreateNoWindow = true;

                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " setting waiting for game..");
                    using (Process obswaitingforgame = Process.Start(processStartInfo))
                    {
                        obswaitingforgame.WaitForExit();
                        //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") +  " obswaitingforgame.ps1 exit code: " + restartcsgo.ExitCode);
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
                    Process[] csgo = Process.GetProcessesByName("csgo");            // if the dialog didn't close csgo then close all csgo
                    for (int i = csgo.Length - 1; i >= 0; i--)
                    {
                        while (!csgo[i].HasExited)
                        {
                            csgo[i].Kill();
                            Thread.Sleep(200);
                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " has csgo[" + i + "] exited? " + csgo[i].HasExited);
                        }
                    }

                    processStartInfo = new ProcessStartInfo();     // then restart csgo with restartcsgo powershell script
                    processStartInfo.FileName = "powershell.exe";
                    processStartInfo.Arguments = "-ExecutionPolicy ByPass -File C:\\Dropbox\\znipeobserver\\powershell\\restartcsgo.ps1";
                    processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    processStartInfo.CreateNoWindow = true;

                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " restarting cs go..");
                    using (Process restartcsgo = Process.Start(processStartInfo))
                    {
                        restartcsgo.WaitForExit();
                        //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") +  " restartcsgo.ps1 exit code: " + restartcsgo.ExitCode);
                    }
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " sleeping for 10 seconds to prevent errors..");
                    Thread.Sleep(10000);

                    processStartInfo = new ProcessStartInfo();                  // then set OBS CS scene
                    processStartInfo.FileName = "powershell.exe";
                    processStartInfo.Arguments = "-ExecutionPolicy ByPass -File C:\\Dropbox\\znipeobserver\\powershell\\obscsgo.ps1";
                    processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    processStartInfo.CreateNoWindow = true;

                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " setting obs scene..");
                    using (Process obscsgo = Process.Start(processStartInfo))
                    {
                        obscsgo.WaitForExit();
                        //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " obscsgo.ps1 exit code: " + obscsgo.ExitCode);
                    }
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " sleeping for 3 seconds to prevent errors..");
                    Thread.Sleep(3000);
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:FFF") + " csgo restart logic completed. watching for new crash..\n");
                }
            }
        }
    }
}