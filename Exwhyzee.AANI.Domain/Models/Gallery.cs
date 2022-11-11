using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class Gallery
    {
        public Gallery()
        {
            Date = DateTime.UtcNow.AddHours(1);
            CodeView = DateTime.UtcNow.AddHours(1).ToString("ddmmyyyyhhmmsstt");
        }
      
        public long Id { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public bool DontShow { get; set; }
        public DateTime Date { get; set; }
        public string CodeView { get; set; }
    }
}
