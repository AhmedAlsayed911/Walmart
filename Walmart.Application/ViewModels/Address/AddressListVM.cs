namespace Walmart.Application.ViewModels.Address
{
    public class AddressListVM
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string ZIP { get; set; }
        public bool IsDefault { get; set; }
    }
}
