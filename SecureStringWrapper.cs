using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace UI;

public sealed class SecureStringWrapper : IDisposable
{
    private readonly SecureString _secureString;
    private byte[] _bytes = null;

    public SecureStringWrapper(SecureString secureString)
    {
        if (secureString == null)
        {
            throw new ArgumentNullException(nameof(secureString));
        }
        _secureString = secureString;
    }

    public unsafe byte[] ToByteArray()
    {

        int maxLength = Encoding.UTF8.GetMaxByteCount(_secureString.Length);

        IntPtr bytes = IntPtr.Zero;
        IntPtr str = IntPtr.Zero;

        try
        {
            bytes = Marshal.AllocHGlobal(maxLength);
            str = Marshal.SecureStringToBSTR(_secureString);

            char* chars = (char*)str.ToPointer();
            byte* bptr = (byte*)bytes.ToPointer();
            int len = Encoding.UTF8.GetBytes(chars, _secureString.Length, bptr, maxLength);

            _bytes = new byte[len];
            for (int i = 0; i < len; ++i)
            {
                _bytes[i] = *bptr;
                bptr++;
            }

            return _bytes;
        }
        finally
        {
            if (bytes != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(bytes);
            }
            if (str != IntPtr.Zero)
            {
                Marshal.ZeroFreeBSTR(str);
            }
        }
    }

    private bool _disposed = false;

    public void Dispose()
    {
        if (!_disposed)
        {
            _secureString.Dispose();
            Destroy();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    private void Destroy()
    {
        if (_bytes == null) { return; }

        for (int i = 0; i < _bytes.Length; i++)
        {
            _bytes[i] = 0;
        }
        _bytes = null;
    }

    ~SecureStringWrapper()
    {
        Dispose();
    }
}

