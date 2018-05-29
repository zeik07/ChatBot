using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBot.ViewModels
{
    public class StreamInfoViewModel
    {
        [Required]
        public string Game { get; set; }

        [Required]
        public string Title { get; set; }

        public List<string> Communities { get; set; }
    }
}
