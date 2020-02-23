using Microsoft.Bot.Builder.Dialogs;
using SimpleBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Configuration;

namespace SimpleBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;
        private readonly BotServices _botServices;

        public MainDialog(BotStateService botStateService, BotServices botServices) : base(nameof(MainDialog))
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            _botServices = botServices ?? throw new System.ArgumentNullException(nameof(botServices));

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };

            AddDialog(new GreetingDialog($"{nameof(MainDialog)}.greeting", _botStateService));
            AddDialog(new BugReportDialog($"{nameof(MainDialog)}.bugReport", _botStateService));
            AddDialog(new BugTypeDialog($"{nameof(MainDialog)}.bugType", _botStateService, _botServices));
            AddDialog(new WaterfallDialog($"{nameof(MainDialog)}.mainFlow", waterfallSteps));

            InitialDialogId = $"{nameof(MainDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var recognizerResult = await _botServices.Dispatch.RecognizeAsync(stepContext.Context, cancellationToken);

            var topIntent = recognizerResult.GetTopScoringIntent();

            switch (topIntent.intent)
            {
                case "GreetingIntent":
                    return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.greeting", null,
                        cancellationToken);
                case "NewBugReportIntent":
                    return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugReport", null,
                        cancellationToken);
                case "QueryBugTypeIntent":
                    return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugType", null, cancellationToken);
                default:
                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.Text($"I'm sorry I don't know what you mean"), cancellationToken);
                break;
            }

            return await stepContext.NextAsync(null, cancellationToken);

            //if (Regex.Match(stepContext.Context.Activity.Text.ToLower(), "hi").Success)
            //{
            //    return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.greeting", null, cancellationToken);
            //}
            //else
            //{
            //    return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugReport", null, cancellationToken);
            //}
        }
    }
}
