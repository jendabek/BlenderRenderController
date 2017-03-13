namespace BlenderRenderController.newLogger
{
    public interface ILogger
    {
        bool Verbose { get; set; }

        void LogError(string message);
        void LogInfo(string message);
        void LogWarn(string message);
    }
}