using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class OrderHeader
    {

        [Key]
        public int Id { get; set; }



        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }



        [Required]
        public DateTime OrderDate { get; set; }


        [Required]
        public DateTime ShippingDate { get; set; }


        [Required]
        public Double OrderTotal { get; set; }


        public string TrackingNumber { get; set; }

        public string Carrier { get; set; }



        public string OrderStatus { get; set; }

        public string PaymentStatus { get; set; }



        public DateTime PaymentDate { get; set; }

        public DateTime PaymentDueDate { get; set; }


        public string TransactionId { get; set; }



        [Required]
        [MaxLength(15, ErrorMessage = "Number should have a maximum of 15 numbers")]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string StreetAddress { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [Required]
        [MaxLength(50)]
        public string State { get; set; }

        [Required]
        [MaxLength(10)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

    }
}
