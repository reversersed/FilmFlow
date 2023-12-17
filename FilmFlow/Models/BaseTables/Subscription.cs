using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Forms;

namespace FilmFlow.Models.BaseTables
{
    public class Subscription
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("price")]
        public int Price { get; set; }
        [Column ("startdate")]
        public DateTime StartDate { get; set; }
        [Column ("enddate")]
        public DateTime EndDate { get; set; }
        [Column("user")]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? user { get; set; }
        public ObservableCollection<SubscriptionGenre> SubGenre { get; set; }
    }
}
