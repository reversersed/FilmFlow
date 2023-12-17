using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmFlow.Models.BaseTables
{
    public class Replenishment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("value")]
        public int Value { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("user")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
