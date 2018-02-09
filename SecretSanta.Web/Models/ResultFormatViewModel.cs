using SecretSanta.Models.Enumerations;

namespace SecretSanta.Web.Models
{
    public class ResultFormatViewModel : PagingViewModel
    {
        public OrderType Order { get; set; }

        public string Search { get; set; }
    }
}