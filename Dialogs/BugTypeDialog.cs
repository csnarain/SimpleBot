using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using SimpleBot.Helpers;
using SimpleBot.Services;

namespace SimpleBot.Dialogs
{

    public class BugTypeDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;
        private readonly BotServices _botServices;
        private BotStateService botStateService;

        public BugTypeDialog(string dialogId, BotStateService botStateService, BotServices botServices): base(dialogId)
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(BotStateService));
            _botServices = botServices ?? throw new System.ArgumentNullException(nameof(BotServices));

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepcontext, CancellationToken cancellationtoken)
        {
            var result = await _botServices.Dispatch.RecognizeAsync(stepcontext.Context, cancellationtoken);
            if (result.Properties["luisResult"] is LuisResult luisResult)
            {
                var entities = luisResult.Entities;
                foreach (var entity in entities)
                {
                    if (Common.BugTypes.Any(s => s.Equals(entity.Entity, StringComparison.OrdinalIgnoreCase)))
                    {
                        await stepcontext.Context.SendActivityAsync(
                            MessageFactory.Text(String.Format("Yes! {0} is a Bug Type!", entity.Entity)),
                            cancellationtoken);
                    }
                    else
                    {
                        await stepcontext.Context.SendActivityAsync(
                            MessageFactory.Text(String.Format("No! {0} is not a Bug Type!", entity.Entity)),
                            cancellationtoken);
                    }
                }
            }
            else
            {
                throw new System.ArgumentNullException(nameof(BotServices.Dispatch.RecognizeAsync));
            }

            return await stepcontext.NextAsync(null, cancellationtoken);

        }

        private Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepcontext, CancellationToken cancellationtoken)
        {
            throw new NotImplementedException();
        }
    }
}
