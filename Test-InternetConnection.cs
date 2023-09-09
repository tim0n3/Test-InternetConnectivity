using System;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

class Program
{
    static string websiteToTest = "www.google.com";
    static string websiteProtocol = "https://";

    static string logFileName = "InternetConnectivityReport.txt";
    static string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", logFileName);

    static string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    static async Task Main()
    {
        bool internetConnected = await TestInternetConnectionAsync();
        bool dnsResolved = TestDNSResolution();
        bool browserConnected = await TestBrowserConnectionAsync();

        string networkSpeedResult = browserConnected ? await TestNetworkSpeedAsync() : "Network speed test skipped due to HTTPS test failure.";

        string report = $@"
Internet Connectivity Report
Timestamp: {timestamp}

Internet Connectivity Status: {(internetConnected ? "Connected" : "Disconnected")}
DNS Resolution Status: {(dnsResolved ? "Resolved" : "Not Resolved")}
Hop Count to {websiteToTest}: {GetHopCount()}
Browser Connection Status: {(browserConnected ? "Connected" : "Disconnected")}
Network Speed Test: {networkSpeedResult}
";

        File.AppendAllText(logFilePath, report);
        Console.WriteLine(report);

        if (!internetConnected)
        {
            Console.WriteLine("Troubleshooting Steps:");
            Console.WriteLine("1. Check your network cable or Wi-Fi connection.");
            Console.WriteLine("2. Restart your router or modem.");
            Console.WriteLine("3. Disable and re-enable your network adapter.");
            Console.WriteLine("4. Contact your Internet Service Provider (ISP) for assistance.");
        }
    }

    static async Task<bool> TestInternetConnectionAsync()
    {
        try
        {
            using (Ping ping = new Ping())
            {
                PingReply reply = await ping.SendPingAsync(websiteToTest, 1000); // 1000ms timeout
                return reply.Status == IPStatus.Success;
            }
        }
        catch
        {
            return false;
        }
    }

    static bool TestDNSResolution()
    {
        try
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(websiteToTest);
            return hostEntry.AddressList.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    static int GetHopCount()
    {
        try
        {
            Process tracertProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "tracert",
                    Arguments = websiteToTest,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            tracertProcess.Start();
            string output = tracertProcess.StandardOutput.ReadToEnd();
            tracertProcess.WaitForExit();
            return output.Split('\n').Length - 2; // Subtract 2 for header lines
        }
        catch
        {
            return -1; // Tracert not available
        }
    }

    static async Task<bool> TestBrowserConnectionAsync()
    {
        try
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromMilliseconds(5000); // 5000ms timeout
                HttpResponseMessage response = await httpClient.GetAsync($"{websiteProtocol}{websiteToTest}");
                return response.IsSuccessStatusCode;
            }
        }
        catch
        {
            return false;
        }
    }

    static async Task<string> TestNetworkSpeedAsync()
    {
        string speedtestExePath = "speedtest.exe";

        if (!File.Exists(speedtestExePath))
        {
            Console.WriteLine("Downloading Speedtest CLI...");
            string zipFileUrl = "https://install.speedtest.net/app/cli/ookla-speedtest-1.2.0-win64.zip";
            string zipFilePath = "speedtest.zip";
            using (HttpClient client = new HttpClient())
            {
                byte[] zipData = await client.GetByteArrayAsync(zipFileUrl);
                File.WriteAllBytes(zipFilePath, zipData);
            }
            System.IO.Compression.ZipFile.ExtractToDirectory(zipFilePath, ".");
            File.Delete(zipFilePath);
        }

        try
        {
            Process speedtestProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = speedtestExePath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = "--accept-gdpr --accept-license"
                }
            };
            speedtestProcess.Start();
            string output = speedtestProcess.StandardOutput.ReadToEnd();
            speedtestProcess.WaitForExit();

            Regex downloadRegex = new Regex("Download:\\s+(\\d+\\.\\d+)\\s+Mbps");
            Regex uploadRegex = new Regex("Upload:\\s+(\\d+\\.\\d+)\\s+Mbps");

            string downloadSpeed = downloadRegex.Match(output).Groups[1].Value;
            string uploadSpeed = uploadRegex.Match(output).Groups[1].Value;

            return $"Download Speed: {downloadSpeed} Mbps, Upload Speed: {uploadSpeed} Mbps";
        }
        catch
        {
            return "Speed test failed";
        }
    }
}
