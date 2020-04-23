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
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Auth;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure;
using Microsoft.Azure.Storage.RetryPolicies;
using System.Net.Http;

namespace AccountBot.Dialogs
{
    public class AccountOpeningDialog : CancelDialog
    {
        private const string StartMsgText = "To Start your application, Could you please provide the requested information.";
        private const string FirstNameText = "First Name";
        private const string LastNameText = "Last Name";
        private const string MiddleNameText = "Middle Name. If you do not have, please type \'NA\'";
        private const string DateOfBirthText = "Date of Birth";
        private const string NationalIDText = "National ID - PAN / AADHAR #";
        private const string EmailAddressText = "Email Address";
        private const string PrimaryMobileText = "Primary Mobile #";
        private const string SecondaryMobileText = "Secondary Mobile #. If you do not have, please type \'NA\'";
        private const string AddressLine1Text = "Address Line1";
        private const string AddressLine2Text = "Address Line2";
        private const string CityText = "City";
        private const string StateText = "State";
        private const string PostalCodeText = "Postal Code";
        private const string CountryText = "Country";

        private const string UploadNationalIdStepMsgText = "Could you please upload your any National ID.";

        private string AccountInfo = "AccountInfo";
        private const string attachmentPromptId = "attachmentPrompt";
        private const string ContainerPrefix = "blob-";

        public AccountOpeningDialog()
            : base(nameof(AccountOpeningDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new AttachmentPrompt(attachmentPromptId));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FirstNameStepAsync,
                LastNameStepAsync,
                MiddleNameStepAsync,
                DateofBirthStepAsync,
                NationalIDStepAsync,
                EmailAddressStepAsync,
                PrimaryMobileStepAsync,
                SecondaryMobileStepAsync,
                AddressLine1StepAsync,
                AddressLine2StepAsync,
                CityStepAsync,
                StateStepAsync,
                PostalStepAsync,
                CountryStepAsync,
                UploadNationalId,
                ConfirmStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> FirstNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Create an object in which to collect the customer's information within the dialog.
            stepContext.Values[AccountInfo] = new AccountDetails();

            var startMessage = MessageFactory.Text(StartMsgText, StartMsgText, InputHints.ExpectingInput);
            await stepContext.Context.SendActivityAsync(startMessage, cancellationToken);

            var promptMessage = MessageFactory.Text(FirstNameText, FirstNameText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);

        }

        private async Task<DialogTurnResult> LastNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.Firstname = (string)stepContext.Result;

            var promptMessage = MessageFactory.Text(LastNameText, LastNameText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> MiddleNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.LastName = (string)stepContext.Result;

            var promptMessage = MessageFactory.Text(MiddleNameText, MiddleNameText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            
        }
       
        private async Task<DialogTurnResult> DateofBirthStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            if (stepContext.Result.ToString().ToUpper() != "NA")
                accountDetails.MiddleName = (string)stepContext.Result;
            else
                accountDetails.MiddleName = String.Empty;

            return await stepContext.BeginDialogAsync(nameof(DateResolverDialog), accountDetails.DateOfBirth, cancellationToken);
        }

        private async Task<DialogTurnResult> NationalIDStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.DateOfBirth = (string)stepContext.Result;

