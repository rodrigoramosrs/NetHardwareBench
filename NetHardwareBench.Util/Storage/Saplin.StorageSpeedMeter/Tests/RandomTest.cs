﻿using System;
using System.Diagnostics;
using System.IO;

namespace Saplin.StorageSpeedMeter
{
    public abstract class RandomTest : Test
    {
        protected const int memoryBuffSize = 128 * 1024 * 1024;
        private const int maxBlocksInTest = 1 * 1024 * 1024; // 8 Mb for shuffled positions, 4GB for 4KB blocks

        protected readonly FileStream fileStream;
        protected readonly TestFile file;

        protected readonly double? percentOfFileToOwerwrite;
        protected readonly int? maxTestTime;
        protected readonly int blocksInMemory;

        protected long maxBlock; // max index of block accesible
        protected long minBlock;

        public RandomTest(FileStream fileStream, TestFile file, int blockSize, int maxTestTimeSecs = 25)
        {
            if (percentOfFileToOwerwrite <= 0) throw new ArgumentOutOfRangeException("percentOfFileToOwerwrite", "Size of file to be overwritten (in percents of it's current size) must greater than 0");
            if (blockSize <= 0) throw new ArgumentOutOfRangeException("blockSize", "Block size cant be negative");

            this.fileStream = fileStream;
            this.file = file;
            this.blockSize = blockSize;
            blocksInMemory = memoryBuffSize / blockSize;
            this.maxTestTime = maxTestTimeSecs;
        }

        private long[] positionsPlan;

        protected virtual void ValidateAndInitParams()
        {
            if (fileStream.Length == 0) throw new InvalidOperationException("File can't be empty");
            if (blockSize > fileStream.Length) throw new InvalidOperationException("Block size cant be greater than file size");
        }

        private void GeneratePositionsPlan() // to avoid RAM caching, it's important to avoid repetative reads of same blocks - if read once, the block might stay in RAM cache and next read will yield RAM value. Possition shuffaling is used
        {
            positionsPlan = new long[Math.Min(maxBlock - minBlock, maxBlocksInTest)];
            for (long i = 0; i < positionsPlan.Length - 1; i++)
                positionsPlan[i] = i*blockSize;

            // EACH SECOND BLOCK
            //for (long i = 0; i < positionsPlan.Length/2 - 1; i++)
              //  positionsPlan[i] = i*2 * blockSize;

            Shuffle(positionsPlan);
        }

        private void Shuffle(long[] list)
        {
            var rng = new Random();
            var n = list.Length;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                long value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public override TestResults Execute()
        {
            Status = TestStatus.Started;

            //Linux
            file.EmptyMemCacheAfterWritesIfNeeded();

            ValidateAndInitParams();
            GeneratePositionsPlan();

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
            long currOffset = 0;

            var i = 0;

            var elapsed = new Stopwatch();
            elapsed.Start();

            Status = TestStatus.Running;

            while (i < positionsPlan.Length + 1)
            {
                if (breakCalled)
                {
                    return results;
                }

                currOffset = positionsPlan[i];

                i++; // easier to calculate progress in precent when starting with 1

                DoOperation(data, sw, currOffset, i);

                results.AddTroughputMbs(blockSize, currOffset, sw);

                curPercent = Math.Min(
                    100, 
                    Math.Max(
                        (int)(elapsed.ElapsedMilliseconds / 10 / maxTestTime), 
                        curPercent = (int)(i * 100 / (double)positionsPlan.Length)
                        )
                );

                if (curPercent > prevPercent)
                {
                    Update(curPercent, results.GetLatest5AvgResult(), results: results);
                    prevPercent = curPercent;

                    if (curPercent == 100) break;
                }
            }

            elapsed.Stop();
            results.TotalTimeMs = elapsed.ElapsedMilliseconds;

            FinalUpdate(results, elapsed.ElapsedMilliseconds);

            //if (cachePurger != null)
            //{
            //    cachePurger.Release();
            //}

            return results;
        }

        protected abstract void DoOperation(byte[] data, Stopwatch sw, long currBlock, int i);

        protected abstract byte[] InitBuffer();
    }
}


