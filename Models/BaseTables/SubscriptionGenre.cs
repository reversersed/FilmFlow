using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Forms;

namespace FilmFlow.Models.BaseTables
{
    public class SubscriptionGenre
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("genreid")]
        public int GenreId { get; set; }
        [ForeignKey(nameof(GenreId))]
        public GenreCollection Genre {  get; set; }
        [Column("subscription")]
        public int SubscriptionId { get; set; }
        [ForeignKey(nameof(SubscriptionId))]
        public Subscription Subscription { get; set; }
    }
}
