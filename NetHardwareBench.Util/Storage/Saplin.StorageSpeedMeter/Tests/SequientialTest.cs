﻿using System;
using System.Diagnostics;
using System.IO;

namespace Saplin.StorageSpeedMeter
{
    public abstract class SequentialTest : Test
    {
        protected readonly FileStream fileStream;
        protected TestFile file;
        protected long totalBlocks;
        protected bool warmUp;
        protected readonly double warmUpBlocksPercentFromTotal = 0.05;

        public long TotalBlocks
        {
            get { return totalBlocks; }
        }

        public SequentialTest(FileStream fileStream, TestFile file, int blockSize, long totalBlocks = 0, bool warmUp = false)
        {
            if (blockSize <= 0) throw new ArgumentOutOfRangeException("blockSize", "Block size cant be negative");
            if (totalBlocks < 0) throw new ArgumentOutOfRangeException("totalBlocks", "Block number cant be negative");

            this.fileStream = fileStream;
            this.file = file;
            this.blockSize = blockSize;
            this.totalBlocks = totalBlocks;
            this.warmUp = warmUp;
        }

        public override TestResults Execute()
        {
            Status = TestStatus.Started;

            //Linux
            file.EmptyMemCacheAfterWritesIfNeeded();

            var results = new TestResults(this);

            byte[] data = null;

            try
            {
                data = InitBuffer();
            }
            catch
            {
                NotEnoughMemUpdate(results, 0);
                return results;
            }

            if (cachePurger != null)
            {
                Status = TestStatus.PurgingMemCache;
                cachePurger.Purge();
            }

            var sw = new Stopwatch();

            int prevPercent = -1;
            int curPercent = -1;
            fileStream.Seek(0, SeekOrigin.Begin);

            RestartElapsed();

            //var sw2 = new Stopwatch();

            var warmUpBlocks = warmUp ? (int)Math.Ceiling(totalBlocks*warmUpBlocksPercentFromTotal) : 0;
            if (warmUp) Status = TestStatus.WarmigUp; else Status = TestStatus.Running;
            for (var i = 1 - warmUpBlocks; i < totalBlocks + 1; i++)
            {
                //sw2.Restart();
                
                if (breakCalled)
                {
                    return results;
                }

                DoOperation(data, sw);

                if (i == 0) // final warm up block
                {
                    Status = TestStatus.Running;
                    fileStream.Seek(0, SeekOrigin.Begin);
                }

                if (i > 0) // not warm up blocks
                {

                    results.AddTroughputMbs(blockSize, fileStream.Position - blockSize, sw);

                    curPercent = (int)(i * 100 / totalBlocks);
                    if (curPercent > prevPercent)
                    {
                        Update(curPercent, results.GetLatest5AvgResult(), results: results);
                        prevPercent = curPercent;
                    }
                }

                //sw2.Stop();

                //var swMs = ((double)sw.ElapsedTicks / Stopwatch.Frequency)*1000 ;
                //var sw2Ms = ((double)sw2.ElapsedTicks / Stopwatch.Frequency)*1000 ;

                //Debug.WriteLine("Block: " + i + 
                //                "; sw(ms): " + swMs +
                //                "; sw2(ms): " + sw2Ms +
                //                "sw/sw2: " + swMs/sw2Ms);
            }

            results.TotalTimeMs = StopElapsed();

            FinalUpdate(results, ElapsedMs);

            //if (cachePurger != null)
            //{
            //    cachePurger.Release();
            //}

            TestCompleted();

            return results;
        }

        protected abstract void DoOperation(byte[] buffer, Stopwatch sw);

        protected abstract byte[] InitBuffer();

        protected virtual void TestCompleted()
        {
            GC.Collect(2, GCCollectionMode.Forced, true);
        }

    }
}
