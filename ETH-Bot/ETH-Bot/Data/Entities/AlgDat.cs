﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETH_Bot.Data.Entities
{
    public class AlgDat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Name { get; set; }
    }
}