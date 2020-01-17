using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerUIMinimapIcons
{
    //List of icons to update I suppose?
    [SerializeField] // for visualizations
    private Dictionary<MINIMAP_ICONS, MinimapIconData> iconData = new Dictionary<MINIMAP_ICONS, MinimapIconData>();

    [SerializeField]
    private MinimapIconData[] minimapIconData;

    [SerializeField]
    private List<MinimapIcons> iconList;

    //Make a query bounds for the minimap to only update certain things
    public enum MINIMAP_ICONS
    {
        DEFAULT,
        LIGHTMECH,
        LIGHTMECH2,
        HEAVYMECH2,
        OBJECTIVES,
        TOTAL_MINIMAPICONS
    }

    [System.Serializable]
    public struct MinimapIconData
    {
        public string iconName;
        public MINIMAP_ICONS type;
        public Sprite iconSprite;
        public float iconSize;

        public MinimapIconData(
            string _iconName = "DEFAULT",
            MINIMAP_ICONS _type = MINIMAP_ICONS.DEFAULT,
            Sprite _iconSprite = default(Sprite),
            float _iconSize = 1)
        {
            iconName = _iconName;
            iconSprite = _iconSprite;
            iconSize = _iconSize;
            type = _type;
        }
    }

    // Start is called before the first frame update
    public void Awake()
    {
        foreach(MinimapIconData data in minimapIconData)
        {
            iconData.Add(data.type, data);
            //Debug.Log("Added " + data.type.ToString());
        }

    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    MinimapIconData ReturnData(MINIMAP_ICONS iconType)
    {
        if (iconData.ContainsKey(iconType))
            return iconData[iconType];

        return new MinimapIconData();
    }

    public void UpdateSprite(MinimapIcons thatIcon)
    {
        //Add that icon 
        iconList.Add(thatIcon);
        //Get that sprite renderer and query
        SpriteRenderer thatRenderer = thatIcon.GetComponent<SpriteRenderer>();
        //Get that type
        MinimapIconData data = ReturnData(thatIcon.iconType);
        thatIcon.gameObject.transform.localScale = new Vector3(data.iconSize, data.iconSize, data.iconSize);
        thatRenderer.sprite = data.iconSprite;

    }

    public IEnumerator CheckNull()
    {
        while(true)
        {
            yield return new WaitForSeconds(5);
            for(int i =0; i < iconList.Count; ++i)
            {
                if (iconList[i] == null)
                    iconList.Remove(iconList[i]);
            }
        }
    }
}
