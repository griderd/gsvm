using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VirtualDiskCMD.FileSystems.VMFS1
{
    public class VMFS1FileSystemInfo : FileSystemInfo<VMFS1DirectoryInfo, VMFS1FileInfo>
    {
        public override int Size => throw new NotImplementedException();

        public override int Attributes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override FileSystemInfo<VMFS1DirectoryInfo, VMFS1FileInfo> Parent => throw new NotImplementedException();

        public override string Name { get; set; }

        public override void Copy(VMFS1DirectoryInfo destination, string newName = null)
        {
            throw new NotImplementedException();
        }

        public override void Create()
        {
            throw new NotImplementedException();
        }

        public override void Delete()
        {
            throw new NotImplementedException();
        }

        public override bool Exists()
        {
            throw new NotImplementedException();
        }

        public override void Rename(string name)
        {
            throw new NotImplementedException();
        }
    }

    public class VMFS1DirectoryInfo : DirectoryInfo<VMFS1DirectoryInfo, VMFS1FileInfo>
    {
        public override int Size => throw new NotImplementedException();

        public override int Attributes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override string Name { get; set; }

        public override string FullName
        {
            get
            {
                FileSystemInfo<VMFS1DirectoryInfo, VMFS1FileInfo> current = this;
                string path = "";
                while (current != null)
                {
                    Path.Combine(path, current.Name);
                    current = current.Parent;
                }

                return path;
            }
        }

        public override FileSystemInfo<VMFS1DirectoryInfo, VMFS1FileInfo> Parent
        {
            get
            {
                return null;
            }
        }

        public override void Copy(VMFS1DirectoryInfo destination, string newName = null)
        {
            throw new NotImplementedException();
        }

        public override void Create()
        {
            throw new NotImplementedException();
        }

        public override FileInfo<VMFS1DirectoryInfo, VMFS1FileInfo> CreateFile(string name)
        {
            throw new NotImplementedException();
        }

        public override DirectoryInfo<VMFS1DirectoryInfo, VMFS1FileInfo> CreateSubdirectory(string name)
        {
            throw new NotImplementedException();
        }

        public override void Delete()
        {
            throw new NotImplementedException();
        }

        public override bool Exists()
        {
            throw new NotImplementedException();
        }

        public override DirectoryInfo<VMFS1DirectoryInfo, VMFS1FileInfo>[] GetDirectories()
        {
            throw new NotImplementedException();
        }

        public override VMFS1FileInfo[] GetFiles()
        {
            throw new NotImplementedException();
        }

        public override void Rename(string name)
        {
            throw new NotImplementedException();
        }
    }

    public class VMFS1FileInfo : FileInfo<VMFS1DirectoryInfo, VMFS1FileInfo>
    {
        VMFS1Volume volume;
        VMFS1FileTableRecord file;

        public VMFS1FileInfo(VMFS1Volume volume, string filename)
        {
            this.volume = volume;
            file = new VMFS1FileTableRecord(filename, 0, new byte[0]);
        }

        public VMFS1FileInfo(VMFS1Volume volume, VMFS1FileTableRecord record)
        {
            this.volume = volume;
            this.file = record;
        }

        public override string Extension { get { return Path.GetExtension(file.Name); } }

        public override string Name 
        { 
            get 
            { 
                return file.Name.Trim(); 
            }
            set
            {
                file.Name = value;
            }
        }

        public override string FullName
        {
            get
            {
                return volume.Name + "/" + Name;
            }
        }

        public override int Size
        {
            get
            {
                return file.Data.Length;
            }
        }

        public override int Attributes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override FileSystemInfo<VMFS1DirectoryInfo, VMFS1FileInfo> Parent
        {
            get { return volume; }
        }

        public override void AppendAllLines(string[] s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                AppendText(s[i]);
                AppendText(Environment.NewLine);
            }
        }

        public override void AppendBytes(byte[] data)
        {
            List<byte> filedata = new List<byte>(file.Data);
            filedata.AddRange(data);
            file.Data = filedata.ToArray();
        }

        public override void AppendText(string s)
        {
            AppendBytes(Encoding.UTF8.GetBytes(s));
        }

        public override void Copy(VMFS1DirectoryInfo destination, string newName = null)
        {
            throw new NotImplementedException();
        }

        public override void Create()
        {
            if (volume.Contains(file.Name))
                SetRecord(file);
            else
                AddRecord(file);
        }

        public override void Delete()
        {
            VMFS1FileTableRecord record = GetRecord(Name);
            RemoveRecord(record);
        }

        public override bool Exists()
        {
            try
            {
                VMFS1FileTableRecord temp = volume[file.Name];
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public override byte[] ReadAllBytes()
        {
            try
            {
                VMFS1FileTableRecord temp = volume[file.Name];
                return temp.Data;
            }
            catch (IndexOutOfRangeException)
            {
                throw new FileNotFoundException();
            }
        }

        public override string[] ReadAllLines()
        {
            try
            {
                string[] lines = ReadText().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                return lines;
            }
            catch
            {
                throw;
            }
        }

        public override string ReadText()
        {
            try
            {
                byte[] data = ReadAllBytes();
                return Encoding.UTF8.GetString(ReadAllBytes());
            }
            catch
            {
                throw;
            }
        }

        public override void WriteAllBytes(byte[] data)
        {
            file.Data = data;
        }

        public override void WriteAllLines(string[] s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                sb.AppendLine(s[i]);
            }
            WriteText(sb.ToString());
        }

        public override void WriteText(string s)
        {
            file.Data = Encoding.UTF8.GetBytes(s);
        }

        VMFS1FileTableRecord GetRecord(string filename)
        {
            try
            {
                VMFS1FileTableRecord temp = volume[file.Name];
                return temp;
            }
            catch (IndexOutOfRangeException)
            {
                throw new FileNotFoundException();
            }
        }

        void SetRecord(VMFS1FileTableRecord record)
        {
            try
            {
                volume[file.Name] = record;
            }
            catch
            {
                throw new FileNotFoundException();
            }
        }

        void AddRecord(VMFS1FileTableRecord record)
        {
            if (!Exists())
                volume.Add(record);
            else
                throw new Exception();
        }

        void RemoveRecord(VMFS1FileTableRecord record)
        {
            if (volume.Contains(record))
                volume.Remove(record);
            else
                throw new FileNotFoundException();
        }

        public override void Rename(string name)
        {
            file.Name = name;
        }
    }
}
