
using Neo4jClient;

namespace KatanaGraph
{
    public class ClientDAL
    {
        private readonly IGraphClient graphClient;

        public ClientDAL(IGraphClient graphClient)
        {
            this.graphClient = graphClient;
        }


        public void CreateProjectNode(ProjectType projectType)
        {
            graphClient.Cypher
                .Create("(project:Project {projectDetails})")
                .WithParam("projectDetails", projectType)
                .ExecuteWithoutResultsAsync().Wait();


        }

        public void CreateFolderNode(FolderType folderType)
        {

            graphClient.Cypher
                .Create("(folder:Folder {folderDetails})")
                .WithParam("folderDetails", folderType)
                .ExecuteWithoutResultsAsync().Wait();

        }

        public void CreateFileTypeNode(FileType fileType)
        {
            graphClient.Cypher
                .Create("(file:File {fileDetails})")
                .WithParam("fileDetails", fileType)
                .ExecuteWithoutResultsAsync().Wait();

        }

        public void CreateFolderContainedInProject(ProjectType projectType, FolderType folderType)
        {
            graphClient.Cypher
                .Match("(project:Project)", "(folder:Folder)")
                .Where((ProjectType project) => project.name == "Super Secret Project")
                .AndWhere((FolderType folder) => folder.name == folderType.name)
                .Merge("(folder)-[:CONTAINED_IN]->(project)")
                .WithParam("folder", folderType)
                .WithParam("project", projectType)
                .ExecuteWithoutResultsAsync().Wait();

        }

        public void CreateFileContainedInFolder(FolderType folderType, FileType fileType)
        {
            graphClient.Cypher
                .Match("(file:File)", "(folder:Folder)")
                .Where((FolderType folder) => folder.uniqueId == folderType.uniqueId)
                .AndWhere((FileType file) => file.uniqueId == fileType.uniqueId)
                .Merge("(file)-[:CONTAINED_IN]->(folder)")
                .WithParam("folder", folderType)
                .WithParam("file", fileType)
                .ExecuteWithoutResultsAsync().Wait();

        }

        public void CreateFolderContainedInFolder(FolderType topFolderType, FolderType bottomFolderType)
        {
            graphClient.Cypher
                .Match("(topFolder:Folder)", "(bottomFolder:Folder)")
                .Where((FolderType topFolder) => topFolder.uniqueId == topFolderType.uniqueId)
                .AndWhere((FolderType bottomFolder) => bottomFolder.uniqueId == bottomFolderType.uniqueId)
                .Merge("(bottomFolder)-[:CONTAINED_IN]->(topFolder)")
                .WithParam("topFolder", topFolderType)
                .WithParam("bottomFolder", bottomFolderType)
                .ExecuteWithoutResultsAsync()
                .Wait();
        }

        public void DeleteGraph()
        {
            graphClient.Cypher.Match("(n)").OptionalMatch("(n)-[r]-()").Delete("r,n").ExecuteWithoutResults();
        }



    }
}