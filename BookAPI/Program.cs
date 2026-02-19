using BookAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<BookService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        builder.WithOrigins("http://localhost:4200", "http://localhost:57482", "http://localhost:61494")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.MapControllers();

app.Run();
