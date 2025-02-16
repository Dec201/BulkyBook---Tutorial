﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class Company
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }


        [DisplayName("Street Address")]
        public string StreetAddress { get; set; }



        public string City { get; set; }



        public string State { get; set; }


        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }


        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }


        [Required]
        public bool IsAuthorizedCompany { get; set; }



    }
}
