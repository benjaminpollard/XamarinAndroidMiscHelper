
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Telephony;
using Java.Lang;
using Java.Security;

namespace XamarinAndroidMiscUtilExmaple
{
    public class MiscUtilites
    {

        public static int Convert(int size, DisplayMetrics displayMetric)
        {
            return (int)TypedValue.ApplyDimension(
             ComplexUnitType.Dip,
             size,
            displayMetric
             );
        }

       
            private static string _pseudoSalt = "BP"; // Extra string to add to the hash text, though not random.
          
            private static string _deviceGuid = null;

            public string GetDeviceUniqueId()
            {
                if (_deviceGuid == null)
                {
                    var tm = (TelephonyManager)Application.Context.GetSystemService(Context.TelephonyService);
                    string tmpGuid = "";
                    if (tm != null)
                    {
                        try
                        {
                            // Use Hardware Device ID where avabilable.
                            tmpGuid = tm.DeviceId;

                            // Fall back to Sim serial number
                            if (string.IsNullOrEmpty(tmpGuid))
                                tmpGuid = tm.SimSerialNumber;
                        }
                        catch (SecurityException e)
                        {
                            // Could happen on Android 6.0 if user denies the READ_PHONE_STATE permission
                        }
                    }
                    if (string.IsNullOrEmpty(tmpGuid)) // Fall back to Android App ID. (though not as good as user can reset this in phone's settings)              
                        tmpGuid = "" + Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                    if (string.IsNullOrEmpty(tmpGuid))  // Fall back to per-install generated UID - won't persist across app installs
                        tmpGuid = Guid.NewGuid().ToString();

                    // Hash the resulting string                
                    _deviceGuid = StringToSHA256String(tmpGuid + _pseudoSalt);
                }
                return _deviceGuid;
            }





            private string StringToSHA256String(string aInput)
            {
                string ret = "";
                try
                {
                    var digest = MessageDigest.GetInstance("SHA-256");
                    var javaStr = new Java.Lang.String(aInput);
                    byte[] digested = digest.Digest(javaStr.GetBytes());
                    var hash = Java.Lang.String.Format("%064x", new Java.Math.BigInteger(1, digested));
                    ret = hash;
                }
                catch (NoSuchAlgorithmException e)
                {
                    // We have a problem... Fail back to 
                    var js = new Java.Lang.String(aInput);
                    var hashcode = new Java.Lang.String("" + js.GetHashCode()).GetBytes();
                    ret = Java.Lang.String.Format("%064x", new Java.Math.BigInteger(1, hashcode));
                }
                var code = ret.GetHashCode();
                if (0 > code)
                {
                    code = code * -1;
                }
                return code.ToString();
            }


            public string GetDeviceName()
            {
                string manufacturer = Build.Manufacturer;
                string model = Build.Model;
                if (model.StartsWith(manufacturer))
                {
                    return model;
                }

                return manufacturer + " " + model; //"Samsung GT-N8010"
            }
        }
}