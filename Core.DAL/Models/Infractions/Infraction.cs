using System;

namespace Core.DAL.Models.Infractions
{
    public class Infraction : Entity
    {
        
        public string Type { get; set; }
        public string Reason { get; set; }
        public DateTime ExpirationDate { get; set; }
        public ulong ownerid { get; set; }
        public ulong guildid { get; set; }
    }
}