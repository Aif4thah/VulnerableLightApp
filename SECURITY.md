# Security Policy

Despite my efforts to add some mitigations, VLA is designed to contain critical vulnerabilities.
Run it at your own risk.

## Tracability

A simple way to keep some logs is to redirect console output into a file :

```powershell
.\VulnerableWebApplication.exe >> C:\Users\<UserName>\log.txt
```