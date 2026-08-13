// Program.cs
using GenericNotificationSystem.Builders;
using GenericNotificationSystem.Handlers;
using GenericNotificationSystem.Models;
using GenericNotificationSystem.Specifications;

static void Section(string title)
{
    Console.WriteLine();
    Console.WriteLine($"{'-',0}")
}


//  Covariance
IMessageReader<EmailMessage> emailReader = new EmailReader();
IMessageReader<IMessage> reader = emailReader;

IMessage next = reader.ReadNext();
Console.WriteLine($"Read: {next.Subject}");

// Contravariance
IMessageHandler<EmailMessage> handler = new UniversalHandler();
DeliveryResult result = handler.Handle(new EmailMessage("Subject", "a@b.com", "hello"));
Console.WriteLine($"Delivery result: {result.Success}");

// Builder pattern
var message = new EmailNotificationBuilder()
    .WithSubject("Hello")
    .WithRecipient("a@b.com")
    .WithBody("Hello there")
    .Build();

var message2 = new EmailNotificationBuilder()
    .WithSubject("Welcome")
    .WithRecipient("a@b.com")
    .WithBody("welcome")
    .Build();

// Specification pattern
List<IMessage> messages = [
    message,
    message2,
    new SmsMessage("Hello", "12345678910", "just saying hi!")
];

ISpecification<IMessage> isEmail = new MessageTypeSpec<EmailMessage>();
ISpecification<IMessage> saysHello = new SubjectContainsSpec("hello");
ISpecification<IMessage> combined = isEmail.And(saysHello);

foreach (var m in messages.Where(combined))
{
    Console.WriteLine($"Message {m.Subject} fits the specification");
}