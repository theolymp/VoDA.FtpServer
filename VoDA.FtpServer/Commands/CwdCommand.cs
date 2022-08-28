﻿using System.IO;
using System.Threading.Tasks;

using VoDA.FtpServer.Attributes;
using VoDA.FtpServer.Contexts;
using VoDA.FtpServer.Interfaces;
using VoDA.FtpServer.Models;

namespace VoDA.FtpServer.Commands
{
    [FtpCommand("CWD")]
    internal class CwdCommand : BaseCommand
    {
        public override Task<IFtpResult> Invoke(FtpClient client, FtpClientParameters configParameters, string? args)
        {
            if (string.IsNullOrWhiteSpace(args))
                return Task.FromResult(FoulderNotFound());
            if(configParameters.FileSystemOptions.ExistFoulder(client, NormalizationPath(Path.Join(client.Root, args))))
                args = NormalizationPath(Path.Join(client.Root, args));
            else if(configParameters.FileSystemOptions.ExistFoulder(client, NormalizationPath(args)))
                args = NormalizationPath(args);
            else
                return Task.FromResult(FoulderNotFound());
            client.Root = args;
            return Task.FromResult(ChangedToNewDirectory());
        }
    }
}
