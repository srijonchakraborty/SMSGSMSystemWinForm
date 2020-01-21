/*
|-------------------------------------------------------------------------------|
|	This code was written by Srijon Chakraborty								    |
|	Main source code link on https://github.com/srijonchakro			        |
|	All my source codes are available on http://srijon.softallybd.com           |
|	C# GSM Serial Read Write	                                            |
|	LinkedIn https://bd.linkedin.com/in/srijon-chakraborty-0ab7aba7				|
|-------------------------------------------------------------------------------|
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSSystemGSM
{
    public class BOComPortGSMSMS
    {
        #region cTor
        public BOComPortGSMSMS()
        {

        }
        #endregion

        #region Properties
        public string ComPortName { get; set; }
        public string ComPortDescription { get; set; }
        public string ComPortANDDescription { get { return ComPortName+":"+ComPortDescription; } }
        #endregion
    }
}
