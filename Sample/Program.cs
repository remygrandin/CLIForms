using System.Diagnostics;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Process.Start("cmd.exe", "/c Powershell.exe -executionpolicy bypass -File Sample.ps1");
        }
    }
}
