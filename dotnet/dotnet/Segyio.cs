using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using dotnet.enums;

namespace dotnet
{

    public class Segyio 
    {

        public static IntPtr _segyPointer = IntPtr.Zero;
        public static byte[] _binHeader;
        public static long _trace0;
        public static int _samplecount;
        public static int _format;
        public static int _trace_bsize;
        public static int _elemsize;
        public static int _tracecount;
        public static bool disposed = false; 

        public class OpenFileException : Exception { }
        public class CouldNotSetBinHeaderException : Exception { }

        public Segyio(IntPtr segyPointer, byte[] binheader, long trace0, int samplecount, int format, int bSize, int elemsize, int tracecount)  
        {
            _segyPointer = segyPointer;
            _binHeader = binheader;
            _trace0 = trace0;
            _samplecount = samplecount;
            _format = format;
            _trace_bsize = bSize;
            _elemsize = elemsize;
            _tracecount = tracecount; 
        }

        ~Segyio()
        {
            if (_segyPointer != IntPtr.Zero)
            {
                Segyiomethod.segy_close(_segyPointer);
            }
 
        }

        public static Segyio Open(string filepath, string mode)
        {
            if (File.Exists(filepath))
            {
                try {

                    IntPtr segyPointer = IntPtr.Zero;

                    if ((mode.Equals("r") || mode.Equals("r+")))
                    {
                        segyPointer = Segyiomethod.segy_open(filepath, mode);
                    }

                    if (segyPointer == IntPtr.Zero)
                    {
                        throw new CustomException<OpenFileException>("OpenFileException: Could not open file");
                    }

                    byte[] binheader = new byte[400];
                    int binResponse = Segyiomethod.segy_binheader(segyPointer, binheader);

                    if (!binResponse.Equals(0))
                    {
                        throw new CustomException<CouldNotSetBinHeaderException>("CouldNotSetBinHeaderException: Could not set bin header");
                    }

                    long trace0 = Segyiomethod.segy_trace0(binheader);
                    int samplecount = Segyiomethod.segy_samples(binheader);
                    int format = Segyiomethod.segy_format(binheader);
                    int bSize = Segyiomethod.segy_trsize(format, samplecount);
                    int elemsize = 4;
                    int tracecount = 0;
                    
                    if(bSize < 0)
                    {
                        bSize = Segyiomethod.segy_trace_bsize(samplecount);
                    }

                    switch (format)
                    {
                        case (int)FormatEnum.SEGY_IBM_FLOAT_4_BYTE:                           break;
                        case (int)FormatEnum.SEGY_SIGNED_INTEGER_4_BYTE:        elemsize = 4; break;
                        case (int)FormatEnum.SEGY_SIGNED_SHORT_2_BYTE:          elemsize = 2; break;
                        case (int)FormatEnum.SEGY_FIXED_POINT_WITH_GAIN_4_BYTE: elemsize = 4; break;
                        case (int)FormatEnum.SEGY_IEEE_FLOAT_4_BYTE:            elemsize = 4; break;
                        case (int)FormatEnum.SEGY_SIGNED_CHAR_1_BYTE:           elemsize = 1; break;

                        case (int)FormatEnum.SEGY_NOT_IN_USE_1:
                        case (int)FormatEnum.SEGY_NOT_IN_USE_2:
                        default:
                        break;
                    }

                    int error = Segyiomethod.segy_traces(segyPointer, ref tracecount, trace0, bSize);

                    switch (error)
                    {
                        case (int)SegyErrorEnum.SEGY_OK: break;
                        case (int)SegyErrorEnum.SEGY_FSEEK_ERROR:         throw new IOException();                            
                        case (int)SegyErrorEnum.SEGY_INVALID_ARGS:        throw new SystemException("unable to count traces, no data traces past headers");
                        case (int)SegyErrorEnum.SEGY_TRACE_SIZE_MISMATCH: throw new SystemException("trace count inconsistent with file size, trace lengths possibly of non-uniform");

                        default:
                             Error(error);
                             break; 
                    }

                    return new Segyio(segyPointer, binheader, trace0, samplecount, format, bSize, elemsize, tracecount);

                } catch(Exception ex)
                { 
                    //should one call close?
                    throw ex; 
                }
                
           }
            throw new FileNotFoundException("File could not be found");
        }

        public int Getfield(int position)
        {

            int returnField = -1;
            int returnValue = Segyiomethod.segy_get_bfield(_binHeader, position, ref returnField);

            switch (returnValue)
            {
                case (int)SegyErrorEnum.SEGY_OK:
                    return returnField;
                case (int)SegyErrorEnum.SEGY_INVALID_FIELD:
                    throw new ArgumentException("Invalid field " + position);
                default:
                    Error(returnValue);
                    break;
            }

            return -1;
        }

        public int SetBfield(int position, int value)
        {
           
            int bfield = Segyiomethod.segy_set_bfield(_binHeader, position, value);

            switch (bfield)
            {
                case (int)SegyErrorEnum.SEGY_OK:
                    return bfield;
                case (int)SegyErrorEnum.SEGY_INVALID_FIELD:
                    throw new ArgumentException("Invalid field " + position);
                default:
                    Error(bfield);
                    break;
            }

            return -1;

        }

        /*Retrives the traces for a specific tracenumber. 
        * We read the tracevalues for a specific trace to a buffer. Because different
        * computer architectures represent float values differently, we call segy_to_native 
        * to get the correct byte representation in the buffer. We can then convert the byte values
        * to float values. 
        */

