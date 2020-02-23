// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SimpleBot.Bots;
using SimpleBot.Dialogs;
using SimpleBot.Services;
using System;

namespace SimpleBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            //Configure the State
            ConfigureState(services);
            ConfigureDialogs(services);

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, DialogBot<MainDialog>>();

            
        }

        private void ConfigureDialogs(IServiceCollection services)
        {
            services.AddSingleton<MainDialog>();
        }

        public void ConfigureState(IServiceCollection services)
        {
            // Initialize the Bot Memory Store
            var storageAccount = "DefaultEndpointsProtocol=https;AccountName=eutxbotstore;AccountKey=CT4jtwLF8AtKq6L8BR3idnAkV6hfjU6wjFJ3z/xAt7d8k1KvtUJcGowA+HKB+J6UQ0zl21jncIZdpmMCpTymBA==;EndpointSuffix=core.windows.net";
            var storageContainer = "eutxbotstore";
            services.AddSingleton<IStorage>(new AzureBlobStorage(storageAccount, storageContainer));
            //services.AddSingleton<IStorage, MemoryStorage>();
            //Create the User State
            services.AddSingleton<UserState>();

            //Create the Conversation State
            services.AddSingleton<ConversationState>();

            //Create an instance of the state service
            services.AddSingleton<BotStateService>();
            services.AddSingleton<BotServices>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
