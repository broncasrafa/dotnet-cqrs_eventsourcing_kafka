using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Command.Api.Commands;
using Post.Command.Api.Commands.Handlers;
using Post.Command.Domain.Aggregates;
using Post.Command.Infrastructure.Configurations;
using Post.Command.Infrastructure.Dispatchers;
using Post.Command.Infrastructure.Handlers;
using Post.Command.Infrastructure.Repositories;
using Post.Command.Infrastructure.Stores;
using Confluent.Kafka;
using Post.Command.Infrastructure.Producers;
using CQRS.Core.Producers;

var builder = WebApplication.CreateBuilder(args);

builder.Services//.Configure<MongoDbConfig>(builder.Configuration.GetSection("MongoDbConfig"));
    .AddOptions<MongoDbConfig>()
    .BindConfiguration("MongoDbConfig")
    .ValidateDataAnnotations().ValidateOnStart();

builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection("ProducerConfig"));

builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddScoped<IEventProducer, EventProducer>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
builder.Services.AddScoped <ICommandHandler, CommandHandler>();

// register command handler methods
#pragma warning disable ASP0000
var commandHandler = builder.Services.BuildServiceProvider().GetRequiredService<ICommandHandler>();
#pragma warning restore ASP0000
var dispatcher = new CommandDispatcher();
dispatcher.RegisterHandler<AddCommentCommand>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<EditCommentCommand>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<RemoveCommentCommand>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<NewPostCommand>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<EditPostCommand>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<LikePostCommand>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<DeletePostCommand>(commandHandler.HandleAsync);
builder.Services.AddSingleton<ICommandDispatcher>(_ => dispatcher);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/swagger");
            return;
        }
        await next();
    });
}

app.UseHttpsRedirection();



await app.RunAsync();

