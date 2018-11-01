using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet
{
    enum FormatEnum: int
    {
       SEGY_IBM_FLOAT_4_BYTE = 1,
       SEGY_SIGNED_INTEGER_4_BYTE = 2,
       SEGY_SIGNED_SHORT_2_BYTE = 3,
       SEGY_FIXED_POINT_WITH_GAIN_4_BYTE = 4, 
       SEGY_IEEE_FLOAT_4_BYTE = 5,
       SEGY_NOT_IN_USE_1 = 6,
       SEGY_NOT_IN_USE_2 = 7,
       SEGY_SIGNED_CHAR_1_BYTE = 8
    }
}
