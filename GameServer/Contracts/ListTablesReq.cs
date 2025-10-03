namespace GameServer.Contracts
{
    public class ListTablesReq
    {
        public string? Mode { get; set; }
        public int     Limit { get; set; } = 50;
    }
}