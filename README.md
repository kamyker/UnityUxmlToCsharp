# Unity Uxml To Csharp

Plugin that generates helper C# classes based on uxml files in Unity. It may be considered as a very simple and automatic alternative to [UxmlFactory](https://docs.unity3d.com/Manual/UIE-UXML.html).

## Quick example
Let's make a panel with 3 buttons. Their actions have to be assigned in C#. Additional we will hide them when window is too small.

### Normal way:
**Page.uxml**
```
<?xml version="1.0" encoding="utf-8"?>
<UXML
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xmlns="UnityEngine.UIElements"
    xsi:noNamespaceSchemaLocation="../UIElementsSchema/UIElements.xsd"
    xsi:schemaLocation="UnityEngine.UIElements ../UIElementsSchema/UnityEngine.UIElements.xsd">
  <Box>
      <Button name="testButton1"/>
      <Button name="testButton2"/>
      <Button name="testButton3"/>
  </Box>
</UXML>
```
**TestWindow.cs**
```
public class TestWindow : EditorWindow
{
    private Button testButton1;
    private Button testButton2;
    private Button testButton3;
    
    private void OnEnable()
    {
        var root = rootVisualElement;
        root.Clear();
        var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("page.uxml"); //change this to correct path
        template.CloneTree(root);
        testButton1 = root.Q<Button>("testButton1");
        testButton2 = root.Q<Button>("testButton2");
        testButton3 = root.Q<Button>("testButton3");
        testButton1.clickable.clicked += () => DebugLog("button1 clicked");
        testButton2.clickable.clicked += () => DebugLog("button2 clicked");
        testButton3.clickable.clicked += () => DebugLog("button3 clicked");
        
        root.RegisterCallback<GeometryChangedEvent>(OnWindowResized);
    }
    
    private void OnWindowResized(GeometryChangedEvent window)
    {
        //using testButton1, testButton2, testButton3 here
    }
}
```

### With this plugin:
Uxml looks the same but we change 3 lines with buttons and their names to custom style "--csName".
```
<Button style="--csName: TestButton1;"/>
<Button style="--csName: TestButton2;"/>
<Button style="--csName: TestButton3;"/>
```
After creating UXML right click it in Unity and select "Uxml To C#/Create C# class". New file called PageConverted.cs will be generated.

**TestWindow.cs**
```
public class TestWindow : EditorWindow
{
    private PageConverted page = new PageConverted();
    
    private void OnEnable()
    {
        var root = rootVisualElement;
        root.Clear();
        root.AddChildren(page); //extension method similar to template.CloneTree(root);
        page.TestButton1.clickable.clicked += () => DebugLog("button1 clicked");
        page.TestButton2.clickable.clicked += () => DebugLog("button2 clicked");
        page.TestButton3.clickable.clicked += () => DebugLog("button3 clicked");
        
        root.RegisterCallback<GeometryChangedEvent>(OnWindowResized);
    }
    
    private void OnWindowResized(GeometryChangedEvent window)
    {
        //using page.TestButton1, page.TestButton2, page.TestButton3 here
    }
}
```

As you can see you no longer have to use Q method and load specific uxml template as it all happens in `new PageConverted()`.

## Important:
1. Converted class will be automatically updated every time uxml is changed IF they are both placed in the same folder.
2. If above is not an option and you want to move it to different folder you can manually update generated class with RMB and "Uxml To C#/Update C# class". It works as it has hardcoded guid to uxml file.
