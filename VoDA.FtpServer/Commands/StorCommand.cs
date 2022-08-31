﻿using System.IO;
using System.Threading.Tasks;

using VoDA.FtpServer.Attributes;
using VoDA.FtpServer.Contexts;
using VoDA.FtpServer.Interfaces;
using VoDA.FtpServer.Models;

namespace VoDA.FtpServer.Commands
{
    [FtpCommand("STOR")]
    internal class StorCommand : BaseCommand
    {
        public override Task<IFtpResult> Invoke(FtpClient client, FtpClientParameters configParameters, string? args)
        {
            if (args == null)
                return Task.FromResult(CustomResponse(450, "Requested file action not taken"));
            args = NormalizationPath(args);
            var folder = Path.GetDirectoryName(args);
            if (configParameters.FileSystemOptions.ExistFoulder(client, NormalizationPath(Path.Join(client.Root, folder))))
                args = NormalizationPath(Path.Join(client.Root, args));
            else if (configParameters.FileSystemOptions.ExistFoulder(client, NormalizationPath(folder)))
                args = NormalizationPath(args);
            else
                return Task.FromResult(FoulderNotFound());
            client.SetupDataConnectionOperation(new DataConnectionOperation(client.StoreOperation, args));
            return Task.FromResult(CustomResponse(150, $"Opening {client.ConnectionType} mode data transfer for STOR"));
        }
    }
}
