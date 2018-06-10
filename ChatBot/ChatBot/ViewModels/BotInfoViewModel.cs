using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBot.ViewModels
{
    public class BotInfoViewModel
    {
        [Required]
        public string BotName { get; set; }

        [Required]
        public string BotOAuth { get; set; }
    }
}
