using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSLManagerWPF
{
    class ProcessOps
    {


        public static void exportDistro(string distro, string path)
        {
            RunCmdCommand("C:\\Windows\\System32\\wsl.exe"," --export " + distro + " " +path);
        }


        public static void setDefaultDistro(string distro)
        {
            RunCmdCommand("C:\\Windows\\System32\\wsl.exe", " --set-default " + distro);
        }


        public static void terminateDistro(string distro)
        {
            RunCmdCommand("C:\\Windows\\System32\\wsl.exe", "  --terminate " + distro);
        }

        public static void unregisterDistro(string distro)
        {
            RunCmdCommand("C:\\Windows\\System32\\wsl.exe", "   --unregister " + distro);
        }

        public static void changeDistroVersion(string distro, int version)
        {
            RunCmdCommand("C:\\Windows\\System32\\wsl.exe", "   --set-version " + distro+ " " + version);

        }

        public static void installDistro(string distro, string tarpath, string installpath, int version)
        {
            RunCmdCommand("C:\\Windows\\System32\\wsl.exe", "  --import " + distro + " " + installpath + " " + tarpath + " --version "+version );
        }

        public static void runDistro(string distro)
        {
            runCmdCommandSeparated("C:\\Windows\\System32\\wsl.exe", "   -d " + distro);
        }
        public static void runCmdCommandSeparated(string fname, string args)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = fname;
            startInfo.Arguments = args;
            process.StartInfo = startInfo;
            bool processStarted = process.Start();
        }


        public static void RunCmdCommand(string fname, string args)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = fname;
            startInfo.Arguments = args;
            process.StartInfo = startInfo;
            bool processStarted = process.Start();
        }


        public static void RunProgram(string commandToExecute)
        {
            Console.WriteLine(commandToExecute);
            // Execute wsl command:
            using (var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"cmd.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true,
                }
            })
            {
                proc.Start();
                proc.StandardInput.WriteLine(commandToExecute);
                proc.StandardInput.Flush();
                proc.StandardInput.Close();
                proc.WaitForExit();
            }
        }

    }
}
