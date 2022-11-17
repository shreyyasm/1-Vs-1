using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] GameObject enemyPanel;

    [SerializeField]
    private TextMeshProUGUI[] m_PartyNames;

    [SerializeField]
    private Slider[] m_PartyHealthSliders;

    ulong[] m_PartyIds;

}
