using Newtonsoft.Json;

namespace PROG3270_GroupProject.Domain.Entities
{
    public class Product
    {
        [JsonProperty("id")]
        public int ProductID { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("category")]
        public string? Category { get; set; }

        [JsonProperty("image")]
        public string? Image { get; set; }


    }
}
