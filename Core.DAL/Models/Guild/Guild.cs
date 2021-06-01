namespace Core.DAL.Models.Guild
{
    public class Guild : Entity
    {
        public ulong guildid { get; set; }
        public string name { get; set; }
        public ulong muteroleid { get; set; }
        public ulong novcroleid { get; set; }
        public ulong botbanroleid { get; set; }
        public int usercount { get; set; }
    }
}