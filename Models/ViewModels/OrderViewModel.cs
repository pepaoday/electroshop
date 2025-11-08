namespace WebBanHang.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public List<CartViewModel> CartItems { get; set; } = new List<CartViewModel>();


        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;

      

    }
}
