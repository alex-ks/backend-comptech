﻿using Comptech.Backend.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comptech.Backend.Service
{
    public interface IRecognitionTaskQueue
    {
        void Enqueue(RecognitionTask task);
        RecognitionTask Dequeue();
    }
}
