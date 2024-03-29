﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceAssignment.DAL.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Subject CRN (Company Registration Number), IČO in Czech
        /// </summary>
        [Display(Name = "CRN")]
        [Required]
        [MaxLength(8)]
        public string Crn { get; set; }
        /// <summary>
        /// Company VAT, DIČ in Czech
        /// </summary>
        [Display(Name = "VAT")]
        [Required]
        [MaxLength(10)]
        public string Vat { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        [MaxLength(255)]
        public string Address { get; set; }
    }
}
