using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class MessageTemplateCategory
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public ICollection<MessageTemplateContent> MessageTemplateContents { get; set; }
    }
}
