using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmptyBot.Classes;
using EmptyBot.Classes.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EmptyBot.Dialogs
{
  public class MainDialog : ComponentDialog
  {
    protected readonly ILogger _logger;

    // Init json handler to reading json files
    public MainDialog(ILogger<MainDialog> logger, NudgeDialog nudgeDialog)
      : base(nameof(MainDialog))
    {
      _logger = logger;

      // Add a different Dialog
      AddDialog(nudgeDialog);
      AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
      AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
      AddDialog(new TextPrompt(nameof(TextPrompt)));
      AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
      {
        FirstStepAsync,
        IntroductoryStepAsync,
        TransitionStepAsync
      }));

      // The initial child Dialog to run.
      InitialDialogId = nameof(WaterfallDialog);
    }
    private async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
      return await stepContext.PromptAsync(nameof(TextPrompt),
        new PromptOptions
        {
          Prompt = MessageFactory.Text("What is your name?"),
        }, cancellationToken);
    }
    private async Task<DialogTurnResult> IntroductoryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
      var name = (string)stepContext.Result;
      var msg = MessageFactory.Text($"Hello {name}, welcome to the nudge bot! I am currently in the making. blah blah blah");

      await stepContext.Context.SendActivityAsync(msg);

      Thread.Sleep(3000);

      return await stepContext.PromptAsync(nameof(ChoicePrompt),
        new PromptOptions
        {
          Prompt = MessageFactory.Text("These are the following functions in my utility. Choose one and I can show you what I can do UWU"),
          Choices = new List<Choice> {
            new Choice() { Value = "Nudge Dialog", Synonyms = new List<string> {"proactive", "nudge"} },
            new Choice() { Value = "Wellness Check", Synonyms = new List<string> {"health", "mental"} },
            new Choice() { Value = "Today's Weather", Synonyms = new List<string> {"weather"} }
          }
        }, cancellationToken);
    }
    private async Task<DialogTurnResult> TransitionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
      switch (((FoundChoice)stepContext.Result).Value)
      {
        case "Nudge Dialog":
          return await stepContext.BeginDialogAsync(nameof(NudgeDialog), null, cancellationToken);

        case "Wellness Check":
          return await stepContext.BeginDialogAsync(nameof(NudgeDialog), null, cancellationToken);

        case "Today's Weather":
          return await stepContext.BeginDialogAsync(nameof(NudgeDialog), null, cancellationToken);

        default:
          return await stepContext.EndDialogAsync(null, cancellationToken);
      }
    }
  }
}