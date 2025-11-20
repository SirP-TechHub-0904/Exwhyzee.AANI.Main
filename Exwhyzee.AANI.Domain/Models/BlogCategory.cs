using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class BlogCategory
    {
        
        public long Id { get; set; }
        public string Title { get; set; }
        public bool Show { get;set;}
        public ICollection<Blog> Blog { get; set; }
    }
}