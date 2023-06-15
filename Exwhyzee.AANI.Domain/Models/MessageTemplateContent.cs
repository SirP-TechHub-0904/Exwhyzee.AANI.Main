using Exwhyzee.AANI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class MessageTemplateContent
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public GenderStatus GenderStatus { get; set; }

        public long MessageTemplateCategoryId { get; set; }
        public MessageTemplateCategory MessageTemplateCategory { get; set; }
    }
}
