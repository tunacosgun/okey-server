using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using GameServer.Services;
using GameServer.Contracts;

namespace GameServer.Hubs
{
    public class GameHub : Hub
    {
        private readonly TableService _tables;

        public GameHub(TableService tables) => _tables = tables;

        // ---- CREATE ----
        // Tek resmi isim: "CreateTable"
        public Task<string> CreateTable(CreateTableReq req)
            => _tables.CreateTableAsync(req);

        // İKİNCİ İMZAYI YENİ İSİMLE AYIRDIK: "CreateTable2"
        public Task<string> CreateTable2(string mode, int capacity)
            => _tables.CreateTableAsync(new CreateTableReq { Mode = mode, Capacity = capacity });

        // Basit versiyon tek başına zaten farklı isimdeydi
        public Task<string> CreateTableSimple()
            => _tables.CreateTableSimpleAsync();

        // ---- JOIN / LEAVE ----
        public async Task JoinTable(string tableId, string player)
        {
            await _tables.AddPlayerAsync(Context.ConnectionId, tableId, player);
            await Groups.AddToGroupAsync(Context.ConnectionId, tableId);

            await Clients.Caller.SendAsync("JoinTableOk", tableId);
            await PushPlayers(tableId);
        }

        public async Task LeaveTable(string tableId)
        {
            await _tables.RemovePlayerAsync(Context.ConnectionId, tableId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, tableId);
            await PushPlayers(tableId);
        }

        // ---- CHAT (opsiyonel) ----
        public Task SendMessage(string tableId, string user, string message)
            => Clients.Group(tableId).SendAsync("ReceiveMessage", user, message);

        // ---- LISTS ----
        public Task<List<TableInfo>> ListTables(ListTablesReq req)
            => _tables.ListTablesAsync(req);

        public Task<string[]> GetPlayers(string tableId)
            => _tables.GetPlayersAsync(tableId);

        // ---- CONNECTION LIFECYCLE ----
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var info = _tables.DropConnection(Context.ConnectionId);
            if (info is { } x)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, x.tableId);
                await PushPlayers(x.tableId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        // ---- HELPERS ----
        private async Task PushPlayers(string tableId)
        {
            var players = await _tables.GetPlayersAsync(tableId);
            // Client tarafı bunu dinliyor: _conn.On<string, string[]>("PlayersChanged", ...)
            await Clients.Group(tableId).SendAsync("PlayersChanged", tableId, players);
        }
    }
}