using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefab : MonoBehaviour
{
    public PlayerObject playerObject;

    public Image border;
    public enum BorderColor
    {
        Active,
        Answered,
        Correct,
        Incorrect
    };

    public Color[] borderCols;

    public TextMeshProUGUI playerNameMesh;
    public TextMeshProUGUI playerScoreMesh;

    public RawImage profilePic;

    public void Init(PlayerObject pl)
    {
        pl.prefab = this;
        playerObject = pl;
        playerNameMesh.text = pl.playerName;
        playerScoreMesh.text = "0";
        profilePic.texture = pl.profileImage;
        SetBorderColor(BorderColor.Active);
    }

    public void SetBorderColor(BorderColor bc)
    {
        border.color = borderCols[(int)bc];
    }
}
