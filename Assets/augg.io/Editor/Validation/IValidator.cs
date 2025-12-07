using System.Collections.Generic;

#if UNITY_EDITOR

namespace Auggio
{
    interface IValidator<T>
    {
         Dictionary<ErrorCode, string> Validate(T value);
    }
}

#endif