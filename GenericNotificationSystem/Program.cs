// Program.cs
using Common;
using GenericNotificationSystem.Builders;
using GenericNotificationSystem.Dispatching;
using GenericNotificationSystem.Formatting;
using GenericNotificationSystem.Handlers;
using GenericNotificationSystem.Models;
using GenericNotificationSystem.Repository;
using GenericNotificationSystem.Specifications;
using GenericNotificationSystem.Validators;

/*
 * =====================================
 * Constraints + Validation
 * =====================================
 */
CustomConsoleLogs.Section("Constraints + Validation");
var validator = MessageValidator<EmailMessage>.Create<EmailValidator>();
var errors = validator.Validate(new EmailMessage("", "", "")).ToList();
Console.WriteLine($"Validation errors for empty email: {(errors.Count == 0 ? "none" : string.Join(", ", errors))}");

var validErrors = validator.Validate(new EmailMessage("Hi", "a@b.com", "Hello")).ToList();
Console.WriteLine($"Validation errors for valid email: {(errors.Count == 0 ? "none" : string.Join(", ", validErrors))}");

/*
 * =====================================
 * Covariance
 * =====================================
 */
CustomConsoleLogs.Section("Covariance");
IMessageReader<EmailMessage> emailReader = new EmailReader();
IMessageReader<IMessage> messageReader = emailReader;

IMessage next = messageReader.ReadNext();
Console.WriteLine($"Read message ({next.GetType().Name}): \"{next.Subject}\"");

/*
 * =====================================
 * Contravariance
 * =====================================
 */
CustomConsoleLogs.Section("Contravariance");
IMessageHandler<EmailMessage> handler = new UniversalHandler();
DeliveryResult result = handler.Handle(new EmailMessage("Subject", "a@b.com", "hello"));
Console.WriteLine($"Delivery result: {result.Success}");

/*
 * =====================================
 * Repository Pattern - Message Log
 * =====================================
 */
CustomConsoleLogs.Section("Repository Pattern - Message Log");
var log = new MessageLog<EmailMessage>();
log.Log(new EmailMessage("Hello", "a@b.com", "Hi"));
log.Log(new EmailMessage("Invoice", "b@c.com", "See attached"));
log.Log(new EmailMessage("Update", "c@d.com", "FYI"));

Console.WriteLine($"All logged: {log.GetAllLogs().Count()}");
Console.WriteLine($"Last 5 seconds: {log.GetRecent(TimeSpan.FromSeconds(5)).Count()} messages");

/*
 * =====================================
 * Strategy Pattern - Formatter
 * =====================================
 */
CustomConsoleLogs.Section("Strategy Pattern - Formatting");
var msg = new EmailMessage("Hello", "a@b.com", "Hi");

new Dispatcher<EmailMessage>(new PlainTextFormatter<EmailMessage>()).Dispatch(msg);
new Dispatcher<EmailMessage>(new JsonFormatter<EmailMessage>()).Dispatch(msg);

/*
 * =====================================
 * Builder Pattern
 * =====================================
 */
CustomConsoleLogs.Section("Builder Pattern");
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

Console.WriteLine($"Built message: {message}");
Console.WriteLine($"Built message: {message2}");

/*
 * =====================================
 * Specification Pattern
 * =====================================
 */
CustomConsoleLogs.Section("Specification Pattern");
List<IMessage> messages = [
    new EmailMessage("Hello", "a@b.com", "Hi"),
    new EmailMessage("Invoice", "b@c.com", "See attached"),
    new SmsMessage("Hello", "12345678910", "just saying hi!")
];

ISpecification<IMessage> isEmail = new MessageTypeSpec<EmailMessage>();
ISpecification<IMessage> saysHello = new SubjectContainsSpec("hello");
ISpecification<IMessage> combined = isEmail.And(saysHello);

foreach (var m in messages.Where(combined))
{
    Console.WriteLine($"Message ({m.GetType().Name}): \"{m.Subject}\" fits the specification");
}