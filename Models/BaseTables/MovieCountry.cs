using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmFlow.Models.BaseTables
{
    public class MovieCountry
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("movie")]
        public int MovieId { get; set; }
        [ForeignKey(nameof(MovieId))]
        public Movie Movie { get; set; }
        [Column("country")]
        public int CountryId { get; set; }
        [ForeignKey(nameof(CountryId))]
        public CountryCollection Country { get; set; }
    }
}
