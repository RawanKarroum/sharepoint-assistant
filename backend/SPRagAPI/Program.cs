using Microsoft.EntityFrameworkCore;
using SPRagAPI.Data;
using SPRagAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddSingleton<ISharePointDocumentService, FakeSharePointDocumentService>();
builder.Services.AddSingleton<GraphOneDriveDocumentService>();
builder.Services.AddSingleton<ISharePointDocumentService>(sp =>
    sp.GetRequiredService<GraphOneDriveDocumentService>());
builder.Services.Configure<GraphOneDriveOptions>(
    builder.Configuration.GetSection(GraphOneDriveOptions.SectionName));
builder.Services.AddSingleton<IFileTextExtractionService, FileTextExtractionService>();
builder.Services.AddSingleton<IDocumentChunkingService, DocumentChunkingService>();

var ragDb = builder.Configuration.GetConnectionString("RagDb");
if (!string.IsNullOrWhiteSpace(ragDb))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(ragDb, sqlOptions =>
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null)));
    builder.Services.AddScoped<IDocumentChunkStore, SqlDocumentChunkStore>();
}
else
{
    builder.Services.AddSingleton<IDocumentChunkStore, InMemoryDocumentChunkStore>();
}

builder.Services.AddScoped<IChunkSearchService, KeywordChunkSearchService>();
builder.Services.Configure<OpenAiOptions>(
    builder.Configuration.GetSection(OpenAiOptions.SectionName));
builder.Services.AddSingleton<IAiAnswerService, OpenAiAnswerService>();
builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddCors(options =>
    options.AddPolicy("frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("frontend");

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
