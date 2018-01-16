# Sigma LoadingScreens

**Custom Loading Screens Mod by Sigma88**


To get support: https://github.com/Sigma88/Sigma-LoadingScreens/issues

Latest Release: https://github.com/Sigma88/Sigma-LoadingScreens/releases/latest



### Built-in support for:

- Galactic Neighborhood
- Sigma Binary
- Sigma Dimensions


# Settings

```
Sigma88LoadingScreens
{
}
```
This is the node, there can be as many nodes as you want. All will be loaded. 

These nodes cannot be edited using
[ModuleManager](http://forum.kerbalspaceprogram.com/index.php?/topic/50533-0/) patches.

<br><br>

#### The Sigma88LoadingScreens node contains the following fields:

<br>

  - **removeStockScreens**, *\<boolean\>*, *default value = false*.

    ```
    This will remove the stock loading screen images.
    
    Only the first "removeStockScreens" is read from every node.
    
    If there is at least one set to "true" the loading screens images will be removed.
    ```

  - **removeStockTips**, *\<boolean\>*, *default value = false*.

    ```
    This will remove the stock loading screen tips.
    
    Only the first "removeStockScreens" is read from every node.
    
    If there is at least one set to "true" the loading screens tips will be removed.
    ```

  - **folder**, *\<string\>*, The path of the images folder.

    ```
    This specifies the folder where the loading screens images are stored.
    
    All supported .dds files in this folder will be loaded.
    
    You can have as many "folder" lines in each node. All will be loaded.
    ```

  - **tipsFile**, *\<string\>*, The path of the tips file.

    ```
    This specifies the file where the loading screens tips are stored.
    
    Each non-empty line of text inside the file will be loaded as one "tip".
    
    You can have as many "tipsFile" lines in each node. All will be loaded.
    ```

  - **themedTips**, *\<boolean\>*, *default value = false*.

    ```
    Only the first "themedTips" is read from every node.
    
    Images and tips defined by a node with "themedTips" set to true will only be displayed together.
    ```

  - **logoScreen**, *\<string\>*, The path to the logo screen image.

    ```
    Only the first "logoScreen" is read from every node.
    
    This specifies the image to be used in the second logo screen
    (after the SQUAD monkey, but before all other loading screens)
    
    If no "logoScreen" are defined, the secondary logo screen will not be used.
    If more than one logo is defined, only one (chosen at random) will be used.
    ```

  - **logoTip**, *\<string\>*, *default value = "Loading..."*, The path to the logo screen image.

    ```
    Only the first "logoTip" is read from every node.
    
    This specifies the tip to be used in the second logo screen
    (after the SQUAD monkey, but before all other loading screens)
    
    "logoTip" is read only if a valid "logoScreen" is defined in the same node.
    
    If no "logoTip" is defined, the default will not be used.
    ```
