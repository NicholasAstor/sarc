namespace sarc;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // DI existente
        builder.Services.AddScoped<Sarc.Repository.Interface.IHelloRepository, Sarc.Repository.HelloRepository>();
        builder.Services.AddScoped<Sarc.Service.Interface.IHelloService, Sarc.Service.HelloService>();

        // Controllers
        builder.Services.AddControllers();

        // >>> Swagger UI (Swashbuckle)
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // (Opcional) Se quiser manter apenas a UI do Swagger, PARE de usar AddOpenApi/MapOpenApi
        // builder.Services.AddOpenApi(); // <-- REMOVA/COMENTE isto se quiser só o Swagger UI

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            // >>> Ativa JSON + UI do Swagger
            app.UseSwagger();
            app.UseSwaggerUI();

            // Se você mantiver o AddOpenApi/MapOpenApi, isso só expõe o JSON em /openapi/v1.json
            // app.MapOpenApi(); // <-- REMOVA/COMENTE para evitar confusão
        }

        // Se não tiver HTTPS configurado, deixe comentado para evitar redirecionamento quebrado
        // app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        // Qualidade de vida: "/" abre o Swagger
        app.MapGet("/", () => Results.Redirect("/swagger"));

        app.Run();
    }
}
