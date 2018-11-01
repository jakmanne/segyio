using System;
using Xunit;
using dotnet;
using Xunit.Abstractions;
using System.IO;
using System.Collections.Generic;

namespace XUnitTests
{
    public class UnitTests
    {

        //These tests will only work with the segy file "small.sgy". 
        private readonly string filepath = "../../../../dotnet/test-data/small.sgy";


        public UnitTests()
        {
            Segyio._binHeader = null;
            Segyio._segyPointer = IntPtr.Zero;
        }

        [Fact(DisplayName = "openSegyfile")]
        public void OpenSegyfile()
        {
      
            Segyio pointer1;
            CustomException<Segyio.OpenFileException> ex = Assert.Throws<CustomException<Segyio.OpenFileException>>(() => pointer1 = Segyio.Open(filepath, "s"));
            Type[] genericType = ex.GetType().GetGenericArguments();
            Assert.Equal("OpenFileException", genericType[0].Name);

            Segyio pointer2 = Segyio.Open(filepath, "r");
            Assert.True(pointer2 != null);

            Segyio pointer3 = Segyio.Open(filepath, "r+");
            Assert.True(pointer3 != null);

            Segyio pointer4;
            Exception ex2 = Assert.Throws<FileNotFoundException>(() => pointer4 = Segyio.Open(null, "r"));
            Assert.Equal("File could not be found", ex2.Message);

        }

        [Fact(DisplayName ="Segyparameters")]
        public void SegyParameters()
        {
            Segyio pointer = Segyio.Open(filepath, "r");
            Assert.Equal(50, Segyio._samplecount);
            Assert.Equal(1, Segyio._format);
            Assert.Equal(3600, Segyio._trace0);
            Assert.Equal(200, Segyio._trace_bsize);
            Assert.Equal(4, Segyio._elemsize);
            Assert.Equal(25, Segyio._tracecount);

        }

        [Fact(DisplayName = "getfield")]
        public void GetField()
        {
            Segyio pointer = Segyio.Open(filepath, "r");

            Exception ex = Assert.Throws<ArgumentException>(() => pointer.Getfield(342342342));
            Assert.Equal("Invalid field " + 342342342, ex.Message);    
            Assert.Equal(25,pointer.Getfield(3213));
            
        }

        [Fact(DisplayName ="setfield")]
        public void SetField()
        {
            Segyio pointer = Segyio.Open(filepath, "r");

            Exception ex = Assert.Throws<ArgumentException>(() => pointer.SetBfield(345454,100));
            Assert.Equal("Invalid field " + 345454, ex.Message);

            int previousvalue = pointer.Getfield(3259);
            pointer.SetBfield(3259, 100);
            Assert.Equal(100, pointer.Getfield(3259));
            pointer.SetBfield(3259, previousvalue);

        }

        [Fact(DisplayName = "readtrace")]
        public void ReadTrace()
        {
            Segyio pointer = Segyio.Open(filepath, "r");

            Exception ex = Assert.Throws<ArgumentException>(() => pointer.Readtrace(2432443));
            Assert.Equal("Invalid arguments", ex.Message);

            float[] array = pointer.Readtrace(0);
            Assert.Equal(50, array.Length);

        }

        [Fact(DisplayName = "readtraceheaer")]
        public void ReadTraceHeader()
        {
            Segyio pointer = Segyio.Open(filepath, "r");

            Exception ex = Assert.Throws<ArgumentException>(() => pointer.Gettraceheader(2432443));
            Assert.Equal("Invalid arguments", ex.Message);

            Dictionary<string, int> test = pointer.Gettraceheader(1);
            Assert.Equal(91, pointer.Gettraceheader(1).Count);

        }


    }

}