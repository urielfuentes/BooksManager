﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksManager.Models
{
    public interface IReadLogsRepository
    {
        void AddLog(ReadLog newLog);
    }
}
