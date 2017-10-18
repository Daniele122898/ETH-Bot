using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETH_Bot.Data.Entities.SubEntities
{
    public class Reminder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public DateTime Time { get; set; }
        public string Message { get; set; }
        
        public ulong UserForeignId { get; set; }
        [ForeignKey("UserForeignId")]
        public virtual User User { get; set; }

    }
}