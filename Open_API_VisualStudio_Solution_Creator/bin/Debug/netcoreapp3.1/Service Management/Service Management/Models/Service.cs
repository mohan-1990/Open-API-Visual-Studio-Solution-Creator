using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Service_Management.Database;
using Microsoft.EntityFrameworkCore;

namespace Service_Management.Models
{
    public class Service
    {        
        [Key]
        public Guid ServiceId { get; set; }
        /* Fill in the rest of the keys */

        [NotMapped]
        public class Dto
        { 
            /* Add or remove more HTTP Verbs as required */

            public class Get
            {
                /* Fill in fields as required */
            }

            public class Post
            {
                /* Fill in fields as required */
            }
        }

        [NotMapped]
        public class Operations
        {
            /* Fill in CRUD Operations for this model as required */
        }
    }
}
