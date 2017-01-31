namespace KatanaGraph
{
    public class FileType
    {
        public string name { get; set; }
        public string author { get; set; }
        public string type { get; set; }
        public int uniqueId { get; set; }

        public FileType(string name)
        {
            this.name = name;

        }
    }

    public class FolderType
    {
        public string name { get; set; }
        public int uniqueId { get; set; }

        public FolderType(string name)
        {
            this.name = name;
        }
    }

    public class ProjectType
    {
        public string name { get; set; }
        public string creationDate { get; set; }
    }

    public class LogType
    {
        public string creationDate { get; set; }
        public string commitVersion { get; set; }
    }
}