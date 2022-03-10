using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace YourGameClient
{
    public class Preloader : MonoBehaviour
    {
        Slider progressBar;
        Animator animator;

        void Awake()
        {
            progressBar = GetComponentInChildren<Slider>();
            animator = GetComponentInChildren<Animator>();
        }

        // Start is called before the first frame update
        async void Start()
        {
            while(!FirebaseInitializer.Done) {
                if(progressBar.value < 0.3f) {
                    progressBar.value += 0.1f * Time.deltaTime;
                }
                await UniTask.NextFrame();
            }

            var op = SceneManager.LoadSceneAsync(1);
            if(op == null) {
                LogError($"Failed load next scene.");
                return;
            }
            op.allowSceneActivation = false;

            while(op.progress < 0.9f) {
                progressBar.value = (op.progress + 0.3f) / 1.3f;
                await UniTask.Yield();
            }

            progressBar.value = op.progress;
            var lastProgress = progressBar.value;
            float t = 0;
            while(t < 1) {
                t += Mathf.Min(1 / 10.0f, Time.deltaTime) * 3f;
                progressBar.value = Mathf.Min(1.0f, Mathf.Lerp(lastProgress, 1.0f, t));
                await UniTask.Yield();
            }
            progressBar.value = 1.0f;

            if(animator) {
                animator.SetTrigger("Exit");
                await UniTask.WaitWhile(() => animator.isActiveAndEnabled);
            }

            op.allowSceneActivation = true;
        }
    }
}