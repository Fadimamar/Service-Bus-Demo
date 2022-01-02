using System;
using SBShared.Models;
using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Text.Json;


namespace SBReceiver
{
    class Program 
    { 
        const string connectionString= "Service Bus Connection String";
        const string Queuename="Queue Name in your Service Bus";
        static IQueueClient queueClient;
        static async Task Main(string[] args)
        {
            queueClient=new QueueClient(connectionString, Queuename);
            var messageHandlerOptions=new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete=false, //done auto complete. Message will be marked complete after peek and capturing of the message body for further processing
                MaxConcurrentCalls=1 //process one meassge at a time
            };
            //start listening to queue by registering a message handler with above options.
            queueClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);
            
            Console.ReadLine(); //keep listening until enter is pressed.
            await queueClient.CloseAsync(); //close connection with Azure Service Bus

         }
        static async Task ProcessMessageAsync(Message message,CancellationToken token )
        {
            var jsonString=Encoding.UTF8.GetString(message.Body); //get json string
            PersonModel person = JsonSerializer.Deserialize<PersonModel>(jsonString); //Convert to person model
            Console.WriteLine($"Person Received:{person.FirstName} {person.LastName}"); // This is where we do the work like sending email to person, update database, etc.
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);// mark message complete withing the 30 seconds locktime or the lock time configured on Azure for the service bus queue. 
        }
        static  Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine($"Message Recevied Exception: {arg.Exception}");
            return Task.CompletedTask; //since a task is expected in return.
        }
    }
}
