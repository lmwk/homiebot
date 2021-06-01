namespace Core.DAL.Models.Quotes
{
    public class Quote : Entity
    {
        public string Text { get; set; }
        public ulong owner { get; set; }
    }
}
