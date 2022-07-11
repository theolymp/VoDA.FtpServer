﻿using System.Net;
using System;

using VoDA.FtpServer.Interfaces;

namespace VoDA.FtpServer
{
    internal class FtpServerOptions : IFtpServerOptions, IValidConfig
    {
        public int Port { get; set; }
        public int MaxConnections { get; set; }
        public IPAddress ServerIp { get; set; } = IPAddress.Any;
        public bool IsEnableLog { get; set; } = true;

        public void Valid()
        {
            if (Port < 0 || Port >= 65536)
                throw new ArgumentOutOfRangeException("Port");
            if (MaxConnections < 0)
                throw new ArgumentOutOfRangeException("MaxConnections");
        }
    }
}
