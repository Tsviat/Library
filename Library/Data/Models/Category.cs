﻿using System.ComponentModel.DataAnnotations;

namespace Library.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }

//    ⦁	Has Id – a unique integer, Primary Key
//⦁	Has Name – a string with min length 5 and max length 50 (required)
//⦁	Has Books – a collection of type Books

}
