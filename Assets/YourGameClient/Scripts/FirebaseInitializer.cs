using UnityEngine;
using CustomUnity;

namespace YourGameClient
{

    static public class FirebaseInitializer
    {
        public static bool Done { get; private set; }

        // Initialize Firebase
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnRuntimeInitialize()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if(dependencyStatus == Firebase.DependencyStatus.Available) {
                    Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;

                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    // Crashlytics will use the DefaultInstance, as well;
                    // this ensures that Crashlytics is initialized.
                    Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

                    // Set a flag here for indicating that your project is ready to use Firebase.
                    Done = true;

                    Log.Info("Firebase initialization finished.");
                }
                else {
                    Log.Error($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }
    }
}
