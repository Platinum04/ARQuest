using System;

namespace Auggio.Utils.Serialization.Plugin
{
    [Serializable]
    public enum OrganizationRole
    {
        OWNER = 0,
        ADMIN = 1,
        DEVELOPER = 2,
        MEMBER = 3
    }
}
