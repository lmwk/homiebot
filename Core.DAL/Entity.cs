

using System.ComponentModel.DataAnnotations;

namespace Core.DAL
{
    public abstract class Entity
    {
        [Key]
        public int id { get; set; }
    }
}
