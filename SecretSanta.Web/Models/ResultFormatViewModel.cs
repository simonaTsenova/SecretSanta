using SecretSanta.Models.Enumerations;

namespace SecretSanta.Web.Models
{
    public class ResultFormatViewModel
    {
        public int Skip { get; set; }
        
        public int Take { get; set; }

        public OrderType Order { get; set; }

        public string Search { get; set; }
    }
}