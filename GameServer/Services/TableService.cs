using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameServer.Contracts;

namespace GameServer.Services
{
    public class TableService
    {
        private readonly ConcurrentDictionary<string, TableInfo> _tables = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, (string tableId, string player)> _conn =
            new(); // connId -> (table, player)

        // ---- CREATE ----
        public Task<string> CreateTableAsync(CreateTableReq? req)
        {
            req ??= new CreateTableReq();

            var id = "table-" + Guid.NewGuid().ToString("N")[..8];
            var table = new TableInfo
            {
                Id         = id,
                Mode       = string.IsNullOrWhiteSpace(req.Mode) ? "classic101" : req.Mode.Trim(),
                Capacity   = req.Capacity <= 0 ? 4 : req.Capacity,
                IsPrivate  = req.IsPrivate,
                Password   = req.Password,
                EntryFee   = req.EntryFee,
                IsActive   = true,
                Players    = new List<string>(),
                PlayerCount= 0
            };

            _tables[id] = table;
            return Task.FromResult(id);
        }

        public Task<string> CreateTableSimpleAsync()
        {
            var id = "table-" + Guid.NewGuid().ToString("N")[..8];
            _tables[id] = new TableInfo
            {
                Id         = id,
                Mode       = "classic101",
                Capacity   = 4,
                IsActive   = true,
                Players    = new List<string>(),
                PlayerCount= 0
            };
            return Task.FromResult(id);
        }

        // ---- LIST ----
        public Task<List<TableInfo>> ListTablesAsync(ListTablesReq? req)
        {
            IEnumerable<TableInfo> q = _tables.Values.Where(t => t.IsActive);

            var mode = req?.Mode?.Trim();
            if (!string.IsNullOrWhiteSpace(mode))
                q = q.Where(t => string.Equals(t.Mode, mode, StringComparison.OrdinalIgnoreCase));

            var list = q.OrderBy(t => t.Id)
                        .Take(req?.Limit is > 0 ? req!.Limit : 50)
                        .ToList();

            return Task.FromResult(list);
        }

        // ---- PLAYERS ----
        public Task<string[]> GetPlayersAsync(string tableId)
        {
            if (string.IsNullOrWhiteSpace(tableId))
                return Task.FromResult(Array.Empty<string>());

            if (_tables.TryGetValue(tableId, out var t))
                return Task.FromResult(t.Players.ToArray());

            return Task.FromResult(Array.Empty<string>());
        }

        // ---- JOIN / LEAVE ----
        public Task AddPlayerAsync(string connectionId, string tableId, string player)
        {
            if (!_tables.TryGetValue(tableId, out var t))
                throw new InvalidOperationException("Table not found.");
            if (!t.IsActive)
                throw new InvalidOperationException("Table inactive.");
            if (t.Players.Count >= t.Capacity)
                throw new InvalidOperationException("Table is full.");

            t.Players.Add(player);
            t.PlayerCount = t.Players.Count;

            _conn[connectionId] = (tableId, player);
            return Task.CompletedTask;
        }

        public Task RemovePlayerAsync(string connectionId, string tableId)
        {
            if (_tables.TryGetValue(tableId, out var t))
            {
                if (_conn.TryGetValue(connectionId, out var info))
                {
                    t.Players.Remove(info.player);
                    t.PlayerCount = t.Players.Count;
                }
            }

            _conn.TryRemove(connectionId, out _);
            return Task.CompletedTask;
        }

        // Conn koptuğunda otomatik düş
        public (string tableId, string player)? DropConnection(string connectionId)
        {
            if (_conn.TryRemove(connectionId, out var info))
            {
                if (_tables.TryGetValue(info.tableId, out var t))
                {
                    t.Players.Remove(info.player);
                    t.PlayerCount = t.Players.Count;
                }
                return info;
            }
            return null;
        }
    }
}