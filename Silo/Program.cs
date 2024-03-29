/// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Orleans.Hosting;
using Orleans.Providers;

/// Copyright (c) Microsoft. All rights reserved.
await Host.CreateDefaultBuilder(args)
.UseOrleans(
        (context, builder) =>
        {
            builder.AddMemoryStreams<DefaultMemoryMessageBodySerializer>("MemoryStreams");
            builder.AddMemoryGrainStorage("PubSubStore");

#if DEBUG
            builder.UseLocalhostClustering()
                    .AddMemoryGrainStorage("adventure")
                    .AddMemoryGrainStorage("adventureLog")
                    .AddMemoryGrainStorage("rooms")
                    .AddMemoryGrainStorage("players")
                    .AddMemoryGrainStorage("monster")
                    .AddMemoryGrainStorage("shopping-cart");
#else
            builder.UseCosmosClustering(
                configureOptions: options =>
                {
                    options.DatabaseName = "AdventureGame";
                    options.ContainerName = "Cluster";
                    options.ConfigureCosmosClient(context.Configuration["CosmosConnectionString"]);
                })
            .AddMemoryGrainStorage("shopping-cart");
            builder.AddCosmosGrainStorage(
                name: "adventure",
                configureOptions: options =>
                {
                    options.DatabaseName = "AdventureGame";
                    options.ContainerName = "Adventure";
                    options.ConfigureCosmosClient(context.Configuration["CosmosConnectionString"]);
                });
            builder.AddCosmosGrainStorage(
                name: "adventureLog",
                configureOptions: options =>
                {
                    options.DatabaseName = "AdventureGame";
                    options.ContainerName = "AdventureLog";
                    options.ConfigureCosmosClient(context.Configuration["CosmosConnectionString"]);
                });
            builder.AddCosmosGrainStorage(
                name: "rooms",
                configureOptions: options =>
                {
                    options.DatabaseName = "AdventureGame";
                    options.ContainerName = "Rooms";
                    options.ConfigureCosmosClient(context.Configuration["CosmosConnectionString"]);
                });
            builder.AddCosmosGrainStorage(
                name: "players",
                configureOptions: options =>
                {
                    options.DatabaseName = "AdventureGame";
                    options.ContainerName = "Players";
                    options.ConfigureCosmosClient(context.Configuration["CosmosConnectionString"]);
                });
            builder.AddCosmosGrainStorage(
                name: "monster",
                configureOptions: options =>
                {
                    options.DatabaseName = "AdventureGame";
                    options.ContainerName = "monster";
                    options.ConfigureCosmosClient(context.Configuration["CosmosConnectionString"]);
                });
#endif
        })
    .ConfigureWebHostDefaults(
        webBuilder => webBuilder.UseStartup<Startup>())
    .RunConsoleAsync();