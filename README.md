<div align="center">

# 🎲 Okey Multiplayer Game Server

<br/>

[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![SignalR](https://img.shields.io/badge/SignalR-WebSockets-008A00?style=for-the-badge&logo=dotnet&logoColor=white)](https://docs.microsoft.com/en-us/aspnet/core/signalr/)

<br/>

An ultra-low latency, scalable backend game server custom-built for the classic tabletop game **Okey**. Leveraging **ASP.NET Core** alongside **SignalR**, this server handles complex game state synchronization, lobby generation, and real-time player interactions flawlessly.

<br/>

[Features](#-features) · [Architecture](#-architecture) · [Initialization](#-initialization)

</div>

<br/>

---

<br/>

## ✨ Features

- **Real-Time WebSockets**: Complete game networking logic powered by SignalR ensuring milliseconds latency round-trips for throwing and drawing tiles.
- **State Matchmaking**: Dynamic lobby rooms allowing users to find tables, sit at empty seats, and broadcast readiness states instantly.
- **Anti-Cheat Validation**: Server-authoritative logic confirming valid rummy melds and preventing invalid throws or draws natively.
- **Resilient Scalability**: Built purely on ASP.NET Core allowing deployment to horizontal cloud environments seamlessly.

<br/>

---

<br/>

## 🏗 Architecture Context

The routing predominantly revolves around long-lived connection hubs rather than traditional HTTP endpoints.

- **GameHub (`/Hubs`)**: Intercepts player actions natively streaming payloads across connected clients inside specific groups.
- **Game Engine**: Internal services calculating Okey tile sets, distributing initial 14-15 tiles, and determining the active indicator tile.

<br/>

---

<br/>

## 🚀 Initialization

### Prerequisites
- .NET SDK (Compatible up to .NET 8)

### Run Sequence

```bash
# Clone and enter the directory
git clone https://github.com/tunacosgun/okey-server.git
cd okey-server

# Restore generic NuGet packages
dotnet restore

# Build strictly to test compilation
dotnet build

# Initiate the Host
dotnet run -p GameServer/GameServer.csproj
```

<br/>

---

<div align="center">
<b>High-performance networking for classic multiplayer experiences.</b>
</div>
