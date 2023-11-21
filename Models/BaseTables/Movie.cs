using System;
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
        [Column("year")]
        public int? Year { get; set; }
        [Column("cover")]
        public string Cover { get; set; }
        [Column("url")]
        public string? Url { get; set; }
        public ICollection<MovieGenre> Genre { get; set; } = new List<MovieGenre>();
        public ICollection<MovieCountry> Country { get; set; } = new List<MovieCountry>();
        [Column("metadata")]
        private int MetadataId;
        [ForeignKey(nameof(MetadataId))]
        public MovieMetaData Metadata { get; set; }
        [Column("rating")]
        public float Rating { get; set; }
    }
}
