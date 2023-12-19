# Vulnerable Web Apps

[![License: GNU GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/Aif4thah/Dojo-101/pulls)
[![Donate Ethereum](https://img.shields.io/badge/ETH-donate-4E8EE9.svg?style=flat-square&logo=ethereum)](https://etherscan.io/address/0xcC424e30Ff6eEAb4E6B3A900c5446038F858b314)
[![Buy me a coffee](https://img.shields.io/badge/buy%20me%20a-coffee-yellow)](https://www.buymeacoffee.com/taisensolutions)

Vulnerable API for educational purposes

## Disclaimer

This repository, together with its tools, is provided by Taisen-Solutions on an “as is” basis. Taisen-Solutions makes no representations or warranties of any kind, express or implied, as to the operation of the information, content, materials, tools, services and/or products included on the repository. Taisen-Solution disclaims, to the full extent permissible by applicable law, all warranties, express or implied, including but not limited to, implied warranties of merchantability and fitness for a particular purpose.

## Contact

If you have any questions about this project feel free to reach out :

 [![Site Web](https://img.shields.io/badge/Website-3776AB?style=for-the-badge)](https://taisen.fr)
 [![Linkedin](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/mvacarella/)
 [![Email](https://img.shields.io/badge/Email-8B89CC?style=for-the-badge&logo=protonmail&logoColor=white)](mailto:michael.vacarella@protonmail.com)

## Vulnerabilities

| CWE | Description | Difficulty | Hint |
|----|---|----|---|
| CWE-23 | Relative Path Traversal | Medium | Bypass the filter |
| CWE-78 | Improper Neutralization of Special Elements used in an OS Command ('OS Command Injection') | Easy | Get the user |
| CWE-79 | Improper Neutralization of Input During Web Page Generation ('Cross-site Scripting') | Easy | Pop the admin |
| CWE-89 | Improper Neutralization of Special Elements used in an SQL Command ('SQL Injection') | Easy | Login without creds |
| CWE-94 | Improper Control of Generation of Code ('Code Injection') | Hard | Write in server console |
| CWE-213 | Exposure of Sensitive Information Due to Incompatible Policies | Very Easy | Read the docs |
| CWE-284 | Improper Access Control | Medium | Bring your own token |
| CWE-287 | Improper Authentication | Medium | go ahead |
| CWE-319 | Cleartext Transmission of Sensitive Information | Very Easy | Sniff |
| CWE-326 | Inadequate Encryption Strength | Easy | Code review |
| CWE-434 | Unrestricted Upload of File with Dangerous Type | Easy | What can go wrong ? |
| CWE-502 | Deserialization of Untrusted Data | Hard | Change NTFS permission |
| CWE-521 | Weak Password Requirements | Very Easy | null |
| CWE-532 | Insertion of Sensitive Information into Log File | Very Easy | Nothing to hide|
| CWE 639 | Insecure Direct Object Reference | Medium | Enumeration is key |
| CWE-611 | Improper Restriction of XML External Entity Reference | Hard | Windows.ini |
| CWE-787 | Out-of-bounds Write | Easy | Don't crash the service |
| CWE-798 | Use of Hard-coded Credentials | Very Easy | Code review |
| CWE-829 | Inclusion of Functionality from Untrusted Control Sphere (Local file Inclusion) | Medium | Read |
| CWE-918 | Server-Side Request Forgery (SSRF) | Medium | choose your domain |
| CWE-1104 | Use of Unmaintained Third Party Components | Easy | old stuff |
| CWE-1270 | Generation of Incorrect Security Tokens | Medium | Code review |

## Diagram

![Class](./ClassesDiagram.png)

## install dotnet

Check `.csproj` file to get the current dotnet version

### windows

https://dotnet.microsoft.com/en-us/download

### Linux

```sh
sudo apt-get update -y 
sudo apt purge -y dotnet-sdk-8.0 aspnetcore-runtime-8.0 dotnet-runtime-8.0
sudo apt autoremove -y
wget "https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb" -O packages-microsoft-prod.deb
chmod +x packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt install dotnet-sdk-8.0 -y
```

## certificate

To trust the certificate (Windows and macOS only) run 

```PowerShell
dotnet dev-certs https --trust
```

For more information on configuring HTTPS [Read the doc](https://go.microsoft.com/fwlink/?linkid=848054).

## run the vulnerable apps

```sh
dotnet run
```

## request

```sh
curl -k https://localhost:3000/
```