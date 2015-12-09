using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using System.IO;
using System.Xml;

public static class FontMaker
{
    [MenuItem("Assets/Generate Bitmap Font")]
    public static void GenerateFont()
    {
        TextAsset ConfigFile = (TextAsset)Selection.activeObject;
        string RootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(ConfigFile));
        string ExportPath = RootPath + "/" + Path.GetFileNameWithoutExtension(ConfigFile.name);
        Work(ConfigFile, RootPath, ExportPath);
    }


    private static void Work(TextAsset ConfigFile, string RootPath, string ExportPath)
    {
        if (!ConfigFile)
        {
            throw new UnityException(ConfigFile.name + "is not a valid font-xml file");
        }

        XmlDocument Xml = new XmlDocument();
        Xml.LoadXml(ConfigFile.text);

        XmlNode Info = Xml.GetElementsByTagName("info")[0];
        XmlNode Common = Xml.GetElementsByTagName("common")[0];
        XmlNode Page = Xml.GetElementsByTagName("page")[0];
        XmlNodeList Chars = Xml.GetElementsByTagName("chars")[0].ChildNodes;

        string TexturePath = RootPath + "/" + Page.Attributes.GetNamedItem("file").InnerText;

        Texture2D Texture = AssetDatabase.LoadAssetAtPath(TexturePath, typeof(Texture2D)) as Texture2D;
        if (!Texture)
        {
            throw new UnityException("Texture2d asset doesn't exist for " + ConfigFile.name);
        }

        float TextureWidth = Texture.width;
        float TextureHeight = Texture.height;

        CharacterInfo[] CharInfos = new CharacterInfo[Chars.Count];

        for (int i = 0; i < Chars.Count; i++)
        {
            XmlNode CharNode = Chars[i];
            CharacterInfo CharInfo = new CharacterInfo();

            CharInfo.index = ToInt(CharNode, "id");
            CharInfo.width = ToInt(CharNode, "xadvance");
            CharInfo.flipped = false;

            Rect CharRect = new Rect();
            CharRect.x = ((float)ToInt(CharNode, "x")) / TextureWidth;
            CharRect.y = ((float)ToInt(CharNode, "y")) / TextureHeight;
            CharRect.width = ((float)ToInt(CharNode, "width")) / TextureWidth;
            CharRect.height = ((float)ToInt(CharNode, "height")) / TextureHeight;
            CharRect.y = 1f - CharRect.y - CharRect.height;
            CharInfo.uv = CharRect;

            CharRect = new Rect();
            CharRect.x = (float)ToInt(CharNode, "xoffset");
            CharRect.y = (float)ToInt(CharNode, "yoffset");
            CharRect.width = (float)ToInt(CharNode, "width");
            CharRect.height = (float)ToInt(CharNode, "height");
            CharRect.y = -CharRect.y;
            CharRect.height = -CharRect.height;
            CharInfo.vert = CharRect;

            CharInfos[i] = CharInfo;
        }

        // Create material
        Shader CharShader = Shader.Find("UI/Default");
        Material CharMaterial = new Material(CharShader);
        CharMaterial.mainTexture = Texture;
        AssetDatabase.CreateAsset(CharMaterial, ExportPath + ".mat");

        // Create font
        Font NewFont = new Font();
        NewFont.material = CharMaterial;
        NewFont.name = Info.Attributes.GetNamedItem("face").InnerText;
        NewFont.characterInfo = CharInfos;

        SerializedObject RawFont = new SerializedObject(NewFont);
        RawFont.FindProperty("m_FontSize").floatValue = float.Parse(Common.Attributes.GetNamedItem("base").InnerText);
        RawFont.FindProperty("m_LineSpacing").floatValue = float.Parse(Common.Attributes.GetNamedItem("lineHeight").InnerText);
        RawFont.ApplyModifiedProperties();

        AssetDatabase.CreateAsset(NewFont, ExportPath + ".fontsettings");
    }

    private static int ToInt(XmlNode node, string name)
    {
        return Convert.ToInt32(node.Attributes.GetNamedItem(name).InnerText);
    }
}
