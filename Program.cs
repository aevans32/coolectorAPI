using CoolectorAPI.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// for Swagger comments
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Fetch secret key from appsettings.Development.json
var secretKey = builder.Configuration["JwtSettings:SecretKey"];

// Use the secret key to create the SymmetricSecurityKey
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

// Add services to the container.
builder.Services.AddControllers();

// Inject UserRepository with connection string from appsettings.json
builder.Services.AddTransient<UserRepository>(provider =>
    new UserRepository(builder.Configuration.GetConnectionString("MyFreeDB")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    // Path to the XML documentation file
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Coolector",
        Version = "v1",
        Description = "API documentation with Swagger"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
