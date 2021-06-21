using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyBook.Models
{
    public class CoverType
    {

        [Key]
        [Required]
        public int Id { get; set; }


        [Required]
        [MaxLength(50)]
        [DisplayName("Cover Type")]
        public string Name { get; set; }



    }
}
