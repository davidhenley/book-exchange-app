﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookExchange.Domain.Models
{
     public class BookCategory : BaseEntity
     {
          [Required]
          public string Label;

          public List<Book> Books;
     }
}
