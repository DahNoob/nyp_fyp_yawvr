using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class holds the data for what the reticle color should change to upon highlighting a certain tag or layer.
/// </summary>
[System.Serializable]
public class GUIReticleColorConfig
{
    [SerializeField]
    [Tooltip("Dictionary of colors such that each layer highlight has its flexibility of colors")]
    private Dictionary<LayerMask, Color> m_colorLayerDictionary;

    [SerializeField]
    [Tooltip("Dictionary of colors such that each tag highlight has its flexibility of colors")]
    private Dictionary<string, Color> m_colorTagDictionary;

    [SerializeField]
    [Tooltip("Array of colors to be affected for layers")]
    private GUIReticleLayerColor[] m_reticleLayerColors;

    [SerializeField]
    [Tooltip("Array of colors to be affected for tags")]
    private GUIReticleTagColor[] m_reticleTagColors;

    /// <summary>
    /// Constructor.
    /// </summary>
    public GUIReticleColorConfig()
    {
        //Initialise dictionaries
        m_colorLayerDictionary = new Dictionary<LayerMask, Color>();
        m_colorTagDictionary = new Dictionary<string, Color>();
    }

    /// <summary>
    /// Assigns colors assigned from inspector into a Dictionary.
    /// </summary>
    public void SetupLayerColors()
    {
        if (m_reticleLayerColors != null)
        {
            //Setup colors
            foreach (GUIReticleLayerColor colors in m_reticleLayerColors)
            {
                //Add to dictionary
                if (!m_colorLayerDictionary.ContainsKey(colors.layerMask))
                {
                    m_colorLayerDictionary.Add(colors.layerMask, colors.layerColor);
                    //Debug.Log("Added " + colors.layerMask.ToString() + " to dictionary array with color " + colors.layerColor.ToString());
                }
            }
        }

        if (m_reticleTagColors != null)
        {
            //Setup colors
            foreach (GUIReticleTagColor colors in m_reticleTagColors)
            {
                //Add to dictionary
                if (!m_colorTagDictionary.ContainsKey(colors.tag))
                {
                    m_colorTagDictionary.Add(colors.tag, colors.tagColor);
                    //Debug.Log("Added " + colors.tag.ToString() + " to tag array with color " + colors.tagColor.ToString());
                }
            }
        }
    }

    /// <summary>
    /// Checks if the color tag dictionary contains this tag.
    /// </summary>
    /// <param name="tag">Tag to be compared.</param>
    /// <returns>True if dictionary contains, false if not.</returns>
    public bool ContainsTag(string tag)
    {
        return m_colorTagDictionary.ContainsKey(tag);
    }

    /// <summary>
    /// Checks if the color layer dictionary contains this layer.
    /// </summary>
    /// <param name="layerMask">LayerMask for comparison.</param>
    /// <returns>True if dictionary contains, false if not.</returns>
    public bool ContainsLayer(LayerMask layerMask)
    {
        return m_colorLayerDictionary.ContainsKey(layerMask);
    }

    //Should assume that contains tag has been called already.
    /// <summary>
    /// Gets the associated color in the dictionary.
    /// </summary>
    /// <param name="tag">Accessor for the dictionary.</param>
    /// <returns>A color.</returns>
    public Color QueryTagColor(string tag)
    {
        return m_colorTagDictionary[tag];
    }

    /// <summary>
    /// Gets the associated color in the dictionary.
    /// </summary>
    /// <param name="layerMask">Accessor for the dictionary</param>
    /// <returns>A color.</returns>
    public Color QueryLayerColor(LayerMask layerMask)
    {
        return m_colorLayerDictionary[layerMask];
    }


}
