// Writed by Hamidreza Karamian | ahengine

using UnityEngine;
using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;

namespace PluginsInterface.FirebaseInterface
{
    public static class FirebaseAuthHandler
    {
        public static bool IsInitialized { private set; get; }
        private static FirebaseAuth instance;

        public static void Initialize()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    Debug.Log("Setting up Firebase Auth");
                    instance = FirebaseAuth.DefaultInstance;
                    IsInitialized = true;
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
                    IsInitialized = false;
                }
            });
        }

        public static async Task Register(string email, string password, string username, Action<AuthError?, UserProfile> callback)
        {
            var RegisterTask = instance.CreateUserWithEmailAndPasswordAsync(email, password);

            await TaskUtils.WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                callback?.Invoke(errorCode, null);
                return;
            }

            FirebaseUser User = RegisterTask.Result;

            if (User != null)
            {
                UserProfile profile = new UserProfile { DisplayName = username };
                var ProfileTask = User.UpdateUserProfileAsync(profile);

                await TaskUtils.WaitUntil(predicate: () => ProfileTask.IsCompleted);

                if (ProfileTask.Exception != null)
                {
                    Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                    FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                    callback?.Invoke(errorCode, null);
                    return;
                }

                callback?.Invoke(null, profile);
            }
        }
        public static async Task Login(string _email, string _password, Action<AuthError?, FirebaseUser> callback)
        {
            var LoginTask = instance.SignInWithEmailAndPasswordAsync(_email, _password);

            await TaskUtils.WaitUntil(predicate: () => LoginTask.IsCompleted);

            if (LoginTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
                FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                callback?.Invoke(errorCode, null);
                return;
            }

            FirebaseUser User = LoginTask.Result;
            callback?.Invoke(null, User);
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
        }
    }


    // Temporary is here
    public static class TaskUtils
    {
        public static async Task WaitUntil(Func<bool> predicate, int sleep = 50)
        {
            while (!predicate())
                await Task.Delay(sleep);
        }
    }
}
