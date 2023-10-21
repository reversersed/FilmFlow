using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmFlow.Models.BaseTables
{
    public class GenreCollection
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name.ru-RU")]
        public string NameRu { get; set; }
        [Column("name.en-US")]
        public string NameEn { get; set; }
    }
}
