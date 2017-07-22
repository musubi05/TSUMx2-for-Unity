namespace TSUMx2 {
    public class SceneManager {

        public static void LoadTitleScene() {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("title");
        }

        public static void LoadGameScene() {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("main");

        }

        public static void LoadResultScene() {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("result");

        }
    }
}

