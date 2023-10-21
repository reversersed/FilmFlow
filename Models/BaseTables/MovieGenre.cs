using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmFlow.Models.BaseTables
{
    public class MovieGenre
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("movieid")]
        private int movieid { get; set; }
        [ForeignKey("movieid")]
        public Movie Movie { get; set; }
        [Column("genreid")]
        private int genreid { get; set; }
        [ForeignKey("genreid")]
        public GenreCollection Genre { get; set; }
    }
}
