using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EmptyBot.Classes;
using EmptyBot.Classes.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EmptyBot.Dialogs
{
    public class NudgeDialog : ComponentDialog
    {
        // Init Json handler 
        JsonHandler jsonHandler = new JsonHandler(); 

        // Init Random class for random selection of items
        Random rnd = new Random();

        // Init collection type
        IMongoCollection<BsonDocument> _collection;

        //Declare data path
        string dataPath = Path.GetFullPath(@"Data");
        public NudgeDialog()
            : base(nameof(NudgeDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ShowChoiceMoodStepAsync,
                GetValueStepAsync,
                PromptYesNoStepAsync,
                SendQouteStepAsync,
                DisplayActivityStepAsync,
                ProactiveStepAsync,
                EndStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
            }
            private async Task<DialogTurnResult> ShowChoiceMoodStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                Prompt = MessageFactory.Text("How are you feeling today?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Happy", "Sad", "Angry" }),
                }, cancellationToken);
            }
            private async Task<DialogTurnResult> GetValueStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
            stepContext.Values["mood"] = ((FoundChoice)stepContext.Result).Value;
            return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
            }
            private async Task<DialogTurnResult> PromptYesNoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
            return await stepContext.PromptAsync(nameof(ConfirmPrompt),
                new PromptOptions {Prompt = MessageFactory.Text("Would you like a qoute?"),}, cancellationToken);
            }
            private async Task<DialogTurnResult> SendQouteStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
            string filePath="";
            if((bool)stepContext.Result) {
                switch ((string)stepContext.Values["mood"])
                {
                case "Happy":
                    filePath = Path.Combine(dataPath,"Quotes/Happy.json");
                    break;
                case "Sad":
                    filePath = Path.Combine(dataPath,"Quotes/Sad.json");
                    break;
                case "Angry":
                    filePath = Path.Combine(dataPath,"Quotes/Angry.json");
                    break;
                default:
                    Console.WriteLine("nothing has been chosen");
                    break;
                }
                
                // Set Data selection
                this.jsonHandler.SetJson(filePath);
                List<Quote> listQoutes = jsonHandler.DeserializeJson();
                int genRand = rnd.Next(listQoutes.Count);
                Quote item = listQoutes[genRand];
                string msg = item.Sentence;
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);

                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt= MessageFactory.Text("How do you feel about your qoute? Did it resonant with you?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "Share", "Nop" }),
                }, cancellationToken);
            }
            //assign value is now sent to user.
            return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
            }
            private async Task<DialogTurnResult> DisplayActivityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
            stepContext.Values["choiceAction"] = ((FoundChoice)stepContext.Result).Value;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("These are your following activites:"), cancellationToken);

            // QUERY DATA FROM DATA BASE
            // TO BE IMPLEMENTED
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("<Insert activity list here>"), cancellationToken);
            return await stepContext.PromptAsync(nameof(ConfirmPrompt),
                new PromptOptions {
                Prompt = MessageFactory.Text("Would you like to have a reminder for these activites?"), }, 
                cancellationToken);
            }
            private async Task<DialogTurnResult> ProactiveStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
            if((bool)stepContext.Result)
            {
                // Sends Proactive message
                return await stepContext.BeginDialogAsync(nameof(NudgeDialog),null,cancellationToken);
            }
            return await stepContext.NextAsync(null, cancellationToken);
            }
            private async Task<DialogTurnResult> EndStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
            return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
            }
    }
}