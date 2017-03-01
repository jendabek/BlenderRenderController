namespace BlenderRenderController
{
    class ProjectData
    {
        public string blendFilePath, outputPath, chunksPath, renderer, renderFormat, afterRenderAction;
        public decimal chunkStart, chunkEnd, chunkLength, start, end, processCount;
        public double fps;
    }
}
