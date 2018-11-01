using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace dotnet
{
    class Segyiomethod
    {
        private const string dllPath = "/../../../../../build/lib/Debug/segyio.dll";

        [DllImport(dllPath)]
        public extern static IntPtr segy_open([MarshalAs(UnmanagedType.LPStr)] string filepath, [MarshalAs(UnmanagedType.LPStr)] string mode);

        [DllImport(dllPath)]
        public extern static int segy_binheader([MarshalAs(UnmanagedType.SysInt)] IntPtr pointer, [MarshalAs(UnmanagedType.LPArray)] [Out] byte[] binheader);

        [DllImport(dllPath)]
        public extern static int segy_get_bfield([MarshalAs(UnmanagedType.LPArray)] [In] byte[] Array, [MarshalAs(UnmanagedType.I4)] int position, [MarshalAs(UnmanagedType.I4)] ref Int32 answer);

        [DllImport(dllPath)]
        public extern static int segy_set_bfield([MarshalAs(UnmanagedType.LPArray)] [Out] byte[] Array, [MarshalAs(UnmanagedType.I4)] int position, [MarshalAs(UnmanagedType.I4)] int value);

        [DllImport(dllPath)]
        public extern static long segy_trace0([MarshalAs(UnmanagedType.LPArray)] [Out] byte[] array);

        [DllImport(dllPath)]
        public extern static int segy_samples([MarshalAs(UnmanagedType.LPArray)] [Out] byte[] array);

        [DllImport(dllPath)]
        public extern static int segy_trace_bsize([MarshalAs(UnmanagedType.I4)] int samples);

        [DllImport(dllPath)]
        public extern static int segy_readtrace([MarshalAs(UnmanagedType.SysInt)] IntPtr pointer, [MarshalAs(UnmanagedType.I4)] int traceno, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] array, [MarshalAs(UnmanagedType.I8)] long trace0, [MarshalAs(UnmanagedType.I4)] int trace_bsize);

        [DllImport(dllPath)]
        public extern static int segy_format([MarshalAs(UnmanagedType.LPArray)] [Out] byte[] array);

        [DllImport(dllPath)]
        public extern static int segy_to_native([MarshalAs(UnmanagedType.I4)] [Out] int format, [MarshalAs(UnmanagedType.I4)] [Out] int size, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] array);

        [DllImport(dllPath)]
        public extern static int segy_traceheader([MarshalAs(UnmanagedType.SysInt)] IntPtr pointer, [MarshalAs(UnmanagedType.I4)] [Out] int tracenum, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] array, [MarshalAs(UnmanagedType.I8)] long trace0, [MarshalAs(UnmanagedType.I4)] int trace_bsize);

        [DllImport(dllPath)]
        public extern static int segy_get_field([MarshalAs(UnmanagedType.LPArray)] [Out] byte[] traceheader, [MarshalAs(UnmanagedType.I4)] [Out] int field, [MarshalAs(UnmanagedType.I4)] [In] ref Int32 answer);

        [DllImport(dllPath)]
        public extern static int segy_offsets([MarshalAs(UnmanagedType.SysInt)] IntPtr pointer, [MarshalAs(UnmanagedType.I4)] [Out] int iline, [MarshalAs(UnmanagedType.I4)] [Out] int xline, [MarshalAs(UnmanagedType.I4)] [Out] int traces, [MarshalAs(UnmanagedType.I4)] [In] ref Int32 outval, [MarshalAs(UnmanagedType.I8)] long trace0, [MarshalAs(UnmanagedType.I4)] int trace_bsize);

        [DllImport(dllPath)]
        public extern static int segy_offset_indices([MarshalAs(UnmanagedType.SysInt)] IntPtr pointer, [MarshalAs(UnmanagedType.I4)] [Out] int offsetfield, [MarshalAs(UnmanagedType.I4)] [Out] int offsets, [MarshalAs(UnmanagedType.LPArray)] [In] int[] array, [MarshalAs(UnmanagedType.I8)] long trace0, [MarshalAs(UnmanagedType.I4)] int trace_bsize);

        [DllImport(dllPath)]
        public extern static int segy_close([MarshalAs(UnmanagedType.SysInt)] IntPtr pointer);

        [DllImport(dllPath)]
        public extern static int segy_trsize([MarshalAs(UnmanagedType.I4)] [Out] int format, [MarshalAs(UnmanagedType.I4)] [Out] int samples);

        [DllImport(dllPath)]
        public extern static int segy_traces([MarshalAs(UnmanagedType.SysInt)] IntPtr pointer, [MarshalAs(UnmanagedType.I4)] [In]  ref int traces, [MarshalAs(UnmanagedType.I8)] long trace0, [MarshalAs(UnmanagedType.I4)] int trace_bsize);

    }

}
