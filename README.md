<p align="center">
    <img src="./VLA-demo.gif" alt="Dojo-101" style="width: 600px;" />
</p>

[![License: GNU GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
![GitHub last commit](https://img.shields.io/github/last-commit/Aif4thah/VulnerableLightApp)
[![.NET](https://github.com/Aif4thah/VulnerableLightApp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Aif4thah/VulnerableLightApp/actions/workflows/dotnet.yml)


> ⚠️ **Disclaimer** : This repository, together with its tools, is provided by Taisen-Solutions on an "as is" basis. Be aware that this application is highly vulnerable, including remote command and code execution. Use it at your own risk. Taisen-Solutions makes no representations or warranties of any kind, express or implied, as to the operation of the information, content, materials, tools, services and/or products included on the repository. Taisen-Solution disclaims, to the full extent permissible by applicable law, all warranties, express or implied, including but not limited to, implied warranties of merchantability and fitness for a particular purpose.


## 🎱 Use Case

![UseCase](./VLAusecase.drawio.png)


## 🐞 Vulnerabilities

| CWE | Description | Difficulty | Estimated Bug Bounty Reward |
|----|---|----|---|
| CWE-22 | Path Traversal | Medium | 500-5.000$ |
| CWE-78 | OS Command Injection | Easy | 1.000-10.000$ |
| CWE-79 | Cross-site Scripting | Easy  | 500-5.000$ |
| CWE-89 | SQL Injection | Easy | 1.000-10.000$ |
| CWE-94 | Code Injection| Hard | 5.000-50.000$ |
| CWE-91 | XML Injection | Hard | 0-500$ |
| CWE-98 | Remote File Inclusion | Hard | 1.000-10.000$ |
| CWE-184 | Incomplete List of Disallowed Inputs | Medium | 500-2.000$ |
| CWE-200 | Exposure of Sensitive Information to an Unauthorized Actor | Medium | 1.000-20.000$ | 
| CWE-213 | Exposure of Sensitive Information Due to Incompatible Policies | Easy | 500-2.000$ |
| CWE-284 | Improper Access Control | Medium | 1.000-5.000$ |
| CWE-287 | Improper Authentication | Medium | 500-5.000$ |
| CWE-319 | Cleartext Transmission of Sensitive Information | Very Easy | 500-2.000$ |
| CWE-326 | Inadequate Encryption Strength | Easy  | 1.000-5.000$ |
| CWE-434 | Unrestricted Upload of File with Dangerous Type | Medium | 500-10.000$ |
| CWE-502 | Deserialization of Untrusted Data | Hard  | 5.000-50.000$ |
| CWE-521 | Weak Password Requirements | Very Easy | 500-2.000$ |
| CWE-532 | Insertion of Sensitive Information into Log File | Very Easy  |100-2.000$ |
| CWE 639 | Insecure Direct Object Reference | Medium | 500-5.000$ |
| CWE-611 | Improper Restriction of XML External Entity Reference | Hard | 1.000-10.000$ |
| CWE-787 | Out-of-bounds Write | Easy | 500-5000$ |
| CWE-798 | Use of Hard-coded Credentials | Very Easy | 1.000-10.000$ |
| CWE-829 | Local File Inclusion | Easy | 500-2.000$ |
| CWE-912 | Backdoor | Hard | 10.000$-100.000$ |
| CWE-918 | Server-Side Request Forgery (SSRF) | Medium | 1.000$-10.000$ |
| CWE-1270 | Generation of Incorrect Security Tokens | Medium | 1.000-20.000$ |


## 🏭 Context

VLA is designed as a vulnerable backend application, running in the following environment : 

![Context](./Context.png)


## 🔑 Hint & Write Up

* Try reading [Dojo-101](https://github.com/Aif4thah/Dojo-101), this project contains all you need to hack this app !
* [Buy me a coffee](https://github.com/sponsors/Aif4thah?frequency=one-time&sponsor=Aif4thah) to get the solution/exploit you want
* [Become a sponsor](https://github.com/sponsors/Aif4thah?frequency=recurring&sponsor=Aif4thah) and get a complete Write Up


## ✅ Prerequisites

Check `.csproj` file to get the current dotnet version and install [.NET SDK](https://dotnet.microsoft.com/en-us/download)


## ⬇️ Download

```PowerShell
git clone https://github.com/Aif4thah/VulnerableLightApp.git
cd .\VulnerableLightApp\
```


## 🔧 Build

```PowerShell
dotnet build
```


## 🔥 Run

```PowerShell
.\bin\Debug\net8.0\VulnerableWebApplication.exe [--url=<url>]
```

Alternatively, you can run directly with

```PowerShell
dotnet run
```

## 🛠️ Debug 

### Dotnet Framework

Verify you use the intended .NET Framework

```cmd
where dotnet
dotnet --version
dotnet --list-sdks
```

### Certificates

To trust the certificate

```PowerShell
dotnet dev-certs https --trust
```


### Dependancies

dependancies have to be dowloaded from [standard sources](https://go.microsoft.com/fwlink/?linkid=848054)

```sh
dotnet nuget add source "https://api.nuget.org/v3/index.json" --name "Microsoft"
```

### Misc

* Be aware that VLA runs Linux and MacOS, but is only tested and supported on Windows.

## 💜 Crédits

* **Special thanks to all the hackers and students who pushed me to improve this work**
* Project maintened by [Michael Vacarella](https://github.com/Aif4thah)
