using System.Collections.Generic;
using UnityEngine;

namespace NBC.ActionEditor
{
    public interface IDirector : IData
    {
        float Length { get; }
        // void Validate();

        void SaveToAssets();
    }
}