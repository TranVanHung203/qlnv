using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Service
{
    public class CauHinhThongBaoScheduledService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<CauHinhThongBaoScheduledService> _logger;

        public CauHinhThongBaoScheduledService(IServiceProvider provider, ILogger<CauHinhThongBaoScheduledService> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CauHinhThongBaoScheduledService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // var now = DateTime.Now;
                    // var next = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);
                    // if (now > next) next = next.AddDays(1);

                    // var delay = next - now;
                    // _logger.LogInformation("CauHinhThongBaoScheduledService sleeping until {NextRun} (in {Delay})", next, delay);

                    // await Task.Delay(delay, stoppingToken);
                    //------------------------------------
                    var delay = TimeSpan.FromMinutes(1);
                    _logger.LogInformation("CauHinhThongBaoScheduledService sleeping for {Delay}", delay);
                    await Task.Delay(delay, stoppingToken);

                    if (stoppingToken.IsCancellationRequested) break;

                    using (var scope = _provider.CreateScope())
                    {
                        var svc = scope.ServiceProvider.GetService<Service.Contracts.ICauHinhThongBaoService>();
                        var httpContextAccessor = scope.ServiceProvider.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();

                        // Provide a temporary system principal so code relying on HttpContext.User works in background runs
                        Microsoft.AspNetCore.Http.DefaultHttpContext? prevCtx = null;
                        if (httpContextAccessor != null)
                        {
                            prevCtx = httpContextAccessor.HttpContext as Microsoft.AspNetCore.Http.DefaultHttpContext;
                            var sysPrincipal = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[] {
                                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "system"),
                                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "System")
                            }, "System"));
                            httpContextAccessor.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = sysPrincipal };
                        }

                        if (svc != null)
                        {
                            _logger.LogInformation("CauHinhThongBaoScheduledService invoking RunCheckAndSendAsync at {Now}", DateTime.Now);
                            try
                            {
                                var sent = await svc.RunCheckAndSendAsync();
                                _logger.LogInformation("CauHinhThongBaoScheduledService finished sending {Count} notifications.", sent);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error when running RunCheckAndSendAsync");
                            }
                        }
                        else
                        {
                            _logger.LogWarning("ICauHinhThongBaoService not registered; skipping scheduled run.");
                        }

                        // Restore previous context (if any)
                        if (httpContextAccessor != null)
                        {
                            httpContextAccessor.HttpContext = prevCtx;
                        }
                    }
                }
                catch (TaskCanceledException) { break; }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error in CauHinhThongBaoScheduledService loop. Will retry in 1 minute.");
                    try { await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); } catch { }
                }
            }

            _logger.LogInformation("CauHinhThongBaoScheduledService stopping.");
        }
    }
}
