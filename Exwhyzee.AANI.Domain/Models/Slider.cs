using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Exwhyzee.AANI.Domain.Models
{
    public class Slider
    {
        public long Id { get; set; }
        public string? Url { get; set; }
        public string? Key { get; set; }

        
        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }

        public bool Show { get; set; }

        [Display(Name = "Title")]
        public string? Title { get; set; }
        public string? MiniTitle { get; set; }

        [Display(Name = "Text")]
        public string? Text { get; set; }

         public string? SliderLinkUrl { get; set; }
         public string? SliderLinkUrlTitle { get; set; }
    }

}
