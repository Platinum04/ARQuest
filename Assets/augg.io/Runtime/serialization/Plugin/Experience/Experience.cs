using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Auggio.Utils.Serialization.Plugin.Experience
{
    [Serializable]
    public class Experience
    { 
        [SerializeField] private string id;
        [SerializeField] private string organizationId;
        [SerializeField] private string name;
        [SerializeField] private List<Location> assignedLocations;
        [SerializeField] private List<AuggioObject> objects;

        public string ID
        {
            get => id;
            set => id = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public List<Location> AssignedLocations
        {
            get => assignedLocations;
            set => assignedLocations = value;
        }

        public string OrganizationId
        {
            get => organizationId;
            set => organizationId = value;
        }

        public List<AuggioObject> Objects
        {
            get => objects;
            set => objects = value;
        }

        public Location FindLocationByAnchorId(string anchorId)
        {
            foreach (Location location in assignedLocations)
            {
                if (location.SingleAnchorList.Any(anchor => anchor.AuggioId.Equals(anchorId)))
                {
                    return location;
                }
            }

            throw new ArgumentException("Anchor id is not present in the experience.");
        }

        public Location FindLocationById(string locationId)
        {
            foreach (Location location in assignedLocations)
            {
                if (location.ID.Equals(locationId))
                {
                    return location;
                }
            }
            throw new ArgumentException("Location with id is not present in the experience.");
        }

        public AuggioObject FindObjectByObjectId(string objectId)
        {
            foreach (AuggioObject auggioObject in objects)
            {
                if (auggioObject.AuggioId.Equals(objectId))
                {
                    return auggioObject;
                }
            }
            throw new ArgumentException("Cannot find auggio object by id " + objectId);
        }
    }
}
