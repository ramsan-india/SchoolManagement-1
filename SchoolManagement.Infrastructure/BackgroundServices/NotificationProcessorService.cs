using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Infrastructure.BackgroundServices
{
    public class NotificationProcessorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationProcessorService> _logger;
        private readonly TimeSpan _processInterval = TimeSpan.FromSeconds(30);

        public NotificationProcessorService(IServiceProvider serviceProvider, ILogger<NotificationProcessorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Processor Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    await ProcessNotificationQueue(scope.ServiceProvider);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during notification processing");
                }

                await Task.Delay(_processInterval, stoppingToken);
            }

            _logger.LogInformation("Notification Processor Service stopped");
        }

        private async Task ProcessNotificationQueue(IServiceProvider serviceProvider)
        {
            var notificationService = serviceProvider.GetRequiredService<INotificationService>();
            var notificationQueue = serviceProvider.GetRequiredService<INotificationQueue>();

            var notifications = await notificationQueue.DequeueAsync(10); // Process 10 at a time

            foreach (var notification in notifications)
            {
                try
                {
                    switch (notification.Type)
                    {
                        case NotificationType.SMS:
                            await notificationService.SendSMSAsync(notification.Recipient, notification.Message);
                            break;
                        case NotificationType.Email:
                            await notificationService.SendEmailAsync(notification.Recipient, notification.Subject, notification.Message);
                            break;
                        case NotificationType.Push:
                            await notificationService.SendPushNotificationAsync(notification.Recipient, notification.Subject, notification.Message);
                            break;
                    }

                    await notificationQueue.MarkAsProcessedAsync(notification.Id);
                    _logger.LogInformation($"Processed notification {notification.Id} of type {notification.Type}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to process notification {notification.Id}");
                    await notificationQueue.MarkAsFailedAsync(notification.Id, ex.Message);
                }
            }
        }
    }
}
