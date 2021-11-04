using System;
using Nest;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace fileExplore
{
    public partial class Form1 : Form
    {
        
        FileStream fileStream, fileStreamRoot, fileStreamRead;
        ConnectionSettings connectionSettings;
        ElasticClient elasticClient;
        List<string> pathFiles = new List<string>();
        List<file> myJson = new List<file>();
        public Form1()
        {
            InitializeComponent();
            PopulateTreeView();
            this.treeViewEx.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.treeViewEx_NodeMouseClick);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            connectionSettings = new ConnectionSettings(new Uri("http://localhost:9200/")); //local PC           
            elasticClient = new ElasticClient(connectionSettings);
            // ghi những dữ liệu mà trên elastic chưa có lên server  
            var bulkIndexResponse = elasticClient.Bulk(b => b
             .Index("filedatasearch")
             .IndexMany(myJson)
               );

            //############################# chỗ này khi hoàn thành phảixóa đi ########################################
            if (bulkIndexResponse.IsValid)
            {
                MessageBox.Show("them thanh cong");
            }
            //############################# chỗ này khi hoàn thành phải xóa đi   ########################################
        }

        private void PopulateTreeView()
        {
            // mở file txt và lưu giá trị vào 1 list, list này dùng để so sánh xem có file nào mới được tạo thêm trên máy khi mà 
            // chương trình đang tắt hay ko, nếu có thì thêm vào file txt, và gửi lên server elastic, nếu ko thì bỏ qua ko gửi gì lên hết 
            //################################# url chỗ này có thể sẽ sửa lại sau này thanh đường dẫn tương đối#####################
            fileStreamRead = new FileStream(@"E:\log.txt", FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader streamReader = new StreamReader(fileStreamRead);
            string pathFile = streamReader.ReadLine();
            while (pathFile != null)
            {
                pathFiles.Add(pathFile); // lưu giá trị đọc từ file vào list
                pathFile = streamReader.ReadLine();
            }
            streamReader.Close();
            fileStreamRead.Close();

            // khởi tạo root gốc trong tree node 
            TreeNode rootNode;

            //###################################### ko được xóa những cái comment này ######################################
            /*  var ListDriverInfor = DriveInfo.GetDrives();// lây tất cả các ổ đĩa ( các ổ đia trong máy, ko bao gồm các file trong ổ đĩa)
              foreach (DriveInfo drive in ListDriverInfor) // bắt đầu tìm kiếm trong các ổ đĩa để lấy ra các folder và các file 
              {
                  string path = drive.Name.ToString();
                  DirectoryInfo info = new DirectoryInfo(path);

                  if (info.Exists)
                  {
                      fileStreamRoot = new FileStream(@"E:\log.txt", FileMode.Append, FileAccess.Write, FileShare.None);
                     StreamWriter sw = new StreamWriter(fileStreamRoot);
                   foreach (FileInfo file in info.GetFiles())
                {
                    if (file.Extension == ".txt" || file.Extension == ".docx" || file.Extension == ".pdf")
                    {

                        if (!pathFiles.Contains(file.FullName)){
                            // lưu dữ liệu vào list để gửi lên electic nếu trên electic chưa có
                            myJson.Add(new file()
                            {
                                name = file.Name.ToString(),
                                path = file.FullName.ToString(),
                                content = "dư lieu tu c# bulk 123"
                            });

                            // ghi file 
                            sw.WriteLine(file.FullName);
                            sw.Flush();
                        }
                    }

                }
                     sw.Close();
                     fileStreamRoot.Close();

                      rootNode = new TreeNode(info.Name);// nếu như có tồn tại thư mục con năm trong path ( path là đường dẫn vd khi bắt đầu với ổ c path sẽ là C) 
                      rootNode.ImageIndex = 2;// gắn image cho root node ( đây là image dành cho ổ đĩa c d e ... , các folder được gắn mặc định ) 
                      rootNode.Tag = info;
                      GetDirectories(info.GetDirectories(), rootNode);// tìm kiếm các folder bên trong ổ đĩa

                      treeViewEx.Nodes.Add(rootNode);// thêm root node vào tree view để tạo ra nhánh của 1 ổ đĩa 
                  }
              }*/
            DirectoryInfo info = new DirectoryInfo(@"E:\test");
            if (info.Exists)
            {
                // ghi path vào file txt
                fileStreamRoot = new FileStream(@"E:\log.txt", FileMode.Append, FileAccess.Write, FileShare.None);// path này có thể đỏi cho phù hợp vì này dùng để test 
                StreamWriter sw = new StreamWriter(fileStreamRoot);
                // ở đây cần phải làm cái code này 2 lần  do là GetDirectories chỉ tìm được những file trong folder con nên cần làm ở đây để có thể tìm được file trong folder ngoài cùng 
                foreach (FileInfo file in info.GetFiles())
                {
                    // nếu thuốc những định dạng sau thì mới lưu lại vào txt và gửi lên elastic
                    if (file.Extension == ".txt" || file.Extension == ".docx" || file.Extension == ".pdf")
                    {

                        if (!pathFiles.Contains(file.FullName)){
                            // lưu dữ liệu vào list để gửi lên electic nếu trên electic chưa có
                            myJson.Add(new file()
                            {
                                name = file.Name.ToString(),
                                path = file.FullName.ToString(),
                                content = "dư lieu tu c# bulk 123"
                            });

                            // ghi file 
                            sw.WriteLine(file.FullName);
                            sw.Flush();
                        }
                    }

                }
                sw.Close();
                fileStreamRoot.Close();
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                rootNode.ImageIndex = 2;
                treeViewEx.Nodes.Add(rootNode);
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
         
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
         
            foreach (DirectoryInfo subDir in subDirs) // bắt đầu tìm kiếm trong từng ổ đĩa 
            {
    
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                // subDir.fullname sẽ show ra tất cả các path nhưng ko bao gồm file
                   // giống chú thích ở bên trên 
                fileStream = new FileStream(@"E:\log.txt", FileMode.Append, FileAccess.Write, FileShare.None);
                StreamWriter sw = new StreamWriter(fileStream);
                foreach (FileInfo file in subDir.GetFiles())
                {
                    // đọc và lấy ra những path có định dạng file là txt, doc, pdf
                    if (file.Extension == ".txt" || file.Extension ==".docx" || file.Extension == ".pdf")
                    {
                        if (!pathFiles.Contains(file.FullName)) // Kiểm tra xem file đó đã có trong txt hay chưa nếu chưa có lưu vào và đưa lên elastic
                        {
                            myJson.Add(new file()
                            {
                                name = file.Name,
                                path = file.FullName,
                                content = "dư lieu tu c# bulk 123" // cái chỗ này sẽ đọc nội dung file ra nhưng chưa làm tới 
                            });

                            sw.WriteLine(file.FullName);
                            sw.Flush();
                        }
                    }
           
                }
                sw.Close();
                fileStream.Close();

                try
                {
                    subSubDirs = subDir.GetDirectories();

                    if (subSubDirs.Length != 0)
                    {
                        GetDirectories(subSubDirs, aNode);// cái này gọi là đệ quy sau khi tìm xong 1 folder sẽ tiếp tục tìm kiếm lại trong folder con của folder đó xem có còn file hay folder nào nữa ko 
                    }
                 
                    nodeToAddTo.Nodes.Add(aNode);// add folder vào ổ đĩa 
              
                }
                catch (UnauthorizedAccessException)
                {

                }
                catch (IOException)
                {

                }
            }
        }
        // thiết lập tree view mỗi khi bấm vào thì list view sẽ chuyển theo ứng vs tree view
        private void treeViewEx_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                    {new ListViewItem.ListViewSubItem(item, "Directory"),
                        new ListViewItem.ListViewSubItem(item,
                            dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                    { new ListViewItem.ListViewSubItem(item, "File"),
                        new ListViewItem.ListViewSubItem(item,
                        file.LastAccessTime.ToShortDateString())};

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void treeViewEx_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
