using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmFlow.Models.BaseTables
{
    public class Movie
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name.ru-RU")]
        public string NameRu { get; set; }
        [Column("name.en-US")]
        public string NameEn { get; set; }
        [Column("description.ru-RU")]
        public string? DescriptionRu { get; set; }
        [Column("description.en-US")]
        public string? DescriptionEn { get; set; }
        [Column("price")]
        public int Price { get; set; }
        [Column("year")]
        public int Year { get; set; }
        //Cover foreign key
        [Column("coverid")]
        private int coverid { get; set; }
        [ForeignKey("coverid")]
        public Cover Cover { get; set; }
        //Movie Url foreign key
        [Column("url")]
        public int url { get; set; }
        [ForeignKey("url")]
        public MovieUrl Url { get; set; }
        //Genres foreign key
        public ICollection<MovieGenre> Genre { get; set; } = new List<MovieGenre>();
    }
}
