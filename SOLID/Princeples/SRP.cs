namespace SOLID.Princeples;

public class SRP
{
    #region Bad

    public class GodFile
    {
        public byte[] Data => _data;

        private readonly byte[] _data;

        public GodFile(byte[] data) => _data = data;

        public Task Load(Uri address)
        {
            //Loading
            return Task.CompletedTask;
        }

        public Task LoadViaWifi(Uri address)
        {
            return Load(address);
        }

        public static IEnumerable<Uri> Search(string directory, string name)
        {
            return new List<Uri>();
        }

        public static FineFile GetFileByName(string name)
        {
            //return file
            return new FineFile(Array.Empty<byte>());
        }
    }

    #endregion

    #region Good

    public class FineFile
    {
        public byte[] Data => _data;

        private readonly byte[] _data;

        public FineFile(byte[] data) => _data = data;
    }

    public class FileUploader
    {
        public virtual Task Load(FineFile file, Uri address)
        {
            //Loading
            return Task.CompletedTask;
        }
    }

    public class WifiFileUploader : FileUploader
    {
        public override Task Load(FineFile file, Uri address)
        {
            return base.Load(file, address);
        }
    }

    public class FileSearcher
    {
        public IEnumerable<Uri> Search(string directory, string name)
        {
            return new List<Uri>();
        }
    }

    public class FileRepository
    {
        private string _rootDirectory;

        public FileRepository(string rootDirectory) => _rootDirectory = rootDirectory;

        public FineFile GetFileByName(string name)
        {
            //return file
            return new FineFile(Array.Empty<byte>());
        }
    }

    #endregion
}