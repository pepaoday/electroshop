using System;
using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime ReviewDate { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}