using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace duelfighteronline.Models
{
    public class DuelHistory
    {
        [Key]
        public int ID { get; set; }

        public int CharacterInfoID { get; set; }
        public string Initiator { get; set; }
        public string Target { get; set; }
        public string Result { get; set; }
        public string Details { get; set; }
    }
}