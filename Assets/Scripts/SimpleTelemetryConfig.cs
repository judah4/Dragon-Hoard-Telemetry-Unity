using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTelemetry.Unity.Examples
{
    [CreateAssetMenu(menuName = "Telemetry/Create Simple Config", fileName = "SimpleConfig")]
    public class SimpleTelemetryConfig : ScriptableObject
    {
        public bool WriteToApi;
        public string ApiUrl;
        public string AuthHeader;
    }
}
