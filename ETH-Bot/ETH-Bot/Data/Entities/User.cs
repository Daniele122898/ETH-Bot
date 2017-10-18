using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ETH_Bot.Data.Entities.SubEntities;

namespace ETH_Bot.Data.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong UserId { get; set; }

        public bool Subscribed { get; set; }
        
        public virtual List<Reminder> Reminders { get; set;}
    }
}