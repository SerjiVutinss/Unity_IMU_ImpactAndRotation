using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._scripts.Data
{
    [Serializable]
    public class LinearAccelModel
    {
        // DO NOT use Getters and Setters here - will not work!
        public string Axis;
        public float AvgInput;
        public float AvgDelta;

        public void Print()
        {
            Debug.Log("Axis: " + this.Axis);
            Debug.Log("AvgIn: " + this.AvgInput);
            Debug.Log("AvgD: " + this.AvgDelta);
        }
    }
}
