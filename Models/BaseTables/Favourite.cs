using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmFlow.Models.BaseTables
{
    public class Favourite
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user")]
        public int UserId { get; set; }
        [Column("movie")]
        public int MovieId { get; set; }
    }
}
