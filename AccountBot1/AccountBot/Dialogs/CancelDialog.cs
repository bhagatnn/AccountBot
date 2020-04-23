// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace AccountBot.Dialogs
{
    public class CancelDialog : ComponentDialog
    {
        private const string CancelMsgText = "Cancelling Your Application!!!  \n Thank you for reaching out to AccountBot!!!  \n Have a wonderful day!!!.";

        public CancelDialog(string id)
            : base(id)
        {
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default)
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message && innerDc.Context.Activity.Attachments is null)
            {
                var text = innerDc.Context.Activity.Text.ToLowerInvariant();

                switch (text)
                {
                    case "cancel":
                    case "quit":
                        var cancelMessage = MessageFactory.Text(CancelMsgText, CancelMsgText, InputHints.IgnoringInput);
                        await innerDc.Context.SendActivityAsync(cancelMessage, cancellationToken);
                        return await innerDc.CancelAllDialogsAsync(cancellationToken);
                

                }
            }

            return null;
        }
    }
}
