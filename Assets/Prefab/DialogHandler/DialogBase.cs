using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    public class DialogBase : MonoBehaviour
    {
        // these lists should be the same size (e.g. speakers.Count == dialogs.Count == presentSpeakers.Count)
        public List<int> speakers;
        public List<string> dialogs;
        public List<string> presentSpeakers;
    }
}
