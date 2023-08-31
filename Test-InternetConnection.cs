using System;
using System.IO;
using System.Net;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        // Define the website to test
        string websiteToTest = "www.google.com";
        string websiteProtocol = "https://";

        // Define the path for the log file
        string logFilePath = "C:\\Path\\To\\InternetConnectivityReport.txt";

        // Get the current date and time for logging
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Function to check internet connectivity
        bool TestInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead(websiteToTest))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // Function to test DNS resolution
        bool TestDNSResolution()
        {
            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(websiteToTest);
                return addresses.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        // Function to count hops to the website
        int GetHopCount()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("tracert", websiteToTest);
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;

                Process process = new Process();
                process.StartInfo = psi;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string[] lines = output.Split('\n');

                // Subtract 2 for header lines
                return lines.Length - 2;
            }
            catch
            {
                return -1; // Tracert not available or failed
            }
        }

        // Check internet connectivity
        bool internetConnected = TestInternetConnection();

        // Check DNS resolution
        bool dnsResolved = TestDNSResolution();

        // Get the hop count
        int hopCount = GetHopCount();

        // Create a report
        string report = $@"
Internet Connectivity Report
Timestamp: {timestamp}

Internet Connectivity Status: {(internetConnected ? "Connected" : "Disconnected")}
DNS Resolution Status: {(dnsResolved ? "Resolved" : "Not Resolved")}
Hop Count to {websiteToTest}: {hopCount}
";

        // Log the report to a file
        File.AppendAllText(logFilePath, report);

        // Display the report in the console
        Console.WriteLine(report);

        // Check if internet is disconnected and suggest troubleshooting steps
        if (!internetConnected)
        {
            Console.WriteLine("Troubleshooting Steps:");
            Console.WriteLine("1. Check your network cable or Wi-Fi connection.");
            Console.WriteLine("2. Restart your router or modem.");
            Console.WriteLine("3. Disable and re-enable your network adapter.");
            Console.WriteLine("4. Contact your Internet Service Provider (ISP) for assistance.");
        }
    }
}
