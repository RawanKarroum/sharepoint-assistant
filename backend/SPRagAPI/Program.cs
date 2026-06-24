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
builder.Services.AddSingleton<IDocumentChunkStore, InMemoryDocumentChunkStore>();
builder.Services.AddSingleton<IChunkSearchService, KeywordChunkSearchService>();
builder.Services.Configure<OpenAiOptions>(
    builder.Configuration.GetSection(OpenAiOptions.SectionName));
builder.Services.AddSingleton<IAiAnswerService, OpenAiAnswerService>();
builder.Services.AddSingleton<IChatService, ChatService>();

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
