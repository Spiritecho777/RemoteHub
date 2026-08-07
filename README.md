# RemoteHub

**RemoteHub** is a lightweight cross-platform Remote Desktop and RemoteApp manager for Windows and Linux.

Designed for system administrators, IT teams, and power users, RemoteHub provides a centralized interface to manage and launch remote connections without maintaining dozens of `.rdp` files or command-line shortcuts.

## Features

- Manage multiple RDP connections from a single interface
- Support for Microsoft Remote Desktop Services (RDS)
- Launch full Remote Desktop sessions
- Launch published RemoteApps
- Secure local storage of connection profiles
- Credential integration on Windows
- Custom icons for each connection
- Audio redirection support
- Cross-platform support (Windows & Linux)
- Lightweight and modern Avalonia UI

## How It Works

RemoteHub stores connection profiles containing:

- Display name
- Server address
- Username
- Password
- RemoteApp executable (optional)
- Connection features
- Custom icon

### Windows

RemoteHub:

1. Generates a temporary `.rdp` file
2. Stores credentials in Windows Credential Manager
3. Launches the session using `mstsc.exe`
4. Cleans up credentials and temporary files when the session ends

### Linux

RemoteHub uses `xfreerdp` to launch remote sessions with the corresponding connection parameters.

## Use Cases

- Remote Desktop administration
- Microsoft RDS environments
- Published enterprise applications
- ERP and business software access
- Infrastructure management
- Homelab and personal servers

### Windows

- Windows 10 / 11
- Remote Desktop Client (`mstsc.exe`)

### Linux

- FreeRDP (`xfreerdp`)

## Technology Stack

- C#
- .NET
- Avalonia UI
- Windows Credential Manager
- FreeRDP

## License

This project is licensed under the MIT License.

---

Built with ❤️ using Avalonia and .NET.
