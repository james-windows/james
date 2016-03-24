using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace James.Web.Models
{
    public class Workflow
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(4)]
        [RegularExpression(@"^[A-Za-z0-9-_\.öäüÖÄÜß]*$", ErrorMessage = "only letters digit and '-' '_' ';' are allowed")]
        public string Name { get; set; }

        public ApplicationUser Author { get; set; }

        public int? Downloads { get; set; } = 0;

        [MaxLength(100)]
        public string ShortDescription { get; set; }

        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime? PublishDate { get; set; }

        public bool Verified { get; set; } = false;

        public long FileSize { get; set; }

        public Platform Platform { get; set; }
    }
}
