

namespace KatanaGraph
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Neo4jClient;
    using Neo4jClient.SchemaManager;


    internal class Program
    {
        #region Methods

        static int StaticCounter;

        private static void Main()
        {


            //ShowUserBoxWarning();

            //make sure Neo4J engine is running before opening the db
            //and that 'my password' is set by successfully navigating to the Uri
            IGraphClient graphClient = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "");
            try
            {
                graphClient.Connect();
            }
            catch (AggregateException)
            {
                //****************To catch this exception*****************
                // go to the Debug Menu -Exceptions..
                //-Common Language Runtime Exceptions-System
                //then untick the System.AggregateException User-Unhandled box
                MessageBox.Show(
                    "Unable to connect to the Server\r\nTry Connecting from a browser\r\n to make sure you can log in",
                    "Error!",
                    MessageBoxButtons.OK);
                Application.Exit();
            }

            Console.WriteLine("Building a test graph please wait.");




            var clientDAL = new ClientDAL(graphClient);
            clientDAL.DeleteGraph();
            graphClient.DropAllIndexes();


            /*graphClient.DropAllIndexes();*/

            //CreateTestDB(graphClient);

            TraverseProject("C:\\Users\\Adam\\Desktop\\Neo4j\\GraphDbExamples\\project\\katana-katana",clientDAL);

            Console.WriteLine("\r\nPlease hit 'return' to exit");
            Console.ReadLine();
        }




        private static void TraverseProject(string projectPath,ClientDAL clientDAL)
        {

            try
            {
                FolderType folderType = new FolderType("Katana");
                folderType.uniqueId = StaticCounter++;
                clientDAL.CreateFolderNode(folderType);
                Console.WriteLine(projectPath);
                DirTraverse(projectPath,folderType,clientDAL);

            }
            catch (UnauthorizedAccessException UAEx)
            {
                Console.WriteLine(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Console.WriteLine(PathEx.Message);
            }
        }

        private static void DirTraverse(string currentDir,FolderType currentFolder,ClientDAL clientDal)
        {
            var files = Directory.EnumerateFiles(currentDir);


            foreach (var file in files)
            {
                string temp = file.Replace(currentDir + "\\", "");
                FileType fileType = new FileType(temp);
                fileType.uniqueId = StaticCounter++;
                clientDal.CreateFileTypeNode(fileType);
                clientDal.CreateFileContainedInFolder(currentFolder,fileType);
            }

            var folders = Directory.EnumerateDirectories(currentDir);
            FolderType folderType;
            foreach (var folder in folders)
            {
                string temp = folder.Replace(currentDir + "\\", "");
                folderType = new FolderType(temp);
                folderType.uniqueId = StaticCounter++;
                clientDal.CreateFolderNode(folderType);
                clientDal.CreateFolderContainedInFolder(currentFolder,folderType);
                DirTraverse(folder,folderType,clientDal);
            }

        }


        private static void ShowUserBoxWarning()
        {
            DialogResult result = MessageBox.Show(
                "About to delete any exisiting Graph and build a new test graph",
                "Warning!",
                MessageBoxButtons.OKCancel);
            if (result == DialogResult.Cancel)
            {
                throw new Exception("User Cancelled Test");
            }
        }



        private static void CreateTestDB(IGraphClient graphClient)
        {
            var folderTest = new FolderType("testFolder");
            var folderTest2 = new FolderType("folder2");

            var fileTest = new FileType("pythonExampleFile");
            var fileTest2 = new FileType("calculation");
            var fileTest3 = new FileType("connecter");


            fileTest2.type = "cs";
            fileTest2.author = "unknown";

            fileTest.type = "python";
            fileTest.author = "authorName";

            var projectTest = new ProjectType();
            projectTest.name = "Super Secret Project";
            projectTest.creationDate = "2017";

            var clientDAL = new ClientDAL(graphClient);

            Task task = new Task(() =>
            {
                try
                {

                    clientDAL.CreateProjectNode(projectTest);
                    clientDAL.CreateFolderNode(folderTest);
                    clientDAL.CreateFolderNode(folderTest2);
                    clientDAL.CreateFileTypeNode(fileTest);
                    clientDAL.CreateFileTypeNode(fileTest2);
                    clientDAL.CreateFileTypeNode(fileTest3);

                    clientDAL.CreateFolderContainedInProject(projectTest, folderTest);
                    clientDAL.CreateFolderContainedInProject(projectTest, folderTest2);

                    clientDAL.CreateFileContainedInFolder(folderTest, fileTest);
                    clientDAL.CreateFileContainedInFolder(folderTest, fileTest2);
                    clientDAL.CreateFileContainedInFolder(folderTest2, fileTest3);
                }
                catch (AggregateException ae)
                {
                    throw ae;
                }
            });



            try
            {
                task.Start();
                task.Wait();
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                {
                    Console.WriteLine(ex.Message);
                }

            }



        }


        #endregion
    }
}