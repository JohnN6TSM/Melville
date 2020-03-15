﻿using  System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace Melville.MVVM.FileSystem
{
  public interface IFileSystemObject
  {
    string Path { get; }
    IDirectory? Directory { get; }
    string Name { get; }
    bool Exists();
    DateTime LastAccess { get; set; }
    DateTime LastWrite { get; }
    DateTime Created { get; }
    FileAttributes Attributes { get; }
  }

  public interface IFile : IFileSystemObject
  {
    Task<Stream> OpenRead();
    Task<Stream> CreateWrite(FileAttributes attributes = FileAttributes.Normal);
    void Delete();
    byte FinalProgress { get; }

    Task WaitForFinal { get; }
    bool ValidFileSystemPath();
    long Size { get; }
  }
  
  public interface IDirectory : IFileSystemObject
  {
    IDirectory SubDirectory(string name);
    IFile File(string name);
    IEnumerable<IFile> AllFiles();
    IEnumerable<IFile> AllFiles(string glob);
    IEnumerable<IDirectory> AllSubDirectories();
    void Create(FileAttributes attributes = FileAttributes.Directory);
    IFile FileFromRawPath(string path);
    bool IsVolitleDirectory();
    IDisposable? WriteToken();
  }
}