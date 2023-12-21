using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmFlow.Models.BaseTables
{
    public class MovieGenre
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("movieid")]
        public int movieid { get; set; }
        [Column("genreid")]
        public int genreid { get; set; }
        [ForeignKey("genreid")]
        public GenreCollection Genre { get; set; }
    }
}
