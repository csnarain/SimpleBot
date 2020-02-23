using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;

namespace SimpleBot.Services
{

    public class BotServices
    {
        public BotServices(IConfiguration configuration)
        {
            Dispatch = new LuisRecognizer(new LuisApplication(
                configuration["LuisAppId"],
                configuration["LuisAPIKey"],
                $"https://{configuration["LuisAPIHostName"]}.api.cognitive.microsoft.com"),
                new LuisPredictionOptions {IncludeAllIntents = true, IncludeInstanceData = true}, true);
        }

        public LuisRecognizer Dispatch { get; private set; }
    }
}
