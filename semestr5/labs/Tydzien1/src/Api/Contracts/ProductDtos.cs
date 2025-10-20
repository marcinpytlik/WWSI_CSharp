using System.ComponentModel.DataAnnotations;

namespace Api.Contracts
{
    public record ProductReadDto(int Id, string Name, decimal Price, int CategoryId);

    public class ProductCreateDto
    {
        [Required, StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Range(0, 1_000_000)]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }

    public class ProductUpdateDto : ProductCreateDto { }
}
