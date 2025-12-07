using System.Collections.Generic;
using Auggio.Plugin.SDK.Model.Ids;
using Auggio.Utils.Serialization.Plugin.Experience;

namespace Auggio.Plugin.SDK.Utils {
    /**
     * Set of methods used to make parsing of tracking data from AuggioTrackingManager easier. Use this if you need basic parsing function for code driven resolving.
     */
    public static class ParserHelper
    {
        /**
         * Returns experience with the given name.
         */
        public static bool GetExperienceByName(string name, Dictionary<ExperienceId, Experience> trackingData, out Experience experience) {
            foreach (KeyValuePair<ExperienceId,Experience> pair in trackingData) {
                if (pair.Value.Name.Equals(name)) {
                    experience = pair.Value;
                    return true;
                }
            }
            experience = null;
            return false;
        }
    
        /**
         * Returns experience with the given id.
         */
        public static bool GetExperienceById(ExperienceId id, Dictionary<ExperienceId, Experience> trackingData, out Experience experience) {
            return trackingData.TryGetValue(id, out experience);
        }
        
       
        
    }
}
