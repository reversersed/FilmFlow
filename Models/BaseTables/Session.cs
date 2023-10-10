using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmFlow.Models.BaseTables
{
    public class Session
    {
        [Key]
        [Column("id")]
        public int Id {  get; set; }
        [Column("userid")]
        public int UserId { get; set; }
        [Column("sessionkey", TypeName = "bytea")]
        public byte[] SessionKey { get; set; }
    }
}
