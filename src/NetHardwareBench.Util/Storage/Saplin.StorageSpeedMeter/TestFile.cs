﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Saplin.StorageSpeedMeter
{
    public class TestFile : IDisposable
    {
        bool disposed = false;
        const int buffer = 4 * 1024;
        string path;
        string folderPath;

        protected internal bool flushWrites, enableMemCache;
        protected internal Action flush;

        public FileStream WriteStream
        {
            get;
        }

        public FileStream ReadStream//FILE_FLAG_NO_BUFFERING attribute breakes unaligned writes, separate read/write handles/streams are needed with different CreateFile attribtes
        {
            get;
        }

        public long TestAreaSizeBytes
        {
            get;
        }

        public void EmptyMemCacheAfterWritesIfNeeded()
        {
            if (WriteStream is PosixUncachedFileStream)
            {
                (WriteStream as PosixUncachedFileStream).EmptyMemCacheAfterWritesIfNeeded();
            }

            if (ReadStream is PosixUncachedFileStream)
            {
                (ReadStream as PosixUncachedFileStream).EmptyMemCacheAfterWritesIfNeeded();
            }
        }

        /// <summary>
        /// Opens write/read streams to test file and prepares the stream for tests (e.g. disabling OS file cache, disabling device's write buffers)
        /// </summary>
        /// <param name="drivePath">Drive to test and store the temp file, the contructor attempts to find user folder in case system drive is selected and writing to rout is resricted</param>
        /// <param name="writeBuffering">If set to <c>true</c> FileOptions.WriteThrough is used when creating System.IO.FileStream - whether to use write buffering or not</param>
        public TestFile(string drivePath, long testAreaSizeBytes, bool writeBuffering = false, bool enableMemCache = false, string filePath = null, Action flush = null, bool mockStream = false, bool disablePosixStream = false)
        {
            path = string.IsNullOrEmpty(filePath) ? RamDiskUtil.GetTempFilePath(drivePath) : filePath;
            folderPath = System.IO.Path.GetDirectoryName(path);

            TestAreaSizeBytes = testAreaSizeBytes;

            this.flush = flush;
            this.flushWrites = RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) ? false : !writeBuffering;
            //this.flushWrites = !writeBuffering;
            this.enableMemCache = enableMemCache;

            // FileOptions.WriteThrough causes OS and App buffer flush on Windows (wihtout device buffer flush) and is ignored on mac and Android
            // FileStram.Flush(true) calls POISX fsync() on mac/Android and doesn't do device buffer flush while on Windows it cuases device flush and significanly lower results
            // Thus if write buffering is turned off, on Windows WriteThrough flag is used while on Mac/Android Stream.Flush(true) 

            if (!mockStream)
            {

                if (!disablePosixStream //somehow iOS get in this category, manually disable
                    && (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX)
                    || RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) //TODO, check Android doesn't get into Linux cat
                    ) ) 
                {
                    WriteStream = new PosixUncachedFileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, buffer,
                        FileOptions.None, enableMemCache);
                    ReadStream = new PosixUncachedFileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, buffer,
                        FileOptions.None, enableMemCache);
                }
                else //Windows and rest
                {
                    WriteStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, buffer, !writeBuffering ? FileOptions.WriteThrough : FileOptions.None);
                    //WriteStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, buffer, FileOptions.None);
                    ReadStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, buffer, enableMemCache ? FileOptions.None : (FileOptions)0x20000000/*FILE_FLAG_NO_BUFFERING*/);
                }

            }
            else
            {
                WriteStream = new MockFileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, buffer, FileOptions.None);
                ReadStream = new MockFileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, buffer, FileOptions.None);
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                ReadStream.Dispose();
                WriteStream.Dispose();
            }

            System.IO.File.Delete(path);

            disposed = true;
        }

        ~TestFile()
        {
            Dispose(false);
        }

        public string Path
        {
            get
            {
                return path;
            }
        }


        public string FolderPath
        {
            get
            {
                return folderPath;
            }
        }
    }
}