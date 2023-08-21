# RazorNetworkingApp

This is an ASP.NET Core Razor pages web app with basic networking features I created when I was first learning Razor pages. This app is not meant for production release and has a lot of beginner coding patterns. 

Each page performs a specific networking function:

- Client IPv4/IPv6 address detection
- Simple FTP client sign-in and file viewing/uploading/downloading/deleting with FluentFTP Nuget package and https://dlptest.com/ftp-test FTP test server
- Determine IP address of a specific host by searching via DNS
- Determine geolocation by IP address
- Ping a host name or specific IP address
- Perform WhoIs queries to search who owns a domain, as well as their contact and hosting information
- Display all available WiFi access points and signal strengths detected by the computer hosting this web app
