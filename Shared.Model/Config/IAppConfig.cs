﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Config
{
    public interface IAppConfig : IDisposable
    {
        AppConfig GetAppConfig();
    }
}
