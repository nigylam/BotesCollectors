using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _counter;

    public void Change(int value)
    {
        _counter.text = value.ToString();
    }
}
