using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class LabelCurrencyAlignAndPosHelper : MonoBehaviour
{
    [MenuItem("HYZ/LabelCurrencyAlignAndPos/货币栏字的对齐和位置统一处理")]
    public static void SetCurrencyAlignAndPosInDic()
    {
        int count = 1;
        //处理一个文件夹下的
        string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] {"Assets/Resources/UI/Prefabs"});
        if (ids.Length > 0)
        {
            foreach (var eachId in ids)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(eachId);
                if (prefabPath.Contains("UICityWindow"))//主城的是四个,略微短一点
                {
                    continue;
                }
                if (prefabPath.Contains("Abyss"))//主城的是四个,略微短一点
                {
                    continue;
                }

                if (prefabPath.Contains("UIFBGqsWindow"))//主城的是四个,略微短一点
                {
                    continue;
                }
                
                EditorUtility.DisplayProgressBar("替换prefab路径", prefabPath, (float) count / ids.Length);
                Object prefabObj = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
                GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefabObj) as GameObject;
                count++;
                if (prefabInstance.layer != LayerManager.UI) //只处理UI层的Prefab
                {
                    Object.DestroyImmediate(prefabInstance);
                    continue;
                }

                MoneyIcon[] currencys = prefabInstance.GetComponentsInChildren<MoneyIcon>();
                if (currencys.Length <= 0)
                {
                    Object.DestroyImmediate(prefabInstance);
                    continue;
                }

                ProcessCurrencyLabel(prefabInstance);

                //apply 替换prefab
                PrefabUtility.ReplacePrefab(prefabInstance, prefabObj, ReplacePrefabOptions.Default);
                Object.DestroyImmediate(prefabInstance);
            }


            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("HYZ/LabelCurrencyAlignAndPos/货币栏字的对齐和位置统一处理selection")]
    public static void SetCurrencyAlignAndPosInSelection()
    {
        int count = 1;
        //处理选中对象的
        Object[] objs = Selection.objects;
        foreach (var item in objs)
        {
            EditorUtility.DisplayProgressBar("替换prefab路径", item.name, (float) count / objs.Length);
            string prefabPath = AssetDatabase.GetAssetPath(item);
            Object prefabObj = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefabObj) as GameObject;
            count++;
            if (prefabInstance.layer != LayerManager.UI) //只处理UI层的Prefab
            {
                Object.DestroyImmediate(prefabInstance);
                continue;
            }

            MoneyIcon[] currencys = prefabInstance.GetComponentsInChildren<MoneyIcon>();
            if (currencys.Length <= 0)
            {
                Object.DestroyImmediate(prefabInstance);
                continue;
            }

            ProcessCurrencyLabel(prefabInstance);

            //apply 替换prefab
            PrefabUtility.ReplacePrefab(prefabInstance, prefabObj, ReplacePrefabOptions.Default);
            Object.DestroyImmediate(prefabInstance);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void SetBG(Transform target,GameObject prefabInstance)
    {
        if (target == null) return;
        //set bg
        UI2DSprite bg = target.GetComponent<UI2DSprite>();
        Sprite tempSp = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/ArtResources/allArtRes_optimize_/Common/CommonBg/Common_Bg_Top.png", typeof(Sprite));
        bg.sprite2D = tempSp;

        bg.type = UIBasicSprite.Type.Sliced;
        //x z
        //y w
        bg.border = new Vector4(1000f, 0f, 200f, 0);

        UIWidget tempWidget = bg.GetComponent<UIWidget>();
        tempWidget.pivot = UIWidget.Pivot.Top;
        tempWidget.depth = 0;
        tempWidget.width = 1280;
        tempWidget.height = 62;
        bg.ResetAnchors();

        bg.leftAnchor = new UIRect.AnchorPoint()
        {
            target = prefabInstance.transform,
            absolute = 0,
        };

        bg.rightAnchor = new UIRect.AnchorPoint()
        {
            target = prefabInstance.transform,
            relative = 1.0f,
            absolute = 0,
        };

        bg.transform.localPosition = Vector3.zero;
    }

    static void ReturnBtn(Transform target,GameObject prefabInstance,Transform bgTrans)
    {
        if (target == null) return;
        if (bgTrans == null) return;
        //set bg
        UI2DSprite bg = target.GetComponent<UI2DSprite>();
        Sprite tempSp = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/ArtResources/allArtRes_optimize_/Common/CommonButton/Common_Btn_Back.png", typeof(Sprite));
        bg.sprite2D = tempSp;

        bg.type = UIBasicSprite.Type.Simple;
        //x z
        //y w
        //bg.border = new Vector4(1000f, 200f, 0, 0);

        UIWidget tempWidget = bg.GetComponent<UIWidget>();
        tempWidget.pivot = UIWidget.Pivot.Center;
        tempWidget.depth = 1;
        tempWidget.width = 138;
        tempWidget.height = 60;

        bg.leftAnchor = new UIRect.AnchorPoint()
        {
            target = bgTrans,
            absolute = 0,
        };

        bg.rightAnchor = new UIRect.AnchorPoint()
        {
            target = bgTrans,
            absolute = 138,
        };
        bg.bottomAnchor = new UIRect.AnchorPoint()
        {
            target = bgTrans,
            relative = 0f,
            absolute = 3,
        };
        bg.topAnchor = new UIRect.AnchorPoint()
        {
            target = bgTrans,
            relative = 1f,
            absolute = 1,
        };

        BoxCollider2D box = target.GetComponent<BoxCollider2D>();
        if (box == null)
        {
            Debug.LogError(prefabInstance.name + " has no box");
            return;
        }

        box.offset = Vector2.zero;
        box.size = new Vector2(138, 62);
    }

    static void SetLabel(Transform target,Transform returnBtn)
    {
        if (target == null) return;
        

        UILabel bg = target.GetComponent<UILabel>();

        UIWidget tempWidget = bg.GetComponent<UIWidget>();
        tempWidget.pivot = UIWidget.Pivot.Left;
        tempWidget.depth = 60;

        bg.overflowMethod = UILabel.Overflow.ResizeFreely;
        bg.ResetAnchors();
        bg.leftAnchor = new UIRect.AnchorPoint()
        {
            target = returnBtn,
            absolute = 140,
            relative = 0f,
        };

        bg.transform.localPosition = new Vector3(-500, -30, 0);
    }
    static void SetList(Transform target, Transform bg)
    {
        if (target == null) return;
        UIPanel p= target.GetComponent<UIPanel>();
        if(p != null)
        {
            DestroyImmediate(p);
        }

        UIWidget tempWidget = target.GetOrAddComponent<UIWidget>();
        tempWidget.pivot = UIWidget.Pivot.Center;
        tempWidget.width = 600;
        tempWidget.height = 62;

        tempWidget.SetAnchor(bg.gameObject,1f, -600,0f, 0,1f, 0,1f, 0);
    }
    private static void ProcessCurrencyLabel(GameObject prefabInstance)
    {
        MoneyIcon[] currencys = prefabInstance.GetComponentsInChildren<MoneyIcon>();
        Transform pa = currencys[0].transform.parent.parent;
        Transform tempBG = null;
        Transform tempBtn = null;
        Transform tempLabel = null;
        Transform tempList = null;

        for (int i = 0; i < pa.childCount; i++)
        {
            Transform temp = pa.GetChild(i);
            if (temp == null) continue;
            if (temp.name.Contains("bg"))
            {
                tempBG = temp;
                continue;
            }

            if(temp.name.Contains("Return") || temp.name.Contains("btn") || temp.GetComponent<BoxCollider2D>()!= null)
            {
                tempBtn = temp;
                continue;
            }

            if(temp.GetComponent<UILabel>()!= null)
            {
                tempLabel = temp;
                continue;
            }

            if (temp.name.Contains("list"))
            {
                tempList = temp;
                continue;
            }
            temp.gameObject.SetActive(false);
        }

        if (tempLabel == null || tempBG == null || tempBtn == null || tempList == null)
        {
            Debug.LogErrorFormat("{0} label:{1}  BG:{2}  Btn:{3} list:{4}", prefabInstance.name, tempLabel != null, tempBG != null, tempBtn != null, tempList != null);
            if (tempLabel == null &&(tempBG != null && tempBtn != null))
            {
                GameObject tempObj = new GameObject("Name");
                tempObj.transform.SetParent(tempBG.transform.parent);
                tempObj.transform.localScale = new Vector3(1, 1, 1);
                tempObj.transform.rotation = Quaternion.identity;
                tempObj.transform.localPosition = Vector3.zero;
                UILabel label = tempObj.AddComponent<UILabel>();

                UIFont tempFont = (UIFont)AssetDatabase.LoadAssetAtPath("Assets/ArtResources/Font/FZZZHUNHJW.prefab", typeof(UIFont));
                label.bitmapFont = tempFont;
                label.text = "Name";
                label.GetOrAddComponent<UIFontAuto>().fontID = 28;
                tempLabel = label.transform;

            }
            else
            {
                return;
            }
            //return;
        }
        tempBG.parent.transform.localPosition = Vector3.zero;
        Debug.Log("Success: " + prefabInstance.name +" : "+ currencys.Length);
        SetBG(tempBG,prefabInstance);
        ReturnBtn(tempBtn, prefabInstance, tempBG);
        SetLabel(tempLabel,tempBG);

        SetList(tempList, tempBG);

        List<Transform> tempArray = new List<Transform>();
        for (int i = 0; i < currencys[0].transform.parent.childCount; i++)
        {
            if (currencys[0].transform.parent.GetChild(i).gameObject.activeSelf == false) continue;
            tempArray.Add(currencys[0].transform.parent.GetChild(i));
        }
        tempArray.Sort((a, b) => { return b.transform.localPosition.x.CompareTo(a.transform.localPosition.x); });
        
        for (int i = tempArray.Count -1; i >=0; i--)
        {
            if (i == 0)
            {
                tempArray[0].transform.localPosition = new Vector3(13, 0, 0);
            }
            if (i== 1)
            {
                tempArray[1].transform.localPosition = new Vector3(-250, 0, 0);
            }
            if (i== 2)
            {
                tempArray[2].transform.localPosition = new Vector3(-513, 0, 0);
            }

        }
        //return;

        //MoneyIcon[] currencys = prefabInstance.GetComponentsInChildren<MoneyIcon>();
        if (currencys.Length > 0)
        {
            foreach (var eachCurrency in currencys)
            {
                //重新排下
                //1.背景
                Sprite spriteBg = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/ArtResources/allArtRes_optimize_/Resources/Icon/CommonCurrency/Common_Zy.png", typeof(Sprite));
                UI2DSprite bgSprite =eachCurrency.GetComponent<UI2DSprite>();
                bgSprite.sprite2D = spriteBg;
                bgSprite.type = UIBasicSprite.Type.Sliced;
                //x z
                //y w
                bgSprite.border=new Vector4(21f, 12f, 21f, 12f);

                UIWidget bgWidget= eachCurrency.GetComponent<UIWidget>();
                bgWidget.pivot = UIWidget.Pivot.Left;
                bgWidget.depth = 2;
                bgSprite.width = 245;
                bgSprite.height = 48;

                //icon
                UI2DSprite iconSprite = eachCurrency.icon.GetComponent<UI2DSprite>();
                iconSprite.type = UIBasicSprite.Type.Simple;
                iconSprite.MakePixelPerfect();

                UIWidget iconWidget = eachCurrency.icon.GetComponent<UIWidget>();
                iconWidget.pivot = UIWidget.Pivot.Center;
                iconWidget.depth = 3;
                iconSprite.transform.localPosition=new Vector3(15f,1f);

                //labelCount
                UIFont lFont = (UIFont)AssetDatabase.LoadAssetAtPath("Assets/ArtResources/Font/FZZZHUNHJW.prefab", typeof(UIFont));
                UILabel labelCurrency = eachCurrency.labelCount;
                labelCurrency.bitmapFont = lFont;
                labelCurrency.alignment = NGUIText.Alignment.Center;
                labelCurrency.GetComponent<UIWidget>().pivot = UIWidget.Pivot.Center;
                labelCurrency.GetComponent<UIWidget>().depth = 60;
                //labelCurrency.fontSize = 23;
                //labelCurrency.spacingX = 0;

                labelCurrency.overflowMethod = UILabel.Overflow.ResizeFreely;
                labelCurrency.transform.localPosition = new Vector3(130, 0f, 0);

                labelCurrency.GetOrAddComponent<UIFontAuto>().fontID = 38;

                //btnadd icon
                //1.背景
                
                if (eachCurrency.buttonAdd != null)//有的没有add按钮
                {
                    UI2DSprite addIconSprite = eachCurrency.buttonAdd.GetComponent<UI2DSprite>();
                    Sprite spriteAddIcon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/ArtResources/allArtRes_optimize_/Resources/Icon/CommonCurrency/Common_Btn_Add.png", typeof(Sprite));
                    addIconSprite.sprite2D = spriteAddIcon;
                    addIconSprite.type = UIBasicSprite.Type.Simple;
                    addIconSprite.MakePixelPerfect();

                    UIWidget addBtnWidget = eachCurrency.buttonAdd.GetComponent<UIWidget>();
                    addBtnWidget.pivot = UIWidget.Pivot.Center;
                    addBtnWidget.depth = 3;
                    addIconSprite.transform.localPosition = new Vector3(222.6f, 0.9f, 0f);
                }
                
                
            }
        }
    }

    [MenuItem("HYZ/LabelCurrencyAlignAndPos/Window打开动画")]
    public static void SetWIndowOpenAdnimtion()
    {
        int count = 1;
        //处理一个文件夹下的
        string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Resources/UI/Prefabs" });
        if (ids.Length > 0)
        {
            foreach (var eachId in ids)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(eachId);
                if (prefabPath.Contains("UICityWindow"))//主城的是四个,略微短一点
                {
                    continue;
                }
                if (prefabPath.Contains("Abyss"))//主城的是四个,略微短一点
                {
                    continue;
                }

                if (prefabPath.Contains("UIFBGqsWindow"))//主城的是四个,略微短一点
                {
                    continue;
                }

                EditorUtility.DisplayProgressBar("替换prefab路径", prefabPath, (float)count / ids.Length);
                try
                {

                    Object prefabObj = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
                    GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefabObj) as GameObject;
                    count++;
                    if (prefabInstance.layer != LayerManager.UI) //只处理UI层的Prefab
                    {
                        Object.DestroyImmediate(prefabInstance);
                        continue;
                    }

                    UIWindowBase window = prefabInstance.GetComponentInChildren<UIWindowBase>();
                    if (window == null)
                    {
                        Object.DestroyImmediate(prefabInstance);
                        continue;
                    }

                    bool b = SetAnimation(window);
                    if (b)
                    {
                        //apply 替换prefab
                        PrefabUtility.ReplacePrefab(prefabInstance, prefabObj, ReplacePrefabOptions.Default);
                    }
                    Object.DestroyImmediate(prefabInstance);
                }catch(Exception e)
                {
                    Debug.LogError(e.ToString());
                    EditorUtility.ClearProgressBar();
                    break;
                }
            }


            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private static bool SetAnimation(UIWindowBase window)
    {
        if(window.animations.Count > 0)
        {
            foreach (var item in window.animations)
            {
                if(item.MoveOffset.y < -300)
                {
                    item.MoveOffset = new Vector3(0, -920, 0);
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}