# Internet Connectivity Tester

This PowerShell script, `InternetConnectivityTester.ps1`, allows you to check the status of your internet connectivity, DNS resolution, the number of hops to a website, and perform a network speed test. The script provides a comprehensive report, which can be logged to a file and displayed in the console.

## Prerequisites

Before using this script, ensure that you have:

1. PowerShell installed on your system.
2. An active internet connection.
3. Administrative privileges (for some operations).

## Usage

1. Download the script: You can download the script from this repository. Save it as `InternetConnectivityTester.ps1`.

2. Open PowerShell: Open a PowerShell terminal with administrative privileges. To do this, right-click on the PowerShell icon and select "Run as administrator."

3. Navigate to the script directory: Use the `cd` (Change Directory) command to navigate to the directory where you saved `InternetConnectivityTester.ps1`.

4. Execute the script: Run the script by entering the following command:

   ```powershell
   .\InternetConnectivityTester.ps1
   ```

   If you encounter an error related to script execution policy, you may need to change the execution policy. To do this, run the following command:

   ```powershell
   Set-ExecutionPolicy RemoteSigned
   ```

   Afterward, you can run the script again.

5. View the report: The script will execute various tests and display a report in the console. The report includes information about your internet connectivity, DNS resolution, the number of hops to the specified website, and network speed test results (if HTTPS test succeeds).

6. Log the report (optional): The report will also be logged to a file named `InternetConnectivityReport.txt` in your Downloads folder.

## Troubleshooting

If the script indicates that your internet is disconnected, it suggests troubleshooting steps, including:

1. Check your network cable or Wi-Fi connection.
2. Restart your router or modem.
3. Disable and re-enable your network adapter.
4. Contact your Internet Service Provider (ISP) for assistance.

## Note

- The script performs a network speed test using Speedtest CLI. If the `speedtest.exe` tool is not available in the script's directory, it will be downloaded automatically.
- You can customize the website to test by modifying the `$websiteToTest` and `$websiteProtocol` variables at the beginning of the script.
- The report includes a timestamp to help you keep track of when the tests were performed.

Enjoy using the `InternetConnectivityTester` script to monitor your internet connectivity and diagnose potential issues.

## License

This script is provided under the [MIT License](LICENSE). Feel free to use and modify it as needed.

## Donations

If you want to show your appreciation, you can donate via [PayPal](https://www.paypal.com/donate?hosted_button_id=ULMMXE4DLQVZS) . Thanks!