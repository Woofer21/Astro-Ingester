using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroIngesterCore
{
    public class FileTools
    {
        private DriveInfo[] drives = DriveInfo.GetDrives();
        private DriveInfo selectedDrive;

        public DriveInfo[] GetDrives()
        {
            return drives;
        }

        public bool SelectDrive(int driveNum, out DriveInfo selected)
        {
            if (driveNum - 1 > drives.Length - 1 || driveNum < 1)
            {
                selected = null;
                return false;
            }

            selectedDrive = drives[driveNum - 1];
            selected = selectedDrive;
            return true;
        }
    }
}
