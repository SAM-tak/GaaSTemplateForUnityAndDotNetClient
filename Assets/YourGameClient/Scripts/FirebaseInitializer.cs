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
            //Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if(dependencyStatus == Firebase.DependencyStatus.Available) {

                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    // Crashlytics will use the DefaultInstance, as well;
                    // this ensures that Crashlytics is initialized.
                    var app = Firebase.FirebaseApp.DefaultInstance;

                    if(Firebase.Crashlytics.Crashlytics.IsCrashlyticsCollectionEnabled) {
                        Log.Info("Firebase Crashlytics is enable");
                    }
                    else {
                        Log.Warning("Firebase Crashlytics is not enable");
                    }

                    Log.Info($"Firebase initialization finished. {app.Name}, {app.Options.AppId}");

                    // Set a flag here for indicating that your project is ready to use Firebase.
                    Done = true;
                }
                else {
                    Log.Error($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }
    }
}
