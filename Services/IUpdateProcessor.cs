using AqualogicJumper.Model.Status;

namespace AqualogicJumper.Services
{
    public interface IUpdateProcessor
    {
        bool TryProcess(StatusUpdate update);
    }
}