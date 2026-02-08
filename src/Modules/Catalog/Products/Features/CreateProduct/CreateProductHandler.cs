using catalog.Data;
using catalog.Products.Dtos;
using catalog.Products.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Features.CreateProduct
{
    public record CreateProductCommand(ProductDto product) : IRequest<CreateProductResult>;

    public record CreateProductResult(Guid Id);

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(c => c.product.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");
            RuleFor(c => c.product.Description)
                .MaximumLength(500).WithMessage("Product description must not exceed 500 characters.");
            RuleFor(c => c.product.Price)
                .GreaterThan(0).WithMessage("Product price must be greater than zero.");
            RuleFor(c => c.product.ImageFile)
                .NotEmpty().WithMessage("Product image file is required.")
                .Must(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                              file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                              file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Product image file must be a valid image format (.jpg, .jpeg, .png).");
            RuleFor(c => c.product.Category)
                .NotEmpty().WithMessage("At least one product category is required.");
        }
    }
    public class CreateProductHandler(
        CatalogDbContext dbContext
        ) : IRequestHandler<CreateProductCommand, CreateProductResult>
    {

         async Task<CreateProductResult> IRequestHandler<CreateProductCommand, CreateProductResult>.Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            

            var product = CreateNewProduct(command.product);

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new CreateProductResult(product.Id);
        }
        private Product CreateNewProduct(ProductDto productDto)
        {
            return Product.Create(
                Guid.NewGuid(),
                productDto.Name,
                productDto.Category,
                productDto.Description,
                productDto.ImageFile,
                productDto.Price);
        }

    }
}
