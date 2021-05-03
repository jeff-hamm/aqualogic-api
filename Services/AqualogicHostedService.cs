using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AqualogicJumper.Services
{
    public class AqualogicHostedService : IHostedService
    {
        private readonly AquaLogicProtocolService _service;
        private readonly ILogger<AqualogicHostedService> _log;

        public AqualogicHostedService(AquaLogicProtocolService service,ILogger<AqualogicHostedService> log)
        {
            _service = service;
            _log = log;
        }
        private Task _executingTask;
        private Task _monitorTask;

        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        public bool Started { get; set; }
        public const int DELAY_MS = 100;

        public Task StartAsync(CancellationToken token)
        {
            _executingTask  = Task.Run(() => ExecuteTask(_stoppingCts.Token), _stoppingCts.Token);
            _monitorTask = MonitorTask(_executingTask, _stoppingCts.Token);

            // If the task is completed then return it,
            // this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        private async Task MonitorTask(Task executingTask,CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (executingTask.IsCompleted)
                {
                    _log.LogDebug("Monitor detected that main task has halted. Restarting");
                    _executingTask = ExecuteTask(_stoppingCts.Token);
                }
                else
                {
                    await Task.Delay(DELAY_MS, token);
                }
            }
        }


        private async Task ExecuteTask(CancellationToken token)
        {
            Started = true;
            var connected = false;
            while (!token.IsCancellationRequested)
            {
                while (!connected && !token.IsCancellationRequested)
                {
                    try
                    {
                        if (_service.Connect())
                        {
                            _log.LogDebug("Connected!");
                            connected = true;
                        }
                        else
                        {
                            _log.LogDebug($"Could not connect, waiting {DELAY_MS}ms to try again");
                        }
                    }
                    catch (Exception ex)
                    {
                        connected = false;
                        _log.LogDebug($"Error connecting {ex}, waiting {DELAY_MS}ms to try again");
                        await Task.Delay(DELAY_MS,token);
                    }
                }

                try
                {
                    connected = await _service.Process(token);
                }
                catch (Exception ex)
                {
                    connected = false;
                    _log.LogDebug(ex.ToString());
                }
            }
            _service.Stop();
            Started = false;
        }

        public async Task StopAsync(CancellationToken token)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,
                    token));
            }
        }
    }
}