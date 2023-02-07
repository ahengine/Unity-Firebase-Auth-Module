// Writed by Hamidreza Karamian | ahengine
using Firebase.Auth;
using UnityEngine;

namespace PluginsInterface.FirebaseInterface.Tests
{

    public class FirebaseAuthRuntimeTester : MonoBehaviour
    {
        [SerializeField] private string email;
        [SerializeField] private string password;
        [SerializeField] private string username;


        private void Awake()
        {
            Init();
        }

        [ContextMenu("Initialize")]
        public void Init()
        {
            FirebaseAuthHandler.Initialize();
        }

        [ContextMenu("Login")]
        public void Login()
        {
            void Callback(AuthError? authError,FirebaseUser profile)
            {
                if(authError != null)
                {
                    print(authError.ToString());
                    return;
                }

                print("FirebaseUser - Email:" + profile.Email+" | UserId: "+profile.UserId+" | Display Name: "+profile.DisplayName);
            }

            _ = FirebaseAuthHandler.Login(email, password, Callback);
        }

        [ContextMenu("Register")]
        public void Register()
        {
            void Callback(AuthError? authError, UserProfile profile)
            {
                if (authError != null)
                {
                    print(authError.ToString());
                    return;
                }

                print("UserProfile - Display Name:" + profile.DisplayName);
            }

            _ = FirebaseAuthHandler.Register(email, password,username, Callback);
        }
    }
}