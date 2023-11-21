using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmFlow.Models.BaseTables
{
    public class MovieMetaData
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("tagline.ru-RU")]
        public string? TaglineRu { get; set; }
        [Column("tagline.en-US")]
        public string? TaglineEn { get; set; }
        [Column("director.ru-RU")]
        public string? DirectorRu { get; set; }
        [Column("director.en-US")]
        public string? DirectorEn { get; set; }
        [Column("screenwriter.ru-RU")]
        public string? ScreenwriterRu { get; set; }
        [Column("screenwriter.en-US")]
        public string? ScreenwriterEn { get; set; }
        [Column("producer.ru-RU")]
        public string? ProducerRu { get; set; }
        [Column("producer.en-US")]
        public string? ProducerEn { get; set; }
        [Column("videographer.ru-RU")]
        public string? VideographerRu { get; set; }
        [Column("videographer.en-US")]
        public string? VideographerEn { get; set; }
        [Column("composer.ru-RU")]
        public string? ComposerRu { get; set; }
        [Column("composer.en-US")]
        public string? ComposerEn { get; set; }
        [Column("drawer.ru-RU")]
        public string? DrawerRu { get; set; }
        [Column("drawer.en-US")]
        public string? DrawerEn { get; set; }
        [Column("montage.ru-RU")]
        public string? MontageRu { get; set; }
        [Column("montage.en-US")]
        public string? MontageEn { get; set; }
        [Column("budget")]
        public int? Budget { get; set; }
        [Column("collected")]
        public int? Collected { get; set; }
        [Column("premier")]
        public DateTime? Premier { get; set; }
        [Column("age")]
        public int? Age { get; set; }
    }
}
