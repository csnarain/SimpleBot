using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using SimpleBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBot.Services
{
    public class BotStateService
    {
        public UserState UserState { get; }
        public ConversationState ConversationState { get; }
        public string UserProfileId { get; set; } = $"{nameof(BotStateService)}.UserProfile";
        public string ConversationDataId { get; set; } = $"{nameof(BotStateService)}.ConversationData";
        public string DialogStateId { get; } = $"{nameof(BotStateService)}.DialogState";
        public IStatePropertyAccessor<UserProfile> userProfileAccessor { get; set; }
        public IStatePropertyAccessor<ConversationData> conversationDataAccessor { get; set; }
        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }
        public BotStateService(UserState userState, ConversationState conversationState)
        {
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            InitializeAccessors();
        }

        private void InitializeAccessors()
        {
            userProfileAccessor = UserState.CreateProperty<UserProfile>(UserProfileId);
            conversationDataAccessor = ConversationState.CreateProperty<ConversationData>(ConversationDataId);
            DialogStateAccessor = ConversationState.CreateProperty<DialogState>(DialogStateId);
        }
    }
}
