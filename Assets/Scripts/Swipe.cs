using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Swipe : MonoBehaviour
{
    #region PRIVATE_VARS
    [SerializeField]
    private ScrollRect scrollView;
    [SerializeField]
    private Scrollbar scrollbar;
    private float scroll_pos = 0;
    float[] pos;
    private float distance;
    [SerializeField]
    private List<RectTransform> allButtons;
    #endregion

    #region PRIVATE_FUNCTIONS

    private void Start()
    {

        pos = new float[allButtons.Count];

        distance = 1f / (allButtons.Count - 1f);

        for (int i = 0; i < allButtons.Count; i++)
        {
            pos[i] = distance * i;
        }

        scrollView.onValueChanged.AddListener(OnValueChange);

    }

    private void OnValueChange(Vector2 value)
    {
        scroll_pos = scrollbar.value;

        for (int i = 0; i < allButtons.Count; i++)
        {
            if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
            {
                Debug.Log("scroll Value After" + value.x);
                allButtons[i].localScale = Vector2.Lerp(allButtons[i].localScale, new Vector2(1f, 1f), 0.1f);

                for (int j = 0; j < allButtons.Count; j++)
                {
                    if (j != i)
                    {

                        allButtons[j].localScale = Vector2.Lerp(allButtons[j].localScale, new Vector2(0.8f, 0.8f), 0.1f);
                    }
                }
            }
        }

     // list


    }
    #endregion
}

#region PUBLIC_FUNCTIONS

#endregion

#region CO-ROUTINES

#endregion

#region EVENT_HANDLERS
#endregion

#region UI_CALLBACKS
#endregion



