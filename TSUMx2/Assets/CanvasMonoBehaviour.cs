using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMonoBehaviour : MonoBehaviour {
    protected int maxScalerHeight = 1920;
        
    public void AdjustCanvasScale(CanvasScaler _scaler) {
        if (Screen.height > maxScalerHeight) {
            _scaler.scaleFactor = 1;
        }
        else {
            _scaler.scaleFactor = (float)Screen.height / maxScalerHeight;
        }
    }
}

