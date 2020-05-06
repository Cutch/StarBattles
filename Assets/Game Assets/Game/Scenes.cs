using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class Scenes
{

    private static Dictionary<string, object> parameters = new Dictionary<string, object>();

    public static void Load(string sceneName)
    {
        Scenes.parameters.Clear();
        SceneManager.LoadScene(sceneName);
    }
    public static void Load(string sceneName, Dictionary<string, object> parameters = null)
    {
        Scenes.parameters.Clear();
        foreach(KeyValuePair<string, object> entry in parameters){
            Scenes.parameters.Add(entry.Key, entry.Value);
        }
        SceneManager.LoadScene(sceneName);
    }

    public static void Load(string sceneName, string paramKey, object paramValue)
    {
        Scenes.parameters.Clear();
        Scenes.parameters.Add(paramKey, paramValue);
        SceneManager.LoadScene(sceneName);
    }

    //public static Dictionary<string, string> getSceneParameters()
    //{
    //    return parameters;
    //}

    public static object getParam(string paramKey)
    {
        if (parameters == null || !parameters.ContainsKey(paramKey)) return null;
        return parameters[paramKey];
    }

    //public static void setParam(string paramKey, string paramValue)
    //{
    //    if (parameters == null)
    //        Scenes.parameters = new Dictionary<string, string>();
    //    Scenes.parameters.Add(paramKey, paramValue);
    //}

}