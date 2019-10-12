using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSLManagerWPF
{

    public class Distro
    {
        public bool succeed;
        public String basePath;
        public String distroName;
        public String kernelCommandLine;
        public IList<string> defaultEnvironmentVariables;
        public bool defaultUid;
        public bool enableInterop;
        public bool enableDriveMounting;
        public bool appendNtPath;
        public bool hasDefaultFlag;
        public int distroFlags;
        public String distroId;
        public bool isDefaultDistro;
        public String distroStatus;
        public int wslVersion;


        Distro()
        {

        }


    }
    public class DistroManager
    {
        public bool succeed;
        public IList<Distro> distros;
    }


    public class DistroManagerStatus
    {
        public static DistroManager manager;
    }

}
