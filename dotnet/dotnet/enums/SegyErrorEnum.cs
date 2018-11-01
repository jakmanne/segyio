using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet.enums
{
    enum SegyErrorEnum
    {
        SEGY_OK = 0,
        SEGY_FOPEN_ERROR,
        SEGY_FSEEK_ERROR,
        SEGY_FREAD_ERROR,
        SEGY_FWRITE_ERROR,
        SEGY_INVALID_FIELD,
        SEGY_INVALID_SORTING,
        SEGY_MISSING_LINE_INDEX,
        SEGY_INVALID_OFFSETS,
        SEGY_TRACE_SIZE_MISMATCH,
        SEGY_INVALID_ARGS,
        SEGY_MMAP_ERROR,
        SEGY_MMAP_INVALID,
        SEGY_READONLY,
        SEGY_NOTFOUND,
    }
}
