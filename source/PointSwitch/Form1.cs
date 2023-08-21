using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace MyPointSwitchApp
{
    public partial class Form1 : Form
    {
        private string iniFilePath;

        public Form1()
        {
            InitializeComponent();

            // 获取程序根目录下的INI文件路径
            iniFilePath = Path.Combine(Application.StartupPath, "config.ini");

            // 在窗体加载时从INI文件中加载配置信息
            LoadConfigFromIni();

        }

        private void LoadConfigFromIni()
        {
            try
            {
                // 检查INI文件是否存在
                if (File.Exists(iniFilePath))
                {
                    // 创建一个INI文件读取器
                    using (StreamReader reader = new StreamReader(iniFilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            // 解析INI文件中的配置项
                            if (line.StartsWith("TextBox1Value="))
                            {
                                textBox1.Text = line.Substring("TextBox1Value=".Length);
                            }
                            else if (line.StartsWith("TextBox2Value="))
                            {
                                textBox2.Text = line.Substring("TextBox2Value=".Length);
                            }
                            else if (line.StartsWith("TextBox3Value="))
                            {
                                textBox3.Text = line.Substring("TextBox3Value=".Length);
                            }
                            else if (line.StartsWith("TextBox4Value="))
                            {
                                textBox4.Text = line.Substring("TextBox4Value=".Length);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"从INI文件加载配置信息时发生错误：{ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowser.ShowDialog();
                if (result == DialogResult.OK)
                {
                    textBox1.Text = folderBrowser.SelectedPath;
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowser.ShowDialog();
                if (result == DialogResult.OK)
                {
                    textBox2.Text = folderBrowser.SelectedPath;
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowser.ShowDialog();
                if (result == DialogResult.OK)
                {
                    textBox3.Text = folderBrowser.SelectedPath;
                    ShowSubDirectoriesInTreeView(textBox3.Text, treeView1);
                }
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowser.ShowDialog();
                if (result == DialogResult.OK)
                {
                    textBox4.Text = folderBrowser.SelectedPath;
                    ShowSubDirectoriesInTreeView(textBox4.Text, treeView2);
                }
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            ShowSubDirectoriesInTreeView(textBox3.Text, treeView1);
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            ShowSubDirectoriesInTreeView(textBox4.Text, treeView2);
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            string sourceFolderPath = textBox1.Text;
            TreeNode selectedNode = treeView1.SelectedNode;

            if (selectedNode == null)
            {
                MessageBox.Show("请先选择一个文件夹。");
                return;
            }

            string targetFolderPath = Path.Combine(textBox3.Text, selectedNode.FullPath);
            ClearFolder(sourceFolderPath);
            int replacedFilesCount = CopyFiles(targetFolderPath, sourceFolderPath);
            MessageBox.Show($"已成功替换 {replacedFilesCount} 个文件。");
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            string sourceFolderPath = textBox2.Text;
            TreeNode selectedNode = treeView2.SelectedNode;

            if (selectedNode == null)
            {
                MessageBox.Show("请先选择一个文件夹。");
                return;
            }

            string targetFolderPath = Path.Combine(textBox4.Text, selectedNode.FullPath);
            ClearFolder(sourceFolderPath);
            int replacedFilesCount = CopyFiles(targetFolderPath, sourceFolderPath);
            MessageBox.Show($"已成功替换 {replacedFilesCount} 个文件。");
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode == null)
            {
                MessageBox.Show("请先选择一个文件夹。");
                return;
            }

            string selectedFolderName = selectedNode.FullPath;
            string sourceFolderPath = Path.Combine(textBox3.Text, selectedFolderName);
            string targetFolderPath = textBox1.Text;

            int copiedFilesCount = CopyFiles(sourceFolderPath, targetFolderPath);
            int targetFilesCount = Directory.GetFiles(targetFolderPath, "*", SearchOption.AllDirectories).Length;

            MessageBox.Show($"成功复制 {copiedFilesCount} 个文件。\n目标文件夹内共有 {targetFilesCount} 个文件。");
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView2.SelectedNode;
            if (selectedNode == null)
            {
                MessageBox.Show("请先选择一个文件夹。");
                return;
            }

            string selectedFolderName = selectedNode.FullPath;
            string sourceFolderPath = Path.Combine(textBox4.Text, selectedFolderName);
            string targetFolderPath = textBox2.Text;

            int copiedFilesCount = CopyFiles(sourceFolderPath, targetFolderPath);
            int targetFilesCount = Directory.GetFiles(targetFolderPath, "*", SearchOption.AllDirectories).Length;

            MessageBox.Show($"成功复制 {copiedFilesCount} 个文件。\n目标文件夹内共有 {targetFilesCount} 个文件。");
        }

        private void ShowSubDirectoriesInTreeView(string folderPath, System.Windows.Forms.TreeView treeView)
        {
            treeView.Nodes.Clear();
            try
            {
                string[] subDirectories = Directory.GetDirectories(folderPath);
                foreach (string subDirectory in subDirectories)
                {
                    string folderName = new DirectoryInfo(subDirectory).Name;
                    TreeNode node = new TreeNode(folderName);
                    treeView.Nodes.Add(node);
                    ShowSubDirectoriesRecursive(subDirectory, node);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("没有权限访问该文件夹。");
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("文件夹不存在。");
            }
            catch (IOException)
            {
                MessageBox.Show("发生IO错误。");
            }
        }

        private void ShowSubDirectoriesRecursive(string path, TreeNode parentNode)
        {
            try
            {
                string[] subDirectories = Directory.GetDirectories(path);
                foreach (string subDirectory in subDirectories)
                {
                    string folderName = new DirectoryInfo(subDirectory).Name;
                    TreeNode node = new TreeNode(folderName);
                    parentNode.Nodes.Add(node);
                    ShowSubDirectoriesRecursive(subDirectory, node);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Handle unauthorized access exception, skip this folder
            }
            catch (DirectoryNotFoundException)
            {
                // Handle directory not found exception, skip this folder
            }
            catch (IOException)
            {
                // Handle other IO errors, skip this folder
            }
        }

        private void ClearFolder(string folderPath)
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(folderPath);
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                {
                    subDirectory.Delete(true);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("没有权限访问文件夹。");
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("文件夹不存在。");
            }
            catch (IOException)
            {
                MessageBox.Show("发生IO错误。");
            }
        }

        private int CopyFiles(string sourceFolderPath, string targetFolderPath)
        {
            int replacedFilesCount = 0;
            try
            {
                Directory.CreateDirectory(targetFolderPath);
                foreach (string sourceFilePath in Directory.GetFiles(sourceFolderPath))
                {
                    string fileName = Path.GetFileName(sourceFilePath);
                    string targetFilePath = Path.Combine(targetFolderPath, fileName);
                    File.Copy(sourceFilePath, targetFilePath, true);
                    replacedFilesCount++;
                }
                foreach (string sourceSubFolderPath in Directory.GetDirectories(sourceFolderPath))
                {
                    string subFolderName = new DirectoryInfo(sourceSubFolderPath).Name;
                    string targetSubFolderPath = Path.Combine(targetFolderPath, subFolderName);
                    replacedFilesCount += CopyFiles(sourceSubFolderPath, targetSubFolderPath);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("没有权限访问文件夹。");
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("文件夹不存在。");
            }
            catch (IOException)
            {
                MessageBox.Show("发生IO错误。");
            }

            return replacedFilesCount;
        }

        private void SaveConfigToIni()
        {
            try
            {
                // 创建一个INI文件写入器
                using (StreamWriter writer = new StreamWriter(iniFilePath))
                {
                    // 写入textBox1、textBox2、textBox3和textBox4的内容到INI文件
                    writer.WriteLine("[Config]");
                    writer.WriteLine($"TextBox1Value={textBox1.Text}");
                    writer.WriteLine($"TextBox2Value={textBox2.Text}");
                    writer.WriteLine($"TextBox3Value={textBox3.Text}");
                    writer.WriteLine($"TextBox4Value={textBox4.Text}");

                    // 确保数据被立即写入到文件中
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                // 错误处理代码，您可以选择输出错误日志或记录日志文件等，而不是弹出消息框
                Console.WriteLine($"从INI文件加载配置信息时发生错误：{ex.Message}");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // 当 textBox1 文本内容发生变化时，保存配置信息到INI文件
            SaveConfigToIni();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // 当 textBox2 文本内容发生变化时，保存配置信息到INI文件
            SaveConfigToIni();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            // 当 textBox3 文本内容发生变化时，保存配置信息到INI文件
            SaveConfigToIni();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            // 当 textBox4 文本内容发生变化时，保存配置信息到INI文件
            SaveConfigToIni();
        }

        private void button01_Click(object sender, EventArgs e)
        {
            string directoryPath = textBox1.Text;

            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                // 使用默认的文件资源管理器打开目录
                Process.Start("explorer.exe", directoryPath);
            }
            else
            {
                MessageBox.Show("请输入一个有效的目录路径。");
            }
        }

        private void button02_Click(object sender, EventArgs e)
        {
            string directoryPath = textBox2.Text;

            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                // 使用默认的文件资源管理器打开目录
                Process.Start("explorer.exe", directoryPath);
            }
            else
            {
                MessageBox.Show("请输入一个有效的目录路径。");
            }
        }

        private void button03_Click(object sender, EventArgs e)
        {
            string directoryPath = textBox3.Text;

            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                // 使用默认的文件资源管理器打开目录
                Process.Start("explorer.exe", directoryPath);
            }
            else
            {
                MessageBox.Show("请输入一个有效的目录路径。");
            }
        }

        private void button04_Click(object sender, EventArgs e)
        {
            string directoryPath = textBox4.Text;

            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                // 使用默认的文件资源管理器打开目录
                Process.Start("explorer.exe", directoryPath);
            }
            else
            {
                MessageBox.Show("请输入一个有效的目录路径。");
            }
        }
    }
}