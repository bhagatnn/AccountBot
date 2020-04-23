// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System.Data.SqlClient;
using System.Text;
using System;

namespace AccountBot.Dialogs
{
    public class AccountStatusDialog : CancelDialog
    {
        private const string StartMsgText = "To Check your application status, Could you please provide the requested information.";
        private const string LastNameText = "Last Name";
        private const string DateOfBirthText = "Date of Birth";
        private const string NationalIDText = "Last 4 Digits of NationalID";
        private const string PostalCodeText = "Postal Code";
        
        private string AccountInfo = "AccountInfo";

        public AccountStatusDialog()
            : base(nameof(AccountStatusDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                LastNameStepAsync,
                DateofBirthStepAsync,
                NationalIDStepAsync,
                PostalStepAsync,
                ConfirmStepAsync//,
                //FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        
        private async Task<DialogTurnResult> LastNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values[AccountInfo] = new AccountDetails();

            var startMessage = MessageFactory.Text(StartMsgText, StartMsgText, InputHints.ExpectingInput);
            await stepContext.Context.SendActivityAsync(startMessage, cancellationToken);

            var promptMessage = MessageFactory.Text(LastNameText, LastNameText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> DateofBirthStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.LastName = (string)stepContext.Result;

            return await stepContext.BeginDialogAsync(nameof(DateResolverDialog), accountDetails.DateOfBirth, cancellationToken);
        }

        private async Task<DialogTurnResult> NationalIDStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.DateOfBirth = (string)stepContext.Result;

            var promptMessage = MessageFactory.Text(NationalIDText, NationalIDText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> PostalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.NationalID = (string)stepContext.Result;

            var promptMessage = MessageFactory.Text(PostalCodeText, PostalCodeText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.PostalCode = Convert.ToUInt32(stepContext.Result);

            var messageText = $"Please confirm, your Application details as below:  \n Last Name : {accountDetails.LastName} " +
                $"  \n Date of Birth: {accountDetails.DateOfBirth}   \n National ID: {accountDetails.NationalID} " +
                $"  \n Postal Code: {accountDetails.PostalCode}    \n Is this correct?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        //private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    if ((bool)stepContext.Result)
        //    {
        //        var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];

        //        try
        //        {
        //            string conn = "Server=tcp:caliber2020sqlserver.database.windows.net,1433;" +
        //                "Initial Catalog=calibercitgroupincsqldb;" +
        //                "User ID=calibone;Password=azure@2020;MultipleActiveResultSets=False;" +
        //                "Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        //            SqlConnection connection = new SqlConnection(conn);

                    
        //            string sql = "INSERT INTO dbo.AccountCustomer (FirstName,LastName,MiddleName,DateOfBirth,NationalID,EmailAddress," +
        //                "PrimaryMobile,SecondaryMobile,AddressLine1,AddressLine2,City,State,PostalCode,Country) VALUES " +
        //                "(@FName, @LName, @MName,@DOB, @NatnID, @Email, @PMob, @SMob, @AddLn1, @AddLn2, @City, @State, @PosCde, @Cntry)";

        //            SqlCommand command = new SqlCommand(sql, connection);
        //            command.Parameters.Add("@FName", System.Data.SqlDbType.VarChar, 100).Value = accountDetails.Firstname;
        //            command.Parameters.Add("@LName", System.Data.SqlDbType.VarChar, 50).Value = accountDetails.LastName;
        //            command.Parameters.Add("@MName", System.Data.SqlDbType.VarChar, 50).Value = accountDetails.MiddleName;
        //            command.Parameters.Add("@DOB", System.Data.SqlDbType.Date).Value = accountDetails.DateOfBirth;
        //            command.Parameters.Add("@NatnID", System.Data.SqlDbType.VarChar, 50).Value = accountDetails.NationalID;
        //            command.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 50).Value = accountDetails.EmailAddress;
        //            command.Parameters.Add("@PMob", System.Data.SqlDbType.BigInt).Value = accountDetails.PrimaryMobile;
        //            command.Parameters.Add("@SMob", System.Data.SqlDbType.BigInt).Value = accountDetails.SecondaryMobile;
        //            command.Parameters.Add("@AddLn1", System.Data.SqlDbType.VarChar, 100).Value = accountDetails.AddressLine1;
        //            command.Parameters.Add("@AddLn2", System.Data.SqlDbType.VarChar, 100).Value = accountDetails.AddressLine2;
        //            command.Parameters.Add("@City", System.Data.SqlDbType.VarChar, 50).Value = accountDetails.City;
        //            command.Parameters.Add("@State", System.Data.SqlDbType.VarChar, 50).Value = accountDetails.State;
        //            command.Parameters.Add("@PosCde", System.Data.SqlDbType.SmallInt).Value = accountDetails.PostalCode;
        //            command.Parameters.Add("@Cntry", System.Data.SqlDbType.VarChar, 50).Value = accountDetails.Country;
        //            connection.Open();
        //            command.ExecuteNonQuery();
        //            connection.Close();

        //        }
        //        catch (SqlException e)
        //        {
        //            throw e;
        //        }

        //        return await stepContext.EndDialogAsync(accountDetails, cancellationToken);
        //    }

        //    return await stepContext.EndDialogAsync(null, cancellationToken);
        //}

        private static bool IsAmbiguous(string timex)
        {
            var timexProperty = new TimexProperty(timex);
            return !timexProperty.Types.Contains(Constants.TimexTypes.Definite);
        }
    }
}
