using back_bd;
using back_bd.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "AnimeApp Backend", Version = "v1" });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Pega SOLO el token JWT (sin 'Bearer').",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddOutputCache(op => op.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60));
builder.Services.AddTransient<ISaveFiles, SaveFilesAzure>();

var appSettings = builder.Configuration.GetValue<string>("CORES")!.Split(",");
builder.Services.AddCors(op => op.AddDefaultPolicy(op => op.WithOrigins(appSettings).AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddDbContext<AppDBContext>(op =>
{
    op.UseSqlServer("name=DefaultConnection");
});

builder.Services.AddScoped<IAuthService, AuthService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("La clave secreta JWT (SecretKey) no está configurada.");

byte[] keyBytes;
try { keyBytes = Convert.FromBase64String(secret); }
catch (FormatException) { keyBytes = Encoding.UTF8.GetBytes(secret); }

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = ctx =>
        {
            Console.WriteLine($"JWT auth failed: {ctx.Exception.Message}");
            return Task.CompletedTask;
        },
        OnChallenge = ctx =>
        {
            Console.WriteLine($"JWT challenge: {ctx.Error}; {ctx.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();
app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();

// ✅ Configurar tipos MIME personalizados para videos
var contentTypeProvider = new FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".mkv"] = "video/x-matroska";
contentTypeProvider.Mappings[".mp4"] = "video/mp4";
contentTypeProvider.Mappings[".avi"] = "video/x-msvideo";
contentTypeProvider.Mappings[".webm"] = "video/webm";

// Servir archivos estáticos desde el SSD externo
var videosPath = builder.Configuration["VideoStorage:Path"] ?? "E:\\ANIME_HD";
if (Directory.Exists(videosPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(videosPath),
        RequestPath = "/videos",
        ContentTypeProvider = contentTypeProvider,
        OnPrepareResponse = ctx =>
        {
            var extension = Path.GetExtension(ctx.File.Name).ToLowerInvariant();
            var isVideo = new[] { ".mkv", ".mp4", ".avi", ".webm", ".m4v", ".mov" }.Contains(extension);

            if (isVideo)
            {
                // ✅ CRÍTICO: Indica al navegador que muestre el video inline (no descargue)
                ctx.Context.Response.Headers.Append("Content-Disposition", "inline");
                
                // ✅ Habilitar streaming de videos (range requests)
                ctx.Context.Response.Headers.Append("Accept-Ranges", "bytes");
                
                // Cache moderado para videos
                ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=3600");
            }
        }
    });
    
    Console.WriteLine($"✅ Sirviendo videos desde: {videosPath} en ruta /videos");
}
else
{
    Console.WriteLine($"⚠️ ADVERTENCIA: No se encontró el directorio de videos: {videosPath}");
}

app.MapControllers();
app.Run();
