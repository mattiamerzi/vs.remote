using VsRemote.Enums;
using VsRemote.Exceptions;
using VsRemote.Interfaces;
using VsRemote.Model;
using VsRemote.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VsRemote.Sample;

/// <summary>
/// Implementazione ottimizzata di LocalFolderFilesystem che implementa direttamente IVsRemoteFileSystem
/// con metodi ottimizzati per le operazioni di lettura e scrittura con offset.
/// </summary>
public class OptimizedLocalFolderFilesystem : IVsRemoteFileSystem
{
    private readonly string _basePath;
    
    private static readonly IVsRemoteINode _root = new VsRemoteRootINode(
        CTime: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        MTime: DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    );

    public OptimizedLocalFolderFilesystem(string basePath)
    {
        _basePath = basePath;
    }

    public IVsRemoteINode RootINode => _root;

    /// <summary>
    /// Converte un percorso virtuale in un percorso fisico del filesystem locale.
    /// </summary>
    private string LocalPath(string path) => 
        Path.Join(_basePath, path.Replace(VsPath.PATH_SEPARATOR, Path.DirectorySeparatorChar));

    #region Implementazione IVsRemoteFileSystem

    public async Task<IVsRemoteINode> Stat(string path)
    {
        string localPath = LocalPath(path);
        
        if (Directory.Exists(localPath) || File.Exists(localPath))
        {
            return await Task.Run(() =>
                new VsRemoteINode(
                    Path.GetFileName(localPath),
                    Directory.Exists(localPath) ? VsRemoteFileType.Directory : VsRemoteFileType.File,
                    Readonly: (File.GetAttributes(localPath) & FileAttributes.ReadOnly) != 0,
                    ((DateTimeOffset)File.GetCreationTimeUtc(localPath)).ToUnixTimeSeconds(),
                    ((DateTimeOffset)File.GetLastWriteTimeUtc(localPath)).ToUnixTimeSeconds(),
                    ((DateTimeOffset)File.GetLastAccessTimeUtc(localPath)).ToUnixTimeSeconds(),
                    Size: File.Exists(localPath) ? new FileInfo(localPath).Length : 0
                )
            );
        }
        else
        {
            throw new NotFound();
        }
    }

    public async Task<IEnumerable<IVsRemoteINode>> ListDirectory(string path)
    {
        string localPath = LocalPath(path);
        
        if (!Directory.Exists(localPath))
            throw new NotFound();
            
        IEnumerable<string> dirFiles = Directory.EnumerateFileSystemEntries(localPath);
        List<IVsRemoteINode> inodes = new(dirFiles.Count());
        
        foreach (var fname in dirFiles)
        {
            try
            {
                inodes.Add(await Stat(VsPath.Join(path, Path.GetFileName(fname))));
            }
            catch (Exception)
            {
                // Ignora eventuali errori durante l'enumerazione
                // (ad esempio per file che vengono rimossi mentre l'enumerazione è in corso)
            }
        }
        
        return inodes;
    }

    public Task CreateDirectory(string path)
    {
        string localPath = LocalPath(path);
        return Task.Run(() => Directory.CreateDirectory(localPath));
    }

    public Task RemoveDirectory(string path, bool recursive)
    {
        string localPath = LocalPath(path);
        
        if (!Directory.Exists(localPath))
            throw new NotFound();
            
        return Task.Run(() => Directory.Delete(localPath, recursive));
    }

    public Task DeleteFile(string path)
    {
        string localPath = LocalPath(path);
        
        if (!File.Exists(localPath))
            throw new NotFound();
            
        return Task.Run(() => File.Delete(localPath));
    }

    public Task RenameFile(string fromPath, string toPath, bool overwrite)
    {
        string localFromPath = LocalPath(fromPath);
        string localToPath = LocalPath(toPath);
        
        if (!File.Exists(localFromPath))
            throw new NotFound();
            
        if (File.Exists(localToPath) && !overwrite)
            throw new FileExists();
            
        return Task.Run(() => File.Move(localFromPath, localToPath, overwrite));
    }

