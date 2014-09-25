using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using FileEncoding;

namespace Srt2UTF8
{
    public partial class Srt2UTF8Form : Form
    {
        List<string> FileTypes = new List<string>();
        List<FileInfo> FileInfoList = new List<FileInfo>();

        public Srt2UTF8Form()
        {
            InitializeComponent();
        }

        private bool ReadFileType()
        {
            FileTypes.Clear();

            string strTypes = FileTypeTextBox.Text;
            char[] spliter = { ',', ';', '\t' };
            string[] strType = strTypes.Split(spliter);

            if (strType.Length == 0)
            {
                MessageBox.Show("File type cannot be empty!");
                return false;
            }

            for (int i = 0; i < strType.Length; i++)
            {
                if (strType[i].Trim().Length > 0)
                    FileTypes.Add(strType[i].Trim().ToLower());
            }

            if (FileTypes.Count == 0)
            {
                MessageBox.Show("File type cannot be empty!");
                return false;
            }

            return true;
        }

        private void OpenBtn_Click(object sender, EventArgs e)
        {
            if(!ReadFileType())
                return;
            FileInfoList.Clear();

            if (FileTypes.Count == 0)
            {
                MessageBox.Show("File type cannot be empty!");
                return;
            }

            FolderBrowserDialog _ofd = new FolderBrowserDialog();

            if (_ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string strPath = _ofd.SelectedPath.Trim();

            DirectoryInfo _di = new DirectoryInfo(strPath);

            FileInfo[] fileInfos;

            foreach (string type in FileTypes)
            {
                fileInfos = _di.GetFiles("*." + type, SearchOption.AllDirectories);
                for (int i = 0; i < fileInfos.Length; i++)
                {
                    FileInfoList.Add(fileInfos[i]);
                }
            }

            GoBtn.Enabled = true;
        }

        private void GoBtn_Click(object sender, EventArgs e)
        {
            IdentifyEncoding _ie = new IdentifyEncoding();

            int sum = 0;
            for (int i = 0; i < FileInfoList.Count; i++)
            {
                FileInfo info = FileInfoList[i];
                //Encoding fileCode = EncodingType.GetType(info.FullName);
                string code = _ie.GetEncodingName(info);
                Console.WriteLine(code + "   " + info.FullName);

                //转换GB2312/Big5/ASCII 到 UTF-8
                FileStream stream;
                Byte[] buffer;
                Byte[] newBuff;
                switch (code)
                {
                    case "GB2312":
                        stream = info.OpenRead();
                        buffer = new Byte[stream.Length];
                        stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
                        stream.Close();
                        stream.Dispose();
                        newBuff = Encoding.Convert(Encoding.GetEncoding("gb2312"), Encoding.UTF8, buffer);
                        info.Delete();
                        stream = info.OpenWrite();
                        stream.Write(newBuff, 0, newBuff.Length);
                        stream.Close();
                        sum++;
                        break;
                    case "Big5":
                        stream = info.OpenRead();
                        buffer = new Byte[stream.Length];
                        stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
                        stream.Close();
                        stream.Dispose();
                        newBuff = Encoding.Convert(Encoding.GetEncoding("BIG5"), Encoding.UTF8, buffer);
                        info.Delete();
                        stream = info.OpenWrite();
                        stream.Write(newBuff, 0, newBuff.Length);
                        stream.Close();
                        sum++;
                        break;
                    case "GBK":
                        stream = info.OpenRead();
                        buffer = new Byte[stream.Length];
                        stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
                        stream.Dispose();
                        newBuff = Encoding.Convert(Encoding.GetEncoding("GBK"), Encoding.UTF8, buffer);
                        info.Delete();
                        stream = info.OpenWrite();
                        stream.Write(newBuff, 0, newBuff.Length);
                        stream.Close();
                        sum++;
                        break;
                    //case "ASSIC":
                    //    stream = info.OpenRead();
                    //    buffer = new Byte[stream.Length];
                    //    stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
                    //    stream.Dispose();
                    //    newBuff = Encoding.Convert(Encoding.ASCII, Encoding.UTF8, buffer);
                    //    info.Delete();
                    //    stream = info.OpenWrite();
                    //    stream.Write(newBuff, 0, newBuff.Length);
                    //    stream.Close();
                    //    break;
                    default:
                        break;
                }
            }
            MessageBox.Show(@"转换完成，共转换" + sum + @"个字幕文件！");

        }
    }
}
