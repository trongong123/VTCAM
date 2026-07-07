namespace EQX.Core.Vision.Grabber
{
    public class GrabData : IDisposable
    {
        public ulong Id { get; set; }
        public byte[]? ImageBuffer { get; set; }
        public bool IsSuccess { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        // TODO: Convert this to enum, may be?
        public long PixelFormat { get; set; }

        public void Dispose()
        {
            ImageBuffer = null;
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}
