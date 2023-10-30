﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Scridon_Grigore_Lab2.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int BookID { get; set; }
        public DateTime OrderDate { get; set; }
        public Customer Customer { get; set; }
        public Book Book { get; set; } 
        
     

    }
}