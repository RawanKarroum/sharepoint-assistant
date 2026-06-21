using SPRagAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISharePointDocumentService, FakeSharePointDocumentService>();
builder.Services.AddSingleton<IDocumentChunkingService, DocumentChunkingService>();
builder.Services.AddSingleton<IDocumentChunkStore, InMemoryDocumentChunkStore>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
