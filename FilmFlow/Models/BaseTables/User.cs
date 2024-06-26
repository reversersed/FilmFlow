﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Forms;

namespace FilmFlow.Models.BaseTables
{
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("username")]
        public string Username {  get; set; }
        [Column("password")]
        public byte[]? Password { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("adminrole")]
        public int Admin { get; set; }
        [Column("balance")]
        public int Balance { get; set; }
        [Column("banned")]
        public bool Banned {  get; set; }
        [Column("sessionkey")]
        public byte[]? Session { get; set; }
        [Column("subscription")]
        public int? SubscriptionId { get; set; }
        [ForeignKey(nameof(SubscriptionId))]
        public Subscription? Subscription { get; set; } = null;
    }
}
