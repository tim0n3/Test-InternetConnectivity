# Define the website to test
$websiteToTest = "www.google.com"
$websiteProtocol = "https://"

# Define the path for the log file
$logFilePath = "C:\Users\TimForbes\Downloads\InternetConnectivityReport2.txt"

# Get the current date and time for logging
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

# Function to check internet connectivity
function Test-InternetConnection {
    try {
        $result = Test-Connection -ComputerName $websiteToTest -Count 1 -ErrorAction Stop
        return $result.ResponseTime -ne $null
    }
    catch {
        return $false
    }
}

# Function to test DNS resolution
function Test-DNSResolution {
    try {
        $dnsResult = [System.Net.Dns]::GetHostAddresses($websiteToTest)
        return $dnsResult.Count -gt 0
    }
    catch {
        return $false
    }
}

# Function to count hops to the website
function Get-HopCount {
    $path = Test-Path -Path "C:\Windows\System32\tracert.exe"
    if ($path -eq $true) {
        $result = tracert $websiteToTest
        $lines = $result -split '\n'
        return ($lines.Count - 2)  # Subtract 2 for header lines
    }
    else {
        return "Tracert not available on this system."
    }
}

# Function to emulate a browser connection
function Test-BrowserConnection {
    try {
        $webRequest = [System.Net.WebRequest]::Create("$websiteProtocol$websiteToTest")
        $webRequest.Timeout = 5000  # Set a timeout in milliseconds
        $webResponse = $webRequest.GetResponse()
        $webResponse.Close()
        return $true
    }
    catch {
        return $false
    }
}

# Check internet connectivity
$internetConnected = Test-InternetConnection

# Check DNS resolution
$dnsResolved = Test-DNSResolution

# Get the hop count
$hopCount = Get-HopCount

# Emulate a browser connection
$browserConnected = Test-BrowserConnection

# Create a report
$report = @"
Internet Connectivity Report
Timestamp: $timestamp

Internet Connectivity Status: $(if ($internetConnected) { "Connected" } else { "Disconnected" })
DNS Resolution Status: $(if ($dnsResolved) { "Resolved" } else { "Not Resolved" })
Hop Count to $websiteToTest - $hopCount
Browser Connection Status: $(if ($browserConnected) { "Connected" } else { "Disconnected" })
"@

# Log the report to a file
$report | Out-File -FilePath $logFilePath -Append

# Display the report in the console
Write-Host $report

# Check if internet is disconnected and suggest troubleshooting steps
if (-not $internetConnected) {
    Write-Host "Troubleshooting Steps:"
    Write-Host "1. Check your network cable or Wi-Fi connection."
    Write-Host "2. Restart your router or modem."
    Write-Host "3. Disable and re-enable your network adapter."
    Write-Host "4. Contact your Internet Service Provider (ISP) for assistance."
}