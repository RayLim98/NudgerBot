// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EmptyBot .NET Template version v4.13.1

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmptyBot.Bots;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace EmptyBot.Bots
{
    public class NudgerBot<T>: DialogBot<T>
        where T : Dialog
    {
        public NudgerBot(
            ConversationState conversationState,
            UserState userState,
            T dialog,
            ILogger<DialogBot<T>> logger)
            : base(conversationState,userState,dialog,logger)
        {
        }
        
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello world!"), cancellationToken);
                }
            }
        }
    }
}
