using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmFlow.Models.BaseTables
{
    public class Cover
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("url")]
        public string Url { get; set; }
    }
}
