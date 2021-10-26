using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; //like sql for dot net codes
using UnityEngine.UI;

namespace TeddyToolKit.Settings
{
    /// <summary>
    /// class to set frame rate and screen resolution, need to be used with dropdowns to display and apply
    /// a UI prefab is included
    /// </summary>
    public class FrameRate : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("drag the Dropdown object for framerate")]
        [SerializeField] private Dropdown ddFramerate;
        [Tooltip("drag the Dropdown object for screen resolution")]
        [SerializeField] private Dropdown ddResolution;

        private Resolution current;
        /// <summary>
        /// set a limit on framerate. why?
        /// adjust the fps to match the monitor's refresh rate
        /// sometimes to prevent screen tearing as vsync can creates lag.
        /// reduce heat on hardware, power consumption, and fan noise
        /// mobile has less framerate for battery, maybe 30. pc probably has more
        /// if you want to use targetframerate, set vync to 0
        /// </summary>
        /// <param name="target">if set to -1, this is the default framerate by the system</param>
        public void LimitFramerate(int index)
        {
            //this line is basically how to limit the framerate, set to 60 or 30 , etc
            var target = Convert.ToInt32(ddFramerate.options[index].text);
            Application.targetFrameRate = target;
            Debug.Log($"changed framerate to {target}");
        }

        /// <summary>
        /// sometimes people want to disable vync because they stutter?
        /// or we just want to get the latest frame no matter the quality
        /// if you want to use targetframerate, set vync to 0
        /// https://docs.unity3d.com/ScriptReference/Application-targetFrameRate.html
        /// higher number is higher quality
        /// </summary>
        /// <param name="value0to4">how many frames to buffer to sync? 0 = don't sync, max value 4</param>
        void VSyncCount(int value0to4 = 0)
        {
            QualitySettings.vSyncCount = value0to4; //range 0-4, 0 = don't sync
        }

        /// <summary>
        /// get an array of Resolutions available for this screen.
        /// use to populate resolutions dropdown list
        /// </summary>
        /// <param name="refreshRate">the refresh rate default 60Hz</param>
        /// <returns>filteredRes</returns>
        public IEnumerable<string> GetResolutionsAsString(int refreshRate = 60)
        {
            var filteredRes = GetResolutions(refreshRate);
            var sRes = new List<string>(filteredRes.Count());
            sRes.AddRange(filteredRes.Select(r => r.ToString()));
            return sRes;
        }

        /// <summary>
        /// get an array of Resolutions available for this screen.
        /// use to populate resolutions dropdown list
        /// </summary>
        /// <param name="refreshRate">the refresh rate default 60Hz</param>
        /// <returns>filteredRes</returns>
        public IEnumerable<Resolution> GetResolutions(int refreshRate = 60)
        {
            //this is using LINQ
            //var example = from s in allowedRes where s.refreshRate == 30 select s.height;
            //allowedRes.Where<>
            Resolution[] allowedRes = Screen.resolutions;
            current = Screen.currentResolution;
        
            var filteredRes = from r in allowedRes where r.refreshRate == current.refreshRate select r; 
            return filteredRes;
        }

        /// <summary>
        /// get the Resolution object from a string passed from dropdown selection
        /// to use for setting resolution
        /// </summary>
        /// <param name="sres"></param>
        /// <returns>Resolution</returns>
        public Resolution GetResolutionFromString(string sres)
        {
            var res = GetResolutions();
            Debug.Log($"sres = {sres}");
            var ff = from r in res where r.ToString() == sres select r;
            return ff.First();
        }
    
        /// <summary>
        /// set the screen resolution from a the dropdown index
        /// the dropdown content is the same as getresolutions() so we'll just use that
        /// </summary>
        /// <param name="sres"></param>
        public void ChangeResolution(int ires)
        {
            ChangeResolution(GetResolutions().ToList()[ires]);
        }
    
        /// <summary>
        /// set the screen resolution, on full screen
        /// </summary>
        /// <param name="res">Resolution</param>
        private static void ChangeResolution(Resolution res)
        {
            Debug.Log($"changing resolution to {res.height}, {res.width}, {res.refreshRate}");
            Screen.SetResolution(res.width, res.height, true, res.refreshRate);
        }

        /// <summary>
        /// populates the resolutions dropdown with the selected refresh rate
        /// </summary>
        /// <param name="refreshRate">default 60</param>
        public void populateResolutions(int refreshRate = 60)
        {
            ddResolution.ClearOptions();
            var res = GetResolutionsAsString(refreshRate);
            ddResolution.AddOptions(res.ToList());
        }

        /// <summary>
        /// call this method from the ui button inspector onClick
        /// </summary>
        public void applyDropdowns()
        {
            ChangeResolution(ddResolution.value);
            LimitFramerate(ddFramerate.value);
        }
        
        // Start is called before the first frame update
        void Start()
        {
            populateResolutions();
        }
    }
}
