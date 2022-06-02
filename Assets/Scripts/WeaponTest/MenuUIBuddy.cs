using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 0649
public class MenuUIBuddy : MonoBehaviour
{
    [SerializeField] private RectTransform curVisiblePanel;
    [SerializeField] private RectTransform[] panels;
    [SerializeField] private float panelSwitchSpeedFactor = 8f;
    [SerializeField] private float panelOffset;

    public void ChangePanel(int panelNum)
    {
        if(curVisiblePanel == panels[panelNum])
        {
            LogUtils.DebugLogWarning("Same panel?");
            return;
        }
        else
        {
            Vector2 newPos = Vector2.zero;

            if(curVisiblePanel.localPosition.x > panels[panelNum].localPosition.x)
            {
                newPos = new Vector2(panelOffset, 0f);
            }
            else
            {
                newPos = new Vector2(-panelOffset, 0f);
            }

            StopAllCoroutines();
            StartCoroutine(MovePanelSmooth(curVisiblePanel, newPos, panelSwitchSpeedFactor));
            curVisiblePanel = panels[panelNum];
            //LogUtils.DebugLog("Current pan: " + curVisiblePanel);
            //LogUtils.DebugLog(Mathf.Abs(curVisiblePanel.localPosition.x));
            //LogUtils.DebugLog(Mathf.Abs(curVisiblePanel.localPosition.x) / panelOffset);
            //LogUtils.DebugLog(Mathf.Abs(curVisiblePanel.localPosition.x) / panelOffset * panelSwitchSpeedFactor);
            StartCoroutine(MovePanelSmooth(curVisiblePanel, Vector2.zero, Mathf.Abs(curVisiblePanel.localPosition.x) / panelOffset * panelSwitchSpeedFactor));
        }
    }

    private IEnumerator MovePanel(RectTransform panel, Vector2 pos, float spd)
    {
        //LogUtils.DebugLog("New pos for " + panel.name + " : " + pos);
        while(Mathf.Abs(panel.localPosition.x - pos.x) >= 0.1f || Mathf.Abs(panel.localPosition.y - pos.y) >= 0.1f)
        {
            panel.localPosition = Vector2.MoveTowards(panel.localPosition, pos, spd);
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }

    private IEnumerator MovePanelSmooth(RectTransform panel, Vector2 pos, float spd)
    {
        Vector2 veloc = Vector2.zero;
        //LogUtils.DebugLog("New pos for " + panel.name + " : " + pos);
        while (Mathf.Abs(panel.localPosition.x - pos.x) >= 0.1f || Mathf.Abs(panel.localPosition.y - pos.y) >= 0.1f)
        {
            panel.localPosition = Vector2.SmoothDamp(panel.localPosition, pos, ref veloc, spd);
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}
#pragma warning restore 0649