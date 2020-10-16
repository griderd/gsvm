using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VirtualDiskCMD.FileSystems.VMFS1;

namespace VirtualDiskCMD
{
    /// <summary>
    /// Represents the VMFS1 file system format
    /// </summary>
    public class VMFS1Volume : VolumeInfo<VMFS1DirectoryInfo, VMFS1FileInfo>, ICollection<VMFS1FileTableRecord>
    {
        byte[] diskData;

        public bool Bootable { get; private set; }
        // Code to jump to the beginning of the bootstrap code for a bootable disk
        byte[] jump = new byte[] { 0x28, 0x00, 0x04, 0x00, 0x18, 0x28, 0x00, 0x00 };

        private string volumeLabel;
        public string VolumeLabel
        {
            get 
            {
                return volumeLabel; 
            }
            set 
            {
                if (value.Length > 8)
                    throw new Exception();
                volumeLabel = value; 
            }
        }

        string fileSystemLabel = "VMFS1\0\0\0";

        private List<byte> bootstrap = new List<byte>();
        public byte[] Bootstrap
        {
            get
            {
                return bootstrap.ToArray();
            }
            set
            {
                if (value.Length != 486)
                {
                    bootstrap.Clear();
                    bootstrap.AddRange(value);
                }
            }
        }

        byte[] bootSignature = new byte[] { 0x66, 0xBB };
        public VMFS1Volume(string volumeLabel)
        {
            VolumeLabel = volumeLabel;
            fileTable = new List<VMFS1FileTableRecord>();
            
        }

        public VMFS1Volume(byte[] data)
        {
            diskData = data;
            byte[] s = new byte[8];

            // Check boot signature and FS label

            // Get file system label
            Array.Copy(diskData, 16, s, 0, 8);
            string fsLabel = Encoding.UTF8.GetString(s);

            if (!((data[510] == 0x66) & (data[511] == 0xBB) & (fsLabel == "VMFS1\0\0\0")))
            {
                throw new Exception("Invalid boot signature or file system label.");
            }

            // Get the jump code
            Array.Copy(diskData, 0, jump, 0, 8);

            // Get the volume label
            Array.Copy(diskData, 8, s, 0, 8);
            volumeLabel = Encoding.UTF8.GetString(s);

            // Get boostrap code
            byte[] a_bootstrap = new byte[486];
            Array.Copy(diskData, 24, a_bootstrap, 0, 486);
            bootstrap.AddRange(a_bootstrap);

            // Get table
            fileTable = new List<VMFS1FileTableRecord>();
            int offset = 512;
            byte[] filename = new byte[28];
            byte[] startSector = new byte[2];
            byte[] fileLength = new byte[2];

            for (int i = 0; i < 2880; i++)
            {
                // Get the raw data
                Array.Copy(diskData, offset, filename, 0, 28);
                Array.Copy(diskData, offset + 28, startSector, 0, 2);
                Array.Copy(diskData, offset + 30, fileLength, 0, 2);

                // Convert the values
                ushort sector = BitConverter.ToUInt16(startSector, 0);
                ushort len = BitConverter.ToUInt16(fileLength, 0);

                // Get the file data
                byte[] fileData = new byte[len];
                if (len > 0)
                {
                    Array.Copy(diskData, sector * 512, fileData, 0, len);
                    VMFS1FileTableRecord record = new VMFS1FileTableRecord(Encoding.UTF8.GetString(filename), sector, fileData);
                    fileTable.Add(record);
                }

                offset = offset + 32;
            }
        }

        List<VMFS1FileTableRecord> fileTable;

        public VMFS1FileTableRecord[] FileTable
        {
            get
            {
                return fileTable.ToArray();
            }
        }

        public VMFS1FileTableRecord this[int index]
        {
            get
            {
                if ((index >= 0) & (index < fileTable.Count))
                    return fileTable[index];
                else
                    throw new IndexOutOfRangeException();
            }
        }

        public VMFS1FileTableRecord this[string filename]
        {
            get
            {
                for (int i = 0; i < fileTable.Count; i++)
                {
                    if (fileTable[i].Name == filename)
                        return fileTable[i];
                }

                throw new IndexOutOfRangeException();
            }
            set
            {
                for (int i = 0; i < fileTable.Count; i++)
                {
                    if (fileTable[i].Name == filename)
                    {
                        fileTable[i] = value;
                        return;
                    }
                }

                throw new IndexOutOfRangeException();
            }
        }

        public int Count => ((ICollection<VMFS1FileTableRecord>)fileTable).Count;

        public bool IsReadOnly => ((ICollection<VMFS1FileTableRecord>)fileTable).IsReadOnly;

        public override int Size
        {
            get
            {
                int size = 0;

                for (int i = 0; i < Count; i++)
                {
                    size += this[i].Data.Length;
                }

                return size;
            }
        }

        public override int Attributes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override int VolumeSize { get { return 1474560; } }

        public override string Name 
        { 
            get 
            { 
                return volumeLabel.Trim('\0'); 
            }
            set
            {
                volumeLabel = value.Trim().PadRight(8, '\0');
            }
        }

        public override string FullName { get { return volumeLabel.Trim('\0'); } }

