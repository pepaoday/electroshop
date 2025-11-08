namespace WebBanHang.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public User User { get; set; } = null!;
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public decimal TotalAmount { get; set; }


        public decimal Discount { get; set; }      // Lưu số tiền được giảm giá
        public string? VoucherCode { get; set; }   // Lưu mã voucher đã áp dụng
        
        
    }
}