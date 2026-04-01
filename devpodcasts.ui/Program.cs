var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSpaProxy();
}
else
{
    app.UseHsts();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

// Redirect root to index.html if not already handled
app.MapGet("/", (context) => 
{
    context.Response.Redirect("/index.html");
    return Task.CompletedTask;
});

app.MapFallbackToFile("index.html");

app.Run();
