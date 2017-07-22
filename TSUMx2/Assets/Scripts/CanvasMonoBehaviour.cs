using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasMonoBehaviour : MonoBehaviour {
    protected int maxScalerHeight = 1920;
    private CanvasScaler _scaler;
        
    public void AdjustCanvasScale() {
        _scaler = this.GetComponent<CanvasScaler>();

        if (Screen.height > maxScalerHeight) {
            _scaler.scaleFactor = 1;
        }
        else {
            _scaler.scaleFactor = (float)Screen.height / maxScalerHeight;
        }
    }
}

