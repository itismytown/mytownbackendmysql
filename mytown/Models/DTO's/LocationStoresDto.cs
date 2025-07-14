namespace mytown.Models.DTO_s
{
    public class LocationStoresDto
    {
        public string Town { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<businessprofile> Stores { get; set; }
    }
}
