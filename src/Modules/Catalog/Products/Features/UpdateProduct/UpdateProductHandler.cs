using catalog.Data;
using catalog.Products.Dtos;
using catalog.Products.Models;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Products.Features.UpdateProduct
{
    public record UpdateProductCommand(ProductDto product) : IRequest<UpdateProductResult>;

    public record UpdateProductResult(bool isSuccess);
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(c => c.product.Id)
                .NotEmpty().WithMessage("Product id is required.");
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
    public class UpdateProductHandler(CatalogDbContext dbContext) : IRequestHandler<UpdateProductCommand, UpdateProductResult>
    {

         async Task<UpdateProductResult> IRequestHandler<UpdateProductCommand, UpdateProductResult>.Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product = await dbContext.Products.FindAsync([ command.product.Id ], cancellationToken);

            if(product is null)
            {
                throw new Exception($"Product with id {command.product.Id} not found.");
            }
            UpdateProductWithNewValues(product, command.product);

            dbContext.Products.Update(product);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateProductResult(true);
        }
        private void UpdateProductWithNewValues(Product newProduct,ProductDto oldProduct)
        {
            newProduct.Update(
                oldProduct.Name,
                oldProduct.Category,
                oldProduct.Description,
                oldProduct.ImageFile,
                oldProduct.Price
                );
        }

    }
}
