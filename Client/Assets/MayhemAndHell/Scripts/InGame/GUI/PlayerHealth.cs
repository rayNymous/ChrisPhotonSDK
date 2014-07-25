using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{

    public UILabel Label;
    public UIProgressBar Health;

    void Start()
    {
        UpdateHealth(10, 100);
    }

    public void UpdateHealth(int current, int max)
    {
        Label.text = current + "/" + max;
        Health.value = current/(float)max;
    }
}
