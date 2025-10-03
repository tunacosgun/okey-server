cat > README.md << 'EOF'
# okey-server

ASP.NET Core + SignalR backend.

## Geliştirme
```bash
dotnet --version
dotnet restore
dotnet build
dotnet run -p GameServer/GameServer.csproj
---

## 3) (İsteğe bağlı) SSH yerine HTTPS kullanmaya devam et
- Sen HTTPS ile push’ladın; sorun yok.
- Anlık olarak bir şey yapman gerekmiyor. (SSH anahtarı eklemek istersen sonra ekleriz.)

---

## 4) Repo görünürlüğü + erişim
- Repo **private** ise müşteriyi “**Settings → Collaborators**”tan davet et.
- “Actions”/CI istemiyorsan şimdilik eklemene gerek yok.

---

## 5) Sonraki adım: VPS’e yayın
SSH erişimi açılır açılmaz (müşteri port/SSH’yi açınca) şunu yapacağız:
```bash
# sunucuda
sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0
git clone https://github.com/tunacosgun/okey-server.git
cd okey-server
dotnet publish GameServer/GameServer.csproj -c Release -o out

# test çalıştırma
cd out
ASPNETCORE_URLS=http://0.0.0.0:5020 dotnet GameServer.dll
