using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using SimpleBot.Models;
using SimpleBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBot.Bots
{
    public class GreetingBot : ActivityHandler
    {
        private readonly BotStateService _botStateService;

        public GreetingBot(BotStateService botStateService)
        {
            _botStateService = botStateService;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await GetName(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await GetName(turnContext, cancellationToken);
                }
            }
        }

        private async Task GetName(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.userProfileAccessor.GetAsync(turnContext, () => new UserProfile());
            ConversationData conversationData = await _botStateService.conversationDataAccessor.GetAsync(turnContext, () => new ConversationData());
            if (!string.IsNullOrEmpty(userProfile.Name))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(string.Format("Hi {0}, How can I help you today?", userProfile.Name)), cancellationToken);
            }
            else
            {
                if (conversationData.PromptedForUserName)
                {
                    userProfile.Name = turnContext.Activity.Text?.Trim();

                    await turnContext.SendActivityAsync(MessageFactory.Text(String.Format("Thanks {0}. How can I help you today?", userProfile.Name)), cancellationToken);

                    conversationData.PromptedForUserName = false;
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"What is your name?"), cancellationToken);
                    conversationData.PromptedForUserName = true;
                }
            }   

            await _botStateService.userProfileAccessor.SetAsync(turnContext, userProfile);
            await _botStateService.conversationDataAccessor.SetAsync(turnContext, conversationData);

            await _botStateService.UserState.SaveChangesAsync(turnContext);
            await _botStateService.ConversationState.SaveChangesAsync(turnContext);
        }
    }
}
