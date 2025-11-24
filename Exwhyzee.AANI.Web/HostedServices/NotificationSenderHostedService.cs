using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.HostedServices
{
    public class NotificationSenderHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotificationSenderHostedService> _logger;
        private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(15);

        public NotificationSenderHostedService(IServiceScopeFactory scopeFactory, ILogger<NotificationSenderHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationSenderHostedService starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AaniDbContext>();
                    var emailSender = scope.ServiceProvider.GetRequiredService<Exwhyzee.AANI.Web.Helper.IEmailSender>();

                    // pick pending notifications due for attempt (order by next attempt)
                    var pending = await db.Notifications
                        .Where(n => n.Status == NotificationStatus.Pending || n.Status == NotificationStatus.Processing)
                         .Take(30)
                        .ToListAsync(stoppingToken);

                    if (pending.Count == 0)
                    {
                        await Task.Delay(_pollInterval, stoppingToken);
                        continue;
                    }

                    foreach (var n in pending)
                    {
                        try
                        {
                            // mark processing to avoid other workers racing
                            n.Status = NotificationStatus.Processing;
                            await db.SaveChangesAsync(stoppingToken);

                            if (n.MessageType == MessageType.Email)
                            {
                                if (string.IsNullOrWhiteSpace(n.Email))
                                    throw new Exception("Missing recipient email");

                               var emailResult = await emailSender.SendEmailAsync(n.Email!, n.Subject ?? string.Empty, n.Content);
                                n.Status = NotificationStatus.Sent;
                                n.SentAt = DateTime.UtcNow;
                                n.ResponseMessage = emailResult;
                            }
                            else if (n.MessageType == MessageType.Sms)
                            {
                                if (string.IsNullOrWhiteSpace(n.Phone))
                                    throw new Exception("Missing recipient phone");

                                var smsResult = await emailSender.SendNotification(n.Phone!, n.Content);
                                n.Status = NotificationStatus.Sent;
                                n.SentAt = DateTime.UtcNow;
                                n.ResponseMessage = smsResult;
                            }
                            else if (n.MessageType == MessageType.Whatsapp)
                            {
                                if (string.IsNullOrWhiteSpace(n.Phone))
                                    throw new Exception("Missing recipient phone");


                                // send free-form text via their whatsapp endpoint
                                var whatsappResult = await emailSender.SendWhatsappAsync(n.Phone!, n.Content ?? string.Empty, null, null);
                                
                                n.Status = NotificationStatus.Sent;
                                n.SentAt = DateTime.UtcNow;
                                n.ResponseMessage = whatsappResult;
                            }
                            else
                            {
                                // Mail (physical) not implemented - mark failed with message
                                n.Status = NotificationStatus.Failed;
                                n.ResponseMessage = "Physical mail is not handled by background service";
                            }

                            n.Retries = 0; // reset retries on success
                        }
                        catch (Exception ex)
                        {
                            // handle failure
                            n.Retries++;
                            n.ResponseMessage = ex.Message;
                            if (n.Retries >= n.MaxRetries)
                            {
                                n.Status = NotificationStatus.Failed;
                                _logger.LogError(ex, "Notification {Id} permanently failed after {Retries} attempts", n.Id, n.Retries);
                            }
                            else
                            {
                                n.Status = NotificationStatus.Pending;
                                // exponential backoff: 2^retries minutes
                                var backoffMinutes = Math.Pow(2, Math.Min(n.Retries, 6)); 
                                _logger.LogWarning(ex, "Notification {Id} failed, will retry (attempt {Retries})", n.Id, n.Retries);
                            }
                        }
                        finally
                        {
                            await db.SaveChangesAsync(stoppingToken);
                        }
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // shutting down
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in NotificationSenderHostedService loop");
                }

                await Task.Delay(_pollInterval, stoppingToken);
            }

            _logger.LogInformation("NotificationSenderHostedService stopping.");
        }
    }
}