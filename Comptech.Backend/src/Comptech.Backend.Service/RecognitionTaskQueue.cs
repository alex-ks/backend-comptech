using Comptech.Backend.Service.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Comptech.Backend.Service
{
    public class RecognitionTaskQueue
    {
        private static volatile RecognitionTaskQueue instance;
        private static Object thislock = new Object();

        private ConcurrentQueue<RecognitionTask> queue = new ConcurrentQueue<RecognitionTask>();  

        public static RecognitionTaskQueue GetRecognitionTaskQueue()
        {
            if(instance == null)
            {
                lock (thislock)
                {
                    if (instance == null)
                        instance = new RecognitionTaskQueue();
                }
            }
            return instance;
        }

        public RecognitionTask Dequeue()
        {
            if(!queue.IsEmpty)
            {
                lock(thislock)
                {
                    if(!queue.IsEmpty)
                    {
                        RecognitionTask task = null;
                        var result = queue.TryDequeue(out task);
                        if (result)
                            return task;
                        //что возвращать в случае неудачи?
                    }
                }
            }

            //очередь пуста, возвращать нечего
            if(queue.IsEmpty)
            {
                lock (thislock)
                {
                    if (queue.IsEmpty)
                    {
                        //что нужно здесь сделать?
                    }
                }
            }

        }

        public void Enqueue(RecognitionTask task)
        {
            //если очередь не пуста, то просто добавляем таск в очередь
            if(!queue.IsEmpty)
            {
                lock(thislock)
                {
                    if(!queue.IsEmpty)
                    {
                        queue.Append(task);
                        return;
                    }
                }
            }

            //если очередь пуста, то делаем что-то особенное
            if(queue.IsEmpty)
            {
                lock (thislock)
                {
                    if (queue.IsEmpty)
                    {
                        //что именно здесь нужно сделать?

                    }
                }
            }
        }

        private RecognitionTaskQueue()
        {

        }
    }
}
