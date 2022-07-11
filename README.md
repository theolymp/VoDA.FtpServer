# VoDA.FtpServer

[![nuget](https://img.shields.io/static/v1?label=NuGet&message=VoDA.FtpServer&color=blue&logo=nuget)](https://www.nuget.org/packages/VoDA.FtpServer)

# Description

VoDA.FtpServer is a simple FTP server library. This library simplifies interaction with the FTP protocol down to the events level. All requests to the server related to authorization or working with data cause events that you must implement.

## Quick start

To start the server, you need to create an [FtpServerBuilder](https://github.com/VoDACode/VoDA.FtpServer/blob/master/VoDA.FtpServer/FtpServerBuilder.cs) object, configure it using functions, as shown in the example below. After configuration, call the ```Build()``` function to create a server.

An example of an FTP server for working with the file system is given in the [Test](https://github.com/VoDACode/VoDA.FtpServer/tree/master/Test) project.

## Example

```c#
var server = new FtpServerBuilder()
    .ListenerSettings((config) =>
    {
        config.Port = 21; // enter the port
        config.ServerIp = System.Net.IPAddress.Any;
    })
    .Certificate((config) =>
    {
        config.CertificatePath = ".\\server.crt";
        config.CertificateKey = ".\\server.key";
    })
    .Authorization((config) =>
    {
        config.UseAuthorization = true; // enable or disable authorization
        config.UsernameVerification += (username) => {...}; // username verification
        config.PasswordVerification += (username, password) => {...}; //verification of username and password
    })
    .FileSystem((fs) =>
    {
        fs.OnDelete += (client, path) => {...}; // delete file event
        fs.OnRename += (client, from, to) => {...}; // rename item event
        fs.OnDownload += (client, path) => {...};   // download file event
        fs.OnGetList += (client, path) => {...};    // get items in folder event
        fs.OnExistFile += (client, path) => {...};  // file check event
        fs.OnExistFoulder += (client, path) => {...};   // folder check event
        fs.OnCreate += (client, path) => {...}; // file creation event
        fs.OnAppend += (client, path) => {...}; // append file event
        fs.OnRemoveDir += (client, path) => {...};  // remove folder event
        fs.OnUpload += (client, path) => {...}; // upload file event
        fs.OnGetFileSize += (client, path) => {...};    // get file size event
    })
    .Build();
// Start FTP-serer
server.StartAsync(System.Threading.CancellationToken.None).Wait();
```
