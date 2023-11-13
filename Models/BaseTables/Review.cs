using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmFlow.Models.BaseTables
{
    public class Review
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("movie")]
        public int MovieId { get; set; }
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }
        [Column("user")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        [Column("text")]
        public string ReviewText { get; set; }
        [Column("rating")]
        public float Rating { get; set; }
        [Column("written")]
        public DateTime WriteDate { get; set; }
    }
}
