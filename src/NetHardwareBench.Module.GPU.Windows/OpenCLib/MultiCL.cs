using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace OpenCL
{
    public class MultiCL
    {
        public AcceleratorDevice[] Accelerators;
        public OpenCL[] Context;
        public event EventHandler<double> ProgressChangedEvent;

        public MultiCL()
        {
            this.Accelerators = AcceleratorDevice.All;
            this.Context = Accelerators.Select(x => new OpenCL() { Accelerator = x }).ToArray();
        }

        public void SetKernel(string Kernel, string Method)
        {
            SetOnAll(x => x.SetKernel(Kernel, Method));
        }

        public void SetParameter(params object[] Arguments)
        {
            SetOnAll(x => x.SetParameter(Arguments));
        }

        Task Enqueue(int from, int to, OpenCL acc)
        {
            return Task.Run(() => acc.Execute(from, to - from, -1));
        }

        void SetOnAll(Action<OpenCL> Setvar)
        {
            Parallel.ForEach(Context, (ctx) =>
            {
                Setvar(ctx);    //compiles kernel to all devices
            });
        }

        public void Invoke(int FromInclusive, int ToInclusive, int Parts)
        {
            if (Parts == 1)
            {
                Task.WaitAll(Enqueue(FromInclusive, ToInclusive, Context.First()));
                ProgressChangedEvent?.Invoke(this, 1.0);
                return;
            }

            Queue<Tuple<int, int>> parts = new Queue<Tuple<int, int>>(); //split Indicess into parts
            int delta = (ToInclusive - FromInclusive) / Parts;
            for (int i = 0; i < Parts; i++)
            {
                Tuple<int, int> local = new Tuple<int, int>(i * delta, (i + 1) * delta);
                parts.Enqueue(local);
            }
            if (Parts * delta != ToInclusive) parts.Enqueue(new Tuple<int, int>(Parts * delta, ToInclusive));

            int startlen = parts.Count;
            var worktodo = parts.Dequeue();
            var worktodo2 = parts.Dequeue();

            List<Task> Tasks = new List<Task>(); //Initialize Tasks Array
            for(int i=0; i < Context.Length && i <= ToInclusive; i++)
            {
                Tasks.Add(Enqueue(worktodo.Item1, worktodo.Item2, Context[i]));
            }

            while (parts.Count >= 1) //finishes when all work is started
            {
                var nextwork = parts.Dequeue();
                int finishedindex = Task.WaitAny(Tasks.ToArray()); //device on which invoke was called
                Tasks[finishedindex] = Enqueue(nextwork.Item1, nextwork.Item2, Context[finishedindex]); //start next workitem

                ProgressChangedEvent?.Invoke(this, (startlen - parts.Count) / (double)startlen);
            }

            Task.WaitAll(Tasks.ToArray()); //waits for all finish
        }


        
    }
}
