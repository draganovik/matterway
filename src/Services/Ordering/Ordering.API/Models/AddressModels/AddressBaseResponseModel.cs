namespace Ordering.API.Models.AddressModels
{
    public class AddressBaseResponseModel
    {
        public Guid Id { get; set; }
        public string? ReceiverName { get; set; }
        public string? Residence { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public string? Note { get; set; }
    }
}
