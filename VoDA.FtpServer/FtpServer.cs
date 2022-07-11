﻿using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using VoDA.FtpServer.Interfaces;
using VoDA.FtpServer.Models;

namespace VoDA.FtpServer
{
    internal class FtpServer : IFtpServerControl
    {
        private TcpListener _serverSocket;
        private FtpServerOptions _serverOptions;
        private FtpServerAuthorization _serverAuthorization;
        private FtpServerFileSystemOptions _serverFileSystemOptions;
        private FtpServerCertificate _serverCertificate;
        private bool _isEnable = false;
        private Task _handlerTask;
        private CancellationToken cancellation;
        private List<FtpClient> ftpClients = new List<FtpClient>();

        public FtpServer(FtpServerOptions serverOptions, FtpServerAuthorization serverAuthorization,
            FtpServerFileSystemOptions serverFileSystemOptions, FtpServerCertificate serverCertificate)
        {
            _serverOptions = serverOptions;
            _serverAuthorization = serverAuthorization;
            _serverFileSystemOptions = serverFileSystemOptions;
            _serverCertificate = serverCertificate;
            _serverSocket = new TcpListener(_serverOptions.ServerIp, _serverOptions.Port);
            var logger = new LoggerConfiguration();
            if (_serverOptions.IsEnableLog)
                logger.WriteTo.Console();
            Log.Logger = logger.CreateLogger();
        }

        public Task StartAsync(CancellationToken token)
        {
            if (_isEnable)
                throw new Exception("Server is runing.");
            _isEnable = true;
            cancellation = token;
            _serverSocket.Start();
            Log.Information($"Server is running (ftp://localhost:{_serverOptions.Port}/)");
            if (_serverOptions.MaxConnections > 0)
            {
                Log.Information($"Enabled limited connections mode.");
                Log.Information($"Max count connections = {_serverOptions.MaxConnections}");
            }
            if (_serverOptions.ServerIp != System.Net.IPAddress.Any)
            {
                Log.Information($"Enabled filter connections mode.");
                Log.Information($"Waiting connection from {_serverOptions.ServerIp}");
            }
            return handler(token);
        }

        public Task StopAsync()
        {
            if (!_isEnable)
                throw new Exception("Server isn`t runing.");
            _isEnable = false;
            return Task.CompletedTask;
        }

        private Task handler(CancellationToken token)
        {
            _handlerTask = Task.Run(() =>
            {
                while (!token.IsCancellationRequested && _isEnable)
                {
                    var tcp = _serverSocket.AcceptTcpClient();
                    if (_serverOptions.MaxConnections > 0 && ftpClients.Count >= _serverOptions.MaxConnections)
                    {
                        StreamWriter stream = new StreamWriter(tcp.GetStream());
                        stream.WriteLine("221 The server is full!");
                        stream.Flush();
                        stream.Close();
                        tcp.Close();
                        stream = null;
                        continue;
                    }
                    var client = new FtpClient(tcp);
                    ftpClients.Add(client);
                    client.OnEndProcessing += Client_OnEndProcessing;
                    client.HandleClient(_serverOptions, _serverAuthorization, _serverFileSystemOptions, _serverCertificate);
                }
                _serverSocket.Stop();
            });
            return _handlerTask;
        }

        private void Client_OnEndProcessing(FtpClient client)
        {
            ftpClients.Remove(client);
        }
    }
}
