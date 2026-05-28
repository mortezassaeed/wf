using WorkflowRBC.Blazor.Components;
using WorkflowRBC.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<WorkflowApiClient>((serviceProvider, client) =>
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var apiBaseUrl = configuration["WorkflowApi:BaseUrl"] ?? "http://localhost:5246";
        client.BaseAddress = new Uri(apiBaseUrl);
    })
    .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
    {
        var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var handler = new HttpClientHandler
        {
            UseProxy = false
        };

        if (environment.IsDevelopment())
        {
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        }

        return handler;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
