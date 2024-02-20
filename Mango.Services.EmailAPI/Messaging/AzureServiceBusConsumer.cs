using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Services;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string userRegistrationQueue;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _userRegistrationProcessor;
        private readonly EmailService _emailService;
        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            userRegistrationQueue = _configuration.GetValue<string>("TopicAndQueueNames:UserRegistrationQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
            _userRegistrationProcessor = client.CreateProcessor(userRegistrationQueue);
            _emailService = emailService;
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            _emailCartProcessor.StartProcessingAsync();
            _userRegistrationProcessor.ProcessMessageAsync += OnUserRegistration;
            _userRegistrationProcessor.ProcessErrorAsync += ErrorHandler;
            _userRegistrationProcessor.StartProcessingAsync();
        }

        private async Task OnUserRegistration(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RegistrationRequestDto objMessage = JsonConvert.DeserializeObject<RegistrationRequestDto>(body);
            try
            {
                await _emailService.EmailUserRegistration(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();
            await _userRegistrationProcessor.StopProcessingAsync();
            await _userRegistrationProcessor.DisposeAsync();
        }
        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CartDto objMessage = JsonConvert.DeserializeObject<CartDto>(body);
            try
            {
                await _emailService.EmailCartAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        
    }
}