    public async Task<ReadOnlyMemory<byte>> ReadFile(string path)
    {
        string localPath = LocalPath(path);
        
        if (!File.Exists(localPath))
            throw new NotFound();
            
        return new(await File.ReadAllBytesAsync(localPath));
    }

    /// <summary>
    /// Implementazione ottimizzata di ReadFileOffset che utilizza FileStream per leggere solo la porzione di file richiesta,
    /// senza caricare l'intero file in memoria.
    /// </summary>
    public async Task<ReadOnlyMemory<byte>> ReadFileOffset(string path, int offset, int length)
    {
        string localPath = LocalPath(path);
        
        if (!File.Exists(localPath))
            throw new NotFound();
            
        using FileStream fs = new(localPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        
        // Verifica dimensione del file e aggiusta offset/length se necessario
        if (offset >= fs.Length)
            return new byte[0]; // Se l'offset è oltre la fine del file, restituisci un array vuoto
            
        // Limita la lunghezza se offset + length supera la dimensione del file
        int adjustedLength = (int)Math.Min(length, fs.Length - offset);
        
        byte[] buffer = new byte[adjustedLength];
        fs.Seek(offset, SeekOrigin.Begin);
        int bytesRead = await fs.ReadAsync(buffer, 0, adjustedLength);
        
        // Se abbiamo letto meno byte di quelli richiesti, ridimensiona il buffer
        if (bytesRead < adjustedLength)
        {
            Array.Resize(ref buffer, bytesRead);
        }
        
        return buffer;
    }

    public Task CreateFile(string path)
    {
        string localPath = LocalPath(path);
        
        if (File.Exists(localPath))
            throw new FileExists();
            
        return Task.Run(() =>
        {
            using FileStream fs = File.Create(localPath);
        });
    }

    public async Task<int> WriteFile(string path, ReadOnlyMemory<byte> content, bool overwriteIfExists, bool createIfNotExists)
    {
        string localPath = LocalPath(path);
        bool exists = File.Exists(localPath);
        
        if (exists && !overwriteIfExists)
            throw new FileExists();
            
        if (!exists && !createIfNotExists)
            throw new NotFound();
            
        using FileStream fs = new(localPath, 
            exists && overwriteIfExists ? FileMode.Truncate : FileMode.Create, 
            FileAccess.Write);
        
        await fs.WriteAsync(content);
        return content.Length;
    }

    /// <summary>
    /// Implementazione ottimizzata di WriteFileOffset che utilizza FileStream per scrivere solo la porzione specificata,
    /// senza caricare l'intero file in memoria.
    /// </summary>
    public async Task<int> WriteFileOffset(string path, int offset, ReadOnlyMemory<byte> content)
    {
        string localPath = LocalPath(path);
        
        if (!File.Exists(localPath))
            throw new NotFound();
            
        using FileStream fs = new(localPath, FileMode.Open, FileAccess.Write, FileShare.None);
        
        // Se l'offset è oltre la fine del file, espandi il file
        if (offset > fs.Length)
        {
            fs.SetLength(offset);
        }
        
        fs.Seek(offset, SeekOrigin.Begin);
        await fs.WriteAsync(content);
        return content.Length;
    }

    /// <summary>
    /// Implementazione ottimizzata di WriteFileAppend che aggiunge dati alla fine del file,
    /// senza caricare l'intero file in memoria.
    /// </summary>
    public async Task<int> WriteFileAppend(string path, ReadOnlyMemory<byte> content)
    {
        string localPath = LocalPath(path);
        
        if (!File.Exists(localPath))
            throw new NotFound();
            
        using FileStream fs = new(localPath, FileMode.Append, FileAccess.Write);
        await fs.WriteAsync(content);
        return content.Length;
    }
    #endregion
}
