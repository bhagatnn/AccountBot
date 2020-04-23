// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System.Data.SqlClient;
using System.Text;

using AccountBot.CognitiveModels;

namespace AccountBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly AccountOpeningRecognizer _luisRecognizer;
        protected readonly ILogger Logger;

        //private readonly Microsoft.Recognizers.Text.Config.IConfiguration configuration;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(AccountOpeningRecognizer luisRecognizer,AccountOpeningDialog accountOpeningDialog, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(accountOpeningDialog);
            AddDialog(new AccountStatusDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Use the text provided in FinalStepAsync or the default if it is the first time.
            var messageText = stepContext.Options?.ToString() ?? "How can I help you today?\nType something like \"Open an account\"";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                // LUIS is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
                return await stepContext.BeginDialogAsync(nameof(AccountOpeningDialog), new AccountDetails(), cancellationToken);
            }

            // Call LUIS and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
            var luisResult = await _luisRecognizer.RecognizeAsync<AccountOpening>(stepContext.Context, cancellationToken);
            switch (luisResult.TopIntent().intent)
            {
                case AccountOpening.Intent.NewApplication:
                    // Run the BookingDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.
                    return await stepContext.BeginDialogAsync(nameof(AccountOpeningDialog),null, cancellationToken);

                case AccountOpening.Intent.Greeting:
                    var welcomeMessageText = $"Hello. I am AccountBot.";
                    var welcomeMessage = MessageFactory.Text(welcomeMessageText, welcomeMessageText, InputHints.ExpectingInput);
                    await stepContext.Context.SendActivityAsync(welcomeMessage, cancellationToken);
                    break;

                case AccountOpening.Intent.StatusCheck:
                    return await stepContext.BeginDialogAsync(nameof(AccountStatusDialog), null, cancellationToken);

                case AccountOpening.Intent.Cancel:
                    var cancelMessageText = $"Thank you for reaching out to AccountBot!!!  \n Please reach out to Helpdesk for any other information.";
                    var cancelMessage = MessageFactory.Text(cancelMessageText, cancelMessageText, InputHints.ExpectingInput);
                    await stepContext.Context.SendActivityAsync(cancelMessage, cancellationToken);
                    return await stepContext.CancelAllDialogsAsync(cancellationToken);

                default:
                    //Catch all for unhandled intents
                    var didntUnderstandMessageText = $"Sorry, I didn't get that. Please try asking in a different way (intent was {luisResult.TopIntent().intent})";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private Task ShowWarningForUnsupportedCities(ITurnContext context, AccountOpening luisResult, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        // Shows a warning if the requested From or To cities are recognized as entities but they are not in the Airport entity list.
        // In some cases LUIS will recognize the From and To composite entities as a valid cities but the From and To Airport values
        // will be empty if those entity values can't be mapped to a canonical item in the Airport.
        //private static async Task ShowWarningForUnsupportedCities(ITurnContext context, AccountOpening luisResult, CancellationToken cancellationToken)
        //{
        //    var unsupportedCities = new List<string>();

        //    var fromEntities = luisResult.FromEntities;
        //    if (!string.IsNullOrEmpty(fromEntities.From) && string.IsNullOrEmpty(fromEntities.Airport))
        //    {
        //        unsupportedCities.Add(fromEntities.From);
        //    }

        //    var toEntities = luisResult.ToEntities;
        //    if (!string.IsNullOrEmpty(toEntities.To) && string.IsNullOrEmpty(toEntities.Airport))
        //    {
        //        unsupportedCities.Add(toEntities.To);
        //    }

        //    if (unsupportedCities.Any())
        //    {
        //        var messageText = $"Sorry but the following airports are not supported: {string.Join(',', unsupportedCities)}";
        //        var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
        //        await context.SendActivityAsync(message, cancellationToken);
        //    }
        //}

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            if (stepContext.Result is AccountDetails result)
            {
                
                var messageText = $"Your application for \" New Account \" creation is submitted.  \n " +
                    $"Once you application is approved, you will receive communication through Email.  \n" +
                    $"You can check you application status through chat or reach out to our helpdesk or walk-in to our nearest Branch.  \n  \n" + 
                    $"Thank you!!!";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }
            else if (stepContext.Reason == DialogReason.NextCalled)
            {
                return await stepContext.ReplaceDialogAsync(InitialDialogId, null, cancellationToken);
            }

            return await stepContext.CancelAllDialogsAsync(cancellationToken);
            // Restart the main dialog with a different message the second time around

        }
    }
}