        public override FileSystemInfo<VMFS1DirectoryInfo, VMFS1FileInfo> Parent { get { return null; } }

        public void Add(VMFS1FileTableRecord item)
        {
            ((ICollection<VMFS1FileTableRecord>)fileTable).Add(item);
        }

        public void Clear()
        {
            ((ICollection<VMFS1FileTableRecord>)fileTable).Clear();
        }

        public bool Contains(VMFS1FileTableRecord item)
        {
            return ((ICollection<VMFS1FileTableRecord>)fileTable).Contains(item);
        }

        public void CopyTo(VMFS1FileTableRecord[] array, int arrayIndex)
        {
            ((ICollection<VMFS1FileTableRecord>)fileTable).CopyTo(array, arrayIndex);
        }

        public bool Remove(VMFS1FileTableRecord item)
        {
            return ((ICollection<VMFS1FileTableRecord>)fileTable).Remove(item);
        }

        public IEnumerator<VMFS1FileTableRecord> GetEnumerator()
        {
            return ((IEnumerable<VMFS1FileTableRecord>)fileTable).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)fileTable).GetEnumerator();
        }

        public override byte[] ToBinary()
        {
            List<byte> value = new List<byte>();

            value.AddRange(jump);
            value.AddRange(Encoding.UTF8.GetBytes(volumeLabel));
            value.AddRange(new byte[8 - volumeLabel.Length]);
            value.AddRange(Encoding.UTF8.GetBytes(fileSystemLabel));
            value.AddRange(new byte[8 - fileSystemLabel.Length]);
            value.AddRange(bootstrap);
            value.AddRange(bootSignature);

            if (value.Count != 512)
                throw new Exception(string.Format("Boot sector is not the correct length. ({0} bytes instead of 512 bytes)", value.Count));

            // Build file table
            for (int i = 0; i < 2880; i++)
            {
                if (i < fileTable.Count)
                {
                    value.AddRange(fileTable[i].RecordData);
                }
                else
                {
                    value.AddRange(new byte[32]);
                }
            }

            if (value.Count != (2880 * 32) + 512)
                throw new Exception(string.Format("File table is not the correct length. ({0} bytes instead of 92572 bytes)", value.Count));

            // Add file data
                for (int i = 0; i < fileTable.Count; i++)
            {
                value.AddRange(fileTable[i].Data);
            }

            byte[] padding = new byte[1474560 - value.Count];
            value.AddRange(padding);

            return value.ToArray();
        }

        public override VMFS1FileInfo[] GetFiles()
        {
            VMFS1FileInfo[] files = new VMFS1FileInfo[Count];

            for (int i = 0; i < Count; i++)
            {
                files[i] = new VMFS1FileInfo(this, this[i]);
            }

            return files;
        }

        public bool Contains(string filename)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Name == filename)
                    return true;
            }

            return false;
        }

        public override void Create()
        {
            System.IO.File.WriteAllBytes(VolumeLabel.Trim() + ".bin", ToBinary());
        }

        public override void Delete()
        {
            throw new NotImplementedException();
        }

        public override void Copy(VMFS1DirectoryInfo destination, string newName = null)
        {
            throw new NotImplementedException();
        }

        public override bool Exists()
        {
            throw new NotImplementedException();
        }

        public override void Defragment()
        {
            ushort sector = 181;

            for (int i = 0; i < Count; i++)
            {
                VMFS1FileTableRecord record = fileTable[i];
                record.StartSector = sector;
                fileTable[i] = record;

                int sectorLength = record.Data.Length / 512;
                if (record.Data.Length % 512 > 0)
                    sectorLength++;
                sector = (ushort)(sector + sectorLength);
            }
        }

        public override void Rename(string name)
        {
            if (name.Length > 8)
                throw new ArgumentOutOfRangeException();
            else
                volumeLabel = name.PadRight(8, '\0');
        }

        public override DirectoryInfo<VMFS1DirectoryInfo, VMFS1FileInfo> CreateSubdirectory(string name)
        {
            throw new NotImplementedException();
        }

        public override DirectoryInfo<VMFS1DirectoryInfo, VMFS1FileInfo>[] GetDirectories()
        {
            throw new NotImplementedException();
        }

        public override FileInfo<VMFS1DirectoryInfo, VMFS1FileInfo> CreateFile(string name)
        {
            VMFS1FileInfo file = new VMFS1FileInfo(this, name);
            return file;
        }
    }

    public struct VMFS1FileTableRecord
    {
        string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value.Length <= 28)
                {
                    name = value;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public ushort StartSector { get; set; }

        public byte[] Data { get; set; }

        public byte[] RecordData
        {
            get
            {
                List<byte> value = new List<byte>();

                value.AddRange(Encoding.UTF8.GetBytes(Name));
                value.AddRange(new byte[28 - name.Length]);
                value.AddRange(BitConverter.GetBytes(StartSector));
                value.AddRange(BitConverter.GetBytes((ushort)Data.Length));     // Convert to ushort to make the value 2 bytes instead of 4

                return value.ToArray();
            }
        }

        public VMFS1FileTableRecord(string name, ushort startSector, byte[] data)
            : this()
        {
            this.name = name;
            StartSector = startSector;
            Data = data;
        }
    }
}
