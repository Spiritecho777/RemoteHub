# RemoteHub

**RemoteHub** is a lightweight cross-platform Remote Desktop and RemoteApp manager for Windows and Linux.

Designed for system administrators, IT teams, and power users, RemoteHub provides a centralized interface to manage and launch remote connections without maintaining dozens of `.rdp` files or command-line shortcuts.

![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Linuxework](https://img.shields.io/badge/.NET-C%23-purple)
![UI](https://img.shields.io/badge/UI-Avalonia-blueviolds.io/badge/License-MIT-green

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

## Screenshots

> Add screenshots here

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

## Requirements

### Windows

- Windows 10 / 11
- Remote Desktop Client (`mstsc.exe`)

### Linux

- FreeRDP (`xfreerdp`)

Example installation:

```bash
sudo apt install freerdp2-x11
```

## Technology Stack

- C#
- .NET
- Avalonia UI
- Windows Credential Manager
- FreeRDP

## Roadmap

- [ ] Connection groups
- [ ] Search and filtering
- [ ] Import / Export profiles
- [ ] Multi-language support
- [ ] Secure vault integration
- [ ] Connection statistics

## Build

```bash
git clone https://github.com/Spiritecho777/RemoteHub.git
cd RemoteHub
dotnet build
```

## License

This project is licensed under the MIT License.

## Contributing

Contributions, bug reports, and feature requests are welcome.

If you have suggestions or improvements, feel free to open an issue or submit a pull request.

---

Built with ❤️ using Avalonia and .NET.