        public float[] Readtrace(int tracenumber)
        {

            byte[] buffer = new byte[_trace_bsize];
            int tonativeResponse = -1;   
            int traceResponse = Segyiomethod.segy_readtrace(_segyPointer, tracenumber, buffer, _trace0, _trace_bsize);

            switch (traceResponse)
            {
                case (int)SegyErrorEnum.SEGY_OK:
                    tonativeResponse = Segyiomethod.segy_to_native(_format, buffer.Length, buffer);
                    break;
                case (int)SegyErrorEnum.SEGY_FREAD_ERROR:
                    throw new ArgumentException("Invalid arguments");
                default:
                    Error(traceResponse);
                    break;
            }


            if (tonativeResponse.Equals((int)SegyErrorEnum.SEGY_INVALID_ARGS))
            {
                throw new ArgumentException("Invalid arguments");
            }

            if (traceResponse == 0 && tonativeResponse == 0)
            {
                float[] traces = new float[buffer.Length/4];
                int number = 0;

                for(int i = 0; i<buffer.Length && number < buffer.Length; i++)
                {
                    float value = BitConverter.ToSingle(buffer, number);
                    traces[i] = value;
                    number += 4;
                }
                return traces;
            }
            return null;           
        }

        /* Retrieves the traceheader for a specific trace. 
         * First we retrieve the traceheader for a specific trace and write it to buffer. 
         * Because the retrieved values in the buffer are of different byte size, we iterate through
         * each of the instances in the enum tracefield to get the correct value. Finally, we create
         * a dictionary where we map together the enum instance name and collected value. 
         * */

        public Dictionary<String, int> Gettraceheader(int position)
        {
            byte[] buffer = new byte[240];
            int tracerResponse = -1;
            string[] enumNames = Enum.GetNames(typeof(Tracefield));
            int[] enumValues = new int[enumNames.Length];
            var traceDictionary = new Dictionary<string, int>();


            tracerResponse = Segyiomethod.segy_traceheader(_segyPointer, position, buffer, _trace0, _trace_bsize);

            switch (tracerResponse)
            {
                case (int)SegyErrorEnum.SEGY_FREAD_ERROR:
                     throw new ArgumentException("Invalid arguments");
                case (int)SegyErrorEnum.SEGY_FSEEK_ERROR:
                    throw new IOException();                 
            }

            if (tracerResponse.Equals(0))
            {
                int tracevalue = -1;
                for (int i = 0; i < enumNames.Length; i++)
                {
                    tracevalue = (Gettracefield(buffer, (int)Enum.Parse(typeof(Tracefield), enumNames[i])));
                    if (tracevalue != -1)
                    {
                        enumValues[i] = tracevalue;
                    }
                }

                for (int index = 0; index < enumValues.Length; index++)
                {
                    traceDictionary.Add(enumNames[index], enumValues[index]);
                }
            }

            return traceDictionary;
        }

        public int Gettracefield(byte[] traceheader, int position)
        {
            int tracefield = -1;
            int response = -1;
         
            response = Segyiomethod.segy_get_field(traceheader, position, ref tracefield);

            switch (response)
            {
                case (int)SegyErrorEnum.SEGY_OK:
                        return tracefield;
                case (int)SegyErrorEnum.SEGY_INVALID_FIELD:
                    throw new ArgumentException("Invalid field " + position);
                default:
                    Error(response);
                    break;
            }

            return -1;
        }

       public int Getoffsets()
        {
            int offsets = -1;   
            int offsetresponse = Segyiomethod.segy_offsets(_segyPointer, 189, 193, Getfield(3213), ref offsets, _trace0, _trace_bsize);

            switch (offsetresponse)
            {
                case (int)SegyErrorEnum.SEGY_OK:
                    return offsets;
                case (int)SegyErrorEnum.SEGY_INVALID_FIELD:
                    throw new ArgumentException();
                case (int)SegyErrorEnum.SEGY_FREAD_ERROR:
                    throw new IOException("I/O operation failed, likely corrupted file");
                default:
                    Error(offsetresponse);
                    break;
            }
            return offsets;
        }

        public int[] Getoffsetsindices()
        {
            int numOffsets = Getoffsets();
            int[] buffer = new int[numOffsets];
            int offsetindices = -1;

            if (!numOffsets.Equals(-1))
            {           
                offsetindices =  Segyiomethod.segy_offset_indices(_segyPointer, 37, numOffsets, buffer, _trace0, _trace_bsize);            
            }

            switch (offsetindices)
            {
                case (int)SegyErrorEnum.SEGY_OK:
                    return buffer;
                case (int)SegyErrorEnum.SEGY_INVALID_FIELD:
                    throw new ArgumentException();
                default:
                    Error(offsetindices);
                    break;                       
            }
            return buffer;
        }


        public static void Error(int err)
        {
            switch (err)
            {
                case (int)SegyErrorEnum.SEGY_FSEEK_ERROR:
                    throw new IOException();
                case (int)SegyErrorEnum.SEGY_FWRITE_ERROR:
                case (int)SegyErrorEnum.SEGY_FREAD_ERROR:
                    throw new IOException("I/O operation failed, likely corrupted file");
                case (int)SegyErrorEnum.SEGY_READONLY:
                    throw new IOException("file not open for writing. open with 'r+'");
                default:
                    throw new SystemException();
            }
       

        }
          
     
    }

}

