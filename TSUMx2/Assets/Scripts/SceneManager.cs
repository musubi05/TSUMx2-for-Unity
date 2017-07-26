namespace TSUMx2 {
    public class SceneManager {

        /// <summary>
        /// Load Title Scene
        /// </summary>
        public static void LoadTitleScene() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("title");
        }

        /// <summary>
        /// Load Game Scene
        /// </summary>
        public static void LoadGameScene() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("main");
        }

        /// <summary>
        ///  Load Result Scene
        /// </summary>
        public static void LoadResultScene() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("result");
        }
    }
}

