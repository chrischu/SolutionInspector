using System;
using System.IO;
using System.Runtime.Remoting;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Win32.SafeHandles;
using Wrapperator.Interfaces;
using Wrapperator.Interfaces.IO;

namespace SolutionInspector.SchemaGenerator.Tests.Commands
{
  public class RecordingFileStream : IFileStream
  {
    private readonly MemoryStream _stream = new MemoryStream();

    public string ToString(Encoding encoding = null)
    {
      encoding = encoding ?? Encoding.UTF8;
      return encoding.GetString(_stream.ToArray());
    }

    public void Dispose ()
    {
      _stream.Dispose();
    }

    public Task CopyToAsync ([NotNull] IStream destination)
    {
      return _stream.CopyToAsync(destination._Stream);
    }

    public Task CopyToAsync ([NotNull] IStream destination, int bufferSize)
    {
      return _stream.CopyToAsync(destination._Stream, bufferSize);
    }

    public Task CopyToAsync ([NotNull] IStream destination, int bufferSize, CancellationToken cancellationToken)
    {
      return _stream.CopyToAsync(destination._Stream, bufferSize, cancellationToken);
    }

    public void CopyTo ([NotNull] IStream destination)
    {
      _stream.CopyTo(destination._Stream);
    }

    public void CopyTo ([NotNull] IStream destination, int bufferSize)
    {
      _stream.CopyTo(destination._Stream, bufferSize);
    }

    public void Close ()
    {
      _stream.Close();
    }

    public void Flush ()
    {
      _stream.Flush();
    }

    public Task FlushAsync ()
    {
      return _stream.FlushAsync();
    }

    public Task FlushAsync (CancellationToken cancellationToken)
    {
      return _stream.FlushAsync(cancellationToken);
    }

    public IAsyncResult BeginRead ([NotNull] byte[] buffer, int offset, int count, [CanBeNull] AsyncCallback callback, [CanBeNull] object state)
    {
      return _stream.BeginRead(buffer, offset, count, callback, state);
    }

    public int EndRead ([NotNull] IAsyncResult asyncResult)
    {
      return _stream.EndRead(asyncResult);
    }

    public Task<int> ReadAsync ([NotNull] byte[] buffer, int offset, int count)
    {
      return _stream.ReadAsync(buffer, offset, count);
    }

    public Task<int> ReadAsync ([NotNull] byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
      return _stream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public IAsyncResult BeginWrite ([NotNull] byte[] buffer, int offset, int count, [CanBeNull] AsyncCallback callback, [CanBeNull] object state)
    {
      return _stream.BeginWrite(buffer, offset, count, callback, state);
    }

    public void EndWrite ([NotNull] IAsyncResult asyncResult)
    {
      _stream.EndWrite(asyncResult);
    }

    public Task WriteAsync ([NotNull] byte[] buffer, int offset, int count)
    {
      return _stream.WriteAsync(buffer, offset, count);
    }

    public Task WriteAsync ([NotNull] byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
      return _stream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public long Seek (long offset, SeekOrigin origin)
    {
      return _stream.Seek(offset, origin);
    }

    public void SetLength (long value)
    {
      _stream.SetLength(value);
    }

    public int Read ([NotNull] byte[] buffer, int offset, int count)
    {
      return _stream.Read(buffer, offset, count);
    }

    public int ReadByte ()
    {
      return _stream.ReadByte();
    }

    public void Write ([NotNull] byte[] buffer, int offset, int count)
    {
      _stream.Write(buffer, offset, count);
    }

    public void WriteByte (byte value)
    {
      _stream.WriteByte(value);
    }

    public object GetLifetimeService ()
    {
      return _stream.GetLifetimeService();
    }

    public object InitializeLifetimeService ()
    {
      return _stream.InitializeLifetimeService();
    }

    public ObjRef CreateObjRef ([NotNull] IType requestedType)
    {
      return _stream.CreateObjRef(requestedType._Type);
    }

    public Stream _Stream => _stream;

    public bool CanRead => _stream.CanRead;

    public bool CanSeek => _stream.CanSeek;

    public bool CanTimeout => _stream.CanTimeout;

    public bool CanWrite => _stream.CanWrite;

    public long Length => _stream.Length;

    public long Position
    {
      get => _stream.Position;
      set => _stream.Position = value;
    }

    public int ReadTimeout
    {
      get => _stream.ReadTimeout;
      set => _stream.ReadTimeout = value;
    }

    public int WriteTimeout
    {
      get => _stream.WriteTimeout;
      set => _stream.WriteTimeout = value;
    }

    public FileSecurity GetAccessControl ()
    {
      return new FileSecurity();
    }

    public void SetAccessControl ([CanBeNull] FileSecurity fileSecurity)
    {
    }

    public void Flush (bool flushToDisk)
    {
      _stream.Flush();
    }

    public void Lock (long position, long length)
    {
    }

    public void Unlock (long position, long length)
    {
    }

    public FileStream _FileStream => throw new NotImplementedException();

    public bool IsAsync => false;

    public string Name => "Name";

    public SafeFileHandle SafeFileHandle => throw new NotImplementedException();
  }
}