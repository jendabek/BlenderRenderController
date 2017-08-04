namespace BlenderRenderController
{
    /// <summary>
    /// Ui Values
    /// </summary>
    class ProjectData
    {
        public string blendFilePath, outputPath, chunksPath, renderer, renderFormat, afterRenderAction;
        public decimal chunkStart, chunkEnd, chunkLength, start, end, processCount;
        public double fps;
    }

    public class BlendData
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string Fps { get; set; }
        public string FpsBase { get; set; }
        public string Resolution { get; set; }
        public string OutputPath { get; set; }
        public string ProjectName { get; set; }
        public string ScenesNum { get; set; }
        public string SceneActive { get; set; }
        public string RenderFormat { get; set; }
    }

}
