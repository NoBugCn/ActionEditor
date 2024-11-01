using System.Collections.Generic;
using UnityEngine;

namespace NBC.ActionEditor
{
    public interface IDirector : IData
    {
        float Length { get; }

        public float ViewTimeMin { get; set; }
        public float ViewTimeMax { get; set; }

        public float ViewTime { get; }

        public float RangeMin { get; set; }
        public float RangeMax { get; set; }

        void DeleteGroup(Group group);

        void UpdateMaxTime();
        void Validate();
    }
}