            var promptMessage = MessageFactory.Text(NationalIDText, NationalIDText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> EmailAddressStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.NationalID = (string)stepContext.Result;

            var promptMessage = MessageFactory.Text(EmailAddressText, EmailAddressText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> PrimaryMobileStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.EmailAddress = (string)stepContext.Result;

            var promptMessage = MessageFactory.Text(PrimaryMobileText, PrimaryMobileText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> SecondaryMobileStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.PrimaryMobile = Convert.ToInt64(stepContext.Result);

            var promptMessage = MessageFactory.Text(SecondaryMobileText, SecondaryMobileText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> AddressLine1StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            if (stepContext.Result.ToString().ToUpper() != "NA")
                accountDetails.SecondaryMobile = Convert.ToInt64(stepContext.Result);

            var promptMessage = MessageFactory.Text(AddressLine1Text, AddressLine1Text, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> AddressLine2StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.AddressLine1 = (string)stepContext.Result;

            var promptMessage = MessageFactory.Text(AddressLine2Text, AddressLine2Text, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> CityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.AddressLine2 = (string)stepContext.Result;

            var promptMessage = MessageFactory.Text(CityText, CityText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> StateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.City = (string)stepContext.Result;

            var promptMessage = MessageFactory.Text(StateText, StateText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> PostalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.State = (string)stepContext.Result;

            var promptMessage = MessageFactory.Text(PostalCodeText, PostalCodeText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> CountryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.PostalCode = Convert.ToUInt32(stepContext.Result);

            var promptMessage = MessageFactory.Text(CountryText, CountryText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> UploadNationalId(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            accountDetails.Country = (string)stepContext.Result;

            var promptMessage = MessageFactory.Text(UploadNationalIdStepMsgText, UploadNationalIdStepMsgText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(attachmentPromptId, new PromptOptions { Prompt = promptMessage }, cancellationToken);


        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
            //accountDetails.Country = (string)stepContext.Result;
            List<Attachment> attachments = (List<Attachment>)stepContext.Result;
            foreach(var file in attachments)
            {
                accountDetails.AttachmentURL = file.ContentUrl;
            }

            var messageText = $"Please confirm, I have your personal details as below:  \n Last Name : {accountDetails.LastName} " +
                $"  \n First Name: {accountDetails.Firstname}   \n Middle Name: {accountDetails.MiddleName}. " +
                $"  \n Date of Birth: {accountDetails.DateOfBirth}   \n National ID: {accountDetails.NationalID} " +
                $"  \n Email Address: {accountDetails.EmailAddress}   \n Primary Mobile: {accountDetails.PrimaryMobile}" +
                $"  \n Secondary Mobile: {accountDetails.SecondaryMobile}   \n Address Line 1: {accountDetails.AddressLine1}" +
                $"  \n Address Line 2: {accountDetails.AddressLine2}   \n City: {accountDetails.City} " +
                $"  \n State: {accountDetails.State}   \n Postal Code: {accountDetails.PostalCode}" +
                $"  \n Country: {accountDetails.Country}   \n Is this correct?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var accountDetails = (AccountDetails)stepContext.Values[AccountInfo];
                
                try
                {
                    string conn = "Server=tcp:caliber2020sqlserver.database.windows.net,1433;" +
                        "Initial Catalog=calibercitgroupincsqldb;" +
                        "User ID=calibone;Password=azure@2020;MultipleActiveResultSets=False;" +
                        "Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                    SqlConnection connection = new SqlConnection(conn);

                    
                    string sql = "INSERT INTO dbo.AccountCustomer (FirstName,LastName,MiddleName,DateOfBirth,NationalID,EmailAddress," +
                        "PrimaryMobile,SecondaryMobile,AddressLine1,AddressLine2,City,State,PostalCode,Country) VALUES " +
                        "(@FName, @LName, @MName,@DOB, @NatnID, @Email, @PMob, @SMob, @AddLn1, @AddLn2, @City, @State, @PosCde, @Cntry)";

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.Add("@FName", System.Data.SqlDbType.VarChar, 100).Value = accountDetails.Firstname;
                    command.Parameters.Add("@LName", System.Data.SqlDbType.VarChar, 50).Value = accountDetails.LastName;
                    command.Parameters.Add("@MName", System.Data.SqlDbType.VarChar, 50).Value = accountDetails.MiddleName;
                    command.Parameters.Add("@DOB", System.Data.SqlDbType.Date).Value = accountDetails.DateOfBirth;
                    command.Parameters.Add("@NatnID", System.Data.SqlDbType.VarChar, 50).Value = accountDetails.NationalID;
                    command.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 50).Value = accountDetails.EmailAddress;
                    command.Parameters.Add("@PMob", System.Data.SqlDbType.BigInt).Value = accountDetails.PrimaryMobile;
                    command.Parameters.Add("@SMob", System.Data.SqlDbType.BigInt).Value = accountDetails.SecondaryMobile;
                    command.Parameters.Add("@AddLn1", System.Data.SqlDbType.VarChar, 100).Value = accountDetails.AddressLine1;
                    command.Parameters.Add("@AddLn2", System.Data.SqlDbType.VarChar, 100).Value = accountDetails.AddressLine2;
                    command.Parameters.Add("@City", System.Data.SqlDbType.VarChar, 50).Value = accountDetails.City;
                    command.Parameters.Add("@State", System.Data.SqlDbType.VarChar, 50).Value = accountDetails.State;
                    command.Parameters.Add("@PosCde", System.Data.SqlDbType.SmallInt).Value = accountDetails.PostalCode;
                    command.Parameters.Add("@Cntry", System.Data.SqlDbType.VarChar, 50).Value = accountDetails.Country;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    string accountName = "blobstoragecitgroupinc";
                    string accountKey = "9HVUQdfQyaQH9Neh+ArstH8FhFFQIQDKsf/yDMCDnp0RhpAPzgBFQQOg3BrCpmO/4btmgQj9lwOA9qu5ZMaWRg==";
                    string newFileName = accountDetails.Firstname + "_"+ accountDetails.LastName +System.DateTime.Today.ToString("MMddyyyyHHmmss") + ".JPG";
                 
                    string containerName = ContainerPrefix + accountDetails.Firstname.ToLower() + System.DateTime.Today.ToString("yyyymmdd");
                    string sourceUrl = accountDetails.AttachmentURL;
                    //string sourceUrl1 = "C:\\Users\\calibone\\Downloads\\original";
                    
                    //storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

                    
                    // Create a blob client for interacting with the blob service.
                    //CloudBlobClient blobClient1 = storageAccount.CreateCloudBlobClient();
                    CloudStorageAccount csa = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);

                    CloudBlobClient blobClient = csa.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference(containerName);
                    //var blobContainer = blobClient.GetContainerReference(containerName);
                    //blobContainer.CreateIfNotExists();
                    BlobRequestOptions requestOptions = new BlobRequestOptions() { RetryPolicy = new NoRetry() };
                    await container.CreateIfNotExistsAsync(requestOptions, null);

                    var newBlockBlob = container.GetBlockBlobReference(newFileName);
                    newBlockBlob.Properties.ContentType = "image/png";
                    //newBlockBlob.BeginStartCopy(new Uri(sourceUrl), null, null);

                    //await newBlockBlob.UploadFromFileAsync(sourceUrl);
                    newBlockBlob.BeginUploadFromFile(sourceUrl, null, null);
                    //CloudBlobContainer container = blobClient.GetContainerReference(destinationContainer);
                    //CloudBlockBlob blockBlob = container.GetBlockBlobReference(sourceUrl);
                    // Set the blob's content type so that the browser knows to treat it as an image.

                    using (HttpClient client = new System.Net.Http.HttpClient())
                    {
                        using (HttpResponseMessage response = await client.GetAsync(sourceUrl))
                        using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        {
                            await newBlockBlob.UploadFromStreamAsync(streamToReadFrom);
                        }
                    }

                }
                catch (SqlException e)
                {
                    throw e;
                }
                catch(Exception ex)
                {

                    throw ex;
                }

                return await stepContext.EndDialogAsync(accountDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private static bool IsAmbiguous(string timex)
        {
            var timexProperty = new TimexProperty(timex);
            return !timexProperty.Types.Contains(Constants.TimexTypes.Definite);
        }
    }
}
