using ProductApi.Domain.Entities;

namespace ProductApi.Domain.Dtos.Conversions;

public static class ProductConversions
{
    public static Product ToEntity(ProductDto product)
        => new()
        {
            Id = product.Id,
            ProductName = product.Name,
            Price = product.Price,
            Quantity = product.Quantity
        };

    public static (ProductDto?, IEnumerable<ProductDto>?) FromEntity(Product product,
        IEnumerable<Product>? products)
    {
        if (product is not null || products is null)
        {
            var singleProduct = new ProductDto(product!.Id, product.ProductName,
                product.Quantity, product.Price);

            return (singleProduct, null);
        }

        if (product is null || products is not null)
        {
            var _products = products.Select(p =>
                new ProductDto(p.Id, p.ProductName, p.Quantity, p.Price)).ToList();

            return (null, _products);
        }

        return (null, null);
    }
}