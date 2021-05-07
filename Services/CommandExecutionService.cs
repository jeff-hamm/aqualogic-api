using AqualogicJumper.Services;
using System.Threading;
using System.Threading.Tasks;

namespace AqualogicJumper.Services
{
    public class CommandExecutionService
    {
        private readonly AqualogicMessageWriter _writer;

        public CommandExecutionService(AqualogicMessageWriter writer)
        {
            _writer = writer;
        }
        public async Task Execute(CancellationToken token)
        {
            await _writer.Process(token);
        }
    }
}