var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCarter();
var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("CatalogDatabase")!);
    // opts.AutoCreateSchemaObjects = AutoCreate.All;
    // opts.Schema.For<Product>().Index(x => x.Id, x => x.Unique = true);
}).UseLightweightSessions();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapCarter();

// app.UseExceptionHandler(exceptionHandlerApp =>
// {
//     exceptionHandlerApp.Run(async context =>
//     {
//         var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
//         if (exception is null) return;
//
//         var problemDetails = new ProblemDetails
//         {
//             Title = exception.Message,
//             Status = StatusCodes.Status500InternalServerError,
//             Detail = exception.StackTrace
//         };
//
//         var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
//         logger.LogError(exception, exception.Message);
//
//         context.Response.StatusCode = StatusCodes.Status500InternalServerError;
//         context.Response.ContentType = "application/problem+json";
//
//         await context.Response.WriteAsJsonAsync(problemDetails);
//     });
// });
// app.MapGet("/", () => "Hello World!");

app.Run();
