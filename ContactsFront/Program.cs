using ContactsFront.Api;
using ContactsFront.Components;
using ContactsFront.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);
Uri _backendURI = new Uri(builder.Configuration["ConnectionStrings:Url"] ?? "");

#if DEBUG
if (_backendURI.ToString() == string.Empty)
    _backendURI = new Uri("https://localhost:7154");
#endif

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<AuthMessageHandler>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthorizationCore();

builder.Services.AddHttpClient<ContactsApiClient>(client =>
{
    client.BaseAddress = _backendURI;
})
.AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = _backendURI;
});

builder.Services.AddHttpClient<CategoryService>(client => {
	client.BaseAddress = _backendURI;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
