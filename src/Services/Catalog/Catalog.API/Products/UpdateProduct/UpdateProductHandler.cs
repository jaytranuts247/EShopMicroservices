using Catalog.API.Exceptions;

namespace Catalog.API.Products.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price)
    : ICommand<UpdateProductResult>;

public record UpdateProductResult(bool IsSuccess);

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(2, 150)
            .WithMessage("Name must be between 2 and 150 characters");
        // RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required");
        // RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
        // RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is required");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}

public class UpdateProductCommandHandler(IDocumentSession session, ILogger<UpdateProductCommandHandler> logger)
    : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("UpdateProductCommandHandler.Handle called with {@Command}", command);
        var products = await session.LoadAsync<Product>(command.Id, cancellationToken);

        if (products is null) throw new ProductNotFoundException(command.Id);

        products.Name = command.Name;
        products.Category = command.Category;
        products.Description = command.Description;
        products.ImageFile = command.ImageFile;
        products.Price = command.Price;

        session.Update(products);
        await session.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult(true);
    }
}
