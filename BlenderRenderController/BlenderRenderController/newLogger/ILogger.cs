namespace BlenderRenderController.newLogger
{
    public interface ILogger
    {
        bool IsActive { get; set; }

        void LogError(string message);
        void LogInfo(string message);
    }
}