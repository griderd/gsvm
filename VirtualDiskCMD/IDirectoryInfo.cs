using System;

namespace VirtualDiskCMD
{
    public interface IFileSystemInfo
    {
        IFileSystemInfo Parent { get; }

        string Name { get; set; }
        int Size { get; }
        int Attributes { get; set; }
        void Create();
        void Rename(string name);
        void Delete();
        void Copy(IDirectoryInfo destination, string newName = null);
        bool Exists();
    }

    public abstract class FileSystemInfo<TDirectoryInfo, TFileInfo> : IFileSystemInfo
        where TDirectoryInfo : VirtualDiskCMD.DirectoryInfo<TDirectoryInfo, TFileInfo>
        where TFileInfo : VirtualDiskCMD.FileInfo<TDirectoryInfo, TFileInfo>
    {
        public abstract FileSystemInfo<TDirectoryInfo, TFileInfo> Parent { get; }
        public abstract int Size { get; }
        public abstract int Attributes { get; set; }

        IFileSystemInfo IFileSystemInfo.Parent => ((IFileSystemInfo)Parent).Parent;

        public abstract string Name { get; set; }

        public abstract void Create();

        public abstract void Rename(string name);

        public abstract void Delete();

        public abstract void Copy(TDirectoryInfo destination, string newName = null);

        public abstract bool Exists();

        void IFileSystemInfo.Copy(IDirectoryInfo destination, string newName)
        {
            Copy((TDirectoryInfo)destination, newName);
        }
    }

    public interface IVolumeInfo : IDirectoryInfo
    {
        int VolumeSize { get; }
        void Defragment();

        byte[] ToBinary();
    }

    public abstract class VolumeInfo<TDirectoryInfo, TFileInfo> : VirtualDiskCMD.DirectoryInfo<TDirectoryInfo, TFileInfo>, IVolumeInfo
        where TDirectoryInfo : VirtualDiskCMD.DirectoryInfo<TDirectoryInfo, TFileInfo>
        where TFileInfo : VirtualDiskCMD.FileInfo<TDirectoryInfo, TFileInfo>
    {
        public abstract int VolumeSize { get; }

        public abstract void Defragment();
        public abstract byte[] ToBinary();

        IDirectoryInfo IDirectoryInfo.CreateSubdirectory(string name)
        {
            return (IDirectoryInfo)CreateSubdirectory(name);
        }

        IDirectoryInfo[] IDirectoryInfo.GetDirectories()
        {
            return (IDirectoryInfo[])GetDirectories();
        }

        IFileInfo[] IDirectoryInfo.GetFiles()
        {
            return (IFileInfo[])GetFiles();
        }
    }

    public interface IDirectoryInfo : IFileSystemInfo
    {
        IFileInfo CreateFile(string name);
        IDirectoryInfo CreateSubdirectory(string name);
        IDirectoryInfo[] GetDirectories();
        IFileInfo[] GetFiles();
        string FullName { get; }
    }

    public abstract class DirectoryInfo<TDirectoryInfo, TFileInfo> : VirtualDiskCMD.FileSystemInfo<TDirectoryInfo, TFileInfo>, IDirectoryInfo
        where TDirectoryInfo : VirtualDiskCMD.DirectoryInfo<TDirectoryInfo, TFileInfo>
        where TFileInfo : VirtualDiskCMD.FileInfo<TDirectoryInfo, TFileInfo>
    {
        public override abstract string Name { get; set; }
        public abstract string FullName { get; }

        public abstract FileInfo<TDirectoryInfo, TFileInfo> CreateFile(string name);

        public abstract DirectoryInfo<TDirectoryInfo, TFileInfo> CreateSubdirectory(string name);

        public abstract DirectoryInfo<TDirectoryInfo, TFileInfo>[] GetDirectories();
        public abstract TFileInfo[] GetFiles();

        IFileInfo IDirectoryInfo.CreateFile(string name)
        {
            return (IFileInfo)CreateFile(name);
        }

        IDirectoryInfo IDirectoryInfo.CreateSubdirectory(string name)
        {
            return (IDirectoryInfo)CreateSubdirectory(name);
        }

        IDirectoryInfo[] IDirectoryInfo.GetDirectories()
        {
            return (IDirectoryInfo[])GetDirectories();
        }

        IFileInfo[] IDirectoryInfo.GetFiles()
        {
            return (IFileInfo[])GetFiles();
        }
    }

    public interface IFileInfo : IFileSystemInfo
    {
        string Extension { get; }
        string FullName { get; }
        byte[] ReadAllBytes();
        string[] ReadAllLines();
        string ReadText();
        void WriteAllBytes(byte[] data);
        void AppendBytes(byte[] data);
        void WriteText(string s);
        void AppendText(string s);
        void WriteAllLines(string[] s);
        void AppendAllLines(string[] s);
    }

    public abstract class FileInfo<TDirectoryInfo, TFileInfo> : VirtualDiskCMD.FileSystemInfo<TDirectoryInfo, TFileInfo>, IFileInfo
        where TDirectoryInfo : VirtualDiskCMD.DirectoryInfo<TDirectoryInfo, TFileInfo>
        where TFileInfo : VirtualDiskCMD.FileInfo<TDirectoryInfo, TFileInfo>
    {
        public abstract string Extension { get; }

        public override abstract string Name { get; set; }

        public abstract string FullName { get; }

        public abstract byte[] ReadAllBytes();
        public abstract string[] ReadAllLines();
        public abstract string ReadText();
        public abstract void WriteAllBytes(byte[] data);
        public abstract void AppendBytes(byte[] data);
        public abstract void WriteText(string s);
        public abstract void AppendText(string s);
        public abstract void WriteAllLines(string[] s);
        public abstract void AppendAllLines(string[] s);
    }
}
