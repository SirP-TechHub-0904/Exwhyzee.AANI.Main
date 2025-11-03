using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Domain.Models
{
    public class OperationYear
    {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Term Name (e.g., 2022-2024 Administration)")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }   
        
        // --- NEW PROPERTY ---
        public bool IsActive { get; set; }
    }
}
