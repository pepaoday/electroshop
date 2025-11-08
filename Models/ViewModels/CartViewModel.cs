namespace WebBanHang.Models
{
    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal SubTotal { get; set; }
        public string? AppliedVoucherCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
    }
}