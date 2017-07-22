namespace TSUMx2 {
    public class SceneManager {

        public static void LoadTitleScene() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("title");
        }

        public static void LoadGameScene() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("main");

        }

        public static void LoadResultScene() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("result");

        }
    }
}

