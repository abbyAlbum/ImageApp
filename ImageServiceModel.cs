﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    interface ImageServiceModel
    {
        string AddFile(string path, out bool result);
    }
}
